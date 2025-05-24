
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpGL;

namespace OpenGrade
{
    public partial class FormGPS
    {
        //extracted Near, Far, Right, Left clipping planes of frustum
        public double[] frustum = new double[24];

        //difference between blade tip and guide line
        public double cutDelta;
        public int bladeOffSetMaster = 0;// in cm
        public int bladeOffSetSlave = 0; // in cm
        private double minDist;
        private double minDistMap;
        private double mappingDist;
        public double SurveyPtDist = 300;
        public double SurveyPtDistSqr = 90000;
        public double NoLineSurveyPt = 10000;

        //All the stuff for the height averaging
        private double cutDeltalMap = 9999;
        private double distanceFromAline;
        private double distanceFromBline;
        private double distanceFromCline;
        private double distanceFromDline;

        private double eastingApt;
        private double northingApt;
        private double altitudeApt;
        private double cutAltApt;

        private double eastingBpt;
        private double northingBpt;
        private double altitudeBpt;
        private double cutAltBpt;

        private double eastingCpt;
        private double northingCpt;
        private double altitudeCpt;
        private double cutAltCpt;

        private double eastingDpt;
        private double northingDpt;
        private double altitudeDpt;
        private double cutAltDpt;
        //------------------------------------------------------
        public bool stopTheProgram, ProgramIsStoped, CalculatingMinMaxEastNort;

        public bool averagePts = Properties.Settings.Default.Set_isAvgPt; // average four near design pts or not
        public double noAvgDist = Properties.Settings.Default.Set_noAvgDist; // distance from a point that will not be averaged
        public double levelDistFactor = Properties.Settings.Default.Set_levelDistFactor; //A factor to set the influance of a design pt according his dist from the blade

        private double minDistMapDist = 400; // how far from a survey point it will draw the map 400 is 20 meters
        private double drawPtWidth = 1; // the size of the map pixel in meter

        //the point in the real world made from clicked screen coords
        vec2 screen2FieldPt = new vec2();

        double fovy = 45;
        double camDistanceFactor = -2;
        int mouseX = 0, mouseY = 0;

        //data buffer for pixels read from off screen buffer
        byte[] grnPixels = new byte[80001];

        /// Handles the OpenGLDraw event of the openGLControl control.
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            if (isGPSPositionInitialized)
            {

                //  Get the OpenGL object.
                OpenGL gl = openGLControl.OpenGL;
                //System.Threading.Thread.Sleep(500);

                //  Clear the color and depth buffer.
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
                gl.LoadIdentity();

                //camera does translations and rotations
                camera.SetWorldCam(gl, pn.easting, pn.northing, camHeading);

                //draw the field ground images
                worldGrid.DrawFieldSurface();

                ////Draw the world grid based on camera position
                gl.Disable(OpenGL.GL_DEPTH_TEST);
                gl.Disable(OpenGL.GL_TEXTURE_2D);


                gl.Enable(OpenGL.GL_LINE_SMOOTH);
                gl.Enable(OpenGL.GL_BLEND);

                gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_FASTEST);
                gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_FASTEST);
                gl.Hint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_FASTEST);

                ////if grid is on draw it
                if (isGridOn) worldGrid.DrawWorldGrid(gridZoom);

                //turn on blend for paths
                gl.Enable(OpenGL.GL_BLEND);

                //section patch color
                gl.Color(redSections, grnSections, bluSections, (byte)160);
                if (isDrawPolygons) gl.PolygonMode(OpenGL.GL_FRONT, OpenGL.GL_LINE);

                gl.PolygonMode(OpenGL.GL_FRONT, OpenGL.GL_FILL);
                gl.Color(1, 1, 1);

                //draw contour line if button on 
                //if (ct.isContourBtnOn)

                ct.DrawContourLine();




                //draw the flags if there are some
                int flagCnt = flagPts.Count;
                if (flagCnt > 0)
                {
                    for (int f = 0; f < flagCnt; f++)
                    {
                        gl.PointSize(8.0f);
                        gl.Begin(OpenGL.GL_POINTS);
                        if (flagPts[f].color == 0) gl.Color((byte)255, (byte)0, (byte)flagPts[f].ID);
                        if (flagPts[f].color == 1) gl.Color((byte)0, (byte)255, (byte)flagPts[f].ID);
                        if (flagPts[f].color == 2) gl.Color((byte)255, (byte)255, (byte)flagPts[f].ID);
                        gl.Vertex(flagPts[f].easting, flagPts[f].northing, 0);
                        gl.End();
                    }

                    if (flagNumberPicked != 0)
                    {
                        ////draw the box around flag
                        gl.LineWidth(4);
                        gl.Color(0.980f, 0.0f, 0.980f);
                        gl.Begin(OpenGL.GL_LINE_STRIP);

                        double offSet = (zoomValue * zoomValue * 0.01);
                        gl.Vertex(flagPts[flagNumberPicked - 1].easting, flagPts[flagNumberPicked - 1].northing + offSet, 0);
                        gl.Vertex(flagPts[flagNumberPicked - 1].easting - offSet, flagPts[flagNumberPicked - 1].northing, 0);
                        gl.Vertex(flagPts[flagNumberPicked - 1].easting, flagPts[flagNumberPicked - 1].northing - offSet, 0);
                        gl.Vertex(flagPts[flagNumberPicked - 1].easting + offSet, flagPts[flagNumberPicked - 1].northing, 0);
                        gl.Vertex(flagPts[flagNumberPicked - 1].easting, flagPts[flagNumberPicked - 1].northing + offSet, 0);

                        gl.End();

                        //draw the flag with a black dot inside
                        gl.PointSize(4.0f);
                        gl.Color(0, 0, 0);
                        gl.Begin(OpenGL.GL_POINTS);
                        gl.Vertex(flagPts[flagNumberPicked - 1].easting, flagPts[flagNumberPicked - 1].northing, 0);
                        gl.End();
                    }
                }

                //screen text for debug
                //gl.DrawText(120, 10, 1, 1, 1, "Courier Bold", 18, "Head: " + saveCounter.ToString("N1"));
                //gl.DrawText(120, 40, 1, 1, 1, "Courier Bold", 18, "Tool: " + distTool.ToString("N1"));
                //gl.DrawText(120, 70, 1, 1, 1, "Courier Bold", 18, "Where: " + yt.whereAmI.ToString());
                //gl.DrawText(120, 100, 1, 1, 1, "Courier Bold", 18, "Seq: " + yt.isSequenceTriggered.ToString());
                //gl.DrawText(120, 40, 1, 1, 1, "Courier Bold", 18, "  GPS: " + Convert.ToString(Math.Round(glm.toDegrees(gpsHeading), 2)));
                //gl.DrawText(120, 70, 1, 1, 1, "Courier Bold", 18, "Fixed: " + Convert.ToString(Math.Round(glm.toDegrees(gyroCorrected), 2)));
                //gl.DrawText(120, 100, 1, 1, 1, "Courier Bold", 18, "L/Min: " + Convert.ToString(rc.CalculateRateLitersPerMinute()));
                //gl.DrawText(120, 130, 1, 1, 1, "Courier", 18, "       Roll: " + Convert.ToString(glm.toDegrees(rollDistance)));
                //gl.DrawText(120, 160, 1, 1, 1, "Courier", 18, "       Turn: " + Convert.ToString(Math.Round(turnDelta, 4)));
                //gl.DrawText(40, 120, 1, 0.5, 1, "Courier", 12, " frame msec " + Convert.ToString((int)(frameTime)));

                //draw the vehicle/implement
                vehicle.DrawVehicle();

                //Back to normal
                gl.Color(0.98f, 0.98f, 0.98f);
                gl.Disable(OpenGL.GL_BLEND);
                gl.Enable(OpenGL.GL_DEPTH_TEST);

                //// 2D Ortho --------------------------
                gl.MatrixMode(OpenGL.GL_PROJECTION);
                gl.PushMatrix();
                gl.LoadIdentity();

                //negative and positive on width, 0 at top to bottom ortho view
                gl.Ortho2D(-(double)Width / 2, (double)Width / 2, (double)Height, 0);

                //  Create the appropriate modelview matrix.
                gl.MatrixMode(OpenGL.GL_MODELVIEW);
                gl.PushMatrix();
                gl.LoadIdentity();

                if (isSkyOn)
                {
                    ////draw the background when in 3D
                    if (camera.camPitch < -60)
                    {
                        //-10 to -32 (top) is camera pitch range. Set skybox to line up with horizon 
                        double hite = (camera.camPitch + 60) / -20 * 0.34;
                        //hite = 0.001;

                        //the background
                        double winLeftPos = -(double)Width / 2;
                        double winRightPos = -winLeftPos;
                        gl.Enable(OpenGL.GL_TEXTURE_2D);
                        gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture[0]);		// Select Our Texture

                        gl.Begin(OpenGL.GL_TRIANGLE_STRIP);				// Build Quad From A Triangle Strip
                        gl.TexCoord(0, 0); gl.Vertex(winRightPos, 0.0); // Top Right
                        gl.TexCoord(1, 0); gl.Vertex(winLeftPos, 0.0); // Top Left
                        gl.TexCoord(0, 1); gl.Vertex(winRightPos, hite * (double)Height); // Bottom Right
                        gl.TexCoord(1, 1); gl.Vertex(winLeftPos, hite * (double)Height); // Bottom Left
                        gl.End();						// Done Building Triangle Strip

                        //disable, straight color
                        gl.Disable(OpenGL.GL_TEXTURE_2D);
                    }
                }

                /*
                //LightBar if AB Line is set and turned on or contour
                if (isLightbarOn)
                {
                    if (ct.isContourBtnOn)
                    {
                        string dist;
                        txtDistanceOffABLine.Visible = true;
                        //lblDelta.Visible = true;
                        if (ct.distanceFromCurrentLine == 32000) ct.distanceFromCurrentLine = 0;

                        DrawLightBar(openGLControl.Width, openGLControl.Height, ct.distanceFromCurrentLine * 0.1);
                        if ((ct.distanceFromCurrentLine) < 0.0)
                        {
                            txtDistanceOffABLine.ForeColor = Color.Green;
                            if (isMetric) dist = ((int)Math.Abs(ct.distanceFromCurrentLine * 0.1)) + " ->";
                            else dist = ((int)Math.Abs(ct.distanceFromCurrentLine / 2.54 * 0.1)) + " ->";
                            txtDistanceOffABLine.Text = dist;
                        }

                        else
                        {
                            txtDistanceOffABLine.ForeColor = Color.Red;
                            if (isMetric) dist = "<- " + ((int)Math.Abs(ct.distanceFromCurrentLine * 0.1));
                            else dist = "<- " + ((int)Math.Abs(ct.distanceFromCurrentLine / 2.54 * 0.1));
                            txtDistanceOffABLine.Text = dist;
                        }

                        //if (guidanceLineHeadingDelta < 0) lblDelta.ForeColor = Color.Red;
                        //else lblDelta.ForeColor = Color.Green;

                        //if (guidanceLineDistanceOff == 32020 | guidanceLineDistanceOff == 32000) btnAutoSteer.Text = "-";
                        //else btnAutoSteer.Text = "Y";
                    }

                    else
                    {
                        if (ABLine.isABLineSet | ABLine.isABLineBeingSet)
                        {
                            string dist;

                            txtDistanceOffABLine.Visible = true;
                            //lblDelta.Visible = true;
                            DrawLightBar(openGLControl.Width, openGLControl.Height, ABLine.distanceFromCurrentLine * 0.1);
                            if ((ABLine.distanceFromCurrentLine) < 0.0)
                            {
                                // --->
                                txtDistanceOffABLine.ForeColor = Color.Green;
                                if (isMetric) dist = ((int)Math.Abs(ABLine.distanceFromCurrentLine * 0.1)) + " ->";
                                else dist = ((int)Math.Abs(ABLine.distanceFromCurrentLine / 2.54 * 0.1)) + " ->";
                                txtDistanceOffABLine.Text = dist;
                            }

                            else
                            {
                                // <----
                                txtDistanceOffABLine.ForeColor = Color.Red;
                                if (isMetric) dist = "<- " + ((int)Math.Abs(ABLine.distanceFromCurrentLine * 0.1));
                                else dist = "<- " + ((int)Math.Abs(ABLine.distanceFromCurrentLine / 2.54 * 0.1));
                                txtDistanceOffABLine.Text = dist;
                            }

                            //if (guidanceLineHeadingDelta < 0) lblDelta.ForeColor = Color.Red;
                            //else lblDelta.ForeColor = Color.Green;
                            //if (guidanceLineDistanceOff == 32020 | guidanceLineDistanceOff == 32000) btnAutoSteer.Text = "-";
                            //else btnAutoSteer.Text = "Y";
                        }
                    }

                    //AB line is not set so turn off numbers
                    if (!ABLine.isABLineSet & !ABLine.isABLineBeingSet & !ct.isContourBtnOn)
                    {
                        txtDistanceOffABLine.Visible = false;
                        //btnAutoSteer.Text = "-";
                    }
                }
                else
                {
                    txtDistanceOffABLine.Visible = false;
                    //btnAutoSteer.Text = "-";
                }
                */

                gl.Flush();//finish openGL commands
                gl.PopMatrix();//  Pop the modelview.

                //  back to the projection and pop it, then back to the model view.
                gl.MatrixMode(OpenGL.GL_PROJECTION);
                gl.PopMatrix();
                gl.MatrixMode(OpenGL.GL_MODELVIEW);

                //reset point size
                gl.PointSize(1.0f);
                gl.Flush();

                if (leftMouseDownOnOpenGL)
                {
                    leftMouseDownOnOpenGL = false;
                    byte[] data1 = new byte[192];

                    //scan the center of click and a set of square points around
                    gl.ReadPixels(mouseX - 4, mouseY - 4, 8, 8, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, data1);

                    //made it here so no flag found
                    flagNumberPicked = 0;

                    for (int ctr = 0; ctr < 192; ctr += 3)
                    {
                        if (data1[ctr] == 255 | data1[ctr + 1] == 255)
                        {
                            flagNumberPicked = data1[ctr + 2];
                            break;
                        }
                    }
                }


                //digital input Master control (WorkSwitch)
                if (isJobStarted && mc.isWorkSwitchEnabled)
                {
                    //check condition of work switch
                    if (mc.isWorkSwitchActiveLow)
                    {
                        //if (mc.workSwitchValue == 0)
                    }
                    else
                    {
                        //if (mc.workSwitchValue == 1)
                    }
                }

                //stop the timer and calc how long it took to do calcs and draw
                frameTime = (double)swFrame.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency * 1000;

                //if a couple minute has elapsed save the field in case of crash and to be able to resume            
                if (saveCounter > 600)       //10 counts per second X 60 seconds = 600 counts per minute.
                {
                    // no auto save for now
                    
                    if (isJobStarted && stripOnlineGPS.Value != 1)
                    {
                        //auto save the field patches, contours accumulated so far
                        //FileSaveField();
                        //FileSaveMapPt();
                        //FileSaveContour();
                        

                        //NMEA log file
                        if (isLogNMEA) FileSaveNMEA();
                    }
                    if (isJobStarted && ct.isSurveyOn && ct.surveyList.Count > 0)
                    {
                        FileSaveSurveyPt2text();
                    }
                    saveCounter = 0;
                }

                openGLControlBack.DoRender();
            }
        }

        /// Handles the OpenGLInitialized event of the openGLControl control.
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //Load all the textures
            LoadGLTextures();

            //  Set the clear color.
            gl.ClearColor(0.22f, 0.2858f, 0.16f, 1.0f);

            // Set The Blending Function For Translucency
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            //gl.Disable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);

            //set the camera to right distance
            SetZoom();

            //now start the timer assuming no errors, otherwise the program will not stop on errors.
            tmrWatchdog.Enabled = true;
        }

        /// Handles the Resized event of the openGLControl control.
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(fovy, (double)openGLControl.Width / (double)openGLControl.Height, 1, camDistanceFactor * camera.camSetDistance);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        //main openGL draw function
        private void openGLControlBack_OpenGLDraw(object sender, RenderEventArgs args)
        {
            OpenGL gl = openGLControlBack.OpenGL;

            //antialiasing - fastest
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);  // Clear The Screen And The Depth Buffer

            gl.Enable(OpenGL.GL_LINE_SMOOTH);
            //gl.Enable(OpenGL.GL_BLEND);

            gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_FASTEST);
            gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_FASTEST);
            gl.Hint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_FASTEST);

            gl.LoadIdentity();                  // Reset The View

            // Fill the eleViewList



            //if adding new points recalc mins maxes
            //if (manualBtnState == btnStates.Rec) CalculateMinMaxZoom();

            //autogain the window
            if ((maxFieldY - minFieldY) != 0)
                altitudeWindowGain = (Math.Abs(cameraDistanceZ / (maxFieldY - minFieldY))) * 0.80;
            else altitudeWindowGain = 900;

            if (altitudeWindowGain > 900) altitudeWindowGain = 900;

            //translate to that spot in the world 
            gl.Translate(0, 0, -cameraDistanceZ);
            gl.Translate(-centerX, -centerY, 0);

            gl.Color(1, 1, 1);

            //reset cut delta for frame
            cutDelta = 9999;

            int closestPoint = 0;
            int ptCnt = ct.ptList.Count;
            gl.LineWidth(4);


            if (cboxLaserModeOnOff.Checked)
            {

                cutDelta = (pn.altitude - ct.zeroAltitude) * 100;

            }


            if (ptCnt > 0)
            {
# region  find cut altitude for actual position
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

                #endregion  find cut altitude for actual position

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

                // Change to eleViewList
                //draw the ground profile

                int elePtCount = ct.eleViewList.Count;
                if (elePtCount > 101)
                {


                    gl.Color(0.32f, 0.32f, 0.32f);
                    gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
                    for (int i = 0; i < elePtCount; i++)
                    {
                        gl.Vertex(i,
                          (((ct.eleViewList[i].altitude - centerY) * altitudeWindowGain) + centerY), 0);
                        gl.Vertex(i, -10000, 0);
                    }
                    gl.End();

                    //cut line drawn in full
                    //int cutPts = ct.mapList.Count;
                    //if (cutPts > 0)
                    //{




                    gl.Color(0.974f, 0.0f, 0.12f);
                    gl.Begin(OpenGL.GL_LINE_STRIP);
                    for (int i = 0; i < elePtCount; i++)
                    {
                        if (ct.eleViewList[i].cutAltitude > -998)
                            gl.Vertex(i, (((ct.eleViewList[i].cutAltitude - centerY) * altitudeWindowGain) + centerY), 0);
                        else
                        {
                            gl.End();
                            gl.Begin(OpenGL.GL_LINE_STRIP);
                        }
                    }
                    gl.End();
                    //}

                    //crosshairs same spot as mouse - long
                    gl.LineWidth(2);
                    gl.Enable(OpenGL.GL_LINE_STIPPLE);
                    gl.LineStipple(1, 0x0300);

                    gl.Begin(OpenGL.GL_LINES);
                    gl.Color(0.90f, 0.90f, 0.70f);
                    gl.Vertex(screen2FieldPt.easting, 3000, 0);
                    gl.Vertex(screen2FieldPt.easting, -3000, 0);
                    gl.Vertex(-10, (((screen2FieldPt.northing - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(1000, (((screen2FieldPt.northing - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.End();
                    gl.Disable(OpenGL.GL_LINE_STIPPLE);

                    //draw last pass if rec on
                    //if (cboxRecLastOnOff.Checked & ct.ptList[closestPoint].cutAltitude > 0)
                    //ct.ptList[closestPoint].lastPassAltitude = pn.altitude;

                    //draw if on
                    //if (cboxLastPass.Checked)
                    //{
                    gl.LineWidth(2);
                    gl.Begin(OpenGL.GL_LINE_STRIP);

                    gl.Color(0.40f, 0.970f, 0.400f);
                    for (int i = 0; i < elePtCount; i++)
                    {
                        if (ct.eleViewList[i].lastPassAltitude > -998)
                            gl.Vertex(i, (((ct.eleViewList[i].lastPassAltitude - centerY) * altitudeWindowGain) + centerY), 0);
                    }
                    gl.End();
                    //}
                    //draw the actual elevation lines and blade
                    gl.LineWidth(8);
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Color(0.95f, 0.90f, 0.0f);
                    gl.Vertex(101, (((pn.altitude - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(101, 10000, 0);
                    gl.End();

                    //the skinny actual elevation lines
                    gl.LineWidth(1);
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Color(0.57f, 0.80f, 0.00f);
                    gl.Vertex(-5, (((pn.altitude - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(305, (((pn.altitude - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(101, -10000, 0);
                    gl.Vertex(101, 10000, 0);
                    gl.End();

                    //draw a skinny line 5cm above blade
                    gl.LineWidth(1);
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Color(0.57f, 0.80f, 0.00f);
                    gl.Vertex(94, (((ct.eleViewList[101].cutAltitude + .05 - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(107, (((ct.eleViewList[101].cutAltitude + .05 - centerY) * altitudeWindowGain) + centerY), 0);
                    //draw a skinny line 10cm above blade
                    gl.Vertex(94, (((ct.eleViewList[101].cutAltitude + .1 - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(107, (((ct.eleViewList[101].cutAltitude + .1 - centerY) * altitudeWindowGain) + centerY), 0);
                    //draw a skinny line 5cm under blade                   
                    gl.Vertex(94, (((ct.eleViewList[101].cutAltitude - .05 - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(107, (((ct.eleViewList[101].cutAltitude - .05 - centerY) * altitudeWindowGain) + centerY), 0);
                    //draw a skinny line 10cm under blade
                    gl.Vertex(94, (((ct.eleViewList[101].cutAltitude - .1 - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.Vertex(107, (((ct.eleViewList[101].cutAltitude - .1 - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.End();

                    //little point at cutting edge of blade
                    gl.Color(0.0f, 0.0f, 0.0f);
                    gl.PointSize(8);
                    gl.Begin(OpenGL.GL_POINTS);
                    gl.Vertex(101, (((pn.altitude - centerY) * altitudeWindowGain) + centerY), 0);
                    gl.End();

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
        }

        private void openGLControlBack_MouseMove(object sender, MouseEventArgs e)
        {
            Point screenPt = new Point();
            screenPt.X = e.Location.X;
            screenPt.Y = ((openGLControlBack.Height - e.Location.Y) - openGLControlBack.Height / 2);

            //convert screen coordinates to field coordinates
            screen2FieldPt.easting = ((double)screenPt.X) * (double)cameraDistanceZ / openGLControlBack.Width;
            screen2FieldPt.northing = ((double)screenPt.Y) * (double)cameraDistanceZ / (openGLControlBack.Height * altitudeWindowGain);
            screen2FieldPt.northing += centerY;

            if (maxFieldX == 0)
            {
                stripTopoLocation.Text = " 0 " + ": " + " 0.000" + ": " + " 0.0";
            }
            else
            {
                if (ct.eleViewList.Count > 0)
                {
                    if (isMetric) stripTopoLocation.Text = ((int)(screen2FieldPt.easting)).ToString() + ": " + screen2FieldPt.northing.ToString("N3") + ": " + ((screen2FieldPt.northing - ct.eleViewList[(int)(screen2FieldPt.easting)].altitude) * 100).ToString("N1");
                    else stripTopoLocation.Text = ((int)(screen2FieldPt.easting)).ToString() + ": " + ((screen2FieldPt.northing) / .0254 / 12).ToString("N3") + ": " + ((screen2FieldPt.northing - ct.eleViewList[(int)(screen2FieldPt.easting)].altitude) / .0254).ToString("N1");
                }
                else stripTopoLocation.Text = " 0 " + ": " + " 0.000" + ": " + " 0.0";
            }


        }

        private void openGLControlBack_MouseClick(object sender, MouseEventArgs e)
        {
            Point fixPt = new Point();
            vec2 plotPt = new vec2();

        }

        #region 3D mapping

        public double eastingMin, eastingMax, northingMin, northingMax;

        // determine ptList min and max easting and northing and build the maplist -by Pat
        public void CalculateMinMaxEastNort()
        {
            stopTheProgram = true;

            //if (ProgramIsStoped)
            {



                eastingMin = 9999999;
                eastingMax = -9999999;
                northingMin = 9999999;
                northingMax = -9999999;

                drawPtWidth = Properties.Vehicle.Default.setVehicle_GradeDistFromLine; //badly named, its used for the display map resolution
                if (drawPtWidth < 1) drawPtWidth = 1;

                int cnt = ct.ptList.Count;

                if (cnt > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        //double x = i;
                        double y = ct.ptList[i].easting;
                        double z = ct.ptList[i].northing;

                        //find min max coordonates
                        if (eastingMin > y) eastingMin = y;
                        if (eastingMax < y) eastingMax = y;
                        if (northingMin > z) northingMin = z;
                        if (northingMax < z) northingMax = z;
                    }
                }
                SurveyPtDist = levelDistFactor;
                SurveyPtDistSqr = SurveyPtDist * SurveyPtDist;
                double minDistMapDist2 = Math.Sqrt(minDistMapDist);

                if (eastingMax == -9999999 | eastingMin == 9999999 | northingMax == -9999999 | northingMin == 9999999)
                {
                    eastingMin = 0; eastingMax = 0; northingMax = 0; northingMin = 0;
                }
                else
                {
                    eastingMin -= minDistMapDist2; eastingMax += minDistMapDist2; northingMax += minDistMapDist2; northingMin -= minDistMapDist2;
                }

                int ptCnt = ct.ptList.Count;

                if (ptCnt > 0)
                {
                    int ptCount = ct.ptList.Count - 1;
                   
                    int eastingMinInt = (int)eastingMin;
                    int eastingMaxInt = (int)eastingMax;
                    int northingMinInt = (int)northingMin;
                    int northingMaxInt = (int)northingMax;
                    int drawPtWidthInt = (int)drawPtWidth;
                    
                    for (int h = eastingMinInt; h < eastingMaxInt; h += drawPtWidthInt)
                    {
                        for (int i = northingMinInt; i < northingMaxInt; i += drawPtWidthInt)
                        {


                            //here is all the calculation to find the blade target at each fit, lot of maths!

                            minDist = 1000000; //original is 1000000               

                            //pt number of the pts
                            int closestPoint = -1;
                            int closestPointMap2 = -1;
                            int closestPointMap3 = -1;
                            int closestPointMap4 = -1;
                            int closestPointMap5 = -1;
                            int closestPointMap6 = -1;
                                                      
                            double minDist2 = SurveyPtDistSqr; // if the point is further than "pts dist for calc" we forget it
                            double minDist3 = SurveyPtDistSqr;
                            double minDist4 = SurveyPtDistSqr;
                            double minDist5 = SurveyPtDistSqr;
                            double minDist6 = SurveyPtDistSqr;

                            //find the 6 closests point to current fix
                            for (int t = 0; t < ptCount; t++)
                            {
                                double distMap = ((h - ct.ptList[t].easting) * (h - ct.ptList[t].easting))
                                                + ((i - ct.ptList[t].northing) * (i - ct.ptList[t].northing));

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

                            //if 20 meter or less from the pt then draw
                            if (minDist < minDistMapDist)
                            {
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
                                    CalcPtOnLineMap(closestPoint, closestPointMap2, h, i, out distanceFromAline, out eastingApt, out northingApt, out altitudeApt, out cutAltApt);

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
                                    CalcPtOnLineMap(closestPoint, closestPointMap3, h, i, out distanceFromBline, out eastingBpt, out northingBpt, out altitudeBpt, out cutAltBpt);

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
                                    CalcPtOnLineMap(closestPointMap2, closestPointMap4, h, i, out distanceFromCline, out eastingCpt, out northingCpt, out altitudeCpt, out cutAltCpt);

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
                                    CalcPtOnLineMap(closestPointMap3, closestPointMap4, h, i, out distanceFromDline, out eastingDpt, out northingDpt, out altitudeDpt, out cutAltDpt);

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
                             
                                // if the pt is near the closest pt or No Average is selected or there is only one survey pt
                                int nbrofPt = 4;
                                if (minDist4 == SurveyPtDistSqr) nbrofPt--;
                                if (minDist3 == SurveyPtDistSqr) nbrofPt--;
                                if (minDist2 == SurveyPtDistSqr) nbrofPt--;

                                if (nbrofPt < 2)
                                {
                                    //use only the closest design pt data witout averaging
                                    avgAltitude = ct.ptList[closestPoint].altitude;
                                    avgCutAltitude = ct.ptList[closestPoint].cutAltitude;
                                }                          
                                else // blade is somewere between 4 points (or less) AND at least 2 pts and ONE line are used
                                {
                                    double sumofCloseDist = 1 / distanceFromAline + 1 / distanceFromBline + 1 / distanceFromCline + 1 / distanceFromDline;

                                    if (altitudeApt < -997)
                                    {
                                        avgAltitude = ct.ptList[closestPoint].altitude;
                                    }
                                    else if (altitudeBpt < -997 | altitudeCpt < -997 | altitudeDpt < -997)
                                    {
                                        avgAltitude = altitudeApt;

                                    }
                                    else
                                    {
                                        avgAltitude = ((altitudeApt / distanceFromAline) + (altitudeBpt / distanceFromBline) +
                                   (altitudeCpt / distanceFromCline) + (altitudeDpt / distanceFromDline)) / sumofCloseDist;
                                    }
                                   

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
                                }



                                //end of copy

                                cutDeltalMap = 9999;
                                if (avgCutAltitude > -998 && avgAltitude > -998) cutDeltalMap = avgCutAltitude - avgAltitude;

                                mapListPt point = new mapListPt(h, i, drawPtWidth, avgAltitude, avgCutAltitude,
                                    cutDeltalMap, ct.ptList[closestPoint].lastPassAltitude);
                                ct.mapList.Add(point);
                            }
                        }
                    }
                    
                }
                FileSaveMapPt(); // For keeping the visual mapping

                stopTheProgram = false;
            }
        }

        private void CalcPtOnLine(int Pt1, int Pt2, out double distFromLine, out double eastingPt, out double northingPt, out double altitudePt, out double cutAltPt)
        {
            double dxN = ct.ptList[Pt2].easting - ct.ptList[Pt1].easting;
            double dyN = ct.ptList[Pt2].northing - ct.ptList[Pt1].northing;

            //how far from Line is fix
            distFromLine = ((dyN * pn.easting) - (dxN * pn.northing) + (ct.ptList[Pt2].easting
                        * ct.ptList[Pt1].northing) - (ct.ptList[Pt2].northing * ct.ptList[Pt1].easting))
                        / Math.Sqrt((dyN * dyN) + (dxN * dxN));

            //absolute the distance
            distFromLine = Math.Abs(distFromLine);


            //calc point onLine closest to current blade position
            double UN = (((pn.easting - ct.ptList[Pt1].easting) * dxN)
                    + ((pn.northing - ct.ptList[Pt1].northing) * dyN))
                    / ((dxN * dxN) + (dyN * dyN));

            //point on line closest to blade center
            eastingPt = ct.ptList[Pt1].easting + (UN * dxN);
            northingPt = ct.ptList[Pt1].northing + (UN * dyN);

            //calc altitude for that point
            altitudePt = ct.ptList[Pt1].altitude + (UN * (ct.ptList[Pt2].altitude - ct.ptList[Pt1].altitude));

            cutAltPt = ct.ptList[Pt1].cutAltitude + (UN * (ct.ptList[Pt2].cutAltitude - ct.ptList[Pt1].cutAltitude));

        }

        private void CalcPtOnLineMap(int Pt1, int Pt2, int h, int i, out double distFromLine, out double eastingPt, out double northingPt, out double altitudePt, out double cutAltPt)
        {
            double dxN = ct.ptList[Pt2].easting - ct.ptList[Pt1].easting;
            double dyN = ct.ptList[Pt2].northing - ct.ptList[Pt1].northing;

            //how far from Line is fix
            distFromLine = ((dyN * h) - (dxN * i) + (ct.ptList[Pt2].easting
                        * ct.ptList[Pt1].northing) - (ct.ptList[Pt2].northing * ct.ptList[Pt1].easting))
                        / Math.Sqrt((dyN * dyN) + (dxN * dxN));

            //absolute the distance
            distFromLine = Math.Abs(distFromLine);


            //calc point onLine closest to current blade position
            double UN = (((h - ct.ptList[Pt1].easting) * dxN)
                    + ((i - ct.ptList[Pt1].northing) * dyN))
                    / ((dxN * dxN) + (dyN * dyN));

            //point on line closest to blade center
            eastingPt = ct.ptList[Pt1].easting + (UN * dxN);
            northingPt = ct.ptList[Pt1].northing + (UN * dyN);

            //calc altitude for that point
            altitudePt = ct.ptList[Pt1].altitude + (UN * (ct.ptList[Pt2].altitude - ct.ptList[Pt1].altitude));

            cutAltPt = ct.ptList[Pt1].cutAltitude + (UN * (ct.ptList[Pt2].cutAltitude - ct.ptList[Pt1].cutAltitude));

        }







        #endregion

        double maxFieldX, maxFieldY, minFieldX, minFieldY, centerX, centerY, cameraDistanceZ, oldMinFieldY, oldMaxFieldY;

        //determine mins maxs of contour and altitude
        private void CalculateMinMaxZoom()
        {
            minFieldX = 0; minFieldY = 9999999;
            maxFieldX = 300; maxFieldY = -9999999;

            //every time the section turns off and on is a new patch
            int cnt = ct.eleViewList.Count;

            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    double x = ct.eleViewList[i].cutAltitude;
                    double y = ct.eleViewList[i].altitude;

                    //also tally the max/min of Cut x and z
                    //if (minFieldX > x) minFieldX = x;
                    //if (maxFieldX < x) maxFieldX = x;
                    if (y > -998)
                    {
                        if (minFieldY > y) minFieldY = y;
                    }
                    if (x > -998)
                    {
                        if (minFieldY > x) minFieldY = x;
                    }
                    
                    if (maxFieldY < y) maxFieldY = y;
                    if (maxFieldY < x) maxFieldY = x;

                }  
                
                
            }
            // stabilise window when small alt changes
            double maxFieldYdiff = Math.Abs(maxFieldY - oldMaxFieldY);
            double minFieldYdiff = Math.Abs(minFieldY - oldMinFieldY);

            oldMaxFieldY = maxFieldY;
            oldMinFieldY = minFieldY;

            if (maxFieldY == -9999999 | minFieldY == 9999999)
            {
                maxFieldX = 0; minFieldX = 0; maxFieldY = 0; minFieldY = 0;
                cameraDistanceZ = 10;
            }
            else if (maxFieldYdiff > .02 | minFieldYdiff > .02)
            {
                //Max horizontal
                cameraDistanceZ = Math.Abs(minFieldX - maxFieldX);

                if (cameraDistanceZ < 10) cameraDistanceZ = 10;
                if (cameraDistanceZ > 6000) cameraDistanceZ = 6000;

                maxFieldY = (maxFieldY + .01 + vehicle.viewDistAboveGnd);
                minFieldY = (minFieldY - .01 - vehicle.viewDistUnderGnd);


                centerX = (maxFieldX + minFieldX) / 2.0;
                centerY = ((maxFieldY) + (minFieldY)) / 2.0;
                if (isMetric) stripMinMax.Text=(minFieldY + vehicle.viewDistUnderGnd).ToString("N2") + ":" + (maxFieldY - vehicle.viewDistAboveGnd).ToString("N2");
                else stripMinMax.Text = ((minFieldY + vehicle.viewDistUnderGnd)/.0254/12).ToString("N2") + ":" + ((maxFieldY - vehicle.viewDistAboveGnd)/.0254/12).ToString("N2");
            }
        }

        
       

       
        
        /*
        private void CalculateContourPointDistances()
        {
            int cnt = ct.ptList.Count;

            if (cnt > 0)
            {
                ct.ptList[0].distance = 0;
                for (int i = 0; i < cnt-1; i++)
                {
                    ct.ptList[i + 1].distance = pn.Distance(ct.ptList[i].northing, ct.ptList[i].easting,ct.ptList[i + 1].northing, ct.ptList[i + 1].easting);
                }
            }
        }
        */

        //Draw section OpenGL window, not visible
        private void openGLControlBack_OpenGLInitialized(object sender, EventArgs e)
        {
            //LoadGLTexturesBack();
            OpenGL gls = openGLControlBack.OpenGL;

            //  Set the clear color.
            gls.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            // Set The Blending Function For Translucency
            gls.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gls.Enable(OpenGL.GL_BLEND);
            //gls.Disable(OpenGL.GL_DEPTH_TEST);

            gls.Enable(OpenGL.GL_CULL_FACE);
            gls.CullFace(OpenGL.GL_BACK);
        }

       //Resize is called upn window creation
        private void openGLControlBack_Resized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gls = openGLControlBack.OpenGL;

            gls.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gls.LoadIdentity();

            // change these at your own peril!!!! Very critical
            //  Create a perspective transformation.
            gls.Perspective(53.1, 1, 1, 6000);

            //  Set the modelview matrix.
            gls.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        //done in ortho mode
        public void DrawLightBar(double Width, double Height, double offlineDistance)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            double down = 20;

            gl.LineWidth(1);
            
            //  Dot distance is representation of how far from AB Line
            int dotDistance = (int)(offlineDistance);

            if (dotDistance < -320) dotDistance = -320;
            if (dotDistance > 320) dotDistance = 320;

            if (dotDistance < -10) dotDistance -= 30;
            if (dotDistance > 10) dotDistance += 30;

            // dot background
            gl.PointSize(8.0f);
            gl.Color(0.00f, 0.0f, 0.0f);
            gl.Begin(OpenGL.GL_POINTS);
            for (int i = -10; i < 0; i++) gl.Vertex((i * 40), down);
            for (int i = 1; i < 11; i++) gl.Vertex((i * 40), down);
            gl.End();

            gl.PointSize(4.0f);

            //red left side
            gl.Color(0.9750f, 0.0f, 0.0f);
            gl.Begin(OpenGL.GL_POINTS);
            for (int i = -10; i < 0; i++) gl.Vertex((i * 40), down);

            //green right side
            gl.Color(0.0f, 0.9750f, 0.0f);
            for (int i = 1; i < 11; i++) gl.Vertex((i * 40), down);
            gl.End();

                //Are you on the right side of line? So its green.
                if ((offlineDistance) < 0.0)
                {
                    int dots = dotDistance * -1 / 32;

                    gl.PointSize(32.0f);
                    gl.Color(0.0f, 0.0f, 0.0f);
                    gl.Begin(OpenGL.GL_POINTS);
                    for (int i = 1; i < dots + 1; i++) gl.Vertex((i * 40), down);
                    gl.End();

                    gl.PointSize(24.0f);
                    gl.Color(0.0f, 0.980f, 0.0f);
                    gl.Begin(OpenGL.GL_POINTS);
                    for (int i = 0; i < dots; i++) gl.Vertex((i * 40 + 40), down);
                    gl.End();
                    //return;
                }

                else
                {
                    int dots = dotDistance / 32;

                    gl.PointSize(32.0f);
                    gl.Color(0.0f, 0.0f, 0.0f);
                    gl.Begin(OpenGL.GL_POINTS);
                    for (int i = 1; i < dots + 1; i++) gl.Vertex((i * -40), down);
                    gl.End();

                    gl.PointSize(24.0f);
                    gl.Color(0.980f, 0.30f, 0.0f);
                    gl.Begin(OpenGL.GL_POINTS);
                    for (int i = 0; i < dots; i++) gl.Vertex((i * -40 - 40), down);
                    gl.End();
                    //return;
                }
            
            //yellow center dot
            if (dotDistance >= -10 && dotDistance <= 10)
            {
                gl.PointSize(32.0f);                
                gl.Color(0.0f, 0.0f, 0.0f);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Vertex(0, down);
                //gl.Vertex(0, down + 50);
                gl.End();

                gl.PointSize(24.0f);
                gl.Color(0.980f, 0.98f, 0.0f);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Vertex(0, down);
                //gl.Vertex(0, down + 50);
                gl.End();
            }

            else
            {

                gl.PointSize(8.0f);
                gl.Color(0.00f, 0.0f, 0.0f);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Vertex(-0, down);
                //gl.Vertex(0, down + 30);
                //gl.Vertex(0, down + 60);
                gl.End();

                //gl.PointSize(4.0f);
                //gl.Color(0.9250f, 0.9250f, 0.250f);
                //gl.Begin(OpenGL.GL_POINTS);
                //gl.Vertex(0, down);
                //gl.Vertex(0, down + 30);
                //gl.Vertex(0, down + 60);
                //gl.End();
            }
        }
        
    }//endo of class
}
