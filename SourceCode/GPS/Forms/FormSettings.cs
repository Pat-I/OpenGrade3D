//Please, if you use this, share the improvements

using System;
using System.IO;
using System.Windows.Forms;

namespace OpenGrade
{
    public partial class FormSettings : Form
    {
       //class variables
        private readonly FormGPS mf = null;

        private double antennaHeight, minSlope, toolWidth, viewDistUnderGnd, viewDistAboveGnd, gradeDistFromLine, maxCuttingDepth;
        private byte PwmGainUp, PwmGainDown, PwmMaxUp, PwmMaxDown, PwmMinUp, PwmMinDown, IntegralMultiplier, Deadband;
        private readonly double metImp2m, m2MetImp, metFt2m, m2metFt;

        private double eastingOffset, northingOffset, altitudeOffset, oldEastingOffset, oldNorthingOffset, oldAltitudeOffset, noAvgDist, levelDistFactor;
        private bool avgPt;

        public readonly CNMEA pn = null; // by Pat

        //constructor
        public FormSettings(Form callingForm, int page)
        {
            //get copy of the calling main form
            mf = callingForm as FormGPS;
            InitializeComponent();

            if (mf.isMetric)
            {
                metImp2m = 0.01;
                m2MetImp = 100.0;
                metFt2m = 1;
                m2metFt = 1;
                lblInchesCm.Text = "Centimeters";
            }
            else
            {
                metImp2m = glm.in2m;
                m2MetImp = glm.m2in;
                metFt2m = .3048;
                m2metFt = (1 / .0254 / 12);
                lblInchesCm.Text = "Inches";
            }
            //select the page as per calling menu or button from mainGPS form
            tabControl1.SelectedIndex = page;
        }

        //do any field initializing for form here
        private void FormSettings_Load(object sender, EventArgs e)
        {
            //Vehicle settings to what it is in the settings page------------------------------------------------
            antennaHeight = Properties.Vehicle.Default.setVehicle_antennaHeight;
            toolWidth = Properties.Vehicle.Default.setVehicle_toolWidth;
            minSlope = Properties.Vehicle.Default.setVehicle_minSlope * 100;
            maxCuttingDepth = Properties.Vehicle.Default.setVehicle_MaxCuttingDepth;

            nudAntennaHeight.ValueChanged -= nudAntennaHeight_ValueChanged;
            nudAntennaHeight.Value = (decimal)(antennaHeight * m2MetImp);
            nudAntennaHeight.ValueChanged += nudAntennaHeight_ValueChanged;

            nudToolWidth.ValueChanged -= nudToolWidth_ValueChanged;
            nudToolWidth.Value = (decimal)(toolWidth * m2MetImp);
            nudToolWidth.ValueChanged += nudToolWidth_ValueChanged;

            nudMinSlope.ValueChanged -= nudMinSlope_ValueChanged;
            nudMinSlope.Value = (decimal)(minSlope);
            nudMinSlope.ValueChanged += nudMinSlope_ValueChanged;

            nudMaxCuttingDepth.ValueChanged -= nudMaxCuttingDepth_ValueChanged;
            nudMaxCuttingDepth.Value = (decimal)(maxCuttingDepth * m2MetImp);
            nudMaxCuttingDepth.ValueChanged += nudMaxCuttingDepth_ValueChanged;

            //Valve settings to what it is in the settings page------------------------------------------------

            PwmMinUp = Properties.Vehicle.Default.setVehicle_pwmMinUp;
            PwmMinDown = Properties.Vehicle.Default.setVehicle_pwmMinDown;
            PwmMaxUp = Properties.Vehicle.Default.setVehicle_pwmMaxUp;
            PwmMaxDown = Properties.Vehicle.Default.setVehicle_pwmMaxDown;
            PwmGainUp = Properties.Vehicle.Default.setVehicle_pwmGainUp;
            PwmGainDown = Properties.Vehicle.Default.setVehicle_pwmGainDown;
            IntegralMultiplier = Properties.Vehicle.Default.setVehicle_integralMultiplier;
            Deadband = Properties.Vehicle.Default.setVehicle_deadband;

            nudPwmMinDown.ValueChanged -= nudPwmMinDown_ValueChanged;
            nudPwmMinDown.Value = PwmMinDown;
            nudPwmMinDown.ValueChanged += nudPwmMinDown_ValueChanged;

            nudPwmMaxDown.ValueChanged -= nudPwmMaxDown_ValueChanged;
            nudPwmMaxDown.Value = PwmMaxDown;
            nudPwmMaxDown.ValueChanged += nudPwmMaxDown_ValueChanged;

            nudPwmGainDown.ValueChanged -= nudPwmGainDown_ValueChanged;
            nudPwmGainDown.Value = PwmGainDown;
            nudPwmGainDown.ValueChanged += nudPwmGainDown_ValueChanged;

            nudPwmMinUp.ValueChanged -= nudPwmMinUp_ValueChanged;
            nudPwmMinUp.Value = PwmMinUp;
            nudPwmMinUp.ValueChanged += nudPwmMinUp_ValueChanged;

            nudPwmMaxUp.ValueChanged -= nudPwmMaxUp_ValueChanged;
            nudPwmMaxUp.Value = PwmMaxUp;
            nudPwmMaxUp.ValueChanged += nudPwmMaxUp_ValueChanged;

            nudPwmGainUp.ValueChanged -= nudPwmGainUp_ValueChanged;
            nudPwmGainUp.Value = PwmGainUp;
            nudPwmGainUp.ValueChanged += nudPwmGainUp_ValueChanged;

            nudIntegralMultiplier.ValueChanged -= nudIntegralMultiplier_ValueChanged;
            nudIntegralMultiplier.Value = IntegralMultiplier;
            nudIntegralMultiplier.ValueChanged += nudIntegralMultiplier_ValueChanged;

            nudDeadband.ValueChanged -= nudDeadband_ValueChanged;
            nudDeadband.Value = Deadband;
            nudDeadband.ValueChanged += nudDeadband_ValueChanged;

            //Display settings to what it is in the settings page------------------------------------------------

            viewDistUnderGnd = Properties.Vehicle.Default.setVehicle_ViewDistUnderGnd;
            viewDistAboveGnd = Properties.Vehicle.Default.setVehicle_ViewDistAboveGnd;
            gradeDistFromLine = Properties.Vehicle.Default.setVehicle_GradeDistFromLine;

            nudViewDistUnderGnd.ValueChanged -= nudViewDistUnderGnd_ValueChanged;
            nudViewDistUnderGnd.Value = (decimal)(viewDistUnderGnd * m2MetImp);
            nudViewDistUnderGnd.ValueChanged += nudViewDistUnderGnd_ValueChanged;

            nudViewDistAboveGnd.ValueChanged -= nudViewDistAboveGnd_ValueChanged;
            nudViewDistAboveGnd.Value = (decimal)(viewDistAboveGnd * m2MetImp);
            nudViewDistAboveGnd.ValueChanged += nudViewDistAboveGnd_ValueChanged;

            nudGradeDistFromLine.ValueChanged -= nudGradeDistFromLine_ValueChanged;
            nudGradeDistFromLine.Value = (decimal)(gradeDistFromLine * m2metFt);
            nudGradeDistFromLine.ValueChanged += nudGradeDistFromLine_ValueChanged;

            // Levelling settings----------------------------------------------------------

            eastingOffset = mf.pn.eastingOffset;
            northingOffset = mf.pn.northingOffset;
            altitudeOffset = mf.pn.altitudeOffset;
            oldEastingOffset = mf.pn.eastingOffset;
            oldNorthingOffset = mf.pn.northingOffset;
            oldAltitudeOffset = mf.pn.altitudeOffset;
            noAvgDist = Properties.Settings.Default.Set_noAvgDist;
            levelDistFactor = Properties.Settings.Default.Set_levelDistFactor;
            avgPt = Properties.Settings.Default.Set_isAvgPt;

            if (avgPt) butAvgDesignPt.Text = "YES";
            else butAvgDesignPt.Text = "NO";

            nudCorrEasting.ValueChanged -= nudCorrEasting_ValueChanged;
            nudCorrEasting.Value = (decimal)eastingOffset;
            nudCorrEasting.ValueChanged += nudCorrEasting_ValueChanged;

            nudCorrNorthing.ValueChanged -= nudCorrNorthing_ValueChanged;
            nudCorrNorthing.Value = (decimal)northingOffset;
            nudCorrNorthing.ValueChanged += nudCorrNorthing_ValueChanged;

            nudCorrAltitude.ValueChanged -= nudCorrAltitude_ValueChanged;
            nudCorrAltitude.Value = (decimal)altitudeOffset;
            nudCorrAltitude.ValueChanged += nudCorrAltitude_ValueChanged;

            nudNoAvgDist.ValueChanged -= nudNoAvgDist_ValueChanged;
            nudNoAvgDist.Value = (decimal)noAvgDist;
            nudNoAvgDist.ValueChanged += nudNoAvgDist_ValueChanged;

            nudLevelDistFactor.ValueChanged -= nudLevelDistFactor_ValueChanged;
            nudLevelDistFactor.Value = (decimal)levelDistFactor;
            nudLevelDistFactor.ValueChanged += nudLevelDistFactor_ValueChanged;

        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            //Vehicle settings -------------------------------------------------------------------------------
            mf.vehicle.minSlope = minSlope/100;
            Properties.Vehicle.Default.setVehicle_minSlope = mf.vehicle.minSlope;

            mf.vehicle.antennaHeight = antennaHeight;
            Properties.Vehicle.Default.setVehicle_antennaHeight = mf.vehicle.antennaHeight;

            mf.vehicle.toolWidth = toolWidth;
            Properties.Vehicle.Default.setVehicle_toolWidth = toolWidth;

            mf.vehicle.maxCuttingDepth = maxCuttingDepth;
            Properties.Vehicle.Default.setVehicle_MaxCuttingDepth = mf.vehicle.maxCuttingDepth;

            //Display settings -------------------------------------------------------------------------------

            mf.vehicle.viewDistUnderGnd = viewDistUnderGnd;
            Properties.Vehicle.Default.setVehicle_ViewDistUnderGnd = mf.vehicle.viewDistUnderGnd;

            mf.vehicle.viewDistAboveGnd = viewDistAboveGnd;
            Properties.Vehicle.Default.setVehicle_ViewDistAboveGnd = mf.vehicle.viewDistAboveGnd;

            mf.vehicle.gradeDistFromLine = gradeDistFromLine;
            Properties.Vehicle.Default.setVehicle_GradeDistFromLine = mf.vehicle.gradeDistFromLine;

            //Valve settings ---------------------------------------------------------------------------------

            mf.vehicle.pwmGainUp = PwmGainUp;
            Properties.Vehicle.Default.setVehicle_pwmGainUp = PwmGainUp;

            mf.vehicle.pwmGainDown = PwmGainDown;
            Properties.Vehicle.Default.setVehicle_pwmGainDown = PwmGainDown;

            mf.vehicle.pwmMaxUp = PwmMaxUp;
            Properties.Vehicle.Default.setVehicle_pwmMaxUp = PwmMaxUp;

            mf.vehicle.pwmMaxDown = PwmMaxDown;
            Properties.Vehicle.Default.setVehicle_pwmMaxDown = PwmMaxDown;

            mf.vehicle.pwmMinUp = PwmMinUp;
            Properties.Vehicle.Default.setVehicle_pwmMinUp = PwmMinUp;

            mf.vehicle.pwmMinDown = PwmMinDown;
            Properties.Vehicle.Default.setVehicle_pwmMinDown = PwmMinDown;

            mf.vehicle.deadband = Deadband;
            Properties.Vehicle.Default.setVehicle_deadband = Deadband;

            mf.vehicle.integralMultiplier = IntegralMultiplier;
            Properties.Vehicle.Default.setVehicle_integralMultiplier = IntegralMultiplier;

            //Levelling settings-------------------------------------------------------

            mf.pn.eastingOffset = eastingOffset;

            mf.pn.northingOffset = northingOffset;

            mf.pn.altitudeOffset = altitudeOffset;

            mf.averagePts = avgPt;
            Properties.Settings.Default.Set_isAvgPt = avgPt;

            mf.noAvgDist = noAvgDist;
            Properties.Settings.Default.Set_noAvgDist = noAvgDist;

            mf.levelDistFactor = levelDistFactor;
            Properties.Settings.Default.Set_levelDistFactor = levelDistFactor;

          
            mf.mc.relayRateSettings[mf.mc.rsPwmGainUp] = Properties.Vehicle.Default.setVehicle_pwmGainUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmGainDown] = Properties.Vehicle.Default.setVehicle_pwmGainDown;
            mf.mc.relayRateSettings[mf.mc.rsPwmMinUp] = Properties.Vehicle.Default.setVehicle_pwmMinUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmMinDown] = Properties.Vehicle.Default.setVehicle_pwmMinDown;
            mf.mc.relayRateSettings[mf.mc.rsPwmMaxUp] = Properties.Vehicle.Default.setVehicle_pwmMaxUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmMaxDown] = Properties.Vehicle.Default.setVehicle_pwmMaxDown;
            mf.mc.relayRateSettings[mf.mc.rsIntegralMultiplier] = Properties.Vehicle.Default.setVehicle_integralMultiplier;
            mf.mc.relayRateSettings[mf.mc.rsDeadband] = Properties.Vehicle.Default.setVehicle_deadband;
            mf.RateRelaySettingsOutToPort();

            //Sections ------------------------------------------------------------------------------------------

            Properties.Settings.Default.Save();
            Properties.Vehicle.Default.Save();
            //CalculateMinMaxZoom();

            //back to FormGPS
            DialogResult = DialogResult.OK;
            Close();
        }

        //don't save anything, leave the settings as before
        private void btnCancel_Click(object sender, EventArgs e)
        { DialogResult = DialogResult.Cancel; Close();

            //to reset the valves values if send button was pressed
            mf.mc.relayRateSettings[mf.mc.rsPwmGainUp] = Properties.Vehicle.Default.setVehicle_pwmGainUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmGainDown] = Properties.Vehicle.Default.setVehicle_pwmGainDown;
            mf.mc.relayRateSettings[mf.mc.rsPwmMinUp] = Properties.Vehicle.Default.setVehicle_pwmMinUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmMinDown] = Properties.Vehicle.Default.setVehicle_pwmMinDown;
            mf.mc.relayRateSettings[mf.mc.rsPwmMaxUp] = Properties.Vehicle.Default.setVehicle_pwmMaxUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmMaxDown] = Properties.Vehicle.Default.setVehicle_pwmMaxDown;
            mf.mc.relayRateSettings[mf.mc.rsIntegralMultiplier] = Properties.Vehicle.Default.setVehicle_integralMultiplier;
            mf.mc.relayRateSettings[mf.mc.rsDeadband] = Properties.Vehicle.Default.setVehicle_deadband;
            mf.RateRelaySettingsOutToPort();

            //reset the position correction values
            mf.pn.eastingOffset = oldEastingOffset;

            mf.pn.northingOffset = oldNorthingOffset;

            mf.pn.altitudeOffset = oldAltitudeOffset;

        }

        #region Vehicle //----------------------------------------------------------------

        private void nudAntennaHeight_ValueChanged(object sender, EventArgs e)
        {
            antennaHeight = (double)nudAntennaHeight.Value * metImp2m;
        }

        private void bntValveSettingsSend_Click(object sender, EventArgs e)
        {
            
            mf.mc.relayRateSettings[mf.mc.rsPwmGainUp] = PwmGainUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmGainDown] = PwmGainDown;
            mf.mc.relayRateSettings[mf.mc.rsPwmMinUp] = PwmMinUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmMinDown] = PwmMinDown;
            mf.mc.relayRateSettings[mf.mc.rsPwmMaxUp] = PwmMaxUp;
            mf.mc.relayRateSettings[mf.mc.rsPwmMaxDown] = PwmMaxDown;
            mf.mc.relayRateSettings[mf.mc.rsIntegralMultiplier] = IntegralMultiplier;
            mf.mc.relayRateSettings[mf.mc.rsDeadband] = Deadband;
            mf.RateRelaySettingsOutToPort();

        }

        private void nudMinSlope_ValueChanged(object sender, EventArgs e)
        {
            minSlope = (double)nudMinSlope.Value;
        }

        private void nudToolWidth_ValueChanged(object sender, EventArgs e)
        {
            toolWidth = (double)nudToolWidth.Value * metImp2m;
        }

        private void nudMaxCuttingDepth_ValueChanged(object sender, EventArgs e)
        {
            maxCuttingDepth = (double)nudMaxCuttingDepth.Value * metImp2m;
        }

        #endregion Vehicle

        #region Valve //--------------------------------------------------------------

        private void nudPwmMinDown_ValueChanged(object sender, EventArgs e)
        {
            PwmMinDown = (byte)nudPwmMinDown.Value;
        }

        private void nudPwmMinUp_ValueChanged(object sender, EventArgs e)
        {
            PwmMinUp = (byte)nudPwmMinUp.Value;
        }

        private void nudPwmGainUp_ValueChanged(object sender, EventArgs e)
        {
            PwmGainUp = (byte)nudPwmGainUp.Value;
        }

        private void nudPwmGainDown_ValueChanged(object sender, EventArgs e)
        {
            PwmGainDown = (byte)nudPwmGainDown.Value;
        }

        private void nudPwmMaxDown_ValueChanged(object sender, EventArgs e)
        {
            PwmMaxDown = (byte)nudPwmMaxDown.Value;
        }

        private void nudPwmMaxUp_ValueChanged(object sender, EventArgs e)
        {
            PwmMaxUp = (byte)nudPwmMaxUp.Value;
        }

        private void nudIntegralMultiplier_ValueChanged(object sender, EventArgs e)
        {
            IntegralMultiplier = (byte)nudIntegralMultiplier.Value;
        }

        private void nudDeadband_ValueChanged(object sender, EventArgs e)
        {
            Deadband = (byte)nudDeadband.Value;
        }

        #endregion Valve

        #region Display //----------------------------------------------------

        private void nudViewDistUnderGnd_ValueChanged(object sender, EventArgs e)
        {
            viewDistUnderGnd = (double)nudViewDistUnderGnd.Value * metImp2m;
            
        }

        private void nudViewDistAboveGnd_ValueChanged(object sender, EventArgs e)
        {
            viewDistAboveGnd = (double)nudViewDistAboveGnd.Value * metImp2m;
            
        }

        private void nudGradeDistFromLine_ValueChanged(object sender, EventArgs e)
        {
            gradeDistFromLine = (double)nudGradeDistFromLine.Value * metFt2m;
            if (gradeDistFromLine < 1) gradeDistFromLine = 1;
        }


        #endregion Display

        private void nudCorrEasting_ValueChanged(object sender, EventArgs e)
        {
            eastingOffset = (double)nudCorrEasting.Value;
        }

        private void nudCorrNorthing_ValueChanged(object sender, EventArgs e)
        {
            northingOffset = (double)nudCorrNorthing.Value;
        }

        private void nudCorrAltitude_ValueChanged(object sender, EventArgs e)
        {
            altitudeOffset = (double)nudCorrAltitude.Value;
        }

        private void nudNoAvgDist_ValueChanged(object sender, EventArgs e)
        {
            noAvgDist = (double)nudNoAvgDist.Value;
        }

        private void nudLevelDistFactor_ValueChanged(object sender, EventArgs e)
        {
            levelDistFactor = (double)nudLevelDistFactor.Value;
        }

        private void btnAvgDesignPt_Click(object sender, EventArgs e)
        {
            if (avgPt)
            {
                avgPt = false;
                butAvgDesignPt.Text = "NO";
            }
            else
            {
                avgPt = true;
                butAvgDesignPt.Text = "YES";
            }
        }

        private void btnPosCorrection_Click(object sender, EventArgs e)
        {
            int ptcount = mf.ct.boundaryList.Count;
            if (ptcount > 0)
            {
                if (mf.ct.boundaryList[0].code == 0)
                {
                    eastingOffset = (mf.pn.easting  + mf.pn.eastingOffset) - mf.ct.boundaryList[0].easting;
                    northingOffset = (mf.pn.northing + mf.pn.northingOffset) - mf.ct.boundaryList[0].northing;
                    altitudeOffset = mf.ct.boundaryList[0].altitude - (mf.pn.altitude - mf.pn.altitudeOffset);

                    mf.pn.eastingOffset = eastingOffset;
                    mf.pn.northingOffset = northingOffset;
                    mf.pn.altitudeOffset = altitudeOffset;

                    nudCorrEasting.Value = (decimal)eastingOffset;
                    nudCorrNorthing.Value = (decimal)northingOffset;
                    nudCorrAltitude.Value = (decimal)altitudeOffset;
                }
            }
        }

        private void btnSendCorr_Click(object sender, EventArgs e)
        {
            mf.pn.eastingOffset = eastingOffset;
            mf.pn.northingOffset = northingOffset;
            mf.pn.altitudeOffset = altitudeOffset;
        }

    }
}