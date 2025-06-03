//Please, if you use this, share the improvements

using System;
using System.Collections.Generic;
using SharpGL;
using System.Drawing;

namespace OpenGrade
{
    public partial class FormGPS
    {
        public  double toLatitude;
        public  double toLongitude;

        //very first fix to setup grid etc
        public bool isFirstFixPositionSet = false, isGPSPositionInitialized = false;

        // autosteer variables for sending serial
        public Int16 guidanceLineDistanceOff, guidanceLineSteerAngle;

        private double sectionTriggerDistance;
        private vec2 prevContourPos = new vec2();

        //how many fix updates per sec
        public int fixUpdateHz = 5;
        public double fixUpdateTime = 0.2;

        //for window lost rtk
        public int lastFixQuality;

        //Current fix positions
        public double fixZ = 0.0;

        public vec2 fix = new vec2(0, 3.0);

        //history
        public vec2 prevFix = new vec2(0, 0);

        //headings
        public double fixHeading = 0.0, camHeading = 0.0, gpsHeading = 0.0, prevGPSHeading = 0.0, prevPrevGPSHeading = 0.0;

        //a distance between previous and current fix
        private double distance = 0.0, userDistance = 0;
          
        //step distances and positions for boundary, 4 meters before next point
        public double boundaryTriggerDistance = 4.0;
        public vec2 prevBoundaryPos = new vec2(0, 0);

        //are we still getting valid data from GPS, resets to 0 in NMEA RMC block, watchdog 
        public int recvCounter = 20;

        //Everything is so wonky at the start
        int startCounter = 0;

        //individual points for the flags in a list
        List<CFlag> flagPts = new List<CFlag>();

        //used to determine NMEA sentence frequency
        private int timerPn = 1;        
        private double et = 0, hzTime = 0;

        public double[] avgSpeed = new double[10];//for average speed
        public int ringCounter = 0;
        
        //IMU 
        double rollCorrectionDistance = 0;
        public double rollZero = 0, pitchZero = 0;
        double gyroDelta, gyroCorrection, gyroRaw, gyroCorrected, turnDelta;

        //step position - slow speed spinner killer
        private int totalFixSteps = 10, currentStepFix = 0;
        private vec3 vHold;
        public vec3[] stepFixPts = new vec3[50];
        public double distanceCurrentStepFix = 0, fixStepDist, minFixStepDist = 0;        
        bool isFixHolding = false, isFixHoldLoaded = false;
        
        //called by watchdog timer every 10 ms
        private void ScanForNMEA()
        {
            //parse any data from pn.rawBuffer
            pn.ParseNMEA();

            //time for a frame update with new valid nmea data
            if (pn.updatedGGA)
            {
                isGNSSrecieved = 0;
                //if saving a file ignore any movement
                if (isSavingFile) return;

                //accumulate time over multiple frames  
                hzTime += swFrame.ElapsedMilliseconds;

                //reset the timer         
                swFrame.Reset();

                //now calculate NMEA rate
                if (timerPn++ == 36)
                {
                    et = (1 / (hzTime * 0.000025));
                    if (et > 13 && et < 18) fixUpdateHz = 15;
                    if (et > 18 && et < 23) fixUpdateHz = 20;
                    if (et > 23 && et < 28) fixUpdateHz = 25;
                    if (et > 28 && et < 38) fixUpdateHz = 30;
                    if (et > 38 && et < 47) fixUpdateHz = 40;
                    if (et > 47 && et < 99) fixUpdateHz = 50;
                    if (et > 4 && et < 7) fixUpdateHz = 5;
                    if (et > 7 && et < 9) fixUpdateHz = 8;
                    if (et > 9 && et < 13) fixUpdateHz = 10;
                    if (et > 1.2 && et < 3) fixUpdateHz = 2;
                    if (et > 0.8 && et < 1.2) fixUpdateHz = 1;
                    fixUpdateTime = 1 / (double)fixUpdateHz;
                    timerPn = 0;
                    hzTime = 0;
                    //fixUpdateHz = 5;
                    //fixUpdateTime = 0.2;
                }

                //start the watch and time till it gets back here
                swFrame.Start();

                //reset both flags
                pn.updatedGGA = false;

                //update all data for new frame
                UpdateFixPosition();
            }
            
            //must make sure arduinos are kept off
            else
            {
                if (!isGPSPositionInitialized)  mc.ResetAllModuleCommValues();
            }
            
            //Update the port connecition counter - is reset every time new sentence is valid and ready
            recvCounter++;
        }

        //call for position update after valid NMEA sentence
        private void UpdateFixPosition()
        {
            startCounter++;
            totalFixSteps = fixUpdateHz * 4;
            if (!isGPSPositionInitialized) { InitializeFirstFewGPSPositions(); return; }

            #region Roll

            if (mc.rollRaw != 9999)
            {
                //calculate how far the antenna moves based on sidehill roll
                double roll = Math.Sin(glm.toRadians(mc.rollRaw/16.0));
                rollCorrectionDistance = Math.Abs(roll * vehicle.antennaHeight);

                // tilt to left is positive  **** important!!
                if (roll > 0)
                {
                    pn.easting = (Math.Cos(fixHeading) * rollCorrectionDistance) + pn.easting;
                    pn.northing = (Math.Sin(fixHeading) * -rollCorrectionDistance) + pn.northing;
                }
                else
                {
                    pn.easting = (Math.Cos(fixHeading) * -rollCorrectionDistance) + pn.easting;
                    pn.northing = (Math.Sin(fixHeading) * rollCorrectionDistance) + pn.northing;
                }
            }

            //tiltDistance = (pitch * vehicle.antennaHeight);
            ////pn.easting = (Math.Sin(fixHeading) * tiltDistance) + pn.easting;
            //pn.northing = (Math.Cos(fixHeading) * tiltDistance) + pn.northing;

            #endregion Roll

            #region Step Fix

            //grab the most current fix and save the distance from the last fix
            distanceCurrentStepFix = pn.Distance(pn.northing, pn.easting, stepFixPts[0].northing, stepFixPts[0].easting);
            fixStepDist = distanceCurrentStepFix;

            //if  min distance isn't exceeded, keep adding old fixes till it does
            if (distanceCurrentStepFix <= minFixStepDist)
            {
                for (currentStepFix = 0; currentStepFix < totalFixSteps; currentStepFix++)
                {
                    fixStepDist += stepFixPts[currentStepFix].heading;
                    if (fixStepDist > minFixStepDist)
                    {
                        //if we reached end, keep the oldest and stay till distance is exceeded
                        if (currentStepFix < (totalFixSteps - 1)) currentStepFix++;
                        isFixHolding = false;
                        break;
                    }
                    else isFixHolding = true;
                }
            }

            // only takes a single fix to exceeed min distance
            else currentStepFix = 0;

            //if total distance is less then the addition of all the fixes, keep last one as reference
            if (isFixHolding)
            {
                if (isFixHoldLoaded == false)
                {
                    vHold = stepFixPts[(totalFixSteps - 1)];
                    isFixHoldLoaded = true;
                }

                //cycle thru like normal
                for (int i = totalFixSteps - 1; i > 0; i--) stepFixPts[i] = stepFixPts[i - 1];

                //fill in the latest distance and fix
                stepFixPts[0].heading = pn.Distance(pn.northing, pn.easting, stepFixPts[0].northing, stepFixPts[0].easting);
                stepFixPts[0].easting = pn.easting;
                stepFixPts[0].northing = pn.northing;

                //reload the last position that was triggered.
                stepFixPts[(totalFixSteps - 1)].heading = pn.Distance(vHold.northing, vHold.easting, stepFixPts[(totalFixSteps - 1)].northing, stepFixPts[(totalFixSteps - 1)].easting);
                stepFixPts[(totalFixSteps - 1)].easting = vHold.easting;
                stepFixPts[(totalFixSteps - 1)].northing = vHold.northing;
            }

            else //distance is exceeded, time to do all calcs and next frame
            {
                //positions and headings 
                CalculatePositionHeading();

                //get rid of hold position
                isFixHoldLoaded = false;

                //don't add the total distance again
                stepFixPts[(totalFixSteps - 1)].heading = 0;

                //grab sentences for logging
                if (isLogNMEA)
                {
                    if (ct.isContourOn)
                    {
                        pn.logNMEASentence.Append(recvSentenceSettings);
                    }
                }

                //add another point if on
                //AddSectionContourPathPoints();

                //To prevent drawing high numbers of triangles, determine and test before drawing vertex
                sectionTriggerDistance = pn.Distance(pn.northing, pn.easting, prevContourPos.northing, prevContourPos.easting);

                //section on off and points, contour points
                if (sectionTriggerDistance > 0.2)
                {
                    prevContourPos.easting = pn.easting;
                    prevContourPos.northing = pn.northing;
                    AddSectionContourPathPoints();
                }


                //calc distance travelled since last GPS fix
                distance = pn.Distance(pn.northing, pn.easting, prevFix.northing, prevFix.easting);
                if ((userDistance += distance) > 9000) userDistance = 0; ;//userDistance can be reset

                //most recent fixes are now the prev ones
                prevFix.easting = pn.easting; prevFix.northing = pn.northing;

                //load up history with valid data
                for (int i = totalFixSteps - 1; i > 0; i--) stepFixPts[i] = stepFixPts[i - 1];
                stepFixPts[0].heading = pn.Distance(pn.northing, pn.easting, stepFixPts[0].northing, stepFixPts[0].easting);
                stepFixPts[0].easting = pn.easting;
                stepFixPts[0].northing = pn.northing;
            }
            #endregion fix

            #region AutoSteer

            guidanceLineDistanceOff = 32000;    //preset the values

            // autosteer at full speed of updates
            if (!isAutoSteerBtnOn) //32020 means auto steer is off
            {
                guidanceLineDistanceOff = 32020;
            }

            // If Drive button enabled be normal, or just fool the autosteer and fill values
            if (!isInFreeDriveMode)
            {

                //fill up0 the auto steer array with new values
                mc.autoSteerData[mc.sdSpeed] = (byte)(pn.speed * 4.0);

                mc.autoSteerData[mc.sdDistanceHi] = (byte)(guidanceLineDistanceOff >> 8);
                mc.autoSteerData[mc.sdDistanceLo] = (byte)guidanceLineDistanceOff;

                mc.autoSteerData[mc.sdSteerAngleHi] = (byte)(guidanceLineSteerAngle >> 8);
                mc.autoSteerData[mc.sdSteerAngleLo] = (byte)guidanceLineSteerAngle;

                //out serial to autosteer module  //indivdual classes load the distance and heading deltas 
                AutoSteerDataOutToPort();
                
                SendUDPMessage(guidanceLineSteerAngle + "," + guidanceLineDistanceOff);
            }

            else
            {
                //fill up the auto steer array with free drive values
                mc.autoSteerData[mc.sdSpeed] = (byte)(pn.speed * 4.0 + 8);

                //make steer module think everything is normal
                mc.autoSteerData[mc.sdDistanceHi] = (byte)(0);
                mc.autoSteerData[mc.sdDistanceLo] = (byte)0;

                //out serial to autosteer module  //indivdual classes load the distance and heading deltas 
                AutoSteerDataOutToPort();
            }
            #endregion

            #region Calculate Heigt
            //reset cut delta for frame
            cutDelta = 9999;

            int closestPoint = 0;
            int ptCnt = ct.ptList.Count;


            if (cboxLaserModeOnOff.Checked)
            {

                cutDelta = (pn.altitude - ct.zeroAltitude) * 100;

            }
            if (ptCnt > 0)
            {
                //here is all the calculation to find the blade target at each fit, lot of maths!

                minDist = 1000000; //original is 1000000               

                //pt number of the pts
                int closestPointMap2 = -1;
                int closestPointMap3 = -1;
                int closestPointMap4 = -1;
                int closestPointMap5 = -1;
                int closestPointMap6 = -1;

                int ptCount = ct.ptList.Count - 1;
                SurveyPtDist = levelDistFactor;
                SurveyPtDistSqr = SurveyPtDist * SurveyPtDist;
                double minDist2 = SurveyPtDistSqr; // if the point is further than "pts dist for calc" we forget it
                double minDist3 = SurveyPtDistSqr;
                double minDist4 = SurveyPtDistSqr;
                double minDist5 = SurveyPtDistSqr;
                double minDist6 = SurveyPtDistSqr;

                //find 6 closest points to current fix, we go throug the design pts
                for (int t = 0; t < ptCount; t++)
                {
                    double distMap = ((pn.easting - ct.ptList[t].easting) * (pn.easting - ct.ptList[t].easting))
                                    + ((pn.northing - ct.ptList[t].northing) * (pn.northing - ct.ptList[t].northing));

                    if (distMap < minDist && distMap > SurveyPtDistSqr) // to at least have one pt if more than "pts dist for calc" away
                    {
                        minDist = distMap;
                        closestPoint = t;

                    }

                    else if (distMap < minDist6) // if less than "pts dist for calc" from position do all this stuff
                    {
                        if (distMap < minDist5)
                        {
                            if (distMap < minDist4)
                            {
                                if (distMap < minDist3)
                                {
                                    if (distMap < minDist2)
                                    {
                                        if (distMap < minDist)
                                        {
                                            minDist6 = minDist5;
                                            minDist5 = minDist4;
                                            minDist4 = minDist3;
                                            minDist3 = minDist2;
                                            minDist2 = minDist;
                                            minDist = distMap;

                                            closestPointMap6 = closestPointMap5;
                                            closestPointMap5 = closestPointMap4;
                                            closestPointMap4 = closestPointMap3;
                                            closestPointMap3 = closestPointMap2;
                                            closestPointMap2 = closestPoint;
                                            closestPoint = t;
                                        }
                                        else
                                        {
                                            minDist6 = minDist5;
                                            minDist5 = minDist4;
                                            minDist4 = minDist3;
                                            minDist3 = minDist2;
                                            minDist2 = distMap;

                                            closestPointMap6 = closestPointMap5;
                                            closestPointMap5 = closestPointMap4;
                                            closestPointMap4 = closestPointMap3;
                                            closestPointMap3 = closestPointMap2;
                                            closestPointMap2 = t;
                                        }
                                    }
                                    else
                                    {
                                        minDist6 = minDist5;
                                        minDist5 = minDist4;
                                        minDist4 = minDist3;
                                        minDist3 = distMap;

                                        closestPointMap6 = closestPointMap5;
                                        closestPointMap5 = closestPointMap4;
                                        closestPointMap4 = closestPointMap3;
                                        closestPointMap3 = t;
                                    }
                                }
                                else
                                {
                                    minDist6 = minDist5;
                                    minDist5 = minDist4;
                                    minDist4 = distMap;

                                    closestPointMap6 = closestPointMap5;
                                    closestPointMap5 = closestPointMap4;
                                    closestPointMap4 = t;
                                }
                            }
                            else
                            {
                                minDist6 = minDist5;
                                minDist5 = distMap;

                                closestPointMap6 = closestPointMap5;
                                closestPointMap5 = t;
                            }
                        }
                        else
                        {
                            minDist6 = distMap;

                            closestPointMap6 = t;
                        }
                    }
                }
                // We found our 6 closest pts

                //Here we check to use only the pts that make a square from the closest pt

                double dist12 = 1000000;
                if (minDist2 < SurveyPtDistSqr) dist12 = pn.DistanceSquared(ct.ptList[closestPoint].northing, ct.ptList[closestPoint].easting, ct.ptList[closestPointMap2].northing, ct.ptList[closestPointMap2].easting);

                if (minDist3 < SurveyPtDistSqr)
                {
                    double dist13 = pn.DistanceSquared(ct.ptList[closestPoint].northing, ct.ptList[closestPoint].easting, ct.ptList[closestPointMap3].northing, ct.ptList[closestPointMap3].easting);
                    double dist23 = pn.DistanceSquared(ct.ptList[closestPointMap3].northing, ct.ptList[closestPointMap3].easting, ct.ptList[closestPointMap2].northing, ct.ptList[closestPointMap2].easting);


                    if (dist13 > (1.1 * dist12) || dist23 > (2.1 * dist12) || minDist2 > (1.02 * dist12))
                    {
                        minDist3 = SurveyPtDistSqr;
                        minDist4 = SurveyPtDistSqr;
                    }// both removed from calculation

                }

                if (minDist4 < SurveyPtDistSqr)
                {

                    double dist24 = pn.DistanceSquared(ct.ptList[closestPointMap4].northing, ct.ptList[closestPointMap4].easting, ct.ptList[closestPointMap2].northing, ct.ptList[closestPointMap2].easting);
                    double dist34 = pn.DistanceSquared(ct.ptList[closestPointMap4].northing, ct.ptList[closestPointMap4].easting, ct.ptList[closestPointMap3].northing, ct.ptList[closestPointMap3].easting);




                    if (minDist5 < SurveyPtDistSqr)
                    {
                        double dist25 = pn.DistanceSquared(ct.ptList[closestPointMap5].northing, ct.ptList[closestPointMap5].easting, ct.ptList[closestPointMap2].northing, ct.ptList[closestPointMap2].easting);
                        double dist35 = pn.DistanceSquared(ct.ptList[closestPointMap5].northing, ct.ptList[closestPointMap5].easting, ct.ptList[closestPointMap3].northing, ct.ptList[closestPointMap3].easting);

                        // Now check for witch of pt 4 5 or 6 is the right pt: it's the pt with the smallest dist from pt 2 + pt3
                        double fourthPtdist = dist24 + dist34;

                        double fifthPtdist = dist35 + dist35;

                        if (fifthPtdist < fourthPtdist)
                        {
                            closestPointMap4 = closestPointMap5;
                            minDist4 = minDist5;
                            fourthPtdist = fifthPtdist;
                            dist24 = dist25;
                            dist34 = dist35;
                        }

                        if (minDist6 < SurveyPtDistSqr)
                        {
                            double dist26 = pn.DistanceSquared(ct.ptList[closestPointMap6].northing, ct.ptList[closestPointMap6].easting, ct.ptList[closestPointMap2].northing, ct.ptList[closestPointMap2].easting);
                            double dist36 = pn.DistanceSquared(ct.ptList[closestPointMap6].northing, ct.ptList[closestPointMap6].easting, ct.ptList[closestPointMap3].northing, ct.ptList[closestPointMap3].easting);

                            double sixthPtdist = dist26 + dist36;


                            if (sixthPtdist < fourthPtdist)
                            {
                                closestPointMap4 = closestPointMap6;
                                minDist4 = minDist6;
                                dist24 = dist26;
                                dist34 = dist36;
                            }
                        }
                    }

                    //check dist squared 1-2, if 1-3 is < than 1.1x 1-3 and 1-4 < than 2.1
                    double dist14 = pn.DistanceSquared(ct.ptList[closestPoint].northing, ct.ptList[closestPoint].easting, ct.ptList[closestPointMap4].northing, ct.ptList[closestPointMap4].easting);
                    if (dist14 > (2.1 * dist12) || dist24 > (1.1 * dist12) || dist34 > (1.1 * dist12))
                    {
                        minDist4 = SurveyPtDistSqr;

                    } //removed from calculation

                }
                //We now have the revelelent pts.


                //here calculate the closest point on each line

                distanceFromAline = NoLineSurveyPt;
                distanceFromBline = NoLineSurveyPt;
                distanceFromCline = NoLineSurveyPt;
                distanceFromDline = NoLineSurveyPt;

                altitudeApt = -999;
                altitudeBpt = -999;
                altitudeCpt = -999;
                altitudeDpt = -999;
                cutAltApt = -999;
                cutAltBpt = -999;
                cutAltCpt = -999;
                cutAltDpt = -999;

                double NoLineAverageAlt = 0;
                double NoLineAverageCutAlt = 0;
                double NoLineCount = 0;
                double NoLineCutCount = 0;

                //Calculate the closest line:A, between closest and second closest pts (ClosestPoint and ClosestPointMap2)
                if (minDist < SurveyPtDistSqr && minDist2 < SurveyPtDistSqr)
                {
                    CalcPtOnLine(closestPoint, closestPointMap2, out distanceFromAline, out eastingApt, out northingApt, out altitudeApt, out cutAltApt);

                    if (ct.ptList[closestPointMap2].cutAltitude > 0 && ct.ptList[closestPoint].cutAltitude > 0)
                    {
                        NoLineAverageCutAlt += cutAltApt;
                        NoLineCutCount++;
                    }
                    else cutAltApt = -999;

                    NoLineAverageAlt += altitudeApt;
                    NoLineCount++;
                }


                //Calculate the second closest line:B, between closest and thrid closest pts (ClosestPoint and ClosestPointMap3)
                if (minDist < SurveyPtDistSqr && minDist3 < SurveyPtDistSqr)
                {
                    CalcPtOnLine(closestPoint, closestPointMap3, out distanceFromBline, out eastingBpt, out northingBpt, out altitudeBpt, out cutAltBpt);

                    if (ct.ptList[closestPointMap3].cutAltitude > 0 && ct.ptList[closestPoint].cutAltitude > 0)
                    {
                        NoLineAverageCutAlt += cutAltBpt;
                        NoLineCutCount++;
                    }
                    else cutAltBpt = -999;

                    NoLineAverageAlt += altitudeBpt;
                    NoLineCount++;
                }

                //Calculate the thirth closest line:C, between  second closest and fourth closest pts (ClosestPointMap2 and ClosestPointMap4)
                if (minDist2 < SurveyPtDistSqr && minDist4 < SurveyPtDistSqr)
                {
                    CalcPtOnLine(closestPointMap2, closestPointMap4, out distanceFromCline, out eastingCpt, out northingCpt, out altitudeCpt, out cutAltCpt);

                    if (ct.ptList[closestPointMap4].cutAltitude > 0 && ct.ptList[closestPointMap2].cutAltitude > 0)
                    {
                        NoLineAverageCutAlt += cutAltCpt;
                        NoLineCutCount++;
                    }
                    else cutAltCpt = -999;

                    NoLineAverageAlt += altitudeCpt;
                    NoLineCount++;
                }

                //Calculate the fourth closest line:D, between  third closest and fourth closest pts (ClosestPointMap3 and ClosestPointMap4)
                if (minDist3 < SurveyPtDistSqr && minDist4 < SurveyPtDistSqr)
                {
                    CalcPtOnLine(closestPointMap3, closestPointMap4, out distanceFromDline, out eastingDpt, out northingDpt, out altitudeDpt, out cutAltDpt);

                    if (ct.ptList[closestPointMap4].cutAltitude > 0 && ct.ptList[closestPointMap3].cutAltitude > 0)
                    {
                        NoLineAverageCutAlt += cutAltDpt;
                        NoLineCutCount++;
                    }
                    else cutAltDpt = -999;

                    NoLineAverageAlt += altitudeDpt;
                    NoLineCount++;
                }

                // Give a value to the lines witout values
                if (NoLineCount > 0)
                {
                    NoLineAverageAlt = NoLineAverageAlt / NoLineCount;
                    if (NoLineCutCount > 0)
                        NoLineAverageCutAlt = NoLineAverageCutAlt / NoLineCutCount;
                    else NoLineAverageCutAlt = -999;

                    if (distanceFromAline == NoLineSurveyPt)
                    {
                        altitudeApt = NoLineAverageAlt;
                        cutAltApt = NoLineAverageCutAlt;
                    }

                    if (distanceFromBline == NoLineSurveyPt)
                    {
                        altitudeBpt = NoLineAverageAlt;
                        cutAltBpt = NoLineAverageCutAlt;
                    }

                    if (distanceFromCline == NoLineSurveyPt)
                    {
                        altitudeCpt = NoLineAverageAlt;
                        cutAltCpt = NoLineAverageCutAlt;
                    }

                    if (distanceFromDline == NoLineSurveyPt)
                    {
                        altitudeDpt = NoLineAverageAlt;
                        cutAltDpt = NoLineAverageCutAlt;
                    }
                }

                // check if the blade is close from a line
                double mindistFromLine = distanceFromAline;
                double eastingLine = eastingApt;
                double northingLine = northingApt;
                double altitudeLine = altitudeApt;
                double cutAltLine = cutAltApt;

                if (distanceFromBline < mindistFromLine)
                {
                    mindistFromLine = distanceFromBline;
                    eastingLine = eastingBpt;
                    northingLine = northingBpt;
                    altitudeLine = altitudeBpt;
                    cutAltLine = cutAltBpt;
                }

                //no more needed, A line slould aways be the closest

                // Calulate the actual position alitude and cutAltitude

                //double cutFillMap;
                double avgAltitude = -999;
                double avgCutAltitude = -999;

                //clear the  design points shown on the map
                ct.usedPtList.Clear();
                //add the closest design pt
                UsedPt point = new UsedPt(ct.ptList[closestPoint].easting, ct.ptList[closestPoint].northing, 1);
                ct.usedPtList.Add(point);

                // if the pt is near the closest pt or No Average is selected or there is only one survey pt
                int nbrofPt = 4;
                if (minDist4 == SurveyPtDistSqr) nbrofPt--;
                if (minDist3 == SurveyPtDistSqr) nbrofPt--;
                if (minDist2 == SurveyPtDistSqr) nbrofPt--;

                if (minDist < (noAvgDist * noAvgDist) | !averagePts | nbrofPt < 2)
                {
                    //use only the closest design pt data witout averaging
                    avgAltitude = ct.ptList[closestPoint].altitude;
                    avgCutAltitude = ct.ptList[closestPoint].cutAltitude;
                }
                else if (mindistFromLine < noAvgDist)
                // if the blade is near a line
                {
                    avgAltitude = altitudeLine;
                    avgCutAltitude = cutAltLine;
                    //add the design pts near (orange)
                    if (minDist2 < SurveyPtDistSqr)
                    {
                        UsedPt point2 = new UsedPt(ct.ptList[closestPointMap2].easting, ct.ptList[closestPointMap2].northing, 1);
                        ct.usedPtList.Add(point2);
                    }

                    if (minDist3 < SurveyPtDistSqr)
                    {
                        UsedPt point2 = new UsedPt(ct.ptList[closestPointMap3].easting, ct.ptList[closestPointMap3].northing, 1);
                        ct.usedPtList.Add(point2);
                    }

                    if (minDist4 < SurveyPtDistSqr)
                    {
                        UsedPt point2 = new UsedPt(ct.ptList[closestPointMap4].easting, ct.ptList[closestPointMap4].northing, 1);
                        ct.usedPtList.Add(point2);
                    }
                    //the pt between 2 design pts used as height (blue)
                    UsedPt point6 = new UsedPt(eastingLine, northingLine, 2);
                    ct.usedPtList.Add(point6);
                }
                else // blade is somewere between 4 points (or less) AND at least 2 pts and ONE line are used
                {
                    double sumofCloseDist = 1 / distanceFromAline + 1 / distanceFromBline + 1 / distanceFromCline + 1 / distanceFromDline;

                    avgAltitude = ((altitudeApt / distanceFromAline) + (altitudeBpt / distanceFromBline) +
                    (altitudeCpt / distanceFromCline) + (altitudeDpt / distanceFromDline)) / sumofCloseDist;

                    if (cutAltApt < -997)
                    {
                        avgCutAltitude = ct.ptList[closestPoint].cutAltitude;
                    }
                    else if (cutAltBpt < -997 | cutAltCpt < -997 | cutAltDpt < -997)
                    {
                        avgCutAltitude = cutAltApt;

                    }
                    else
                    {
                        avgCutAltitude = (cutAltApt / distanceFromAline + cutAltBpt / distanceFromBline +
                    cutAltCpt / distanceFromCline + cutAltDpt / distanceFromDline) / sumofCloseDist;

                    }

                    // //add the design pts near (orange)
                    if (minDist2 < SurveyPtDistSqr)
                    {
                        UsedPt point2 = new UsedPt(ct.ptList[closestPointMap2].easting, ct.ptList[closestPointMap2].northing, 1);
                        ct.usedPtList.Add(point2);
                    }

                    if (minDist3 < SurveyPtDistSqr)
                    {
                        UsedPt point2 = new UsedPt(ct.ptList[closestPointMap3].easting, ct.ptList[closestPointMap3].northing, 1);
                        ct.usedPtList.Add(point2);
                    }

                    if (minDist4 < SurveyPtDistSqr)
                    {
                        UsedPt point2 = new UsedPt(ct.ptList[closestPointMap4].easting, ct.ptList[closestPointMap4].northing, 1);
                        ct.usedPtList.Add(point2);
                    }

                    if (distanceFromAline < NoLineSurveyPt)
                    {
                        UsedPt point6 = new UsedPt(eastingApt, northingApt, 2);
                        ct.usedPtList.Add(point6);
                    }

                    if (distanceFromBline < NoLineSurveyPt)
                    {
                        UsedPt point6 = new UsedPt(eastingBpt, northingBpt, 2);
                        ct.usedPtList.Add(point6);
                    }

                    if (distanceFromCline < NoLineSurveyPt)
                    {
                        UsedPt point6 = new UsedPt(eastingCpt, northingCpt, 2);
                        ct.usedPtList.Add(point6);
                    }

                    if (distanceFromDline < NoLineSurveyPt)
                    {
                        UsedPt point6 = new UsedPt(eastingDpt, northingDpt, 2);
                        ct.usedPtList.Add(point6);
                    }
                }
             
                if (!ct.surveyMode && ct.eleViewList.Count > 0)
                {
                    //fill in the latest distance and fix
                    double fixDist = ((ct.eleViewList[101].easting - pn.easting) * (ct.eleViewList[101].easting - pn.easting) + (ct.eleViewList[101].northing - pn.northing) * (ct.eleViewList[101].northing - pn.northing));
                    // if the dist is more than 0.5m
                    if (fixDist > 0.25)
                    {

                        //copy each point one count back: 0 take 1, 1 take 2 etc.

                        for (int i = 0; i < 101; i++)
                        {


                            ct.eleViewList[i].lastPassAltitude = ct.eleViewList[i + 1].lastPassAltitude;
                            ct.eleViewList[i].easting = ct.eleViewList[i + 1].easting;
                            ct.eleViewList[i].northing = ct.eleViewList[i + 1].northing;
                            ct.eleViewList[i].heading = ct.eleViewList[i + 1].heading;
                            ct.eleViewList[i].altitude = ct.eleViewList[i + 1].altitude;
                            ct.eleViewList[i].cutAltitude = ct.eleViewList[i + 1].cutAltitude;

                        }

                        //for (int i = 0; i < 101; i++) ct.eleViewList[i] = ct.eleViewList[i + 1]; this is not working

                        // fill the current point (101)
                        ct.eleViewList[101].lastPassAltitude = pn.altitude;
                        ct.eleViewList[101].easting = pn.easting;
                        ct.eleViewList[101].northing = pn.northing;
                        ct.eleViewList[101].heading = fixHeading;

                        if (minDist < 90000)
                        {
                            ct.eleViewList[101].altitude = avgAltitude;
                            ct.eleViewList[101].cutAltitude = avgCutAltitude;
                        }
                        else
                        {
                            ct.eleViewList[101].altitude = -999;
                            ct.eleViewList[101].cutAltitude = -999;
                        }

                        // make the look ahead view

                        //but only if the window is showing
                        if (openGLControlBack.Visible)
                        {


                            // 200 points to fill, 102 to 298? 4 * 49 = 196

                            for (int j = 1; j < 50; j++)
                            {

                                double AheadEasting = pn.easting + Math.Cos(fixHeading + glm.PIBy2) * -2 * j;
                                double AheadNorthing = pn.northing + Math.Sin(fixHeading - glm.PIBy2) * -2 * j;


                                double mindist = 1000000;
                                int ClosestLookAheadPt = 999999;
                                int lookPtCt = ct.ptList.Count;

                                if (lookPtCt > 0)
                                {
                                    for (int m = 0; m < lookPtCt; m++)
                                    {
                                        double distA = (AheadEasting - ct.ptList[m].easting) * (AheadEasting - ct.ptList[m].easting) +
                                            (AheadNorthing - ct.ptList[m].northing) * (AheadNorthing - ct.ptList[m].northing);

                                        if (distA < mindist)
                                        {
                                            mindist = distA;
                                            ClosestLookAheadPt = m;
                                        }
                                    }
                                }







                                for (int k = 0; k < 4; k++)
                                {
                                    ct.eleViewList[101 + j * 4 - k].easting = AheadEasting;
                                    ct.eleViewList[101 + j * 4 - k].northing = AheadNorthing;

                                    if (ClosestLookAheadPt != 999999 && mindist < 100)
                                    {
                                        //ct.eleViewList[101 + j * 10 - k].lastPassAltitude = pn.altitude;
                                        ct.eleViewList[101 + j * 4 - k].altitude = ct.ptList[ClosestLookAheadPt].altitude;
                                        ct.eleViewList[101 + j * 4 - k].cutAltitude = ct.ptList[ClosestLookAheadPt].cutAltitude;


                                    }
                                    else
                                    {
                                        //ct.eleViewList[101 + j * 10 - k].lastPassAltitude = pn.altitude;
                                        ct.eleViewList[101 + j * 4 - k].altitude = -999;
                                        ct.eleViewList[101 + j * 4 - k].cutAltitude = -999;

                                    }

                                }
                            }
                        }
                        // the last pass stuff for the map
                        //find the map resolution
                        if (mappingDist < 1) mappingDist = 10;

                        // check the dist from curent pt to paint
                        int ptsBehind = 101 - (int)Math.Round(mappingDist * 1.5 + .9);

                        double paintEasting = ct.eleViewList[ptsBehind].easting;
                        double paintNorting = ct.eleViewList[ptsBehind].northing;
                        double paintHeading = ct.eleViewList[ptsBehind].heading;
                        double paintAltitude = ct.eleViewList[ptsBehind].altitude;
                        double paintCutAlt = ct.eleViewList[ptsBehind].cutAltitude;
                        double paintLastPass = ct.eleViewList[ptsBehind].lastPassAltitude;

                        if (paintAltitude > -998 && paintCutAlt > -998 && paintLastPass > -998)
                        {
                            // calculate the number of pts from to make calculation
                            double paintToolDist = (vehicle.toolWidth - mappingDist) / 2;

                            if (paintToolDist <= 0) paintToolDist = 0;

                            //search for all near pts
                            int ptct = ct.mapList.Count;
                            if (ptct > 5)
                            {
                                mappingDist = ct.mapList[5].drawPtWidthMap;
                                //double paintDist = (mappingDist * .75) * (mappingDist * .75);
                                for (int i = 0; i < ptct; i++)
                                {
                                    //double dist = (paintEasting - ct.mapList[i].eastingMap) * (paintEasting - ct.mapList[i].eastingMap) +
                                    //(paintNorting - ct.mapList[i].northingMap) * (paintNorting - ct.mapList[i].northingMap);
                                    double distEasting = Math.Abs(paintEasting - ct.mapList[i].eastingMap);
                                    double distNorthing = Math.Abs(paintNorting - ct.mapList[i].northingMap);
                                    if (distEasting < mappingDist * .7 && distNorthing < mappingDist * .7)
                                    //if (dist < paintDist)
                                    {
                                        // fill the lastpass value
                                        if (paintLastPass < ct.mapList[i].lastPassAltitudeMap | ct.mapList[i].lastPassAltitudeMap < -997)
                                        {
                                            if (paintLastPass < ct.mapList[i].cutAltitudeMap) ct.mapList[i].lastPassAltitudeMap = ct.mapList[i].cutAltitudeMap;
                                            else ct.mapList[i].lastPassAltitudeMap = paintLastPass;
                                        }
                                        // fill the last real pass
                                        if (ct.mapList[i].cutDeltaMap <= 0) //area to cut
                                        {
                                            if (paintLastPass <= ct.mapList[i].cutAltitudeMap) ct.mapList[i].lastPassRealAltitudeMap = ct.mapList[i].cutAltitudeMap;
                                            else
                                            {
                                                if (ct.mapList[i].lastPassRealAltitudeMap > paintLastPass | ct.mapList[i].lastPassRealAltitudeMap < -997)
                                                    ct.mapList[i].lastPassRealAltitudeMap = paintLastPass;
                                            }
                                        }
                                        else // area to fill
                                        {
                                            if (ct.mapList[i].lastPassRealAltitudeMap > paintLastPass | ct.mapList[i].lastPassRealAltitudeMap < -997)
                                            {

                                                if (paintLastPass >= ct.mapList[i].cutAltitudeMap) ct.mapList[i].lastPassRealAltitudeMap = ct.mapList[i].cutAltitudeMap;
                                                else if (paintLastPass <= ct.mapList[i].altitudeMap) ct.mapList[i].lastPassRealAltitudeMap = ct.mapList[i].altitudeMap;
                                                else ct.mapList[i].lastPassRealAltitudeMap = paintLastPass;
                                            }
                                        }
                                    }
                                }


                                if (paintToolDist > 0)
                                {
                                    for (double h = paintToolDist; h > 0; h -= mappingDist)
                                    {

                                    }
                                }
                            }
                        }

                        CalculateMinMaxZoom();
                    }
                }

                if (minDist < 90000) // original is 15, meter form the line scare, for 5 meter put 25
                {


                    //record last pass

                    ////draw last pass
                    //if (cboxLastPass.Checked)
                    //{
                    //    ct.ptList[closestPoint].lastPassAltitude = pn.altitude;
                    //    gl.LineWidth(2);
                    //    gl.Begin(OpenGL.GL_LINE_STRIP);

                    //    //the dashed accent of ground profile
                    //    gl.Color(0.40f, 0.970f, 0.400f);
                    //    for (int i = 0; i < ptCnt; i++)
                    //    {
                    //        if (ct.ptList[i].lastPassAltitude > 0)
                    //            gl.Vertex(i, (((ct.ptList[i].lastPassAltitude - centerY) * altitudeWindowGain) + centerY), 0);
                    //    }
                    //    gl.End();
                    //}

                    //calculate blade to guideline delta
                    //double temp = (double)closestPoint / (double)count2;
                    if (cboxLaserModeOnOff.Checked)
                    {

                        cutDelta = (pn.altitude - ct.zeroAltitude) * 100;

                    }
                    else
                    {
                        if (avgCutAltitude > -998)
                        {
                            //in cm
                            cutDelta = (pn.altitude - avgCutAltitude) * 100;
                        }
                    }
                }
            }
            #endregion Calculate Heigh

            #region Send to blade port
            if (cutDelta == 9999)
            {
                //Output to serial for blade control 
                mc.relayRateData[mc.cutValve] = (byte)(100);
            }
            else
            {
                if (cutDelta < -9.9) //par Pat
                {
                    mc.relayRateData[mc.cutValve] = (byte)(1);
                }
                else
                {
                    if (cutDelta > 9.9)
                    {
                        mc.relayRateData[mc.cutValve] = (byte)(199);
                    }
                    else
                    {
                        mc.relayRateData[mc.cutValve] = (byte)((cutDelta * 10) + 100);
                    }
                }        
            }
            //blade offset from arduino here
            if (!spRelay.IsOpen)
            {
                bladeOffSetSlave = 0;
            }

            bladeOffSetMaster = (int)numBladeOffset.Value;

            if (bladeOffSetSlave > 2 && bladeOffSetSlave != 100)
            {
                bladeOffSetMaster += (bladeOffSetSlave - 100);
                numBladeOffset.Value = (decimal)(bladeOffSetMaster);
            }

            mc.relayRateData[mc.bladeOffset] = (byte)(bladeOffSetMaster + 100);

            RateRelayDataOutToPort();
            #endregion

            //openGLControl_Draw routine triggered manually
            openGLControl.DoRender();



        //end of UppdateFixPosition
        }

        //all the hitch, pivot, section, trailing hitch, headings and fixes
        private void CalculatePositionHeading()
        {
            gpsHeading = Math.Atan2(pn.easting - stepFixPts[currentStepFix].easting, pn.northing - stepFixPts[currentStepFix].northing);
            if (gpsHeading < 0) gpsHeading += glm.twoPI;
            fixHeading = gpsHeading;

            //determine fix positions and heading
            //in degrees for glRotate opengl methods.
            int camStep = currentStepFix*4;
            if (camStep > (totalFixSteps - 1)) camStep = (totalFixSteps - 1);
            camHeading = Math.Atan2(pn.easting - stepFixPts[camStep].easting, pn.northing - stepFixPts[camStep].northing);
            if (camHeading < 0) camHeading += glm.twoPI;
            camHeading = glm.toDegrees(camHeading);

            //make sure there is a gyro otherwise 9999 are sent from autosteer
            if (mc.gyroHeading != 9999)
            {
                //current gyro angle in radians
                gyroRaw = (glm.toRadians((double)mc.prevGyroHeading * 0.0625));

                //Difference between the IMU heading and the GPS heading
                gyroDelta = (gyroRaw + gyroCorrection) - gpsHeading;
                if (gyroDelta < 0) gyroDelta += glm.twoPI;

                //calculate delta based on circular data problem 0 to 360 to 0, clamp to +- 2 Pi
                if (gyroDelta >= -glm.PIBy2 && gyroDelta <= glm.PIBy2) gyroDelta *= -1.0;
                else
                {
                    if (gyroDelta > glm.PIBy2) { gyroDelta = glm.twoPI - gyroDelta; }
                    else { gyroDelta = (glm.twoPI + gyroDelta) * -1.0; }
                }
                if (gyroDelta > glm.twoPI) gyroDelta -= glm.twoPI;
                if (gyroDelta < -glm.twoPI) gyroDelta += glm.twoPI;

                //calculate current turn rate of vehicle
                prevPrevGPSHeading = prevGPSHeading;
                prevGPSHeading = gpsHeading;
                turnDelta = Math.Abs(Math.Atan2(Math.Sin(fixHeading - prevPrevGPSHeading), Math.Cos(fixHeading - prevPrevGPSHeading)));

                //Only adjust gyro if going in a straight line 
                if (turnDelta < 0.01 && pn.speed > 1)
                {
                    //a bit of delta and add to correction to current gyro
                    gyroCorrection += (gyroDelta * (0.4 / fixUpdateHz));
                    if (gyroCorrection > glm.twoPI) gyroCorrection -= glm.twoPI;
                    if (gyroCorrection < -glm.twoPI) gyroCorrection += glm.twoPI;
                    gyroRaw = (glm.toRadians((double)mc.gyroHeading * 0.0625));
                }

                //if the gyro and GPS delta are > 10 degrees speed up filter
                if (Math.Abs(gyroDelta) > 0.18)
                {
                    //a bit of delta and add to correction to current gyro
                    gyroCorrection += (gyroDelta * (2.0 / fixUpdateHz));
                    if (gyroCorrection > glm.twoPI) gyroCorrection -= glm.twoPI;
                    if (gyroCorrection < -glm.twoPI) gyroCorrection += glm.twoPI;
                    gyroRaw = (glm.toRadians((double)mc.gyroHeading * 0.0625));
                }
                //determine the Corrected heading based on gyro and GPS
                gyroCorrected = gyroRaw + gyroCorrection;
                if (gyroCorrected > glm.twoPI) gyroCorrected -= glm.twoPI;
                if (gyroCorrected < 0) gyroCorrected += glm.twoPI;

                fixHeading = gyroCorrected;
            }

            //check to make sure the grid is big enough
            worldGrid.checkZoomWorldGrid(pn.northing, pn.easting);
        }

        //add the points for section, contour line points, Area Calc feature
        private void AddSectionContourPathPoints()
        {
            if (isJobStarted)//add the pathpoint
            {
                
                //Contour Base Track....
                if (manualBtnState == btnStates.Rec)
                {
                    //keep the line going, everything is on for recording path
                    if (ct.isContourOn) ct.AddPoint();
                    else { ct.StartContourLine(); ct.AddPoint(); }
                }

                //All sections OFF so if on, turn off
                else { if (ct.isContourOn) { ct.StopContourLine(); }  }
            }
        }
       
        //the start of first few frames to initialize entire program
        private void InitializeFirstFewGPSPositions()
        {
            if (!isFirstFixPositionSet)
            {
                //reduce the huge utm coordinates
                //pn.utmEast = (int)(pn.easting);
                //pn.utmNorth = (int)(pn.northing);
                //pn.easting = pn.easting - pn.utmEast;
                //pn.northing = pn.northing - pn.utmNorth;
                pn.latStart = pn.latitude;
                pn.lonStart = pn.longitude;

                
                pn.SetLocalMetersPerDegree();

                


                //Draw a grid once we know where in the world we are.
                isFirstFixPositionSet = true;
                worldGrid.CreateWorldGrid( 0, 0);

                //most recent fixes
                prevFix.easting = pn.easting;
                prevFix.northing = pn.northing;

                stepFixPts[0].easting = pn.easting;
                stepFixPts[0].northing = pn.northing;
                stepFixPts[0].heading = 0;

                //preset the zero height button
                //ct.zeroAltitude = pn.altitude;

                //run once and return
                isFirstFixPositionSet = true;
                return;
            }

            else
            {
 
                //most recent fixes
                prevFix.easting = pn.easting; prevFix.northing = pn.northing;

                //load up history with valid data
                for (int i = totalFixSteps - 1; i > 0; i--)
                {
                    stepFixPts[i].easting = stepFixPts[i - 1].easting;
                    stepFixPts[i].northing = stepFixPts[i - 1].northing;
                    stepFixPts[i].heading = stepFixPts[i - 1].heading;
                }

                stepFixPts[0].heading = pn.Distance(pn.northing, pn.easting, stepFixPts[0].northing, stepFixPts[0].easting);
                stepFixPts[0].easting = pn.easting;
                stepFixPts[0].northing = pn.northing;

                //keep here till valid data
                if (startCounter > (totalFixSteps/2)) isGPSPositionInitialized = true;

                //in radians
                fixHeading = Math.Atan2(pn.easting - stepFixPts[totalFixSteps - 1].easting, pn.northing - stepFixPts[totalFixSteps - 1].northing); 
                if (fixHeading < 0) fixHeading += glm.twoPI;

                //send out initial zero settings
                if (isGPSPositionInitialized) AutoSteerSettingsOutToPort();

                return;
            }
        }

        

        



    }//end class
}//end namespace