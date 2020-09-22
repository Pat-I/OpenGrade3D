//Please, if you use this, share the improvements

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using SharpGL;
using OpenGrade.Properties;
using Microsoft.Win32;

namespace OpenGrade
{
    public partial class FormGPS
    {


        private void LoadGUI()
        {
            //set the flag mark button to red dot
            btnFlag.Image = Properties.Resources.FlagRed;

            //metric settings
            isMetric = Settings.Default.setMenu_isMetric;
            metricToolStrip.Checked = isMetric;

            if (isMetric)
            {
                lblSpeedUnits.Text = "kmh";
                metricToolStrip.Checked = true;
                imperialToolStrip.Checked = false;
            }
            else
            {
                lblSpeedUnits.Text = "mph";
                metricToolStrip.Checked = false;
                imperialToolStrip.Checked = true;
            }

            //load up colors
            redField = (Settings.Default.setF_FieldColorR);
            grnField = (Settings.Default.setF_FieldColorG);
            bluField = (Settings.Default.setF_FieldColorB);

            redSections = Settings.Default.setF_SectionColorR;
            grnSections = Settings.Default.setF_SectionColorG;
            bluSections = Settings.Default.setF_SectionColorB;

            redCut = Settings.Default.setF_CutColorR;
            grnCut = Settings.Default.setF_CutColorG;
            bluCut = Settings.Default.setF_CutColorB;

            redCenter = Settings.Default.setF_CenterColorR;
            grnCenter = Settings.Default.setF_CenterColorG;
            bluCenter = Settings.Default.setF_CenterColorB;

            redFill = Settings.Default.setF_FillColorR;
            grnFill = Settings.Default.setF_FillColorG;
            bluFill = Settings.Default.setF_FillColorB;

            btnColorFill.BackColor = System.Drawing.Color.FromArgb(redFill, grnFill, bluFill);
            btnColorCenter.BackColor = System.Drawing.Color.FromArgb(redCenter, grnCenter, bluCenter);
            btnColorCut.BackColor = System.Drawing.Color.FromArgb(redCut, grnCut, bluCut);

            //set up grid and lightbar
            isGridOn = Settings.Default.setMenu_isGridOn;
            gridToolStripMenuItem.Checked = isGridOn;

            //log NMEA 
            isLogNMEA = Settings.Default.setMenu_isLogNMEA;
            logNMEAMenuItem.Checked = isLogNMEA;

            isLightbarOn = Settings.Default.setMenu_isLightbarOn;
            lightbarToolStripMenuItem.Checked = isLightbarOn;

            isPureDisplayOn = Settings.Default.setMenu_isPureOn;
            pursuitLineToolStripMenuItem.Checked = isPureDisplayOn;

            isSkyOn = Settings.Default.setMenu_isSkyOn;
            skyToolStripMenu.Checked = isSkyOn;

            simulatorOnToolStripMenuItem.Checked = Settings.Default.setMenu_isSimulatorOn;
            if (simulatorOnToolStripMenuItem.Checked)
            {
                panelSimControls.Visible = true;
                timerSim.Enabled = true;
            }
            else
            {
                panelSimControls.Visible = false;
                timerSim.Enabled = false;
            }

            //btnDoneDraw.Enabled = false;
            //btnDeleteLastPoint.Enabled = false;
            //btnStartDraw.Enabled = true;
            lblBarGraphMax.Text = barGraphMax.ToString();

            // send value to bladeoffset
            numBladeOffset.Value = (decimal)Properties.Vehicle.Default.setVehicle_bladeOffset * 100;
        }

        //hide the left panel
        public void HideTabControl()
        {
            if (openGLControlBack.Visible)
            {
                openGLControl.Height = this.Height - 150;
                openGLControlBack.Visible = false;
                ct.isOpenGLControlBackVisible = false;
            }
            else
            {
                openGLControl.Height = 300;
                openGLControlBack.Visible = true;
                ct.isOpenGLControlBackVisible = true;
            }
        }

        // Select between Grade Mode and Survey Mode
        public void SelectMode()
        {
            if (ct.surveyMode)
            {
                if (ct.isSurveyOn)
                {

                }
                else
                {
                    ct.surveyMode = false;
                    stripSelectMode.Text = "Grade Mode";
                    btnManualOffOn.Visible = false;
                    btnCutFillElev.Visible = true;
                    btnPropExist.Visible = true;
                    btnFixQuality.Visible = false;
                }
            }
            else
            {
                ct.surveyMode = true;
                stripSelectMode.Text = "Survey Mode";
                btnManualOffOn.Visible = true;
                btnCutFillElev.Visible = false;
                btnPropExist.Visible = false;
                btnFixQuality.Visible = true;
            }
        }

        //Open the dialog of tabbed settings
        private void SettingsPageOpen(int page)
        {
            using (var form = new FormSettings(this, page))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK) { }
            }
        }

        public void fillCutFillLbl()
        {
            if (ct.isElevation)
            {
                lblCut.Text = "Max";
                lblFill.Text = "Min";
                if (isMetric)
                {
                    lblCutValue.Text = Convert.ToString(Math.Round(ct.maxAltitude, 2));
                    lblFillValue.Text = Convert.ToString(Math.Round(ct.minAltitude, 2));
                }
                else
                {
                    lblCutValue.Text = Convert.ToString(Math.Round(ct.maxAltitude * 3.28084, 2));
                    lblFillValue.Text = Convert.ToString(Math.Round(ct.minAltitude * 3.28084, 2));
                }
            }
            else
            {
                lblCut.Text = "CUT";
                lblFill.Text = "FILL";
                if (isMetric)
                {
                    lblCutValue.Text = Convert.ToString(Math.Round(ct.maxCut * 100, 0)) + " cm";
                    lblFillValue.Text = Convert.ToString(Math.Round(ct.maxFill * 100, 0)) + " cm";
                }
                else
                {
                    lblCutValue.Text = Convert.ToString(Math.Round(ct.maxCut * 39.37, 1)) + " in";
                    lblFillValue.Text = Convert.ToString(Math.Round(ct.maxFill * 39.37, 1)) + " in";
                }
            }

        }


        // Buttons //-----------------------------------------------------------------------

        //auto steer off and on
        private void btnAutoSteer_Click(object sender, EventArgs e)
        {
            
            if (isAutoSteerBtnOn)
            {
                isAutoSteerBtnOn = false;
                btnAutoSteer.Image = Properties.Resources.AutoSteerOff;
                
            }
            else
            {
                if (ABLine.isABLineSet | ct.isContourBtnOn)
                {
                    //isAutoSteerBtnOn = true;
                    btnAutoSteer.Image = Properties.Resources.AutoSteerOn;
                }
                else
                {
                    //var form = new FormTimedMessage(2000, (gStr.gsNoGuidanceLines), (gStr.gsTurnOnContourOrABLine));
                    //form.Show();
                }
            }
        }

        private void importAgsFile_click(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                if (ct.ptList.Count < 1)
                FileOpenAgdDesign();
                else
                {
                    var form = new FormTimedMessage(3000, "Contour.txt already exist", "Delete the Contour.txt or create a new Field");
                    form.Show();
                }
            }
            else
            {
                var form = new FormTimedMessage(3000, "No field open", "Open a field First");
                form.Show();
            }
        }



        //ABLine
        private void btnABLine_Click(object sender, EventArgs e)
        {
            //if contour is on, turn it off
            if (ct.isContourBtnOn)
            {
                ct.isContourBtnOn = !ct.isContourBtnOn;
                btnContour.Image = Properties.Resources.ContourOff;
            }

            using (var form = new FormABLine(this))
            {
                ABLine.isABLineBeingSet = true;
                txtDistanceOffABLine.Visible = true;
                var result = form.ShowDialog();

                //Comes back

                //if ABLine isn't set, turn off the YouTurn
                if (!ABLine.isABLineSet)
                {
                    ABLine.isABLineBeingSet = false;
                    txtDistanceOffABLine.Visible = false;
                    //change image to reflect on off
                    btnABLine.Image = Properties.Resources.ABLineOff;
                    ABLine.isABLineBeingSet = false;

                    if (isAutoSteerBtnOn) btnAutoSteer.PerformClick();
                }
                //ab line is made
                else
                {
                    //change image to reflect on off
                    btnABLine.Image = Properties.Resources.ABLineOn;
                    ABLine.isABLineBeingSet = false;
                }
            }
        }
        //turn on contour guidance or off
        private void btnContour_Click(object sender, EventArgs e)
        {
            ct.isContourBtnOn = !ct.isContourBtnOn;
            btnContour.Image = ct.isContourBtnOn ? Properties.Resources.ContourOn : Properties.Resources.ContourOff;

            ct.DrawContourLine();
            if (ct.isContourBtnOn) btnContour.Text = "Cut/Fill";
            else btnContour.Text = "Altitude";
                  
        }

        //zoom up close and far away
        private void btnMinMax_Click(object sender, EventArgs e)
        {
            //keep a copy to go back to previous zoom
            if (zoomValue < 56)
            {
                previousZoom = zoomValue;
                zoomValue = 60;
            }
            else
            {
                zoomValue = previousZoom;
            }
            camera.camSetDistance = zoomValue * zoomValue * -1;
            SetZoom();
        }

        //button for Manual On Off of the sections
        private void btnManualOffOn_Click(object sender, EventArgs e)
        {
            switch (manualBtnState)
            {
                case btnStates.Off:
                    manualBtnState = btnStates.StandBy;
                    btnManualOffOn.Image = null;
                    btnManualOffOn.Text = "BenchMark";
                    userDistance = 0;
                    

                    cboxLastPass.Checked = false;
                    //cboxRecLastOnOff.Checked = false;
                    cboxLaserModeOnOff.Checked = false;
                    //btnDoneDraw.Enabled = false;
                    //btnDeleteLastPoint.Enabled = false;
                    //btnStartDraw.Enabled = false;
                    ct.isSurveyOn = true;
                    ct.clearSurveyList = true;
                    ct.isBtnStartPause = false;
                    btnStartPause.Text = "START";

                    break;

                case btnStates.StandBy:
                    manualBtnState = btnStates.RecBnd;
                    btnManualOffOn.Image = null;
                    btnManualOffOn.Text = "Recording Boundary";
                    userDistance = 0;                  
                    btnStartPause.Visible = true;
                    btnBoundarySide.Visible = true;
                    ct.isBtnStartPause = false;
                    btnStartPause.Text = "START";


                    cboxLastPass.Checked = false;
                    //cboxRecLastOnOff.Checked = false;
                    cboxLaserModeOnOff.Checked = false;
                    //btnDoneDraw.Enabled = false;
                    //btnDeleteLastPoint.Enabled = false;
                    //btnStartDraw.Enabled = false;
                    ct.isSurveyOn = true;
                    ct.markBM = true;
                    


                    break;

                case btnStates.RecBnd:
                    manualBtnState = btnStates.Rec;
                    btnManualOffOn.Image = Properties.Resources.ManualOn;
                    btnManualOffOn.Text = null;
                    userDistance = 0;                  
                    btnBoundarySide.Visible = false;
                    ct.isBtnStartPause = false;
                    btnStartPause.Text = "START";

                    cboxLastPass.Checked = false;
                    //cboxRecLastOnOff.Checked = false;
                    cboxLaserModeOnOff.Checked = false;
                    //btnDoneDraw.Enabled = false;
                    //btnDeleteLastPoint.Enabled = false;
                    //btnStartDraw.Enabled = false;
                    ct.isSurveyOn = true;
                    ct.recBoundary = false;
                    ct.recSurveyPt = true;


                    break;

                case btnStates.Rec:
                    manualBtnState = btnStates.Off;
                    btnManualOffOn.Image = Properties.Resources.ManualOff;
                    btnManualOffOn.Text = null;
                    //CalculateContourPointDistances();
                    //FileSaveContour();
                    //btnDoneDraw.Enabled = false;
                    //btnDeleteLastPoint.Enabled = false;
                    //btnStartDraw.Enabled = true;
                    ct.isSurveyOn = false;
                    btnStartPause.Visible = false;
                    btnManualOffOn.Enabled = false;


                    break;
            }
        }

        private void btnStartPause_Click(object sender, EventArgs e)
        {
            if (ct.isBtnStartPause)
            {
                btnStartPause.Text = "START";
                ct.isBtnStartPause = false;
            }
            else
            {
                btnStartPause.Text = "PAUSE";
                ct.isBtnStartPause = true;
            }
        }

        private void btnBoundarySide_Click(object sender, EventArgs e)
        {
            if (ct.isBoundarySideRight)
            {
                btnBoundarySide.Text = "Boundary Left";
                ct.isBoundarySideRight = false;
            }
            else
            {
                btnBoundarySide.Text = "Boundary Right";
                ct.isBoundarySideRight = true;
            }
        }

        private void btnCutFillElev_Click(object sender, EventArgs e)
        {
            if(ct.isElevation)
            {
                ct.isElevation = false;
                ct.drawTheMap = true;
                btnCutFillElev.Text = "Cut/Fill";
                if (ct.isActualCut) btnPropExist.Text = "actual Cut";
                else if (ct.isActualFill) btnPropExist.Text = "act Cut/Fill";
                else btnPropExist.Text = "Proposed";
            }
            else
            {
                ct.isElevation = true;
                ct.drawTheMap = true;
                btnCutFillElev.Text = "Elevation";
                if (ct.isExistingElevation) btnPropExist.Text = "Existing";
                else btnPropExist.Text = "Proposed";
            }
        }

        private void btnPropExist_Click(object sender, EventArgs e)
        {
            if(ct.isElevation)
            {
                if(ct.isExistingElevation)
                {
                    ct.isExistingElevation = false;
                    btnPropExist.Text = "Proposed";
                    ct.drawTheMap = true;
                }
                else
                {
                    ct.isExistingElevation = true;
                    btnPropExist.Text = "Existing";
                    ct.drawTheMap = true;
                }
            }
            else
            {
                if (!ct.isActualCut)
                {
                    if (ct.isActualFill)
                    {
                        ct.isActualFill = false;
                        btnPropExist.Text = "Proposed";
                        ct.drawTheMap = true;
                    }
                    else
                    {
                        ct.isActualCut = true;
                        btnPropExist.Text = "actual Cut";
                        ct.drawTheMap = true;
                    }
                }
                else
                {
                    ct.isActualCut = false;
                    ct.isActualFill = true;
                    btnPropExist.Text = "act Cut/Fill";
                    ct.drawTheMap = true;
                }
            }
        }

        //The main flag marker button 
        private void btnFlag_Click(object sender, EventArgs e)
        {
            int nextflag = flagPts.Count + 1;
            CFlag flagPt = new CFlag(pn.latitude, pn.longitude, pn.easting, pn.northing, flagColor, nextflag);
            flagPts.Add(flagPt);
            FileSaveFlags();
        }

        //The zoom buttons in out
        private void btnZoomIn_MouseDown(object sender, MouseEventArgs e)
        {
            if (zoomValue <= 20) zoomValue += zoomValue * 0.1;
            else zoomValue += zoomValue * 0.05;
            camera.camSetDistance = zoomValue * zoomValue * -1;
            SetZoom();
        }
        private void btnZoomOut_MouseDown(object sender, MouseEventArgs e)
        {
            if (zoomValue <= 20)
            { if ((zoomValue -= zoomValue * 0.1) < 6.0) zoomValue = 6.0; }
            else { if ((zoomValue -= zoomValue * 0.05) < 6.0) zoomValue = 6.0; }

            camera.camSetDistance = zoomValue * zoomValue * -1;
            SetZoom();
        }

        //view tilt up down and saving in settings
        private void btnTiltUp_MouseDown(object sender, MouseEventArgs e)
        {
            camera.camPitch -= ((camera.camPitch * 0.03) - 1);
            if (camera.camPitch > 0) camera.camPitch = 0;
        }
        private void btnTiltDown_MouseDown(object sender, MouseEventArgs e)
        {
            camera.camPitch += ((camera.camPitch * 0.03) - 1);
            if (camera.camPitch < -80) camera.camPitch = -80;
        }

        private void btnSnap_Click(object sender, EventArgs e)
        {
            ABLine.SnapABLine();
        }

        //panel buttons
        private void btnSettings_Click_1(object sender, EventArgs e)
        {
            SettingsPageOpen(0);
        }
        private void btnComm_Click(object sender, EventArgs e)
        {
            SettingsCommunications();
        }
        private void btnUDPSettings_Click(object sender, EventArgs e)
        {
            SettingsUDP();
        }
        private void btnUnits_Click(object sender, EventArgs e)
        {
            isMetric = !isMetric;
            Settings.Default.setMenu_isMetric = isMetric;
            Settings.Default.Save();
            if (isMetric)
            {
                lblSpeedUnits.Text = "kmh";
                metricToolStrip.Checked = true;
                imperialToolStrip.Checked = false;
            }
            else
            {
                lblSpeedUnits.Text = "mph";
                metricToolStrip.Checked = false;
                imperialToolStrip.Checked = true;
            }
            CalculateMinMaxZoom();
            fillCutFillLbl();
        }
        private void btnGPSData_Click(object sender, EventArgs e)
        {
            Form form = new FormGPSData(this);
            form.Show();
        }
        private void btnAutoSteerConfig_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fc = Application.OpenForms["FormSteer"];

            if (fc != null)
            {
                fc.Focus();
                return;
            }

            //
            Form form = new FormSteer(this);
            form.Show();
        }
        private void btnFileExplorer_Click(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                FileSaveFlagsKML();
            }
            Process.Start(fieldsDirectory + currentFieldDirectory);
        }

        private void btnClearLastPass_Click(object sender, EventArgs e)
        {
            int cnnt = ct.ptList.Count;
            if (cnnt > 0)
            {
                for (int i = 0; i < cnnt; i++) ct.ptList[i].lastPassAltitude = -1;
            }
        }
        private void btnZeroAltitude_Click(object sender, EventArgs e)
        {
            ct.zeroAltitude = pn.altitude;
        }


        
        //progress bar "buttons" for gain
        private void pbarCutAbove_Click(object sender, EventArgs e)
        {
            barGraphMax++;
            lblBarGraphMax.Text = barGraphMax.ToString();

        }

        private void pbarCutBelow_Click(object sender, EventArgs e)
        {
            if (barGraphMax-- < 1.99) barGraphMax = 1;
            lblBarGraphMax.Text = barGraphMax.ToString();
        }

        // Menu Items ------------------------------------------------------------------

        //File drop down items
        private void loadVehicleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                var form = new FormTimedMessage(2000, gStr.gsFieldIsOpen, gStr.gsCloseFieldFirst);
                form.Show();
                return;
            }
            FileOpenVehicle();
        }
        private void saveVehicleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSaveVehicle();
        }
        private void fieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JobNewOpenResume();
        }
        private void setWorkingDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                var form = new FormTimedMessage(2000, gStr.gsFieldIsOpen, gStr.gsCloseFieldFirst);
                form.Show();
                return;
            }

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.Description = "Currently: " + Settings.Default.setF_workingDirectory;

            if (Settings.Default.setF_workingDirectory == "Default") fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else fbd.SelectedPath = Settings.Default.setF_workingDirectory;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OpenGrade", true);

                if (fbd.SelectedPath != Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                {
                    //save the user set directory in Registry
                    regKey.SetValue("Directory", fbd.SelectedPath);
                    regKey.Close();
                    Settings.Default.setF_workingDirectory = fbd.SelectedPath;
                    Settings.Default.Save();
                }
                else
                {
                    regKey.SetValue("Directory", "Default");
                    regKey.Close();
                    Settings.Default.setF_workingDirectory = "Default";
                    Settings.Default.Save();
                }

                //restart program
                MessageBox.Show(gStr.gsProgramExitAndRestart);
                Close();
            }
        }

        //Help menu drop down items
        private void aboutToolStripMenuHelpAbout_Click(object sender, EventArgs e)
        {
            using (var form = new Form_About())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK) { }
            }
        }
        private void helpToolStripMenuHelpHelp_Click(object sender, EventArgs e)
        {
            //string appPath = Assembly.GetEntryAssembly().Location;
            //string filename = Path.Combine(Path.GetDirectoryName(appPath), "help.htm");
            //Process.Start("http://OpenGrade.gh-ortner.com/doku.php");
        }

        //Options Drop down menu items
        private void resetALLToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                MessageBox.Show(gStr.gsCloseFieldFirst);
            }
            else
            {
                DialogResult result2 = MessageBox.Show("Really Reset Everything?", "Reset settings",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result2 == DialogResult.Yes)
                {
                    Settings.Default.Reset();
                    Settings.Default.Save();
                    Vehicle.Default.Reset();
                    Vehicle.Default.Save();
                    MessageBox.Show(gStr.gsProgramExitAndRestart);
                    Application.Exit();
                }
            }
        }
        private void logNMEAMenuItem_Click(object sender, EventArgs e)
        {
            isLogNMEA = !isLogNMEA;
            logNMEAMenuItem.Checked = isLogNMEA;
            Settings.Default.setMenu_isLogNMEA = isLogNMEA;
            Settings.Default.Save();
        }
        private void lightbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isLightbarOn = !isLightbarOn;
            lightbarToolStripMenuItem.Checked = isLightbarOn;
            Settings.Default.setMenu_isLightbarOn = isLightbarOn;
            Settings.Default.Save();
        }
        private void polygonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDrawPolygons = !isDrawPolygons;
            polygonsToolStripMenuItem.Checked = !polygonsToolStripMenuItem.Checked;
        }
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isGridOn = !isGridOn;
            gridToolStripMenuItem.Checked = isGridOn;
            Settings.Default.setMenu_isGridOn = isGridOn;
            Settings.Default.Save();
        }
        private void pursuitLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isPureDisplayOn = !isPureDisplayOn;
            pursuitLineToolStripMenuItem.Checked = isPureDisplayOn;
            Settings.Default.setMenu_isPureOn = isPureDisplayOn;
            Settings.Default.Save();
        }
        private void metricToolStrip_Click(object sender, EventArgs e)
        {
            metricToolStrip.Checked = true;
            imperialToolStrip.Checked = false;
            isMetric = true;
            Settings.Default.setMenu_isMetric = isMetric;
            Settings.Default.Save();
            lblSpeedUnits.Text = "kmh";
            CalculateMinMaxZoom();
            fillCutFillLbl();
        }
        private void skyToolStripMenu_Click(object sender, EventArgs e)
        {
            isSkyOn = !isSkyOn;
            skyToolStripMenu.Checked = isSkyOn;
            Settings.Default.setMenu_isSkyOn = isSkyOn;
            Settings.Default.Save();
        }
        private void imperialToolStrip_Click(object sender, EventArgs e)
        {
            metricToolStrip.Checked = false;
            imperialToolStrip.Checked = true;
            isMetric = false;
            Settings.Default.setMenu_isMetric = isMetric;
            Settings.Default.Save();
            lblSpeedUnits.Text = "mph";
            CalculateMinMaxZoom();
            fillCutFillLbl();
        }
        private void simulatorOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (simulatorOnToolStripMenuItem.Checked)
            {
                panelSimControls.Visible = true;
                nudElevation.Visible = true;
                timerSim.Enabled = true;
                //ct.isSimulatorOn = true;
                SerialPortCloseGPS();
            }
            else
            {
                panelSimControls.Visible = false;
                nudElevation.Visible = false;
                timerSim.Enabled = false;
                //ct.isSimulatorOn = false;
            }

            Settings.Default.setMenu_isSimulatorOn = simulatorOnToolStripMenuItem.Checked;
            Settings.Default.Save();
        }

        public void closeSimulator()
        {
            simulatorOnToolStripMenuItem.Checked = false;
            
            
                panelSimControls.Visible = false;
                nudElevation.Visible = false;
                timerSim.Enabled = false;
                //ct.isSimulatorOn = false;
            

            Settings.Default.setMenu_isSimulatorOn = false;
            Settings.Default.Save();
        }

        //setting color off Options Menu
        private void sectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //color picker for sections
            ColorDialog colorDlg = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                SolidColorOnly = false,
                Color = Color.FromArgb(255, redSections, grnSections, bluSections)
            };

            if (colorDlg.ShowDialog() != DialogResult.OK) return;

            redSections = colorDlg.Color.R;
            if (redSections > 253) redSections = 253;
            grnSections = colorDlg.Color.G;
            if (grnSections > 253) grnSections = 253;
            bluSections = colorDlg.Color.B;
            if (bluSections > 253) bluSections = 253;

            Settings.Default.setF_SectionColorR = redSections;
            Settings.Default.setF_SectionColorG = grnSections;
            Settings.Default.setF_SectionColorB = bluSections;
            Settings.Default.Save();
        }
        //setting Fill color off Options Menu
        private void btnColorFill_Click(object sender, EventArgs e)
        {
            //color picker for sections
            ColorDialog colorDlg = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                SolidColorOnly = false,
                Color = Color.FromArgb(255, redFill, grnFill, bluFill)
            };

            if (colorDlg.ShowDialog() != DialogResult.OK) return;

            redFill = colorDlg.Color.R;
            if (redFill > 253) redFill = 253;
            grnFill = colorDlg.Color.G;
            if (grnFill > 253) grnFill = 253;
            bluFill = colorDlg.Color.B;
            if (bluFill > 253) bluFill = 253;

            Settings.Default.setF_FillColorR = redFill;
            Settings.Default.setF_FillColorG = grnFill;
            Settings.Default.setF_FillColorB = bluFill;
            Settings.Default.Save();

            btnColorFill.BackColor = System.Drawing.Color.FromArgb(redFill, grnFill, bluFill);
        }
        //setting  center color off Options Menu
        private void btnColorCenter_Click(object sender, EventArgs e)
        {
            //color picker for sections
            ColorDialog colorDlg = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                SolidColorOnly = false,
                Color = Color.FromArgb(255, redCenter, grnCenter, bluCenter)
            };

            if (colorDlg.ShowDialog() != DialogResult.OK) return;

            redCenter = colorDlg.Color.R;
            if (redCenter > 253) redCenter = 253;
            grnCenter = colorDlg.Color.G;
            if (grnCenter > 253) grnCenter = 253;
            bluCenter = colorDlg.Color.B;
            if (bluCenter > 253) bluCenter = 253;

            Settings.Default.setF_CenterColorR = redCenter;
            Settings.Default.setF_CenterColorG = grnCenter;
            Settings.Default.setF_CenterColorB = bluCenter;
            Settings.Default.Save();

            btnColorCenter.BackColor = System.Drawing.Color.FromArgb(redCenter, grnCenter, bluCenter);
        }
        //setting cut color off Options Menu
        private void btnColorCut_Click(object sender, EventArgs e)
        {
            //color picker for sections
            ColorDialog colorDlg = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                SolidColorOnly = false,
                Color = Color.FromArgb(255, redCut, grnCut, bluCut)
            };

            if (colorDlg.ShowDialog() != DialogResult.OK) return;

            redCut = colorDlg.Color.R;
            if (redCut > 253) redCut = 253;
            grnCut = colorDlg.Color.G;
            if (grnCut > 253) grnCut = 253;
            bluCut = colorDlg.Color.B;
            if (bluCut > 253) bluCut = 253;

            Settings.Default.setF_CutColorR = redCut;
            Settings.Default.setF_CutColorG = grnCut;
            Settings.Default.setF_CutColorB = bluCut;
            Settings.Default.Save();
            
            btnColorCut.BackColor = System.Drawing.Color.FromArgb(redCut, grnCut, bluCut);
        }

        private void btnResetMapColor_Click(object sender, EventArgs e)
        {
            redFill = 0;
            grnFill = 191;
            bluFill = 0;

            redCenter = 191;
            grnCenter = 191;
            bluCenter = 191;

            redCut = 191;
            grnCut = 0;
            bluCut = 0;

            Settings.Default.setF_FillColorR = redFill;
            Settings.Default.setF_FillColorG = grnFill;
            Settings.Default.setF_FillColorB = bluFill;

            Settings.Default.setF_CenterColorR = redCenter;
            Settings.Default.setF_CenterColorG = grnCenter;
            Settings.Default.setF_CenterColorB = bluCenter;

            Settings.Default.setF_CutColorR = redCut;
            Settings.Default.setF_CutColorG = grnCut;
            Settings.Default.setF_CutColorB = bluCut;
            Settings.Default.Save();

            btnColorCenter.BackColor = System.Drawing.Color.FromArgb(redCenter, grnCenter, bluCenter);
            btnColorFill.BackColor = System.Drawing.Color.FromArgb(redFill, grnFill, bluFill);
            btnColorCut.BackColor = System.Drawing.Color.FromArgb(redCut, grnCut, bluCut);
        }

        private void fieldToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //color picker for fields

            ColorDialog colorDlg = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                SolidColorOnly = false,
                Color = Color.FromArgb(255, Settings.Default.setF_FieldColorR,
                Settings.Default.setF_FieldColorG, Settings.Default.setF_FieldColorB)
            };

            if (colorDlg.ShowDialog() != DialogResult.OK) return;

            redField = colorDlg.Color.R;
            if (redField > 253) redField = 253;
            grnField = colorDlg.Color.G;
            if (grnField > 253) grnField = 253;
            bluField = colorDlg.Color.B;
            if (bluField > 253) bluField = 253;

            Settings.Default.setF_FieldColorR = redField;
            Settings.Default.setF_FieldColorG = grnField;
            Settings.Default.setF_FieldColorB = bluField;
            Settings.Default.Save();
        }

        //Tools drop down items
        private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                FileSaveFlagsKML();
            }
            Process.Start(fieldsDirectory + currentFieldDirectory);
        }
        private void webCamToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form form = new FormWebCam();
            form.Show();
        }
        private void googleEarthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                //save new copy of flags
                FileSaveFlagsKML();

                //Process.Start(@"C:\Program Files (x86)\Google\Google Earth\client\googleearth", workingDirectory + currentFieldDirectory + "\\Flags.KML");
                Process.Start(fieldsDirectory + currentFieldDirectory + "\\Flags.KML");
            }
            else
            {
                var form = new FormTimedMessage(1500, gStr.gsFieldNotOpen, gStr.gsStartNewField);
                form.Show();
            }
        }

        private void btnGoogleEarth_Click(object sender, EventArgs e)
        {
            //save new copy of contour
            FileSaveCutKML();

            //make sure google is installed
            Process.Start(fieldsDirectory + currentFieldDirectory + "\\Cut.KML");
        }

        private void fieldViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //in the current application directory
            //string AOGViewer = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\AOG.exe";
            //Process.Start(AOGViewer);
            {
                var form = new FormTimedMessage(2000, "Not yet Implemented", "But soon....");
                form.Show();
            }
        }
        private void gPSDataToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form form = new FormGPSData(this);
            form.Show();
        }

        //The flag context menus
        private void toolStripMenuItemFlagRed_Click(object sender, EventArgs e)
        {
            flagColor = 0;
            btnFlag.Image = Properties.Resources.FlagRed;
        }
        private void toolStripMenuGrn_Click(object sender, EventArgs e)
        {
            flagColor = 1;
            btnFlag.Image = Properties.Resources.FlagGrn;
        }
        private void toolStripMenuYel_Click(object sender, EventArgs e)
        {
            flagColor = 2;
            btnFlag.Image = Properties.Resources.FlagYel;
        }
        private void toolStripMenuFlagDelete_Click(object sender, EventArgs e)
        {
            //delete selected flag and set selected to none
            DeleteSelectedFlag();
            FileSaveFlags();
        }
        private void toolStripMenuFlagDeleteAll_Click(object sender, EventArgs e)
        {
            flagNumberPicked = 0;
            flagPts.Clear();
            FileSaveFlags();
        }
        private void contextMenuStripFlag_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            toolStripMenuFlagDelete.Enabled = flagNumberPicked != 0;

            toolStripMenuFlagDeleteAll.Enabled = flagPts.Count > 0;
        }

        //OpenGL Window context Menu and functions
        private void deleteFlagToolOpenGLContextMenu_Click(object sender, EventArgs e)
        {
            //delete selected flag and set selected to none
            DeleteSelectedFlag();
        }
        private void contextMenuStripOpenGL_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //dont bring up menu if no flag selected
            if (flagNumberPicked == 0) e.Cancel = true;
        }
        private void googleEarthOpenGLContextMenu_Click(object sender, EventArgs e)
        {
            if (isJobStarted)
            {
                //save new copy of kml with selected flag and view in GoogleEarth
                FileSaveSingleFlagKML(flagNumberPicked);

                //Process.Start(@"C:\Program Files (x86)\Google\Google Earth\client\googleearth", workingDirectory + currentFieldDirectory + "\\Flags.KML");
                Process.Start(fieldsDirectory + currentFieldDirectory + "\\Flag.KML");
            }
        }

        //function mouse down in window for picking
        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //0 at bottom for opengl, 0 at top for windows, so invert Y value
                Point point = openGLControl.PointToClient(Cursor.Position);
                mouseX = point.X;
                mouseY = openGLControl.Height - point.Y;
                leftMouseDownOnOpenGL = true;
            }
        }

        //taskbar buttons
        private void toolstripAutoSteerConfig_Click(object sender, EventArgs e)
        {
            //check if window already exists
            Form fc = Application.OpenForms["FormSteer"];

            if (fc != null)
            {
                fc.Focus();
                return;
            }

            //
            Form form = new FormSteer(this);
            form.Show();
        }
        private void toolstripVehicleConfig_Click(object sender, EventArgs e)
        {
            SettingsPageOpen(0);
            
        }
        private void toolstripUSBPortsConfig_Click(object sender, EventArgs e)
        {
            SettingsCommunications();
        }
        private void toolstripUDPConfig_Click(object sender, EventArgs e)
        {
            SettingsUDP();
        }
        private void toolstripNtripConfig_Click(object sender, EventArgs e)
        {
            SettingsNTRIP();
        }
        private void toolstripResetTrip_Click_1(object sender, EventArgs e)
        {
            userDistance = 0;
        }
        private void toolstripField_Click(object sender, EventArgs e)
        {
            JobNewOpenResume();
        }

        //batman to maximize GPS mapping - hide tab control
        private void btnHideTabs_Click(object sender, EventArgs e)
        {
            HideTabControl();
        }

        private void stripSelectMode_Click(object sender, EventArgs e)
        {
            SelectMode();
        }

        //Sim controls
        private void timerSim_Tick(object sender, EventArgs e)
        {
            //if a GPS is connected disable sim
            if (!sp.IsOpen)
            {
                if (isAutoSteerBtnOn) sim.DoSimTick(guidanceLineSteerAngle / 10.0);
                else sim.DoSimTick(sim.steerAngleScrollBar);
            }
        }
        private void tbarStepDistance_Scroll(object sender, EventArgs e)
        {
            sim.stepDistance = ((double)(tbarStepDistance.Value)) / 10.0 / (double)fixUpdateHz;
        }
        private void tbarSteerAngle_Scroll(object sender, EventArgs e)
        {
            sim.steerAngleScrollBar = (tbarSteerAngle.Value) * 0.1;
            lblSteerAngle.Text = sim.steerAngleScrollBar.ToString("N1");
        }
        private void btnResetSteerAngle_Click(object sender, EventArgs e)
        {
            sim.steerAngleScrollBar = 0;
            tbarSteerAngle.Value = 0;
            lblSteerAngle.Text = sim.steerAngleScrollBar.ToString("N1");
        }
        private void btnResetSim_Click(object sender, EventArgs e)
        {
            sim.ResetSim();
        }

        //simulator altitude
        private void nudElevation_ValueChanged(object sender, EventArgs e)
        {
            sim.altitude = (double)nudElevation.Value + Properties.Vehicle.Default.setVehicle_antennaHeight;
        }

        #region Properties // ---------------------------------------------------------------------

        public string Zone { get { return Convert.ToString(pn.zone); } }
        public string FixNorthing { get { return Convert.ToString(Math.Round(pn.northing + pn.utmNorth, 2)); } }
        public string FixEasting { get { return Convert.ToString(Math.Round(pn.easting + pn.utmEast, 2)); } }
        public string Latitude { get { return Convert.ToString(Math.Round(pn.latitude, 7)); } }
        public string Longitude { get { return Convert.ToString(Math.Round(pn.longitude, 7)); } }

        public string SatsTracked { get { return Convert.ToString(pn.satellitesTracked); } }
        public string HDOP { get { return Convert.ToString(pn.hdop); } }
        public string NMEAHz { get { return Convert.ToString(fixUpdateHz); } }
        public string PassNumber { get { return Convert.ToString(ABLine.passNumber); } }
        public string Heading { get { return Convert.ToString(Math.Round(glm.toDegrees(fixHeading), 1)) + "\u00B0"; } }
        public string GPSHeading { get { return (Math.Round(glm.toDegrees(gpsHeading), 1)) + "\u00B0"; } }
        public string Status { get { if (pn.status == "A") return "Active"; else return "Void"; } }
        public string FixQuality
        {
            get
            {
                if (pn.fixQuality == 0) return "Invalid";
                else if (pn.fixQuality == 1) return "GPS";
                else if (pn.fixQuality == 2) return "DGPS";
                else if (pn.fixQuality == 3) return "PPS";
                else if (pn.fixQuality == 4) return "RTK fix";
                else if (pn.fixQuality == 5) return "Flt RTK";
                else if (pn.fixQuality == 6) return "Estimate";
                else if (pn.fixQuality == 7) return "Man IP";
                else if (pn.fixQuality == 8) return "Sim";
                else return "Unknown";
            }
        }

        public string GyroInDegrees
        {
            get
            {
                if (mc.gyroHeading != 9999)
                    return Math.Round(mc.gyroHeading * 0.0625, 1) + "\u00B0";
                else return "-";
            }
        }
        public string RollInDegrees
        {
            get
            {
                if (mc.rollRaw != 9999)
                    return Math.Round(mc.rollRaw * 0.0625, 1) + "\u00B0";
                else return "-";
            }
        }
        public string PureSteerAngle { get { return ((double)(guidanceLineSteerAngle) * 0.1) + "\u00B0"; } }

        public string FixHeading { get { return Math.Round(fixHeading, 4).ToString(); } }

        public string StepFixNum { get { return (currentStepFix).ToString(); } }
        public string CurrentStepDistance { get { return Math.Round(distanceCurrentStepFix, 3).ToString(); } }
        public string TotalStepDistance { get { return Math.Round(fixStepDist, 3).ToString(); } }

        public string WorkSwitchValue { get { return mc.workSwitchValue.ToString(); } }
        public string AgeDiff { get { return pn.ageDiff.ToString(); } }

        //Metric and Imperial Properties
        public string SpeedMPH
        {
            get
            {
                double spd = 0;
                for (int c = 0; c < 10; c++) spd += avgSpeed[c];
                spd *= 0.0621371;
                return Convert.ToString(Math.Round(spd, 1));
            }
        }
        public string SpeedKPH
        {
            get
            {
                double spd = 0;
                for (int c = 0; c < 10; c++) spd += avgSpeed[c];
                spd *= 0.1;
                return Convert.ToString(Math.Round(spd, 1));
            }
        }

        public string Altitude { get { return Convert.ToString(Math.Round(pn.altitude,3)); } }
        public string AltitudeFeet { get { return Convert.ToString((Math.Round((pn.altitude * 3.28084),3))); } }

        public Texture ParticleTexture { get; set; }

        #endregion properties 

        private void DoNTRIPSecondRoutine()
        {
            //count up the ntrip clock only if everything is alive
            if (startCounter > 50 && recvCounter < 20 && isNTRIP_RequiredOn)
            {
                IncrementNTRIPWatchDog();
            }

            if (Properties.Settings.Default.setNTRIP_isOn && sp.IsOpen)
            {
                    isNTRIP_RequiredOn = true;
                    stripDistance.Text = gStr.gsWaiting;
            }
            

            //Have we connection
            if (isNTRIP_RequiredOn && !isNTRIP_Connected && !isNTRIP_Connecting)
            {
                if (!isNTRIP_Starting && ntripCounter > 20)
                {
                    StartNTRIP();
                }
            }

            if (isNTRIP_Connecting)
            {
                if (ntripCounter > 50)
                {
                    TimedMessageBox(2000, gStr.gsSocketConnectionProblem, gStr.gsNotConnectingToCaster);
                    ReconnectRequest();
                }
                if (clientSocket != null && clientSocket.Connected)
                {
                    //TimedMessageBox(2000, "NTRIP Not Connected", " At the StartNTRIP() ");
                    //ReconnectRequest();
                    //return;
                    SendAuthorization();
                }

            }

            if (isNTRIP_RequiredOn)
            {
                //update byte counter and up counter
                //if (ntripCounter > 59) NTRIPStartStopStrip.Text = (ntripCounter / 60) + " Mins";
                //else if (ntripCounter < 60 && ntripCounter > 22) NTRIPStartStopStrip.Text = ntripCounter + " Secs";
                //else NTRIPStartStopStrip.Text = "In " + (Math.Abs(ntripCounter - 22)) + " secs";

                //pbarNtripMenu.Value = unchecked((byte)(tripBytes * 0.02));
                //NTRIPBytesMenu.Text = ((tripBytes) * 0.001).ToString("###,###,###") + " kb";

                //watchdog for Ntrip
                if (isNTRIP_Connecting) lblWatch.Text = gStr.gsAuthourizing;
                else
                {
                    if (NTRIP_Watchdog > 10) lblWatch.Text = gStr.gsWaiting;
                    else lblWatch.Text = gStr.gsListening;
                }

                if (sendGGAInterval > 0 && isNTRIP_Sending)
                {
                    lblWatch.Text = "Send GGA";
                    isNTRIP_Sending = false;
                }
            }
        }


        //Timer triggers at 50 msec, 20 hz, and is THE clock of the whole program//
        private void tmrWatchdog_tick(object sender, EventArgs e)
        {
            if (!stopTheProgram)
            {


                //go see if data ready for draw and position updates
                //tmrWatchdog.Enabled = false;
                ScanForNMEA();
                //tmrWatchdog.Enabled = true;
                statusUpdateCounter++;

                if (fiveSecondCounter++ > 20)
                { 
                    //do all the NTRIP routines
                    DoNTRIPSecondRoutine(); 
                    fiveSecondCounter = 0; 
                }

                //GPS Update rate
                lblFixUpdateHz.Text = NMEAHz + " Hz " + FixQuality + " " + (int)(frameTime) + "ms";

                //1 for every .100 of a second update all status ,now 10hz was 4hz
                if (statusUpdateCounter > 1)
                {
                    //reset the counter
                    statusUpdateCounter = 0;

                    //counter used for saving field in background
                    saveCounter++;

                    if (tabControl1.SelectedIndex == 0 && tabControl1.Visible)
                    {

                        //both
                        lblLatitude.Text = Latitude;
                        lblLongitude.Text = Longitude;
                        lblFixQuality.Text = FixQuality;
                        lblSats.Text = SatsTracked;

                        lblRoll.Text = RollInDegrees;
                        lblGyroHeading.Text = GyroInDegrees;
                        lblGPSHeading.Text = GPSHeading;

                        //up in the menu a few pieces of info
                        if (isJobStarted)
                        {
                            lblEasting.Text = "E: " + Math.Round(pn.easting, 1).ToString();
                            lblNorthing.Text = "N: " + Math.Round(pn.northing, 1).ToString();
                        }
                        else
                        {
                            lblEasting.Text = "E: " + ((int)pn.actualEasting).ToString();
                            lblNorthing.Text = "N: " + ((int)pn.actualNorthing).ToString();
                        }

                        lblZone.Text = pn.zone.ToString();
                        tboxSentence.Text = recvSentenceSettings;
                    }


                    //the main formgps window
                    if (isMetric)  //metric or imperial
                    {
                        //Hectares on the master section soft control and sections
                        lblSpeed.Text = SpeedKPH;

                        //status strip values
                        stripDistance.Text = Convert.ToString((UInt16)(userDistance)) + " m";
                        lblAltitude.Text = Altitude;
                        btnZeroAltitude.Text = (pn.altitude - ct.zeroAltitude).ToString("N2");
                    }
                    else  //Imperial Measurements
                    {
                        //acres on the master section soft control and sections
                        lblSpeed.Text = SpeedMPH;

                        //status strip values
                        stripDistance.Text = Convert.ToString((UInt16)(userDistance * 3.28084)) + " ft";
                        lblAltitude.Text = AltitudeFeet;
                        btnZeroAltitude.Text = ((pn.altitude - ct.zeroAltitude) * glm.m2ft).ToString("N2");
                    }

                    //not Metric/Standard units sensitive
                    lblHeading.Text = Heading;
                    btnABLine.Text = PassNumber;
                    lblPureSteerAngle.Text = PureSteerAngle;

                    

                    //check for the fix quality
                    if (pn.fixQuality != 4 && lastFixQuality == 4)
                    {
                        var form = new FormTimedMessage(1000, "Lost RTK fix", "RTK fix is lost!");
                        form.Show();
                    }

                    lastFixQuality = pn.fixQuality;

                    if (cutDelta == 9999)
                    {
                        lblCutDelta.Text = "--";
                        lblCutDelta.BackColor = Color.Lavender;
                        pbarCutAbove.Value = 0;
                        pbarCutBelow.Value = 0;

                        //Output to serial for blade control 
                        mc.relayRateData[mc.cutValve] = (byte)(100);
                        RateRelayDataOutToPort();

                        //
                    }
                    else
                    {
                        if (cutDelta < -9.9) //par Pat

                        {
                            mc.relayRateData[mc.cutValve] = (byte)(1);
                            RateRelayDataOutToPort();
                        }
                        else
                        {
                            if (cutDelta > 9.9)
                            {
                                mc.relayRateData[mc.cutValve] = (byte)(199);
                                RateRelayDataOutToPort();
                            }
                            else
                            {
                                mc.relayRateData[mc.cutValve] = (byte)((cutDelta * 10) + 100);
                                RateRelayDataOutToPort();
                            }
                        }




                        if (isMetric)  //metric or imperial
                        {
                            lblCutDelta.Text = cutDelta.ToString("N1");

                        }
                        else
                        {
                            lblCutDelta.Text = (0.3937 * cutDelta).ToString("N2");
                        }

                        lblCutDelta.BackColor = SystemColors.ControlText;

                        if (cutDelta < 0)
                        {
                            int val = (int)(cutDelta / barGraphMax * -100);
                            pbarCutAbove.Value = 0;
                            pbarCutBelow.Value = val;
                            lblCutDelta.BackColor = Color.Tomato;
                        }
                        else
                        {
                            int val = (int)(cutDelta / barGraphMax * 100);
                            pbarCutAbove.Value = val;
                            pbarCutBelow.Value = 0;
                            lblCutDelta.BackColor = Color.Lime;

                        }
                    }

                    //update the online indicator
                    if (recvCounter > 50)
                    {
                        stripOnlineGPS.Value = 1;
                        lblEasting.Text = "-";
                        lblNorthing.Text = gStr.gsNoGPS;
                        lblZone.Text = "-";
                        tboxSentence.Text = gStr.gsNoSentenceData;
                    }
                    else stripOnlineGPS.Value = 100;
                }
                //wait till timer fires again.  
            }
        }

       
    }//end class
}//end namespace