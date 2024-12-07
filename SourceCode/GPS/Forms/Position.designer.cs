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
        
        //called by watchdog timer every 50 ms
        private void ScanForNMEA()
        {
            //parse any data from pn.rawBuffer
            pn.ParseNMEA();

            //time for a frame update with new valid nmea data
            if (pn.updatedGGA)
            {
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
                worldGrid.CreateWorldGrid(pn.northing, pn.easting);

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