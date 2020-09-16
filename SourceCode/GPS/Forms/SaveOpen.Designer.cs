using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace OpenGrade
{
    
    public partial class FormGPS
    {
        //list of the list of patch data individual triangles for field sections
        public List<List<vec2>> patchSaveList = new List<List<vec2>>();

        /*
         * The agd file is read near line 988 --public void FileOpenAgdDesign()
         * codes in .agd -> code in opengrade
         * MB -> 0
         * 2PER -> 2
         * 3GRD -> 3
         * 2SUBZONE1 -> 21
         * 2SUBZONE2 -> 22
         * to
         *2SUBZONE9 -> 29
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */

        //function that save vehicle and section settings
        public void FileSaveVehicle()
        {
            //in the current application directory
            //string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string fieldDir = dir + "\\fields\\";

            string dirVehicle = vehiclesDirectory;

            string directoryName = Path.GetDirectoryName(dirVehicle).ToString(CultureInfo.InvariantCulture);

            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Title = "Save Vehicle";
            saveDialog.Filter = "Text Files (*.txt)|*.txt";
            saveDialog.InitialDirectory = directoryName;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                vehiclefileName = Path.GetFileNameWithoutExtension(saveDialog.FileName) + " - ";
                Properties.Vehicle.Default.setVehicle_Name = vehiclefileName;
                Properties.Vehicle.Default.Save();

                using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                {
                    writer.WriteLine("Version," + " OpenGrade3D v1.0");
                    writer.WriteLine("Wheelbase," + Properties.Vehicle.Default.setVehicle_wheelbase.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("AntennaHeight," + Properties.Vehicle.Default.setVehicle_antennaHeight.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("ToolWidth," + Properties.Vehicle.Default.setVehicle_toolWidth.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("MinimumSlope," + Properties.Vehicle.Default.setVehicle_minSlope.ToString(CultureInfo.InvariantCulture));


                    writer.WriteLine("IsMetric," + Properties.Settings.Default.setMenu_isMetric.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("IsGridOn," + Properties.Settings.Default.setMenu_isGridOn.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("IsLightBarOn," + Properties.Settings.Default.setMenu_isLightbarOn.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("IsPurePursuitLineOn," + Properties.Settings.Default.setMenu_isPureOn.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine("FieldColorR," + Properties.Settings.Default.setF_FieldColorR.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("FieldColorG," + Properties.Settings.Default.setF_FieldColorG.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("FieldColorB," + Properties.Settings.Default.setF_FieldColorB.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("SectionColorR," + Properties.Settings.Default.setF_SectionColorR.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("SectionColorG," + Properties.Settings.Default.setF_SectionColorG.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("SectionColorB," + Properties.Settings.Default.setF_SectionColorB.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine("IMUPitchZero," + Properties.Settings.Default.setIMU_pitchZero.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("IMURollZero," + Properties.Settings.Default.setIMU_rollZero.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("IsLogNMEA," + Properties.Settings.Default.setMenu_isLogNMEA.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("MinFixStep," + Properties.Settings.Default.setF_minFixStep.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine("pidP," + Properties.Settings.Default.setAS_Kp.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("pidI," + Properties.Settings.Default.setAS_Ki.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("pidD," + Properties.Settings.Default.setAS_Kd.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("pidO," + Properties.Settings.Default.setAS_Ko.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("SteerAngleOffset," + Properties.Settings.Default.setAS_steerAngleOffset.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("minPWM," + Properties.Settings.Default.setAS_minSteerPWM.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("MaxIntegral," + Properties.Settings.Default.setAS_maxIntegral.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("CountsPerDegree," + Properties.Settings.Default.setAS_countsPerDegree.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine("GoalPointLookAhead," + Properties.Vehicle.Default.setVehicle_goalPointLookAhead.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("MaxSteerAngle," + Properties.Vehicle.Default.setVehicle_maxSteerAngle.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("MaxAngularVelocity," + Properties.Vehicle.Default.setVehicle_maxAngularVelocity.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine("Pwm Gain Up," + Properties.Vehicle.Default.setVehicle_pwmGainUp.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Pwm Gain Down," + Properties.Vehicle.Default.setVehicle_pwmGainDown.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Pwm Min Up," + Properties.Vehicle.Default.setVehicle_pwmMinUp.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Pwm Min Down," + Properties.Vehicle.Default.setVehicle_pwmMinDown.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Pwm Max Up," + Properties.Vehicle.Default.setVehicle_pwmMaxUp.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Pwm Max Down," + Properties.Vehicle.Default.setVehicle_pwmMaxDown.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Integral Multiplier," + Properties.Vehicle.Default.setVehicle_integralMultiplier.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Deadband," + Properties.Vehicle.Default.setVehicle_deadband.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine("ViewDistUnderGnd," + Properties.Vehicle.Default.setVehicle_ViewDistUnderGnd.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("ViewDistAboveGnd," + Properties.Vehicle.Default.setVehicle_ViewDistAboveGnd.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Map Resolution," + Properties.Vehicle.Default.setVehicle_GradeDistFromLine.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("MaxCuttingDepth," + Properties.Vehicle.Default.setVehicle_MaxCuttingDepth.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("Blade Offset," + Properties.Vehicle.Default.setVehicle_bladeOffset.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                    writer.WriteLine("Empty," + "10");
                }

                //little show to say saved and where
                var form = new FormTimedMessage(3000, "Saved in Folder: ", dirVehicle);
                form.Show();
            }

        }

        //function to open a previously saved field
        public void FileOpenVehicle()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            //get the directory where the fields are stored
            string directoryName = vehiclesDirectory;

            //make sure the directory exists, if not, create it
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            //the initial directory, fields, for the open dialog
            ofd.InitialDirectory = directoryName;

            //When leaving dialog put windows back where it was
            ofd.RestoreDirectory = true;

            //set the filter to text files only
            ofd.Filter = "txt files (*.txt)|*.txt";

            //was a file selected
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //if job started close it
                if (isJobStarted) JobClose();

                //make sure the file if fully valid and vehicle matches sections
                using (StreamReader reader = new StreamReader(ofd.FileName))
                {
                    try
                    {
                        string line;
                        Properties.Vehicle.Default.setVehicle_Name = ofd.FileName;
                        string[] words;
                        line = reader.ReadLine(); words = line.Split(',');

                        if (words[0] != "Version")

                        {
                            var form = new FormTimedMessage(5000, "Vehicle File is Wrong Version", "Must be Version 2.16 or higher");
                            form.Show();
                            return;
                        }

                        

                        if (words[1] != " OpenGrade3D v1.0")
                        {
                            var form = new FormTimedMessage(5000, "Vehicle File is Wrong Version", "Must be OpenGrade3D v1.0");
                            form.Show();
                            return;
                        }

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_wheelbase = double.Parse(words[1], CultureInfo.InvariantCulture);

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_antennaHeight = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_toolWidth = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_minSlope = double.Parse(words[1], CultureInfo.InvariantCulture);

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setMenu_isMetric = bool.Parse(words[1]);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setMenu_isGridOn = bool.Parse(words[1]);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setMenu_isLightbarOn = bool.Parse(words[1]);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setMenu_isPureOn = bool.Parse(words[1]);

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setF_FieldColorR = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setF_FieldColorG = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setF_FieldColorB = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setF_SectionColorR = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setF_SectionColorG = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setF_SectionColorB = byte.Parse(words[1], CultureInfo.InvariantCulture);

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setIMU_pitchZero = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setIMU_rollZero = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setMenu_isLogNMEA = bool.Parse(words[1]);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setF_minFixStep = double.Parse(words[1], CultureInfo.InvariantCulture);

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setAS_Kp = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setAS_Ki = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setAS_Kd = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setAS_Ko = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setAS_steerAngleOffset = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setAS_minSteerPWM = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Settings.Default.setAS_maxIntegral = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine();words = line.Split(',');
                        Properties.Settings.Default.setAS_countsPerDegree = byte.Parse(words[1], CultureInfo.InvariantCulture);

                        line = reader.ReadLine(); // GoalPointLookAhead

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_maxSteerAngle = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_maxAngularVelocity = double.Parse(words[1], CultureInfo.InvariantCulture);

                        //Valves settings
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_pwmGainUp = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_pwmGainDown = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_pwmMinUp = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_pwmMinDown = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_pwmMaxUp = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_pwmMaxDown = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_integralMultiplier = byte.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_deadband = byte.Parse(words[1], CultureInfo.InvariantCulture);

                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_ViewDistUnderGnd = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_ViewDistAboveGnd = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_GradeDistFromLine = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_MaxCuttingDepth = double.Parse(words[1], CultureInfo.InvariantCulture);
                        line = reader.ReadLine(); words = line.Split(',');
                        Properties.Vehicle.Default.setVehicle_bladeOffset = double.Parse(words[1], CultureInfo.InvariantCulture);

                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();

                        //fill in the current variables with restored data
                        vehiclefileName = Path.GetFileNameWithoutExtension(ofd.FileName) + " - ";
                        Properties.Vehicle.Default.setVehicle_Name = vehiclefileName;

                        Properties.Settings.Default.Save();
                        Properties.Vehicle.Default.Save();

                        //from settings grab the vehicle specifics
                        vehicle.antennaHeight = Properties.Vehicle.Default.setVehicle_antennaHeight;
                        vehicle.wheelbase = Properties.Vehicle.Default.setVehicle_wheelbase;

                        vehicle.toolWidth = Properties.Vehicle.Default.setVehicle_toolWidth;
                        vehicle.minSlope = Properties.Vehicle.Default.setVehicle_minSlope;

                        vehicle.viewDistUnderGnd = Properties.Vehicle.Default.setVehicle_ViewDistUnderGnd;
                        vehicle.viewDistAboveGnd = Properties.Vehicle.Default.setVehicle_ViewDistAboveGnd;
                        vehicle.gradeDistFromLine = Properties.Vehicle.Default.setVehicle_GradeDistFromLine;
                        vehicle.maxCuttingDepth = Properties.Vehicle.Default.setVehicle_MaxCuttingDepth;
                        vehicle.bladeOffset = Properties.Vehicle.Default.setVehicle_bladeOffset;
                        numBladeOffset.Value = (decimal)Properties.Vehicle.Default.setVehicle_bladeOffset *100;

                        //Valve settings

                        vehicle.pwmGainUp = Properties.Vehicle.Default.setVehicle_pwmGainUp;
                        vehicle.pwmGainDown = Properties.Vehicle.Default.setVehicle_pwmGainDown;
                        vehicle.pwmMinUp = Properties.Vehicle.Default.setVehicle_pwmMinUp;
                        vehicle.pwmMinDown = Properties.Vehicle.Default.setVehicle_pwmMinDown;
                        vehicle.pwmMaxUp = Properties.Vehicle.Default.setVehicle_pwmMaxUp;
                        vehicle.pwmMaxDown = Properties.Vehicle.Default.setVehicle_pwmMaxDown;
                        vehicle.integralMultiplier = Properties.Vehicle.Default.setVehicle_integralMultiplier;
                        vehicle.deadband = Properties.Vehicle.Default.setVehicle_deadband;

                        vehicle.maxAngularVelocity = Properties.Vehicle.Default.setVehicle_maxAngularVelocity;
                        vehicle.maxSteerAngle = Properties.Vehicle.Default.setVehicle_maxSteerAngle;

                        mc.autoSteerSettings[mc.ssKp] = Properties.Settings.Default.setAS_Kp;
                        mc.autoSteerSettings[mc.ssKi] = Properties.Settings.Default.setAS_Ki;
                        mc.autoSteerSettings[mc.ssKd] = Properties.Settings.Default.setAS_Kd;
                        mc.autoSteerSettings[mc.ssKo] = Properties.Settings.Default.setAS_Ko;
                        mc.autoSteerSettings[mc.ssSteerOffset] = Properties.Settings.Default.setAS_steerAngleOffset;
                        mc.autoSteerSettings[mc.ssMinPWM] = Properties.Settings.Default.setAS_minSteerPWM;
                        mc.autoSteerSettings[mc.ssMaxIntegral] = Properties.Settings.Default.setAS_maxIntegral;
                        mc.autoSteerSettings[mc.ssCountsPerDegree] = Properties.Settings.Default.setAS_countsPerDegree;

                        mc.isWorkSwitchEnabled = Properties.Settings.Default.setF_IsWorkSwitchEnabled;
                        mc.isWorkSwitchActiveLow = Properties.Settings.Default.setF_IsWorkSwitchActiveLow;

                        camera.camPitch = Properties.Settings.Default.setCam_pitch;

                        isMetric = Properties.Settings.Default.setMenu_isMetric;
                        metricToolStrip.Checked = isMetric;
                        imperialToolStrip.Checked = isMetric;

                        isGridOn = Properties.Settings.Default.setMenu_isGridOn;
                        gridToolStripMenuItem.Checked = (isGridOn);

                        isLightbarOn = Properties.Settings.Default.setMenu_isLightbarOn;
                        lightbarToolStripMenuItem.Checked = isLightbarOn;

                        isPureDisplayOn = Properties.Settings.Default.setMenu_isPureOn;
                        pursuitLineToolStripMenuItem.Checked = isPureDisplayOn;

                        redSections = Properties.Settings.Default.setF_SectionColorR;
                        grnSections = Properties.Settings.Default.setF_SectionColorG;
                        bluSections = Properties.Settings.Default.setF_SectionColorB;
                        redField = Properties.Settings.Default.setF_FieldColorR;
                        grnField = Properties.Settings.Default.setF_FieldColorG;
                        bluField = Properties.Settings.Default.setF_FieldColorB;

                        pitchZero = Properties.Settings.Default.setIMU_pitchZero;
                        rollZero = Properties.Settings.Default.setIMU_rollZero;
                        isLogNMEA = Properties.Settings.Default.setMenu_isLogNMEA;
                        minFixStepDist = Properties.Settings.Default.setF_minFixStep;

                        //Application.Exit();
                    }
                    catch (Exception e) //FormatException e || IndexOutOfRangeException e2)
                    {
                        WriteErrorLog("Open Vehicle" + e.ToString());

                        //vehicle is corrupt, reload with all default information
                        Properties.Settings.Default.Reset();
                        Properties.Settings.Default.Save();
                        MessageBox.Show("Program will Reset to Recover. Please Restart", "Vehicle file is Corrupt", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Application.Exit();
                    }
                }
            }      //cancelled out of open file
        }//end of open file

        //function to open a previously saved field, Contour, Flags, Boundary
        public void FileOpenField(string _openType)
        {
            string fileAndDirectory;
            //get the directory where the fields are stored
            //string directoryName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+ "\\fields\\";

            if (_openType == "Resume")
            {
                //Either exit or update running save
                fileAndDirectory = fieldsDirectory + currentFieldDirectory + "\\Field.txt";
                if (!File.Exists(fileAndDirectory)) return;
            }

            //open file dialog instead
            else
            {
                //create the dialog instance
                OpenFileDialog ofd = new OpenFileDialog();

                //the initial directory, fields, for the open dialog
                ofd.InitialDirectory = fieldsDirectory;

                //When leaving dialog put windows back where it was
                ofd.RestoreDirectory = true;

                //set the filter to text files only
                ofd.Filter = "Field files (Field.txt)|Field.txt";

                //was a file selected
                if (ofd.ShowDialog() == DialogResult.Cancel) return;
                else fileAndDirectory = ofd.FileName;
            }

            //close the existing job and reset everything
            this.JobClose();

            //and open a new job
            this.JobNew();

            /*
            2020 - September - 02 09:35:40 PM
            OpenGrade3D v1.0.1
            $FieldDir
            test optisurface 10m bnd Sep02
            $Offsets
            657908,4522604,14
            */

            //start to read the file
            string line;
            using (StreamReader reader = new StreamReader(fileAndDirectory))
            {
                try
                {
                    //Date time line
                    line = reader.ReadLine();

                    //Check the version
                    string[] words;
                    line = reader.ReadLine(); words = line.Split(',');

                    if (words[0] != "OpenGrade3D v1.0.1")
                    {
                        var form = new FormTimedMessage(5000, "Field is Wrong Version", "Must be OpenGrade3D v1.0.1");
                        form.Show();
                        JobClose();
                        return;
                    }

                    //dir header $FieldDir
                    line = reader.ReadLine();

                    //read field directory
                    line = reader.ReadLine();

                    currentFieldDirectory = line.Trim();

                    //Offset header
                    line = reader.ReadLine();

                    //read the Offsets 
                    line = reader.ReadLine();
                    string[] offs = line.Split(',');
                    pn.utmEast = int.Parse(offs[0]);
                    pn.utmNorth = int.Parse(offs[1]);
                    pn.zone = int.Parse(offs[2]);

                    //create a new grid
                    worldGrid.CreateWorldGrid(pn.actualNorthing - pn.utmNorth, pn.actualEasting - pn.utmEast);
                }

                catch (Exception e)
                {
                    WriteErrorLog("While Opening Field" + e.ToString());

                    var form = new FormTimedMessage(4000, "Field File is Corrupt", "Choose a different field");
                    form.Show();
                    JobClose();
                    return;
                }
            }

            // Contour points ----------------------------------------------------------------------------

            fileAndDirectory = fieldsDirectory + currentFieldDirectory + "\\Contour.txt";
            if (!File.Exists(fileAndDirectory))
            {
                var form = new FormTimedMessage(4000, "Missing Contour File", "But Field is Loaded");
                form.Show();
                return;
            }

            /*
                2020-September-09 04:42:11 PM
                easting, heading, northing, altitude, latitude, longitude, cutAltitude, lastPassAltitude, distance
                OpenGrade3D v1.0.1
                test optisurface 2m Sep07
                $Offsets
                657744,4522397,14
                $Position correction
                0.000,0.000,0.000
                35852
                -66.439,0,-132.687,418.856,40.83626581,-97.1298216,418.784,-1,-1             
             */
            else
            {
                using (StreamReader reader = new StreamReader(fileAndDirectory))
                {
                    try
                    {
                        //read the lines and skip them
                        line = reader.ReadLine();
                        line = reader.ReadLine();

                        //check the version
                        string[] words;
                        line = reader.ReadLine(); words = line.Split(',');
                       
                        if (words[0] != "OpenGrade3D v1.0.1")
                        {
                            var form = new FormTimedMessage(5000, "Contour.txt File is Wrong Version", "Must be OpenGrade3D v1.0.1");
                            form.Show();
                            return;
                        }
                        //read the lines and skip them
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();

                        //read the position corrections
                        line = reader.ReadLine(); words = line.Split(',');
                        pn.eastingOffset = double.Parse(words[0], CultureInfo.InvariantCulture);
                        pn.northingOffset = double.Parse(words[1], CultureInfo.InvariantCulture);
                        pn.altitudeOffset = double.Parse(words[2], CultureInfo.InvariantCulture);

                        while (!reader.EndOfStream)
                        {
                            //read how many vertices in the following patch
                            line = reader.ReadLine();
                            int verts = int.Parse(line);
                            //CContourPt vecFix = new vec4(0, 0, 0, 0);

                            for (int v = 0; v < verts; v++)
                            {
                                line = reader.ReadLine(); words = line.Split(',');

                                CContourPt point = new CContourPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    double.Parse(words[3], CultureInfo.InvariantCulture),
                                    double.Parse(words[4], CultureInfo.InvariantCulture),
                                    double.Parse(words[5], CultureInfo.InvariantCulture),
                                    double.Parse(words[6], CultureInfo.InvariantCulture),
                                    double.Parse(words[7], CultureInfo.InvariantCulture),
                                    double.Parse(words[8], CultureInfo.InvariantCulture));

                            ct.ptList.Add(point);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        WriteErrorLog("Loading Contour file" + e.ToString());

                        var form = new FormTimedMessage(4000, "Contour File is Corrupt", "But Field is Loaded");
                        form.Show();
                    }

                    //calc mins maxes
                    //CalculateMinMaxZoom();                 
                    //ct.mapList.Clear(); Not here only when opening an agd file.
                    //CalculateMinMaxEastNort(); Not here only when opening an agd file.
                    isOKtoOpenMap = true;
                }
            }

            // Map points ----------------------------------------------------------------------------

            if (isOKtoOpenMap)
            {
                isOKtoOpenMap = false;

                fileAndDirectory = fieldsDirectory + currentFieldDirectory + "\\MapPt.txt";
                if (!File.Exists(fileAndDirectory))
                {
                    var form = new FormTimedMessage(4000, "Missing Mapping File", "But Field is Loaded");
                    form.Show();
                    //return;
                }

                /*
                    May-14-17  -->  7:42:47 PM
                    Points in Patch followed by easting, heading, northing, altitude
                    $ContourDir
                    cdert_May14
                    $Offsets
                    533631,5927279,12
                    19
                    2.866,2.575,-4.07,0             
                 */
                else
                {
                    using (StreamReader reader = new StreamReader(fileAndDirectory))
                    {
                        try
                        {
                            //read the lines and skip them
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();

                            while (!reader.EndOfStream)
                            {
                                //read how many vertices in the following patch
                                line = reader.ReadLine();
                                int verts = int.Parse(line);
                                //CContourPt vecFix = new vec4(0, 0, 0, 0);

                                for (int v = 0; v < verts; v++)
                                {
                                    line = reader.ReadLine();
                                    string[] words = line.Split(',');

                                    mapListPt point = new mapListPt(
                                        double.Parse(words[0], CultureInfo.InvariantCulture),
                                        double.Parse(words[1], CultureInfo.InvariantCulture),
                                        double.Parse(words[2], CultureInfo.InvariantCulture),
                                        double.Parse(words[3], CultureInfo.InvariantCulture),
                                        double.Parse(words[4], CultureInfo.InvariantCulture),
                                        double.Parse(words[5], CultureInfo.InvariantCulture),
                                        double.Parse(words[6], CultureInfo.InvariantCulture),
                                        double.Parse(words[7], CultureInfo.InvariantCulture));

                                    ct.mapList.Add(point);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            WriteErrorLog("Loading Contour file" + e.ToString());

                            var form = new FormTimedMessage(4000, "MapPt File is Corrupt", "But Field is Loaded");
                            form.Show();
                        }


                    }
                }

                // Boundary points ----------------------------------------------------------------------------

                fileAndDirectory = fieldsDirectory + currentFieldDirectory + "\\BoundaryList.txt";
                if (!File.Exists(fileAndDirectory))
                {
                    var form = new FormTimedMessage(4000, "Missing Boundary Pts File", "But Field is Loaded");
                    form.Show();
                    //return;
                }

                /*
                    May-14-17  -->  7:42:47 PM
                    Points in Patch followed by easting, heading, northing, altitude
                    $ContourDir
                    cdert_May14
                    $Offsets
                    533631,5927279,12
                    19
                    2.866,2.575,-4.07,0             
                 */
                else
                {
                    using (StreamReader reader = new StreamReader(fileAndDirectory))
                    {
                        try
                        {
                            //read the lines and skip them
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();
                            line = reader.ReadLine();

                            while (!reader.EndOfStream)
                            {
                                //read how many vertices in the following patch
                                line = reader.ReadLine();
                                int verts = int.Parse(line);
                                //CContourPt vecFix = new vec4(0, 0, 0, 0);

                                for (int v = 0; v < verts; v++)
                                {
                                    line = reader.ReadLine();
                                    string[] words = line.Split(',');

                                    BoundaryPt point = new BoundaryPt(
                                        double.Parse(words[0], CultureInfo.InvariantCulture),
                                        double.Parse(words[1], CultureInfo.InvariantCulture),
                                        double.Parse(words[2], CultureInfo.InvariantCulture),
                                        double.Parse(words[3], CultureInfo.InvariantCulture),
                                        double.Parse(words[4], CultureInfo.InvariantCulture),
                                        double.Parse(words[5], CultureInfo.InvariantCulture),
                                        double.Parse(words[6], CultureInfo.InvariantCulture),
                                        double.Parse(words[7], CultureInfo.InvariantCulture));

                                    ct.boundaryList.Add(point);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            WriteErrorLog("Loading Contour file" + e.ToString());

                            var form = new FormTimedMessage(4000, "MapPt File is Corrupt", "But Field is Loaded");
                            form.Show();
                        }


                    }
                }
            }


            // Flags -------------------------------------------------------------------------------------------------

            //Either exit or update running save
            fileAndDirectory = fieldsDirectory + currentFieldDirectory + "\\Flags.txt";
            if (!File.Exists(fileAndDirectory))
            {
                var form = new FormTimedMessage(4000, "Missing Flags File", "But Field is Loaded");
                form.Show();
            }

            else
            {
                using (StreamReader reader = new StreamReader(fileAndDirectory))
                {
                    try
                    {
                        //read the lines and skip them
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();

                        line = reader.ReadLine();
                        int points = int.Parse(line);

                        if (points > 0)
                        {
                            double lat;
                            double longi;
                            double east;
                            double nort;
                            int color, ID;

                            for (int v = 0; v < points; v++)
                            {

                                line = reader.ReadLine();
                                string[] words = line.Split(',');

                                lat = double.Parse(words[0], CultureInfo.InvariantCulture);
                                longi = double.Parse(words[1], CultureInfo.InvariantCulture);
                                east = double.Parse(words[2], CultureInfo.InvariantCulture);
                                nort = double.Parse(words[3], CultureInfo.InvariantCulture);
                                color = int.Parse(words[4]);
                                ID = int.Parse(words[5]);

                                CFlag flagPt = new CFlag(lat, longi, east, nort, color, ID);
                                flagPts.Add(flagPt);
                            }

                        }
                    }

                    catch (Exception e)
                    {
                        var form = new FormTimedMessage(4000, "Flag File is Corrupt", "But Field is Loaded");
                        form.Show();
                        WriteErrorLog("FieldOpen, Loading Flags, Corrupt Flag File" + e.ToString());
                    }
                }
            }


            // ABLine -------------------------------------------------------------------------------------------------

            //Either exit or update running save
            fileAndDirectory = fieldsDirectory + currentFieldDirectory + "\\ABLine.txt";
            if (!File.Exists(fileAndDirectory))
            {
                var form = new FormTimedMessage(4000, "Missing ABLine File", "But Field is Loaded");
                form.Show();
            }

            else
            {
                using (StreamReader reader = new StreamReader(fileAndDirectory))
                {
                    try
                    {
                        //read the lines and skip them
                        line = reader.ReadLine();
                        line = reader.ReadLine();

                        line = reader.ReadLine();
                        bool isAB = bool.Parse(line);

                        if (isAB)
                        {
                            //set gui image button on
                            btnABLine.Image = global::OpenGrade.Properties.Resources.ABLineOn;

                            //Heading  , ,refPoint2x,z                    
                            line = reader.ReadLine();
                            ABLine.abHeading = double.Parse(line, CultureInfo.InvariantCulture);

                            //refPoint1x,z
                            line = reader.ReadLine();
                            string[] words = line.Split(',');
                            ABLine.refPoint1.easting = double.Parse(words[0], CultureInfo.InvariantCulture);
                            ABLine.refPoint1.northing = double.Parse(words[1], CultureInfo.InvariantCulture);

                            //refPoint2x,z
                            line = reader.ReadLine();
                            words = line.Split(',');
                            ABLine.refPoint2.easting = double.Parse(words[0], CultureInfo.InvariantCulture);
                            ABLine.refPoint2.northing = double.Parse(words[1], CultureInfo.InvariantCulture);

                            //Tramline
                            line = reader.ReadLine();
                            words = line.Split(',');
                            ABLine.tramPassEvery = int.Parse(words[0]);
                            ABLine.passBasedOn = int.Parse(words[1]);

                            ABLine.refABLineP1.easting = ABLine.refPoint1.easting - Math.Sin(ABLine.abHeading) * 10000.0;
                            ABLine.refABLineP1.northing = ABLine.refPoint1.northing - Math.Cos(ABLine.abHeading) * 10000.0;

                            ABLine.refABLineP2.easting = ABLine.refPoint1.easting + Math.Sin(ABLine.abHeading) * 10000.0;
                            ABLine.refABLineP2.northing = ABLine.refPoint1.northing + Math.Cos(ABLine.abHeading) * 10000.0;

                            ABLine.isABLineSet = true;
                        }
                    }

                    catch (Exception e)
                    {
                        var form = new FormTimedMessage(4000, "AB Line File is Corrupt", "But Field is Loaded");
                        form.Show();
                        WriteErrorLog("Load AB Line" + e.ToString());

                    }
                }
            }
        }//end of open file

        //creates the field file when starting new field
        public void FileCreateField()
        {
            /*
            2020 - September - 02 09:35:40 PM
            OpenGrade3D v1.0.1
            $FieldDir
            test optisurface 10m bnd Sep02
            $Offsets
            657908,4522604,14
            */

            if (!isJobStarted)
            {
                using (var form = new FormTimedMessage(3000, "Ooops, Job Not Started", "Start a Job First"))
                { form.Show(); }
                return;
            }
            string myFileName, dirField;

            //get the directory and make sure it exists, create if not
            dirField = fieldsDirectory + currentFieldDirectory + "\\";
            string directoryName = Path.GetDirectoryName(dirField);

            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            myFileName = "Field.txt";

            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {
                //Write out the date
                writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                //Write the version
                writer.WriteLine("OpenGrade3D v1.0.1");                
                writer.WriteLine("$FieldDir");
                writer.WriteLine(currentFieldDirectory.ToString(CultureInfo.InvariantCulture));

                //write out the easting and northing Offsets
                writer.WriteLine("$Offsets");
                writer.WriteLine(pn.utmEast.ToString(CultureInfo.InvariantCulture) + "," + 
                    pn.utmNorth.ToString(CultureInfo.InvariantCulture) + "," + 
                    pn.zone.ToString(CultureInfo.InvariantCulture));
            }

        }

        //save field Patches
        public void FileSaveField()
        {
            //make sure there is something to save
            if (patchSaveList.Count() > 0)
            {
                //Append the current list to the field file
                using (StreamWriter writer = new StreamWriter((fieldsDirectory + currentFieldDirectory + "\\Field.txt"), true))
                {
                    //for each patch, write out the list of triangles to the file
                    foreach (var triList in patchSaveList)
                    {
                        int count2 = triList.Count();
                        writer.WriteLine(count2.ToString(CultureInfo.InvariantCulture));

                        for (int i = 0; i < count2; i++)
                            writer.WriteLine(triList[i].easting.ToString(CultureInfo.InvariantCulture) +
                                "," + triList[i].northing.ToString(CultureInfo.InvariantCulture));
                    }
                }

                //clear out that patchList and begin adding new ones for next save
                patchSaveList.Clear();
            }
        }

        //Create contour file, not used?
        public void FileCreateContour()
        {
            /*
                2020-September-09 04:42:11 PM
                easting, heading, northing, altitude, latitude, longitude, cutAltitude, lastPassAltitude, distance
                $ContourDir
                test optisurface 2m Sep07
                $Offsets
                657744,4522397,14
                35852
                -66.439,0,-132.687,418.856,40.83626581,-97.1298216,418.784,-1,-1
             */


            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName = "Contour.txt";

            //write out the file
            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {
                //Write out the date
                writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                writer.WriteLine("Points in Patch followed by easting, heading, northing, altitude");

                //which field directory
                writer.WriteLine("$ContourDir");
                writer.WriteLine(currentFieldDirectory);

                //write out the easting and northing Offsets
                writer.WriteLine("$Offsets");
                writer.WriteLine(pn.utmEast.ToString(CultureInfo.InvariantCulture) + 
                    "," + pn.utmNorth.ToString(CultureInfo.InvariantCulture) + "," + pn.zone.ToString(CultureInfo.InvariantCulture));
            }
        }

        #region Load Optisuface .agd file

        //Load the Optisuface design file, by Pat

        public void FileOpenAgdDesign()
        {
            ct.designList.Clear();

            OpenFileDialog ofd = new OpenFileDialog();

            //get the directory where the fields are stored
            string directoryName = fieldsDirectory;

            //make sure the directory exists, if not, create it
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            //the initial directory, fields, for the open dialog
            ofd.InitialDirectory = directoryName;

            //When leaving dialog put windows back where it was
            ofd.RestoreDirectory = true;

            //set the filter to agd files only
            ofd.Filter = "agd files (*.agd)|*.agd";

            //was a file selected
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //if job started close it
                //if (isJobStarted) JobClose();

                //make sure the file if fully valid and vehicle matches sections
                string line;
                using (StreamReader reader = new StreamReader(ofd.FileName))
                {
                    try
                    {
                        //read the lines and skip them
                        line = reader.ReadLine();
                        

                        while (!reader.EndOfStream)
                        {
                            //read how many vertices in the following patch
                            //line = reader.ReadLine();
                            //int verts = int.Parse(line);
                            //CContourPt vecFix = new vec4(0, 0, 0, 0);

                            //for (int v = 0; v < verts; v++)
                            {
                                line = reader.ReadLine();
                                string[] words = line.Split(',');

                                if (words[5] == "MB" | words[5] == " MB" | words[5] == "0MB" | words[5] == " 0MB")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    0, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2PER" | words[5] == " 2PER")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    2, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE1" | words[5] == "2SUBZONE1")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    21, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE2" | words[5] == "2SUBZONE2")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    22, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE3" | words[5] == "2SUBZONE3")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    23, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE4" | words[5] == "2SUBZONE4")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    24, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE5" | words[5] == "2SUBZONE5")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    25, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE6" | words[5] == "2SUBZONE6")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    26, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE7" | words[5] == "2SUBZONE7")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    27, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE8" | words[5] == "2SUBZONE8")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    28, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "2SUBZONE9" | words[5] == "2SUBZONE9")
                                {
                                    designPt point = new designPt(
                                    double.Parse(words[0], CultureInfo.InvariantCulture),
                                    double.Parse(words[1], CultureInfo.InvariantCulture),
                                    double.Parse(words[2], CultureInfo.InvariantCulture),
                                    -1,
                                    -1,
                                    29, 0, 0
                                    );

                                    ct.designList.Add(point);
                                }

                                if (words[5] == "3GRD" | words[5] == " 3GRD")
                                {
                                    //if (!String.IsNullOrEmpty(words[2]) && !String.IsNullOrEmpty(words[3]) && !String.IsNullOrEmpty(words[4]))
                                    //{


                                        designPt point = new designPt(
                                        double.Parse(words[0], CultureInfo.InvariantCulture),
                                        double.Parse(words[1], CultureInfo.InvariantCulture),
                                        double.Parse(words[2], CultureInfo.InvariantCulture),
                                        double.Parse(words[3], CultureInfo.InvariantCulture),
                                        double.Parse(words[4], CultureInfo.InvariantCulture),
                                        3, 0, 0
                                        );

                                        ct.designList.Add(point);
                                    //}

                                }
                                

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        WriteErrorLog("Loading Design file" + e.ToString());

                        var form = new FormTimedMessage(4000, "Design File is Corrupt", "But Field is Loaded");
                        form.Show();
                    }

                    //calc mins maxes
                    //CalculateMinMaxZoom();
                    //CalculateTotalCutFillLabels();
                    //ct.mapList.Clear();
                    //CalculateMinMaxEastNort();
                }


                FileSaveDesignList(); // for testing
                ct.designList2ptList();

            }
            //cancelled out of open file

            
            
        }//end of open file

        #endregion

        //save the design (the converted agd pts) list for testing only
        public void FileSaveDesignList()
        {
            //1  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //Saturday, February 11, 2017  -->  7:26:52 AM
            //12  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //$ContourDir
            //Bob_Feb11
            //$Offsets
            //533172,5927719,12

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName = "DesignList.txt";

            //write out the file
            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {
                //Write out the date
                writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                writer.WriteLine("Latitude (deg), Longitude (deg), Elevation Existing(m), Elevation Proposed(m), CutFill(m), Code, easting");

                //which field directory
                writer.WriteLine("$DesignListDir");
                writer.WriteLine(currentFieldDirectory);

                //write out the easting and northing Offsets
                writer.WriteLine("$Offsets");
                writer.WriteLine(pn.utmEast.ToString(CultureInfo.InvariantCulture) +
                    "," + pn.utmNorth.ToString(CultureInfo.InvariantCulture) + "," + pn.zone.ToString(CultureInfo.InvariantCulture));


                //make sure there is something to save
                if (ct.designList.Count() > 0)
                {
                    int count3 = ct.designList.Count;

                    //for every new chunk of patch in the whole section

                    writer.WriteLine(count3.ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < count3; i++)
                    {
                        writer.WriteLine(Math.Round((ct.designList[i].latitude), 9).ToString(CultureInfo.InvariantCulture) + "," +

                            Math.Round(ct.designList[i].longitude, 9).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.designList[i].altitude, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.designList[i].cutAltitude, 3).ToString(CultureInfo.InvariantCulture) + "," +

                            Math.Round(ct.designList[i].cutfill, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.designList[i].code, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.designList[i].easting, 3).ToString(CultureInfo.InvariantCulture));

                    }
                }
            }
            //set saving flag off
            //isSavingFile = false;
        }

        //save the contour points which include elevation values
        public void FileSaveContour()
        {
            /*
                2020-September-09 04:42:11 PM
                easting, heading, northing, altitude, latitude, longitude, cutAltitude, lastPassAltitude, distance
                OpenGrade3D v1.0.1
                test optisurface 2m Sep07
                $Offsets
                657744,4522397,14
                $Position correction
                0.000,0.000,0.000
                35852
                -66.439,0,-132.687,418.856,40.83626581,-97.1298216,418.784,-1,-1
              
             */

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName = "Contour.txt";

            //write out the file
            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {
                //Write out the date
                writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                writer.WriteLine("easting, heading, northing, altitude, latitude, longitude, cutAltitude, lastPassAltitude, distance");

                //which field directory
                writer.WriteLine("OpenGrade3D v1.0.1");
                writer.WriteLine(currentFieldDirectory);

                //write out the easting and northing Offsets
                writer.WriteLine("$Offsets");
                writer.WriteLine(pn.utmEast.ToString(CultureInfo.InvariantCulture) +
                    "," + pn.utmNorth.ToString(CultureInfo.InvariantCulture) + "," + pn.zone.ToString(CultureInfo.InvariantCulture));

                //write the position correction offset
                writer.WriteLine("$Position correction");
                writer.WriteLine(Math.Round(pn.eastingOffset, 3).ToString(CultureInfo.InvariantCulture) +
                    "," + Math.Round(pn.northingOffset, 3).ToString(CultureInfo.InvariantCulture) + "," + Math.Round(pn.altitudeOffset, 3).ToString(CultureInfo.InvariantCulture));

                //make sure there is something to save
                if (ct.ptList.Count() > 0)
                {
                    int count2 = ct.ptList.Count;

                    //for every new chunk of patch in the whole section

                    writer.WriteLine(count2.ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < count2; i++)
                    {
                        writer.WriteLine(Math.Round((ct.ptList[i].easting), 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].heading, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].northing, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].altitude, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].latitude, 9).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].longitude, 9).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].cutAltitude, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].lastPassAltitude, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.ptList[i].distance, 3).ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
            //set saving flag off
            isSavingFile = false;
        }

        //save the boundary points which include elevation values
        public void FileSaveBoundaryList()
        {
            //1  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //Saturday, February 11, 2017  -->  7:26:52 AM
            //12  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //$ContourDir
            //Bob_Feb11
            //$Offsets
            //533172,5927719,12

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName = "BoundaryList.txt";

            //write out the file
            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {
                //Write out the date
                writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                writer.WriteLine("easting, heading, northing, altitude, latitude, longitude, cutAltitude, code");

                //which field directory
                writer.WriteLine("$BoundaryDir");
                writer.WriteLine(currentFieldDirectory);

                //write out the easting and northing Offsets
                writer.WriteLine("$Offsets");
                writer.WriteLine(pn.utmEast.ToString(CultureInfo.InvariantCulture) +
                    "," + pn.utmNorth.ToString(CultureInfo.InvariantCulture) + "," + pn.zone.ToString(CultureInfo.InvariantCulture));


                //make sure there is something to save
                if (ct.boundaryList.Count() > 0)
                {
                    int count2 = ct.boundaryList.Count;

                    //for every new chunk of patch in the whole section

                    writer.WriteLine(count2.ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < count2; i++)
                    {
                        writer.WriteLine(Math.Round((ct.boundaryList[i].easting), 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.boundaryList[i].heading, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.boundaryList[i].northing, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.boundaryList[i].altitude, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.boundaryList[i].latitude, 9).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.boundaryList[i].longitude, 9).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.boundaryList[i].cutAltitude, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.boundaryList[i].code, 0).ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
            //set saving flag off
            //isSavingFile = false;
        }

        //save the contour (mapping) points which include elevation values
        public void FileSaveMapPt()
        {
            //1  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //Saturday, February 11, 2017  -->  7:26:52 AM
            //12  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //$ContourDir
            //Bob_Feb11
            //$Offsets
            //533172,5927719,12

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName = "MapPt.txt";

            //write out the file
            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {
                //Write out the date
                writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                writer.WriteLine("easting, northing, pts distance, Elevation Existing(m), Elevation Proposed(m), CutFill(m), last pass altitude, saved blade altitude ");

                //which field directory
                writer.WriteLine("$MapPtDir");
                writer.WriteLine(currentFieldDirectory);

                //write out the easting and northing Offsets
                writer.WriteLine("$Offsets");
                writer.WriteLine(pn.utmEast.ToString(CultureInfo.InvariantCulture) +
                    "," + pn.utmNorth.ToString(CultureInfo.InvariantCulture) + "," + pn.zone.ToString(CultureInfo.InvariantCulture));


                //make sure there is something to save
                if (ct.mapList.Count() > 0)
                {
                    int count3 = ct.mapList.Count;

                    //for every new chunk of patch in the whole section

                    writer.WriteLine(count3.ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < count3; i++)
                    {
                        writer.WriteLine(Math.Round((ct.mapList[i].eastingMap), 3).ToString(CultureInfo.InvariantCulture) + "," +

                            Math.Round(ct.mapList[i].northingMap, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.mapList[i].drawPtWidthMap, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.mapList[i].altitudeMap, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            
                            Math.Round(ct.mapList[i].cutAltitudeMap, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.mapList[i].cutDeltaMap, 3).ToString(CultureInfo.InvariantCulture) + "," +
                            Math.Round(ct.mapList[i].lastPassAltitudeMap, 3).ToString(CultureInfo.InvariantCulture) + "," + 
                            Math.Round(ct.mapList[i].lastPassRealAltitudeMap, 3).ToString(CultureInfo.InvariantCulture));
                            
                    }
                }
            }
            //set saving flag off
            //isSavingFile = false;
        }


        //save the contour points which include elevation values in a optisurface compatible ags file.
        public void FileSaveSurveyPt()
        {
            //1  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //Saturday, February 11, 2017  -->  7:26:52 AM
            //12  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //$ContourDir
            //Bob_Feb11
            //$Offsets
            //533172,5927719,12

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName = "Survey.ags";

            //write out the file
            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {
                


                //make sure there is something to save
                if (ct.surveyList.Count() > 0)
                {
                    int count4 = ct.surveyList.Count;

                    //for every new chunk of patch in the whole section

                    //writer.WriteLine(count4.ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < count4; i++)
                    {
                        if (ct.surveyList[i].code == 0)
                        {
                            writer.WriteLine(Math.Round((ct.surveyList[i].latitude), 9).ToString(CultureInfo.InvariantCulture) + ", " +

                            Math.Round(ct.surveyList[i].longitude, 9).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].altitude, 3).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].code, 0).ToString(CultureInfo.InvariantCulture) + "mb_4g");

                        }

                        if (ct.surveyList[i].code == 2)
                        {
                            writer.WriteLine(Math.Round((ct.surveyList[i].latitude), 9).ToString(CultureInfo.InvariantCulture) + ", " +

                            Math.Round(ct.surveyList[i].longitude, 9).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].altitude, 3).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].code, 0).ToString(CultureInfo.InvariantCulture) + "PER");

                        }

                        if (ct.surveyList[i].code == 3)
                        {
                            writer.WriteLine(Math.Round((ct.surveyList[i].latitude), 9).ToString(CultureInfo.InvariantCulture) + ", " +

                            Math.Round(ct.surveyList[i].longitude, 9).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].altitude, 3).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].code, 0).ToString(CultureInfo.InvariantCulture) + "GRD");

                        }



                    }
                }
            }
            FileSaveSurveyPt2text();
            //set saving flag off
            //isSavingFile = false;
        }

        //save the survey points for testing (check easting/northing to lat/lon conversion
        public void FileSaveSurveyPt2text()
        {
            //1  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //Saturday, February 11, 2017  -->  7:26:52 AM
            //12  - points in patch
            //64.697,0.168,-21.654,0 - east, heading, north, altitude
            //$ContourDir
            //Bob_Feb11
            //$Offsets
            //533172,5927719,12

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName = "Survey.txt";

            //write out the file
            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {



                //make sure there is something to save
                if (ct.surveyList.Count() > 0)
                {
                    int count4 = ct.surveyList.Count;

                    //for every new chunk of patch in the whole section
                    writer.WriteLine("easting, northing, latitude, longitude, altitude, code, fixQuality");
                    //writer.WriteLine(count4.ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < count4; i++)
                    {
                       
                            writer.WriteLine(Math.Round((ct.surveyList[i].easting), 3).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round((ct.surveyList[i].northing), 3).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round((ct.surveyList[i].latitude), 9).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].longitude, 9).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].altitude, 3).ToString(CultureInfo.InvariantCulture) + ", " +
                            Math.Round(ct.surveyList[i].code, 0).ToString(CultureInfo.InvariantCulture) + ", " + 
                            (ct.surveyList[i].fixQuality).ToString(CultureInfo.InvariantCulture));
                       
                    }
                }
            }

            //set saving flag off
            //isSavingFile = false;
        }



        //save all the flag markers
        public void FileSaveFlags()
        {
            //Saturday, February 11, 2017  -->  7:26:52 AM
            //$FlagsDir
            //Bob_Feb11
            //$Offsets
            //533172,5927719,12 - offset easting, northing, zone

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            //use Streamwriter to create and overwrite existing flag file
            using (StreamWriter writer = new StreamWriter(dirField + "Flags.txt"))
            {
                try
                {
                    //Write out the date time
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));

                    //which field directory
                    writer.WriteLine("$FlagsDir");
                    writer.WriteLine(currentFieldDirectory);

                    //write out the easting and northing Offsets
                    writer.WriteLine("$Offsets");
                    writer.WriteLine(pn.utmEast.ToString(CultureInfo.InvariantCulture) + "," +
                                pn.utmNorth.ToString(CultureInfo.InvariantCulture) + "," + pn.zone.ToString(CultureInfo.InvariantCulture));

                    int count2 = flagPts.Count;

                    writer.WriteLine(count2);

                    for (int i = 0; i < count2; i++)
                    {
                        writer.WriteLine(
                            flagPts[i].latitude.ToString(CultureInfo.InvariantCulture) + "," +
                            flagPts[i].longitude.ToString(CultureInfo.InvariantCulture) + "," +
                            flagPts[i].easting.ToString(CultureInfo.InvariantCulture) + "," +
                            flagPts[i].northing.ToString(CultureInfo.InvariantCulture) + "," +
                            flagPts[i].color.ToString(CultureInfo.InvariantCulture) + "," +
                            flagPts[i].ID.ToString(CultureInfo.InvariantCulture));
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n Cannot write to file.");
                    WriteErrorLog("Saving Flags" + e.ToString());

                    return;
                }

            }
        }

        //save all the flag markers
        public void FileSaveABLine()
        {
            //Saturday, February 11, 2017  -->  7:26:52 AM

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            //use Streamwriter to create and overwrite existing ABLine file
            using (StreamWriter writer = new StreamWriter(dirField + "ABLine.txt"))
            {
                try
                {
                    //Write out the date time
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));

                    //write out the ABLine
                    writer.WriteLine("$Heading");

                    //true or false if ABLine is set
                    if (ABLine.isABLineSet) writer.WriteLine(true);
                    else writer.WriteLine(false);

                    writer.WriteLine(ABLine.abHeading.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(ABLine.refPoint1.easting.ToString(CultureInfo.InvariantCulture) + "," + ABLine.refPoint1.northing.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(ABLine.refPoint2.easting.ToString(CultureInfo.InvariantCulture) + "," + ABLine.refPoint2.northing.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(ABLine.tramPassEvery.ToString(CultureInfo.InvariantCulture) + "," + ABLine.passBasedOn.ToString(CultureInfo.InvariantCulture));
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n Cannot write to file.");
                    WriteErrorLog("Saving AB Line" + e.ToString());

                    return;
                }

            }
        }

        //save nmea sentences
        public void FileSaveNMEA()
        {
            using (StreamWriter writer =  new StreamWriter((fieldsDirectory + currentFieldDirectory + "\\NMEA_log.txt"), true))
            {
                writer.Write(pn.logNMEASentence.ToString());
            }
            pn.logNMEASentence.Clear();
        }

        //generate KML file from flags
        public void FileSaveFlagsKML()
        {

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName;
            myFileName = "Flags.kml";

            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {

                writer.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>     ");
                writer.WriteLine(@"<kml xmlns=""http://www.opengis.net/kml/2.2""> ");

                int count2 = flagPts.Count;

                writer.WriteLine(@"<Document>");

                for (int i = 0; i < count2; i++)
                {
                    writer.WriteLine(@"  <Placemark>                                  ");
                    writer.WriteLine(@"<Style> <IconStyle>");
                    if (flagPts[i].color == 0)  //red - xbgr
                        writer.WriteLine(@"<color>ff4400ff</color>");
                    if (flagPts[i].color == 1)  //grn - xbgr
                        writer.WriteLine(@"<color>ff44ff00</color>");
                    if (flagPts[i].color == 2)  //yel - xbgr
                        writer.WriteLine(@"<color>ff44ffff</color>");

                    writer.WriteLine(@"</IconStyle> </Style>");
                    writer.WriteLine(@" <name> " + (i + 1) + @"</name>");
                    writer.WriteLine(@"<Point><coordinates> " +
                                    flagPts[i].longitude.ToString(CultureInfo.InvariantCulture) + "," + flagPts[i].latitude.ToString(CultureInfo.InvariantCulture) + ",0" +
                                    @"</coordinates> </Point> ");
                    writer.WriteLine(@"  </Placemark>                                 ");

                }

                writer.WriteLine(@"</Document>");

                writer.WriteLine(@"</kml>                                         ");
            }

        }

        public void FileSaveCutKML()
        {

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName;
            myFileName = "Cut.kml";

            int cnt = ct.ptList.Count;
            
            using (StreamWriter sw = new StreamWriter(dirField + myFileName))
            {
                sw.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                sw.WriteLine(@"<kml xsi:schemaLocation=""http://earth.google.com/kml/2.1 http://earth.google.com/kml2.1.xsd"" xmlns=""http://earth.google.com/kml/2.1"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">");
                sw.WriteLine(@"  <Placemark>");
                sw.Write(@"    <name>");
                sw.Write(currentFieldDirectory);
                sw.WriteLine(@" </name> ");
                sw.WriteLine(@"    <Style><LineStyle><color>FFFF00FF</color><width>3.0</width></LineStyle></Style>");
                sw.WriteLine(@"    <LineString><extrude>false</extrude><tessellate>true</tessellate><altitudeMode>clampToGround</altitudeMode>");
                sw.WriteLine(@"       <coordinates>");

                if (cnt > 0)
                {
                    for (int i = 0; i < cnt; i++)
                        sw.Write(Convert.ToString(ct.ptList[i].longitude) + ',' + Convert.ToString(ct.ptList[i].latitude) + ",0 ");
                }
                else sw.Write(Convert.ToString(pn.longitude) + ',' + Convert.ToString(pn.latitude) + ",0 ");

                sw.WriteLine(@"       </coordinates>");
                sw.WriteLine(@"    </LineString>");
                sw.WriteLine(@"</Placemark>");
                sw.WriteLine(@"</kml>");
            }
        }

        //generate KML file from flag
        public void FileSaveSingleFlagKML(int flagNumber)
        {

            //get the directory and make sure it exists, create if not
            string dirField = fieldsDirectory + currentFieldDirectory + "\\";

            string directoryName = Path.GetDirectoryName(dirField);
            if ((directoryName.Length > 0) && (!Directory.Exists(directoryName)))
            { Directory.CreateDirectory(directoryName); }

            string myFileName;
            myFileName = "Flag.kml";

            using (StreamWriter writer = new StreamWriter(dirField + myFileName))
            {

                writer.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>     ");
                writer.WriteLine(@"<kml xmlns=""http://www.opengis.net/kml/2.2""> ");

                int count2 = flagPts.Count;

                writer.WriteLine(@"<Document>");

                    writer.WriteLine(@"  <Placemark>                                  ");
                    writer.WriteLine(@"<Style> <IconStyle>");
                    if (flagPts[flagNumber - 1].color == 0)  //red - xbgr
                        writer.WriteLine(@"<color>ff4400ff</color>");
                    if (flagPts[flagNumber - 1].color == 1)  //grn - xbgr
                        writer.WriteLine(@"<color>ff44ff00</color>");
                    if (flagPts[flagNumber - 1].color == 2)  //yel - xbgr
                        writer.WriteLine(@"<color>ff44ffff</color>");
                    writer.WriteLine(@"</IconStyle> </Style>");
                    writer.WriteLine(@" <name> " + flagNumber.ToString(CultureInfo.InvariantCulture) + @"</name>");
                    writer.WriteLine(@"<Point><coordinates> " +
                                    flagPts[flagNumber-1].longitude.ToString(CultureInfo.InvariantCulture) + "," + flagPts[flagNumber-1].latitude.ToString(CultureInfo.InvariantCulture) + ",0" +
                                    @"</coordinates> </Point> ");
                    writer.WriteLine(@"  </Placemark>                                 ");
                writer.WriteLine(@"</Document>");
                writer.WriteLine(@"</kml>                                         ");

            }
        }
    }
}