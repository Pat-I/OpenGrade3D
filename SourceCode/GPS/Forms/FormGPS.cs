﻿//Please, if you use this, share the improvements

using OpenGrade.Properties;
using SharpGL;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace OpenGrade
{
    //the main form object
    public partial class FormGPS : Form
    {
        #region // Class Props and instances

        //The base directory where OpenGrade will be stored and fields and vehicles branch from
        public string baseDirectory;

        //current directory of vehicle
        public string vehiclesDirectory, vehiclefileName = "";

        //current fields and field directory
        public string fieldsDirectory, currentFieldDirectory;

        //ABLines directory
        public string ablinesDirectory;

        //colors for sections and field background
        private byte redSections, grnSections, bluSections;

        public byte redField, grnField, bluField;

        //colors for mapping
        //Fill and lowest altitude  default
        public byte redFill, grnFill, bluFill;
        //Fill and lowest altitude  default(mid)
        public byte redFillMid, grnFillMid, bluFillMid;
        //Fill and lowest altitude  default(min)
        public byte redFillMin, grnFillMin, bluFillMin;
        // center color
        public byte redCenter, grnCenter, bluCenter;
        // Cut and highest altitude default(min)
        public byte redCutMin, grnCutMin, bluCutMin;
        // Cut and highest altitude default(mid)
        public byte redCutMid, grnCutMid, bluCutMid;
        // Cut and highest altitude default
        public byte redCut, grnCut, bluCut;

        //polygon mode for section drawing
        private bool isDrawPolygons;

        //flag for free drive window to control autosteer
        public bool isInFreeDriveMode;

        //Flag stuff
        public byte flagColor = 0;

        private bool leftMouseDownOnOpenGL = false; //mousedown event in opengl window
        private int flagNumberPicked = 0;

        //Is it in 2D or 3D, metric or imperial, display lightbar, display grid etc
        public bool isIn3D = true, isMetric = true, isLightbarOn = true, isGridOn, isSideGuideLines = true, isGradual = false, isGradualMulticolor = true;

        public bool isPureDisplayOn = true, isSkyOn = true, isBigAltitudeOn = false;

        //bool for whether or not a job is active
        public bool isJobStarted = false, isAreaOnRight = true, isAutoSteerBtnOn = false;

        // Manual, 3 states possible
        public enum btnStates { Off, Rec, RecBnd, Work, StandBy }

        public btnStates manualBtnState = btnStates.Off;

        //if we are saving a file
        public bool isSavingFile = false, isLogNMEA = false, isOKtoOpenMap = false;

        //Zoom variables
        public double gridZoom;

        public double zoomValue = 10;
        public double triangleResolution = 1.0;
        private double previousZoom = 25;
        public uint[] texture = new uint[3];

        //create the scene camera
        public CCamera camera = new CCamera();

        //create world grid
        public CWorldGrid worldGrid;

        //create instance of a stopwatch for timing of frames and NMEA hz determination
        private readonly Stopwatch swFrame = new Stopwatch();

        //Time to do fix position update and draw routine
        private double frameTime = 0;

        //For field saving in background
        private int saveCounter = 1;

        //used to update the screen status bar etc
        private int statusUpdateCounter = 1;

        private int fiveSecondCounter = 0;

        //used in altitude window for gain
        private double altitudeWindowGain = 10.0;

        private double barGraphMax = 10;

        /// <summary>
        /// The NMEA class that decodes it
        /// </summary>
        public CNMEA pn;

        /// <summary>
        /// AB Line object
        /// </summary>
        public CABLine ABLine;

        /// <summary>
        /// Contour Mode Instance
        /// </summary>
        public CContour ct;

        /// <summary>
        /// Our vehicle including the tool
        /// </summary>
        public CVehicle vehicle;

        /// <summary>
        /// All the structs for recv and send of information out ports
        /// </summary>
        public CModuleComm mc;

        /// <summary>
        /// The internal simulator
        /// </summary>
        public CSim sim;

        /// <summary>
        /// Resource manager for gloabal strings
        /// </summary>
        public ResourceManager _rm;

        #endregion // Class Props and instances

        #region Gesture

        // Private variables used to maintain the state of gestures
        //private DrawingObject _dwo = new DrawingObject();
        private Point _ptFirst = new Point();

        //for the NTRIP CLient counting
        private int ntripCounter = 10;

        private Point _ptSecond = new Point();
        private int _iArguments = 0;

        // One of the fields in GESTUREINFO structure is type of Int64 (8 bytes).
        // The relevant gesture information is stored in lower 4 bytes. This
        // bit mask is used to get 4 lower bytes from this argument.
        private const Int64 ULL_ARGUMENTS_BIT_MASK = 0x00000000FFFFFFFF;

        //-----------------------------------------------------------------------
        // Multitouch/Touch glue (from winuser.h file)
        // Since the managed layer between C# and WinAPI functions does not
        // exist at the moment for multi-touch related functions this part of
        // code is required to replicate definitions from winuser.h file.
        //-----------------------------------------------------------------------
        // Touch event window message constants [winuser.h]
        private const int WM_GESTURENOTIFY = 0x011A;

        private const int WM_GESTURE = 0x0119;

        private const int GC_ALLGESTURES = 0x00000001;

        // Gesture IDs
        private const int GID_BEGIN = 1;

        private const int GID_END = 2;
        private const int GID_ZOOM = 3;
        private const int GID_PAN = 4;
        private const int GID_ROTATE = 5;
        private const int GID_TWOFINGERTAP = 6;
        private const int GID_PRESSANDTAP = 7;

        // Gesture flags - GESTUREINFO.dwFlags
        private const int GF_BEGIN = 0x00000001;

        private const int GF_INERTIA = 0x00000002;
        private const int GF_END = 0x00000004;

        //
        // Gesture configuration structure
        //   - Used in SetGestureConfig and GetGestureConfig
        //   - Note that any setting not included in either GESTURECONFIG.dwWant
        //     or GESTURECONFIG.dwBlock will use the parent window's preferences
        //     or system defaults.
        //
        // Touch API defined structures [winuser.h]
        [StructLayout(LayoutKind.Sequential)]
        private struct GESTURECONFIG
        {
            public int dwID;    // gesture ID
            public int dwWant;  // settings related to gesture ID that are to be

                                // turned on
            public int dwBlock; // settings related to gesture ID that are to be

                                // turned off
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTS
        {
            public short x;
            public short y;
        }

        private void btnUseSavedAGS_Click(object sender, EventArgs e)
        {
            int ctn = ct.surveyList.Count;
            double code = ct.surveyList[(ctn - 1)].code;

            if (code == 2 || code == 0)
            {
                recordSurveyBoundary();
                ct.recBoundary = true;
            }
            if (code == 3) recordSurveyPts();

            btnUseSavedAGS.Visible = false;
        }

        //
        // Gesture information structure
        //   - Pass the HGESTUREINFO received in the WM_GESTURE message lParam
        //     into the GetGestureInfo function to retrieve this information.
        //   - If cbExtraArgs is non-zero, pass the HGESTUREINFO received in
        //     the WM_GESTURE message lParam into the GetGestureExtraArgs
        //     function to retrieve extended argument information.
        //
        [StructLayout(LayoutKind.Sequential)]
        private struct GESTUREINFO
        {
            public int cbSize;           // size, in bytes, of this structure

                                         // (including variable length Args
                                         // field)
            public int dwFlags;          // see GF_* flags

            public int dwID;             // gesture ID, see GID_* defines
            public IntPtr hwndTarget;    // handle to window targeted by this

                                         // gesture
            [MarshalAs(UnmanagedType.Struct)]
            internal POINTS ptsLocation; // current location of this gesture

            public int dwInstanceID;     // internally used
            public int dwSequenceID;     // internally used
            public Int64 ullArguments;   // arguments for gestures whose

                                         // arguments fit in 8 BYTES
            public int cbExtraArgs;      // size, in bytes, of extra arguments,

                                         // if any, that accompany this gesture
        }

        // Currently touch/multitouch access is done through unmanaged code
        // We must p/invoke into user32 [winuser.h]
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetGestureConfig(IntPtr hWnd, int dwReserved, int cIDs, ref GESTURECONFIG pGestureConfig, int cbSize);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

        // size of GESTURECONFIG structure
        private int _gestureConfigSize;

        // size of GESTUREINFO structure
        private int _gestureInfoSize;

        [SecurityPermission(SecurityAction.Demand)]
        private void SetupStructSizes()
        {
            // Both GetGestureCommandInfo and GetTouchInputInfo need to be
            // passed the size of the structure they will be filling
            // we get the sizes upfront so they can be used later.
            _gestureConfigSize = Marshal.SizeOf(new GESTURECONFIG());
            _gestureInfoSize = Marshal.SizeOf(new GESTUREINFO());
        }

        //-------------------------------------------------------------
        // Since there is no managed layer at the moment that supports
        // event handlers for WM_GESTURENOTIFY and WM_GESTURE
        // messages we have to override WndProc function
        //
        // in
        //   m - Message object
        //-------------------------------------------------------------
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            bool handled = false;

            switch (m.Msg)
            {
                case WM_GESTURENOTIFY:
                    {
                        // This is the right place to define the list of gestures
                        // that this application will support. By populating
                        // GESTURECONFIG structure and calling SetGestureConfig
                        // function. We can choose gestures that we want to
                        // handle in our application. In this app we decide to
                        // handle all gestures.
                        GESTURECONFIG gc = new GESTURECONFIG
                        {
                            dwID = 0,                // gesture ID
                            dwWant = GC_ALLGESTURES, // settings related to gesture
                                                     // ID that are to be turned on
                            dwBlock = 0 // settings related to gesture ID that are
                        };
                        // to be

                        // We must p/invoke into user32 [winuser.h]
                        bool bResult = SetGestureConfig(
                            Handle, // window for which configuration is specified
                            0,      // reserved, must be 0
                            1,      // count of GESTURECONFIG structures
                            ref gc, // array of GESTURECONFIG structures, dwIDs
                                    // will be processed in the order specified
                                    // and repeated occurances will overwrite
                                    // previous ones
                            _gestureConfigSize // sizeof(GESTURECONFIG)
                        );

                        if (!bResult)
                        {
                            throw new Exception("Error in execution of SetGestureConfig");
                        }
                    }
                    handled = true;
                    break;

                case WM_GESTURE:
                    // The gesture processing code is implemented in
                    // the DecodeGesture method
                    handled = DecodeGesture(ref m);
                    break;

                default:
                    handled = false;
                    break;
            }

            // Filter message back up to parents.
            base.WndProc(ref m);

            if (handled)
            {
                // Acknowledge event if handled.
                try
                {
                    m.Result = new System.IntPtr(1);
                }
                catch (Exception excep)
                {
                    Debug.Print("Could not allocate result ptr");
                    Debug.Print(excep.ToString());
                }
            }
        }

        // Taken from GCI_ROTATE_ANGLE_FROM_ARGUMENT.
        // Converts from "binary radians" to traditional radians.
        static protected double ArgToRadians(Int64 arg)
        {
            return (((double)(arg) / 65535.0) * 4.0 * 3.14159265) - (2.0 * 3.14159265);
        }

        // Handler of gestures
        //in:
        //  m - Message object
        private bool DecodeGesture(ref Message m)
        {
            GESTUREINFO gi;

            try
            {
                gi = new GESTUREINFO();
            }
            catch (Exception excep)
            {
                Debug.Print("Could not allocate resources to decode gesture");
                Debug.Print(excep.ToString());

                return false;
            }

            gi.cbSize = _gestureInfoSize;

            // Load the gesture information.
            // We must p/invoke into user32 [winuser.h]
            if (!GetGestureInfo(m.LParam, ref gi))
            {
                return false;
            }

            switch (gi.dwID)
            {
                case GID_BEGIN:
                case GID_END:
                    break;

                case GID_ZOOM:
                    switch (gi.dwFlags)
                    {
                        case GF_BEGIN:
                            _iArguments = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK);
                            _ptFirst.X = gi.ptsLocation.x;
                            _ptFirst.Y = gi.ptsLocation.y;
                            _ptFirst = PointToClient(_ptFirst);
                            break;

                        default:
                            // We read here the second point of the gesture. This
                            // is middle point between fingers in this new
                            // position.
                            _ptSecond.X = gi.ptsLocation.x;
                            _ptSecond.Y = gi.ptsLocation.y;
                            _ptSecond = PointToClient(_ptSecond);
                            {
                                // The zoom factor is the ratio of the new
                                // and the old distance. The new distance
                                // between two fingers is stored in
                                // gi.ullArguments (lower 4 bytes) and the old
                                // distance is stored in _iArguments.
                                double k = (double)(_iArguments)
                                            / (double)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK);
                                //lblX.Text = k.ToString();
                                zoomValue *= k;
                                if (zoomValue < 6.0) zoomValue = 6;
                                camera.camSetDistance = zoomValue * zoomValue * -1;
                                SetZoom();
                            }

                            // Now we have to store new information as a starting
                            // information for the next step in this gesture.
                            _ptFirst = _ptSecond;
                            _iArguments = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK);
                            break;
                    }
                    break;

                //case GID_PAN:
                //    switch (gi.dwFlags)
                //    {
                //        case GF_BEGIN:
                //            _ptFirst.X = gi.ptsLocation.x;
                //            _ptFirst.Y = gi.ptsLocation.y;
                //            _ptFirst = PointToClient(_ptFirst);
                //            break;

                //        default:
                //            // We read the second point of this gesture. It is a
                //            // middle point between fingers in this new position
                //            _ptSecond.X = gi.ptsLocation.x;
                //            _ptSecond.Y = gi.ptsLocation.y;
                //            _ptSecond = PointToClient(_ptSecond);

                //            // We apply move operation of the object
                //            _dwo.Move(_ptSecond.X - _ptFirst.X, _ptSecond.Y - _ptFirst.Y);

                //            Invalidate();

                //            // We have to copy second point into first one to
                //            // prepare for the next step of this gesture.
                //            _ptFirst = _ptSecond;
                //            break;
                //    }
                //    break;

                case GID_ROTATE:
                    switch (gi.dwFlags)
                    {
                        case GF_BEGIN:
                            _iArguments = 32768;
                            break;

                        default:
                            // Gesture handler returns cumulative rotation angle. However we
                            // have to pass the delta angle to our function responsible
                            // to process the rotation gesture.
                            double k = ((int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK) - _iArguments) * 0.01;
                            camera.camPitch -= k;
                            if (camera.camPitch < -80) camera.camPitch = -80;
                            if (camera.camPitch > 0) camera.camPitch = 0;
                            _iArguments = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK);
                            break;
                    }
                    break;

                    //case GID_TWOFINGERTAP:
                    //    // Toggle drawing of diagonals
                    //    _dwo.ToggleDrawDiagonals();
                    //    Invalidate();
                    //    break;

                    //case GID_PRESSANDTAP:
                    //    if (gi.dwFlags == GF_BEGIN)
                    //    {
                    //        // Shift drawing color
                    //        _dwo.ShiftColor();
                    //        Invalidate();
                    //    }
                    //    break;
            }

            return true;
        }

        #endregion Gesture

        // Constructor, Initializes a new instance of the "FormGPS" class.
        public FormGPS()
        {
            //winform initialization
            InitializeComponent();

            //build the gesture structures
            SetupStructSizes();

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            OpenGL glBack = openGLControlBack.OpenGL;

            //create the world grid
            worldGrid = new CWorldGrid(gl, this);

            //our vehicle made with gl object and pointer of mainform
            vehicle = new CVehicle(gl, this);

            //our NMEA parser
            pn = new CNMEA(this);

            //create the ABLine instance
            ABLine = new CABLine(gl, this);

            //new instance of contour mode
            ct = new CContour(gl, this);

            //module communication
            mc = new CModuleComm(this);

            sim = new CSim(this);

            // Add Message Event handler for Form decoupling from client socket thread
            updateRTCM_DataEvent = new UpdateRTCM_Data(OnAddMessage);

            //start the stopwatch
            swFrame.Start();

            //resource for gloabal language strings
            _rm = new ResourceManager("OpenGrade.gStr", Assembly.GetExecutingAssembly());
        }


        //keystrokes for easy and quick startup
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //reset Sim
            if (keyData == Keys.L)
            {
                btnResetSim.PerformClick();
                return true;
            }

            //speed up
            if (keyData == Keys.K)
            {
                sim.stepDistance += 0.05;
                if (sim.stepDistance > 5.8) sim.stepDistance = 5.8;
                tbarStepDistance.Value = (int)(sim.stepDistance * 10.0 * fixUpdateHz);

                return true;
            }

            //Stop
            if (keyData == Keys.J)
            {
                sim.stepDistance = 0;
                tbarStepDistance.Value = 0;
                return true;
            }

            //slow down
            if (keyData == Keys.H)
            {
                sim.stepDistance -= 0.05;
                if (sim.stepDistance < 0) sim.stepDistance = 0;
                tbarStepDistance.Value = (int)(sim.stepDistance * 10.0 * fixUpdateHz);
                return true;
            }

            //turn right
            if (keyData == Keys.M)
            {
                sim.steerAngle++;
                if (sim.steerAngle > 30) sim.steerAngle = 30;
                sim.steerAngleScrollBar = sim.steerAngle;
                lblSteerAngle.Text = sim.steerAngle.ToString();
                tbarSteerAngle.Value = (int)(10 * sim.steerAngle);
                return true;
            }

            //turn left
            if (keyData == Keys.B)
            {
                sim.steerAngle--;
                if (sim.steerAngle < -30) sim.steerAngle = -30;
                sim.steerAngleScrollBar = sim.steerAngle;
                lblSteerAngle.Text = sim.steerAngle.ToString();
                tbarSteerAngle.Value = (int)(10 * sim.steerAngle);
                return true;
            }

            //zero steering
            if (keyData == Keys.N)
            {
                sim.steerAngle = 0.0;
                sim.steerAngleScrollBar = sim.steerAngle;
                lblSteerAngle.Text = sim.steerAngle.ToString();
                tbarSteerAngle.Value = (int)(10 * sim.steerAngle);
                return true;
            }

            if (keyData == (Keys.F))
            {
                JobNewOpenResume();
                return true;    // indicate that you handled this keystroke
            }

            if (keyData == (Keys.A))
            {
                btnAutoSteer.PerformClick();
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //Initialize items before the form Loads or is visible
        private void FormGPS_Load(object sender, EventArgs e)
        {
            //tooltips of controls
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(btnABLine, "Set and configure\n an ABLine");

            if (Settings.Default.setF_workingDirectory == "Default")
                baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OpenGrade\\";
            else baseDirectory = Settings.Default.setF_workingDirectory + "\\OpenGrade\\";

            //get the fields directory, if not exist, create
            fieldsDirectory = baseDirectory + "Fields\\";
            string dir = Path.GetDirectoryName(fieldsDirectory);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            //get the fields directory, if not exist, create
            vehiclesDirectory = baseDirectory + "Vehicles\\";
            dir = Path.GetDirectoryName(vehiclesDirectory);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            //make sure current field directory exists, null if not
            currentFieldDirectory = Settings.Default.setF_CurrentDir;

            string curDir;
            if (currentFieldDirectory != "")
            {
                curDir = fieldsDirectory + currentFieldDirectory + "//";
                dir = Path.GetDirectoryName(curDir);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    currentFieldDirectory = "";
                    Settings.Default.setF_CurrentDir = "";
                    Settings.Default.Save();
                }
            }

            //grab the current vehicle filename - make sure it exists
            vehiclefileName = Vehicle.Default.setVehicle_Name;

            //get the abLines directory, if not exist, create
            ablinesDirectory = baseDirectory + "ABLines\\";
            dir = Path.GetDirectoryName(fieldsDirectory);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            //set baud and port from last time run
            baudRateGPS = Settings.Default.setPort_baudRate;
            portNameGPS = Settings.Default.setPort_portNameGPS;

            //try and open
            SerialPortOpenGPS();

            LoadUDPNetwork(); // from AgIO v6.3.3

            //same for SectionRelay port
            portNameRelaySection = Settings.Default.setPort_portNameRateRelay;
            wasRateRelayConnectedLastRun = Settings.Default.setPort_wasRateRelayConnected;
            if (wasRateRelayConnectedLastRun) SerialPortRateRelayOpen();

            //same for AutoSteer port
            portNameAutoSteer = Settings.Default.setPort_portNameAutoSteer;
            wasAutoSteerConnectedLastRun = Settings.Default.setPort_wasAutoSteerConnected;
            if (wasAutoSteerConnectedLastRun) SerialPortAutoSteerOpen();

            //start udp server
            //StartUDPServer();

            //set the correct zoom and grid
            camera.camSetDistance = zoomValue * zoomValue * -1;
            SetZoom();

            //remembered window position
            if (Settings.Default.setWindow_Maximized)
            {
                WindowState = FormWindowState.Maximized;
                Location = Settings.Default.setWindow_Location;
                Size = Settings.Default.setWindow_Size;
            }
            else if (Settings.Default.setWindow_Minimized)
            {
                //WindowState = FormWindowState.Minimized;
                Location = Settings.Default.setWindow_Location;
                Size = Settings.Default.setWindow_Size;
            }
            else
            {
                Location = Settings.Default.setWindow_Location;
                Size = Settings.Default.setWindow_Size;
            }

            //clear the flags
            flagPts.Clear();
            btnFlag.Enabled = false;

            //workswitch stuff
            mc.isWorkSwitchEnabled = Settings.Default.setF_IsWorkSwitchEnabled;
            mc.isWorkSwitchActiveLow = Settings.Default.setF_IsWorkSwitchActiveLow;

            minFixStepDist = Settings.Default.setF_minFixStep;

            pitchZero = Settings.Default.setIMU_pitchZero;
            rollZero = Settings.Default.setIMU_rollZero;

            //sim.latitude = Settings.Default.setSim_lastLat;
            //sim.longitude = Settings.Default.setSim_lastLong;

            // load all the gui elements in gui.designer.cs
            LoadGUI();


           
            HideTabControl();


        }           




                

        

        //form is closing so tidy up and save settings
        private void FormGPS_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save, return, cancel save
            if (isJobStarted)
            {
                int choice = SaveOrNot();

                switch (choice)
                {
                    //OK
                    case 0:
                        isSendConnected = false;
                        //sendSocket.Shutdown(SocketShutdown.Both);
                        //recvSocket.Shutdown(SocketShutdown.Both);

                        Settings.Default.setSim_lastLong = pn.longitude;
                        Settings.Default.setSim_lastLat = pn.latitude;
                        Settings.Default.setF_CurrentDir = currentFieldDirectory;
                        Settings.Default.Save();

                        FileSaveEverythingBeforeClosingField();

                        //shutdown and reset all module data
                        mc.ResetAllModuleCommValues();

                        //sendSocket.Disconnect(true);
                        //recvSocket.Disconnect(true);

                        break;

                    //Ignore and return
                    case 1:
                        e.Cancel = true;
                        break;
                }
            }

            //save window settings
            if (WindowState == FormWindowState.Maximized)
            {
                Settings.Default.setWindow_Location = RestoreBounds.Location;
                Settings.Default.setWindow_Size = RestoreBounds.Size;
                Settings.Default.setWindow_Maximized = true;
                Settings.Default.setWindow_Minimized = false;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                Settings.Default.setWindow_Location = Location;
                Settings.Default.setWindow_Size = Size;
                Settings.Default.setWindow_Maximized = false;
                Settings.Default.setWindow_Minimized = false;
            }
            else
            {
                Settings.Default.setWindow_Location = RestoreBounds.Location;
                Settings.Default.setWindow_Size = RestoreBounds.Size;
                Settings.Default.setWindow_Maximized = false;
                Settings.Default.setWindow_Minimized = true;
            }

            Settings.Default.setCam_pitch = camera.camPitch;
            Settings.Default.Save();
        }

        private void btnFixQuality_click(object sender, EventArgs e)
        {
            if(ct.FloatIsOK)
            {
                ct.FloatIsOK = false;
                btnFixQuality.Text = "RTK fix only";
            }
            else
            {
                ct.FloatIsOK = true;
                btnFixQuality.Text = "RTK or float";
            }
        }

        private void btnSimGoTo_click(object sender, EventArgs e)
        {
            //check dist from old position and show a warning message if too far
            double oldLat = Properties.Settings.Default.setSim_lastLat;
            double oldLong = Properties.Settings.Default.setSim_lastLong;
            double newLat = (double)nudLatitude.Value;
            double newLong = (double)nudLongitude.Value;
            double newSimDist;

            newSimDist = 2 * (oldLat - newLat) * (oldLat - newLat) + (oldLong - newLong) * (oldLong - newLong);

            if (newSimDist > 2)
            {
                var form = new FormTimedMessage(4000, "Please restart OpenGrade", "To avoid strange beviavour!");
                form.Show();
            }

            Properties.Settings.Default.setSim_lastLat = (double)nudLatitude.Value;
            Properties.Settings.Default.setSim_lastLong = (double)nudLongitude.Value;
            Properties.Settings.Default.Save();
            sim.ResetSim();
            
        }

        //called everytime window is resized, clean up button positions
        private void FormGPS_Resize(object sender, EventArgs e)
        {
            if (openGLControlBack.Visible)
            {
                openGLControl.Height = 300;
                pbarCutAbove.Height = openGLControlBack.Height / 2;
                pbarCutBelow.Height = pbarCutAbove.Height;
                pbarCutBelow.Top = pbarCutAbove.Top + pbarCutAbove.Height;
            }
            else
            {
                openGLControl.Height = this.Height - 200;
            }
        }

        // Procedures and Functions ---------------------------------------
        public uint LoadGLTextures()
        {
            OpenGL gl = openGLControlBack.OpenGL;

            try
            {
                //  Background
                ParticleTexture = new Texture();
                ParticleTexture.Create(gl, @".\Dependencies\landscape.png");
                texture[0] = ParticleTexture.TextureName;
            }
            catch (Exception e)
            {
                MessageBox.Show("Texture File LANDSCAPE.PNG is Missing", e.Message);
            }

            try
            {
                //  Floor
                ParticleTexture = new Texture();
                ParticleTexture.Create(gl, @".\Dependencies\Floor.png");
                texture[1] = ParticleTexture.TextureName;
            }
            catch (Exception e)
            {
                MessageBox.Show("Texture File FLOOR.PNG is Missing", e.Message);
            }

            return texture[0];
        }
        /*
        //start the UDP server
        private void StartUDPServer()
        {
            try
            {
                // Initialise the delegate which updates the message received
                updateRecvMessageDelegate = UpdateRecvMessage;

                // Initialise the socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server and listen on port 9999
                IPEndPoint recv = new IPEndPoint(IPAddress.Any, Properties.Settings.Default.setIP_thisPort);

                // Associate the socket with this IP address and port
                recvSocket.Bind(recv);

                // Initialise the send socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server to send on port 9998
                IPEndPoint server = new IPEndPoint(IPAddress.Any, 9998);
                sendSocket.Bind(server);

                //IP address and port of Auto Steer server
                IPAddress epIP = IPAddress.Parse(Properties.Settings.Default.setIP_autoSteerIP);
                epAutoSteer = new IPEndPoint(epIP, Properties.Settings.Default.setIP_autoSteerPort);

                // Initialise the IPEndPoint for the client - async listner client only!
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                // Start listening for incoming data
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                                                ref client, ReceiveData, recvSocket);
                isSendConnected = true;
            }
            catch (Exception e)
            {
                WriteErrorLog("UDP Server" + e);
                MessageBox.Show("Load Error: " + e.Message, "UDP Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        //dialog for requesting user to save or cancel
        public int SaveOrNot()
        {
            using (var form = new FormSaveOrNot())
            {
                var result = form.ShowDialog();

                if (result == DialogResult.OK) return 0;
                if (result == DialogResult.Ignore) return 1;
                if (result == DialogResult.Cancel) return 2;
                return 3;
            }
        }

        //show the communications window
        private void SettingsCommunications()
        {
            using (var form = new FormCommSet(this))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fixUpdateTime = 1 / (double)fixUpdateHz;
                }
            }

            AutoSteerSettingsOutToPort();
        }

        //show the UDP ethernet settings page
        private void SettingsUDP()
        {
            using (var form = new FormUDP(this))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //Clicked Save
                }
                else
                {
                    //Clicked X - No Save
                }
            }
        }

        //show the UDP ethernet settings page
        private void SettingsNTRIP()
        {
            using (var form = new FormNtrip(this))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //Clicked Save
                }
                else
                {
                    //Clicked X - No Save
                }
            }
        }

        public void KeypadToNUD(NumericUpDown sender)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            nud.BackColor = System.Drawing.Color.Red;
            using (var form = new FormNumeric((double)nud.Minimum, (double)nud.Maximum, (double)nud.Value))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    nud.Value = (decimal)form.ReturnValue;
                }
            }
            nud.BackColor = System.Drawing.Color.AliceBlue;
        }

        //request a new job
        public void JobNew()
        {
            AutoSteerSettingsOutToPort();
            isJobStarted = true;
            startCounter = 0;

            btnManualOffOn.Enabled = true;
            btnManualOffOn.Image = Properties.Resources.ManualOff;

            btnABLine.Enabled = true;
            btnContour.Enabled = true;
            btnAutoSteer.Enabled = true;
            ABLine.abHeading = 0.00;

            btnFlag.Enabled = true;

            ct.isContourBtnOn = false;
            ct.isContourOn = false;
            
            
            
            cboxLastPass.Checked = false;
            
            cboxLaserModeOnOff.Checked = false;

            // by Pat
            ct.Build_eleViewList();
            ct.drawTheMap = true;

            //update the menu
            fieldToolStripMenuItem.Text = gStr.gsCloseField;
        }

        //close the current job
        public void JobClose()
        {
            //turn manual button off
            manualBtnState = btnStates.Off;
            btnManualOffOn.Image = Properties.Resources.ManualOff;

            //job is closed
            isJobStarted = false;

            //clear out the contour Lists
            ct.ResetContour();

            //clear the flags
            flagPts.Clear();
            btnFlag.Enabled = false;

            //reset the buttons
            btnABLine.Enabled = false;
            btnContour.Enabled = false;
            btnAutoSteer.Enabled = false;
            isAutoSteerBtnOn = false;

            ct.isContourBtnOn = false;
            ct.isContourOn = false;
            ct.ptList.Clear();
            

            cboxLastPass.Checked = false;
            cboxLaserModeOnOff.Checked = false;
            

            //change images to reflect on off
            btnABLine.Image = Properties.Resources.ABLineOff;
            btnContour.Image = Properties.Resources.ContourOff;
            btnAutoSteer.Image = Properties.Resources.AutoSteerOff;

            //fix ManualOffOnAuto buttons
            btnManualOffOn.Enabled = false;
            btnManualOffOn.Image = Properties.Resources.ManualOff;

            //reset all the ABLine stuff
            ABLine.ResetABLine();

            //update the menu
            fieldToolStripMenuItem.Text = gStr.gsStartNewField;

            //reset all Port Module values
            mc.ResetAllModuleCommValues();

            //by Pat
            ct.eleViewList.Clear();
            ct.mapList.Clear();
            ct.boundaryList.Clear();
            ct.usedPtList.Clear();
            pn.eastingOffset = 0;
            pn.northingOffset = 0;
            pn.altitudeOffset = 0;
            CancelSurvey();
            ct.ScaleFactor = 100;
            fillCutFillLbl();
        }

        //bring up field dialog for new/open/resume
        private void JobNewOpenResume()
        {
            //bring up dialog if no job active, close job if one is

            if (!isJobStarted)
            {
                if (stripOnlineGPS.Value == 1)
                {
                    var form = new FormTimedMessage(3000, gStr.gsNoGPS, gStr.gsIsGPSOff);
                    form.Show();
                    return;
                }

                using (var form = new FormJob(this))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.Yes)
                    {
                        //ask for a directory name
                        using (var form2 = new FormFieldDir(this))
                        { form2.ShowDialog(); }
                    }
                }

                if (isJobStarted) Text = "OpenGrade3D - " + currentFieldDirectory;
                else Text = "OpenGrade3D";
            }

            //close the current job and ask how to or if to save
            else
            {
                int choice = SaveOrNot();
                switch (choice)
                {
                    //OK
                    case 0:
                        Settings.Default.setF_CurrentDir = currentFieldDirectory;
                        Settings.Default.Save();
                        FileSaveEverythingBeforeClosingField();
                        break;
                    //Ignore and return
                    case 1:
                        break;
                }
            }
        }

        //take the distance from object and convert to camera data
        private void SetZoom()
        {
            //match grid to cam distance and redo perspective
            if (camera.camSetDistance <= -20000) gridZoom = 2000;
            if (camera.camSetDistance >= -20000 && camera.camSetDistance < -10000) gridZoom = 2000;
            if (camera.camSetDistance >= -10000 && camera.camSetDistance < -5000) gridZoom = 1000;
            if (camera.camSetDistance >= -5000 && camera.camSetDistance < -2000) gridZoom = 503;
            if (camera.camSetDistance >= -2000 && camera.camSetDistance < -1000) gridZoom = 201.2;
            if (camera.camSetDistance >= -1000 && camera.camSetDistance < -500) gridZoom = 100.6;
            if (camera.camSetDistance >= -500 && camera.camSetDistance < -250) gridZoom = 50.3;
            if (camera.camSetDistance >= -250 && camera.camSetDistance < -150) gridZoom = 25.15;
            if (camera.camSetDistance >= -150 && camera.camSetDistance < -50) gridZoom = 10.06;
            if (camera.camSetDistance >= -50 && camera.camSetDistance < -1) gridZoom = 5.03;
            //1.216 2.532

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(fovy, openGLControl.Width / (double)openGLControl.Height, 1, camDistanceFactor * camera.camSetDistance);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        //All the files that need to be saved when closing field or app
        private void FileSaveEverythingBeforeClosingField()
        {
            //turn off contour line if on
            if (ct.isContourOn) ct.StopContourLine();

            FileSaveField();
            FileSaveContour();
            FileSaveFlagsKML();
            FileSaveMapPt();

            JobClose();
            Text = "OpenGrade3D";
        }

        //function called by menu items to delete a selected flag
        private void DeleteSelectedFlag()
        {
            //delete selected flag and set selected to none
            flagPts.RemoveAt(flagNumberPicked - 1);
            flagNumberPicked = 0;

            // re-sort the id's based on how many flags left
            int flagCnt = flagPts.Count;
            if (flagCnt > 0)
            {
                for (int i = 0; i < flagCnt; i++) flagPts[i].ID = i + 1;
            }
        }

        //an error log called by all try catches
        public void WriteErrorLog(string strErrorText)
        {
            try
            {
                //set up file and folder if it doesn't exist
                const string strFileName = "Error Log.txt";
                string strPath = Application.StartupPath;

                //Write out the error appending to existing
                File.AppendAllText(strPath + "\\" + strFileName, strErrorText + " - " +
                    DateTime.Now.ToString() + "\r\n\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in WriteErrorLog: " + ex.Message, "Error Logging", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void numBladeOffset_ValueChanged(object sender, EventArgs e)
        {
            Vehicle.Default.setVehicle_bladeOffset = (double)numBladeOffset.Value /100;
            vehicle.bladeOffset = (double)numBladeOffset.Value /100;
            //bladeOffSetMaster = (int)numBladeOffset.Value;
        }

        //message box pops up with info then goes away
        public void TimedMessageBox(int timeout, string s1, string s2)
        {
            var form = new FormTimedMessage(timeout, s1, s2);
            form.Show();
        }
    }//class FormGPS
}//namespace OpenGrade

/*The order is:
 *
 * The watchdog timer times out and runs this function tmrWatchdog_tick().
 * 50 times per second so statusUpdateCounter counts to 25 and updates strip menu etc at 2 hz
 * it also makes sure there is new sentences showing up otherwise it shows -* No GGA....
 * saveCounter ticks 2 x per second, used at end of draw routine every minute to save a backup of field
 * then ScanForNMEA function checks for a complete sentence if contained in pn.rawbuffer
 * if not it comes right back and waits for next watchdog trigger and starts all over
 * if a new sentence is there, UpdateFix() is called
 * Right away CalculateLookAhead(), no skips, is called to determine lookaheads and trigger distances to save triangles plotted
 * Then UpdateFix() continues.
 * Hitch, pivot, antenna locations etc and directions are figured out if trigDistance is triggered
 * When that is done, DoRender() is called on the visible OpenGL screen and its draw routine _draw is run
 * before triangles are drawn, frustum cull figures out how many of the triangles should be drawn
 * When its all the way thru, it triggers the sectioncontrol Draw, its frustum cull, and determines if sections should be on
 * ProcessSectionOnOffRequests() runs and that does the section on off magic
 * SectionControlToArduino() runs and spits out the port relay control based on sections on or off
 * If field needs saving (1.5 minute since last time) field is saved
 * Now the program is "Done" and waits for the next watchdog trigger, determines if a new sentence is available etc
 * and starts all over from the top.
 */