﻿namespace OpenGrade
{
    partial class FormGPS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGPS));
            ProgBar.cBlendItems cBlendItems1 = new ProgBar.cBlendItems();
            ProgBar.cFocalPoints cFocalPoints1 = new ProgBar.cFocalPoints();
            ProgBar.cBlendItems cBlendItems2 = new ProgBar.cBlendItems();
            ProgBar.cFocalPoints cFocalPoints2 = new ProgBar.cFocalPoints();
            this.openGLControl = new SharpGL.OpenGLControl();
            this.contextMenuStripOpenGL = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteFlagToolOpenGLContextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.googleEarthOpenGLContextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.txtDistanceOffABLine = new System.Windows.Forms.TextBox();
            this.openGLControlBack = new SharpGL.OpenGLControl();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.setWorkingDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.loadVehicleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveVehicleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.fieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.resetALLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.colorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fieldToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.MapColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rstColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GradToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.StepToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.GradMultiToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripUnitsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.metricToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.imperialToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logNMEAMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polygonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pursuitLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skyToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.simulatorOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.explorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webCamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fieldViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleEarthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gPSDataToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuHelpHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrWatchdog = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.stripMinMax = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnHideTabs = new System.Windows.Forms.ToolStripDropDownButton();
            this.stripDistance = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownBtnFuncs = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolstripField = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripResetTrip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolstripUDPConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripUSBPortsConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripNTRIPConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripVehicleConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolstripAutoSteerConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.stripSelectMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.stripTopoLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.stripOnlineGPS = new System.Windows.Forms.ToolStripProgressBar();
            this.stripOnlineAutoSteer = new System.Windows.Forms.ToolStripProgressBar();
            this.lblNorthing = new System.Windows.Forms.Label();
            this.lblEasting = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.contextMenuStripFlag = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemFlagRed = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuFlagGrn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuFlagYel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuFlagDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuFlagDeleteAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tboxSentence = new System.Windows.Forms.TextBox();
            this.lblZone = new System.Windows.Forms.Label();
            this.lblSpeedUnits = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.DataPage = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblSats = new System.Windows.Forms.Label();
            this.lblFixQuality = new System.Windows.Forms.Label();
            this.lblGPSHeading = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.lblLatitude = new System.Windows.Forms.Label();
            this.lblRoll = new System.Windows.Forms.Label();
            this.lblGyroHeading = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.configPage1 = new System.Windows.Forms.TabPage();
            this.btnABLine = new System.Windows.Forms.Button();
            this.btnUnits = new System.Windows.Forms.Button();
            this.btnFileExplorer = new System.Windows.Forms.Button();
            this.btnGPSData = new System.Windows.Forms.Button();
            this.btnFlag = new System.Windows.Forms.Button();
            this.btnSnap = new System.Windows.Forms.Button();
            this.controlPage2 = new System.Windows.Forms.TabPage();
            this.lblCutDelta = new System.Windows.Forms.Label();
            this.btnZeroAltitude = new System.Windows.Forms.Button();
            this.cboxLastPass = new System.Windows.Forms.CheckBox();
            this.cboxLaserModeOnOff = new System.Windows.Forms.CheckBox();
            this.lblCut = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.lblAltitude = new System.Windows.Forms.Label();
            this.panelSimControls = new System.Windows.Forms.Panel();
            this.nudLongitude = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudLatitude = new System.Windows.Forms.NumericUpDown();
            this.btnSimGoTo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnResetSteerAngle = new System.Windows.Forms.Button();
            this.btnResetSim = new System.Windows.Forms.Button();
            this.lblSteerAngle = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbarSteerAngle = new System.Windows.Forms.TrackBar();
            this.tbarStepDistance = new System.Windows.Forms.TrackBar();
            this.nudElevation = new System.Windows.Forms.NumericUpDown();
            this.lblPureSteerAngle = new System.Windows.Forms.Label();
            this.timerSim = new System.Windows.Forms.Timer(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.lblScale = new System.Windows.Forms.Label();
            this.pbarCutBelow = new ProgBar.ProgBarPlus();
            this.pbarCutAbove = new ProgBar.ProgBarPlus();
            this.lblBarGraphMax = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnGoogleEarth = new System.Windows.Forms.Button();
            this.btnAutoSteer = new System.Windows.Forms.Button();
            this.btnContour = new System.Windows.Forms.Button();
            this.btnZoomIn = new ProXoft.WinForms.RepeatButton();
            this.btnTiltDown = new ProXoft.WinForms.RepeatButton();
            this.btnZoomOut = new ProXoft.WinForms.RepeatButton();
            this.btnTiltUp = new ProXoft.WinForms.RepeatButton();
            this.btnManualOffOn = new System.Windows.Forms.Button();
            this.lblFixUpdateHz = new System.Windows.Forms.Label();
            this.btnStartPause = new System.Windows.Forms.Button();
            this.btnBoundarySide = new System.Windows.Forms.Button();
            this.numBladeOffset = new System.Windows.Forms.NumericUpDown();
            this.btnCutFillElev = new System.Windows.Forms.Button();
            this.btnPropExist = new System.Windows.Forms.Button();
            this.btnFixQuality = new System.Windows.Forms.Button();
            this.btnColorCut = new System.Windows.Forms.Button();
            this.btnColorCenter = new System.Windows.Forms.Button();
            this.btnColorFill = new System.Windows.Forms.Button();
            this.lblFill = new System.Windows.Forms.Label();
            this.lblFillValue = new System.Windows.Forms.Label();
            this.lblCutValue = new System.Windows.Forms.Label();
            this.lblWatch = new System.Windows.Forms.Label();
            this.btnColorFillMid = new System.Windows.Forms.Button();
            this.btnColorFillMin = new System.Windows.Forms.Button();
            this.btnColorCutMid = new System.Windows.Forms.Button();
            this.btnColorCutMin = new System.Windows.Forms.Button();
            this.btnScalePlus = new System.Windows.Forms.Button();
            this.btnScaleMinus = new System.Windows.Forms.Button();
            this.lblFillMinValue = new System.Windows.Forms.Label();
            this.lblCutMinValue = new System.Windows.Forms.Label();
            this.lblCutMidValue = new System.Windows.Forms.Label();
            this.lblFillMidValue = new System.Windows.Forms.Label();
            this.lblCentreValue = new System.Windows.Forms.Label();
            this.btnUseSavedAGS = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl)).BeginInit();
            this.contextMenuStripOpenGL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.openGLControlBack)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStripFlag.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.DataPage.SuspendLayout();
            this.configPage1.SuspendLayout();
            this.controlPage2.SuspendLayout();
            this.panelSimControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLatitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarSteerAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarStepDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBladeOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // openGLControl
            // 
            this.openGLControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openGLControl.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.openGLControl.ContextMenuStrip = this.contextMenuStripOpenGL;
            this.openGLControl.DrawFPS = false;
            this.openGLControl.Font = new System.Drawing.Font("Tahoma", 12F);
            this.openGLControl.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.openGLControl.FrameRate = 5;
            this.openGLControl.Location = new System.Drawing.Point(0, 41);
            this.openGLControl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.openGLControl.Name = "openGLControl";
            this.openGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.openGLControl.RenderContextType = SharpGL.RenderContextType.NativeWindow;
            this.openGLControl.RenderTrigger = SharpGL.RenderTrigger.Manual;
            this.openGLControl.Size = new System.Drawing.Size(962, 300);
            this.openGLControl.TabIndex = 6;
            this.openGLControl.OpenGLInitialized += new System.EventHandler(this.openGLControl_OpenGLInitialized);
            this.openGLControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControl_OpenGLDraw);
            this.openGLControl.Resized += new System.EventHandler(this.openGLControl_Resized);
            this.openGLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openGLControl_MouseDown);
            // 
            // contextMenuStripOpenGL
            // 
            this.contextMenuStripOpenGL.AutoSize = false;
            this.contextMenuStripOpenGL.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteFlagToolOpenGLContextMenu,
            this.toolStripSeparator5,
            this.googleEarthOpenGLContextMenu});
            this.contextMenuStripOpenGL.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.contextMenuStripOpenGL.Name = "contextMenuStripOpenGL";
            this.contextMenuStripOpenGL.Size = new System.Drawing.Size(72, 160);
            this.contextMenuStripOpenGL.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripOpenGL_Opening);
            // 
            // deleteFlagToolOpenGLContextMenu
            // 
            this.deleteFlagToolOpenGLContextMenu.AutoSize = false;
            this.deleteFlagToolOpenGLContextMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteFlagToolOpenGLContextMenu.Image = ((System.Drawing.Image)(resources.GetObject("deleteFlagToolOpenGLContextMenu.Image")));
            this.deleteFlagToolOpenGLContextMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deleteFlagToolOpenGLContextMenu.Name = "deleteFlagToolOpenGLContextMenu";
            this.deleteFlagToolOpenGLContextMenu.Size = new System.Drawing.Size(70, 70);
            this.deleteFlagToolOpenGLContextMenu.Text = ".";
            this.deleteFlagToolOpenGLContextMenu.Click += new System.EventHandler(this.deleteFlagToolOpenGLContextMenu_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(68, 6);
            // 
            // googleEarthOpenGLContextMenu
            // 
            this.googleEarthOpenGLContextMenu.AutoSize = false;
            this.googleEarthOpenGLContextMenu.Image = ((System.Drawing.Image)(resources.GetObject("googleEarthOpenGLContextMenu.Image")));
            this.googleEarthOpenGLContextMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.googleEarthOpenGLContextMenu.Name = "googleEarthOpenGLContextMenu";
            this.googleEarthOpenGLContextMenu.Size = new System.Drawing.Size(70, 70);
            this.googleEarthOpenGLContextMenu.Text = ".";
            this.googleEarthOpenGLContextMenu.Click += new System.EventHandler(this.googleEarthOpenGLContextMenu_Click);
            // 
            // txtDistanceOffABLine
            // 
            this.txtDistanceOffABLine.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtDistanceOffABLine.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtDistanceOffABLine.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDistanceOffABLine.Font = new System.Drawing.Font("Tahoma", 21.75F);
            this.txtDistanceOffABLine.ForeColor = System.Drawing.Color.Green;
            this.txtDistanceOffABLine.Location = new System.Drawing.Point(393, -1);
            this.txtDistanceOffABLine.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.txtDistanceOffABLine.Name = "txtDistanceOffABLine";
            this.txtDistanceOffABLine.ReadOnly = true;
            this.txtDistanceOffABLine.Size = new System.Drawing.Size(110, 36);
            this.txtDistanceOffABLine.TabIndex = 7;
            this.txtDistanceOffABLine.Text = "00000";
            this.txtDistanceOffABLine.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDistanceOffABLine.Visible = false;
            // 
            // openGLControlBack
            // 
            this.openGLControlBack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openGLControlBack.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.openGLControlBack.Cursor = System.Windows.Forms.Cursors.Cross;
            this.openGLControlBack.DrawFPS = false;
            this.openGLControlBack.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.openGLControlBack.FrameRate = 1;
            this.openGLControlBack.Location = new System.Drawing.Point(0, 343);
            this.openGLControlBack.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.openGLControlBack.Name = "openGLControlBack";
            this.openGLControlBack.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.openGLControlBack.RenderContextType = SharpGL.RenderContextType.NativeWindow;
            this.openGLControlBack.RenderTrigger = SharpGL.RenderTrigger.Manual;
            this.openGLControlBack.Size = new System.Drawing.Size(962, 300);
            this.openGLControlBack.TabIndex = 91;
            this.openGLControlBack.OpenGLInitialized += new System.EventHandler(this.openGLControlBack_OpenGLInitialized);
            this.openGLControlBack.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControlBack_OpenGLDraw);
            this.openGLControlBack.Resized += new System.EventHandler(this.openGLControlBack_Resized);
            this.openGLControlBack.MouseClick += new System.Windows.Forms.MouseEventHandler(this.openGLControlBack_MouseClick);
            this.openGLControlBack.MouseMove += new System.Windows.Forms.MouseEventHandler(this.openGLControlBack_MouseMove);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator9,
            this.toolStripSeparator11,
            this.setWorkingDirectoryToolStripMenuItem,
            this.toolStripSeparator10,
            this.loadVehicleToolStripMenuItem,
            this.saveVehicleToolStripMenuItem,
            this.toolStripSeparator8,
            this.fieldToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(69, 38);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(255, 6);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(255, 6);
            // 
            // setWorkingDirectoryToolStripMenuItem
            // 
            this.setWorkingDirectoryToolStripMenuItem.Name = "setWorkingDirectoryToolStripMenuItem";
            this.setWorkingDirectoryToolStripMenuItem.Size = new System.Drawing.Size(258, 40);
            this.setWorkingDirectoryToolStripMenuItem.Text = "Directories";
            this.setWorkingDirectoryToolStripMenuItem.Click += new System.EventHandler(this.setWorkingDirectoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(255, 6);
            // 
            // loadVehicleToolStripMenuItem
            // 
            this.loadVehicleToolStripMenuItem.Name = "loadVehicleToolStripMenuItem";
            this.loadVehicleToolStripMenuItem.Size = new System.Drawing.Size(258, 40);
            this.loadVehicleToolStripMenuItem.Text = "Load Vehicle";
            this.loadVehicleToolStripMenuItem.Click += new System.EventHandler(this.loadVehicleToolStripMenuItem_Click);
            // 
            // saveVehicleToolStripMenuItem
            // 
            this.saveVehicleToolStripMenuItem.Name = "saveVehicleToolStripMenuItem";
            this.saveVehicleToolStripMenuItem.Size = new System.Drawing.Size(258, 40);
            this.saveVehicleToolStripMenuItem.Text = "Save Vehicle";
            this.saveVehicleToolStripMenuItem.Click += new System.EventHandler(this.saveVehicleToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(255, 6);
            // 
            // fieldToolStripMenuItem
            // 
            this.fieldToolStripMenuItem.Name = "fieldToolStripMenuItem";
            this.fieldToolStripMenuItem.Size = new System.Drawing.Size(258, 40);
            this.fieldToolStripMenuItem.Text = "Start Field";
            this.fieldToolStripMenuItem.Click += new System.EventHandler(this.fieldToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.resetALLToolStripMenuItem,
            this.toolStripSeparator2,
            this.colorsToolStripMenuItem,
            this.toolStripUnitsMenu,
            this.gridToolStripMenuItem,
            this.lightbarToolStripMenuItem,
            this.logNMEAMenuItem,
            this.polygonsToolStripMenuItem,
            this.pursuitLineToolStripMenuItem,
            this.skyToolStripMenu,
            this.toolStripSeparator6,
            this.simulatorOnToolStripMenuItem,
            this.toolStripSeparator7});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(118, 38);
            this.settingsToolStripMenuItem.Text = "Display";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(294, 6);
            // 
            // resetALLToolStripMenuItem
            // 
            this.resetALLToolStripMenuItem.Name = "resetALLToolStripMenuItem";
            this.resetALLToolStripMenuItem.Size = new System.Drawing.Size(297, 40);
            this.resetALLToolStripMenuItem.Text = "Reset ALL";
            this.resetALLToolStripMenuItem.Click += new System.EventHandler(this.resetALLToolStripMenuItem_Click_1);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(294, 6);
            // 
            // colorsToolStripMenuItem
            // 
            this.colorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sectionToolStripMenuItem,
            this.fieldToolStripMenuItem1,
            this.MapColorToolStripMenuItem});
            this.colorsToolStripMenuItem.Name = "colorsToolStripMenuItem";
            this.colorsToolStripMenuItem.Size = new System.Drawing.Size(297, 40);
            this.colorsToolStripMenuItem.Text = "Colors";
            // 
            // sectionToolStripMenuItem
            // 
            this.sectionToolStripMenuItem.Name = "sectionToolStripMenuItem";
            this.sectionToolStripMenuItem.Size = new System.Drawing.Size(223, 40);
            this.sectionToolStripMenuItem.Text = "Section";
            this.sectionToolStripMenuItem.Click += new System.EventHandler(this.sectionToolStripMenuItem_Click);
            // 
            // fieldToolStripMenuItem1
            // 
            this.fieldToolStripMenuItem1.Name = "fieldToolStripMenuItem1";
            this.fieldToolStripMenuItem1.Size = new System.Drawing.Size(223, 40);
            this.fieldToolStripMenuItem1.Text = "Field";
            this.fieldToolStripMenuItem1.Click += new System.EventHandler(this.fieldToolStripMenuItem1_Click);
            // 
            // MapColorToolStripMenuItem
            // 
            this.MapColorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rstColorToolStripMenuItem,
            this.GradToolStrip,
            this.StepToolStrip,
            this.GradMultiToolStrip});
            this.MapColorToolStripMenuItem.Name = "MapColorToolStripMenuItem";
            this.MapColorToolStripMenuItem.Size = new System.Drawing.Size(223, 40);
            this.MapColorToolStripMenuItem.Text = "Map Color";
            // 
            // rstColorToolStripMenuItem
            // 
            this.rstColorToolStripMenuItem.Name = "rstColorToolStripMenuItem";
            this.rstColorToolStripMenuItem.Size = new System.Drawing.Size(291, 40);
            this.rstColorToolStripMenuItem.Text = "Reset Colors";
            this.rstColorToolStripMenuItem.Click += new System.EventHandler(this.btnResetMapColor_Click);
            // 
            // GradToolStrip
            // 
            this.GradToolStrip.Name = "GradToolStrip";
            this.GradToolStrip.Size = new System.Drawing.Size(291, 40);
            this.GradToolStrip.Text = "Gradual Scale";
            this.GradToolStrip.Click += new System.EventHandler(this.GradToolStrip_Click);
            // 
            // StepToolStrip
            // 
            this.StepToolStrip.Name = "StepToolStrip";
            this.StepToolStrip.Size = new System.Drawing.Size(291, 40);
            this.StepToolStrip.Text = "Step Multicolor";
            this.StepToolStrip.Click += new System.EventHandler(this.StepToolStrip_Click);
            // 
            // GradMultiToolStrip
            // 
            this.GradMultiToolStrip.Name = "GradMultiToolStrip";
            this.GradMultiToolStrip.Size = new System.Drawing.Size(291, 40);
            this.GradMultiToolStrip.Text = "Grad Multicolor";
            this.GradMultiToolStrip.Click += new System.EventHandler(this.GradMultiToolStrip_Click);
            // 
            // toolStripUnitsMenu
            // 
            this.toolStripUnitsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.metricToolStrip,
            this.imperialToolStrip});
            this.toolStripUnitsMenu.Name = "toolStripUnitsMenu";
            this.toolStripUnitsMenu.Size = new System.Drawing.Size(297, 40);
            this.toolStripUnitsMenu.Text = "Units";
            // 
            // metricToolStrip
            // 
            this.metricToolStrip.CheckOnClick = true;
            this.metricToolStrip.Name = "metricToolStrip";
            this.metricToolStrip.Size = new System.Drawing.Size(200, 40);
            this.metricToolStrip.Text = "Metric";
            this.metricToolStrip.Click += new System.EventHandler(this.metricToolStrip_Click);
            // 
            // imperialToolStrip
            // 
            this.imperialToolStrip.CheckOnClick = true;
            this.imperialToolStrip.Name = "imperialToolStrip";
            this.imperialToolStrip.Size = new System.Drawing.Size(200, 40);
            this.imperialToolStrip.Text = "Imperial";
            this.imperialToolStrip.Click += new System.EventHandler(this.imperialToolStrip_Click);
            // 
            // gridToolStripMenuItem
            // 
            this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
            this.gridToolStripMenuItem.Size = new System.Drawing.Size(297, 40);
            this.gridToolStripMenuItem.Text = "Grid On";
            this.gridToolStripMenuItem.Click += new System.EventHandler(this.gridToolStripMenuItem_Click);
            // 
            // lightbarToolStripMenuItem
            // 
            this.lightbarToolStripMenuItem.Name = "lightbarToolStripMenuItem";
            this.lightbarToolStripMenuItem.Size = new System.Drawing.Size(297, 40);
            this.lightbarToolStripMenuItem.Text = "Show Design Pt";
            this.lightbarToolStripMenuItem.Click += new System.EventHandler(this.lightbarToolStripMenuItem_Click);
            // 
            // logNMEAMenuItem
            // 
            this.logNMEAMenuItem.Name = "logNMEAMenuItem";
            this.logNMEAMenuItem.Size = new System.Drawing.Size(297, 40);
            this.logNMEAMenuItem.Text = "Log NMEA";
            this.logNMEAMenuItem.Click += new System.EventHandler(this.logNMEAMenuItem_Click);
            // 
            // polygonsToolStripMenuItem
            // 
            this.polygonsToolStripMenuItem.Checked = true;
            this.polygonsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.polygonsToolStripMenuItem.Name = "polygonsToolStripMenuItem";
            this.polygonsToolStripMenuItem.Size = new System.Drawing.Size(297, 40);
            this.polygonsToolStripMenuItem.Text = "Polygons On";
            this.polygonsToolStripMenuItem.Click += new System.EventHandler(this.polygonsToolStripMenuItem_Click);
            // 
            // pursuitLineToolStripMenuItem
            // 
            this.pursuitLineToolStripMenuItem.Checked = true;
            this.pursuitLineToolStripMenuItem.CheckOnClick = true;
            this.pursuitLineToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pursuitLineToolStripMenuItem.Name = "pursuitLineToolStripMenuItem";
            this.pursuitLineToolStripMenuItem.Size = new System.Drawing.Size(297, 40);
            this.pursuitLineToolStripMenuItem.Text = "Pursuit Line";
            this.pursuitLineToolStripMenuItem.Click += new System.EventHandler(this.pursuitLineToolStripMenuItem_Click);
            // 
            // skyToolStripMenu
            // 
            this.skyToolStripMenu.Checked = true;
            this.skyToolStripMenu.CheckOnClick = true;
            this.skyToolStripMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.skyToolStripMenu.Name = "skyToolStripMenu";
            this.skyToolStripMenu.Size = new System.Drawing.Size(297, 40);
            this.skyToolStripMenu.Text = "Sky On";
            this.skyToolStripMenu.Click += new System.EventHandler(this.skyToolStripMenu_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(294, 6);
            // 
            // simulatorOnToolStripMenuItem
            // 
            this.simulatorOnToolStripMenuItem.Checked = true;
            this.simulatorOnToolStripMenuItem.CheckOnClick = true;
            this.simulatorOnToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.simulatorOnToolStripMenuItem.Name = "simulatorOnToolStripMenuItem";
            this.simulatorOnToolStripMenuItem.Size = new System.Drawing.Size(297, 40);
            this.simulatorOnToolStripMenuItem.Text = "Simulator On";
            this.simulatorOnToolStripMenuItem.Click += new System.EventHandler(this.simulatorOnToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(294, 6);
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 22F);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Size = new System.Drawing.Size(1306, 38);
            this.menuStrip1.TabIndex = 49;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.explorerToolStripMenuItem,
            this.webCamToolStripMenuItem,
            this.fieldViewerToolStripMenuItem,
            this.googleEarthToolStripMenuItem,
            this.gPSDataToolStripMenuItem1,
            this.helpToolStripMenuItem1});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(93, 38);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // explorerToolStripMenuItem
            // 
            this.explorerToolStripMenuItem.Name = "explorerToolStripMenuItem";
            this.explorerToolStripMenuItem.Size = new System.Drawing.Size(360, 40);
            this.explorerToolStripMenuItem.Text = "Windows Explorer";
            this.explorerToolStripMenuItem.Click += new System.EventHandler(this.explorerToolStripMenuItem_Click);
            // 
            // webCamToolStripMenuItem
            // 
            this.webCamToolStripMenuItem.Name = "webCamToolStripMenuItem";
            this.webCamToolStripMenuItem.Size = new System.Drawing.Size(360, 40);
            this.webCamToolStripMenuItem.Text = "Web Cam";
            this.webCamToolStripMenuItem.Click += new System.EventHandler(this.webCamToolStripMenuItem_Click_1);
            // 
            // fieldViewerToolStripMenuItem
            // 
            this.fieldViewerToolStripMenuItem.Name = "fieldViewerToolStripMenuItem";
            this.fieldViewerToolStripMenuItem.Size = new System.Drawing.Size(360, 40);
            this.fieldViewerToolStripMenuItem.Text = "Field Viewer";
            this.fieldViewerToolStripMenuItem.Click += new System.EventHandler(this.fieldViewerToolStripMenuItem_Click);
            // 
            // googleEarthToolStripMenuItem
            // 
            this.googleEarthToolStripMenuItem.Name = "googleEarthToolStripMenuItem";
            this.googleEarthToolStripMenuItem.Size = new System.Drawing.Size(360, 40);
            this.googleEarthToolStripMenuItem.Text = "Google Earth - Flags";
            this.googleEarthToolStripMenuItem.Click += new System.EventHandler(this.googleEarthToolStripMenuItem_Click);
            // 
            // gPSDataToolStripMenuItem1
            // 
            this.gPSDataToolStripMenuItem1.Name = "gPSDataToolStripMenuItem1";
            this.gPSDataToolStripMenuItem1.Size = new System.Drawing.Size(360, 40);
            this.gPSDataToolStripMenuItem1.Text = "GPS Data";
            this.gPSDataToolStripMenuItem1.Click += new System.EventHandler(this.gPSDataToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuHelpAbout,
            this.helpToolStripMenuHelpHelp});
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(360, 40);
            this.helpToolStripMenuItem1.Text = "Help";
            // 
            // aboutToolStripMenuHelpAbout
            // 
            this.aboutToolStripMenuHelpAbout.Name = "aboutToolStripMenuHelpAbout";
            this.aboutToolStripMenuHelpAbout.Size = new System.Drawing.Size(168, 40);
            this.aboutToolStripMenuHelpAbout.Text = "About";
            this.aboutToolStripMenuHelpAbout.Click += new System.EventHandler(this.aboutToolStripMenuHelpAbout_Click);
            // 
            // helpToolStripMenuHelpHelp
            // 
            this.helpToolStripMenuHelpHelp.Name = "helpToolStripMenuHelpHelp";
            this.helpToolStripMenuHelpHelp.Size = new System.Drawing.Size(168, 40);
            this.helpToolStripMenuHelpHelp.Text = "Help";
            this.helpToolStripMenuHelpHelp.Click += new System.EventHandler(this.helpToolStripMenuHelpHelp_Click);
            // 
            // tmrWatchdog
            // 
            this.tmrWatchdog.Interval = 25;
            this.tmrWatchdog.Tick += new System.EventHandler(this.tmrWatchdog_tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.statusStrip1.Font = new System.Drawing.Font("Tahoma", 12F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripMinMax,
            this.btnHideTabs,
            this.stripDistance,
            this.toolStripDropDownBtnFuncs,
            this.toolStripDropDownButton2,
            this.stripSelectMode,
            this.stripTopoLocation,
            this.stripOnlineGPS,
            this.stripOnlineAutoSteer});
            this.statusStrip1.Location = new System.Drawing.Point(0, 643);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(1306, 41);
            this.statusStrip1.TabIndex = 95;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // stripMinMax
            // 
            this.stripMinMax.AutoSize = false;
            this.stripMinMax.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stripMinMax.Margin = new System.Windows.Forms.Padding(-4, 0, 0, 0);
            this.stripMinMax.Name = "stripMinMax";
            this.stripMinMax.Size = new System.Drawing.Size(150, 41);
            this.stripMinMax.Text = "Min:Max";
            // 
            // btnHideTabs
            // 
            this.btnHideTabs.AutoSize = false;
            this.btnHideTabs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnHideTabs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnHideTabs.Font = new System.Drawing.Font("Tahoma", 18F);
            this.btnHideTabs.Image = ((System.Drawing.Image)(resources.GetObject("btnHideTabs.Image")));
            this.btnHideTabs.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnHideTabs.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnHideTabs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHideTabs.Name = "btnHideTabs";
            this.btnHideTabs.ShowDropDownArrow = false;
            this.btnHideTabs.Size = new System.Drawing.Size(64, 39);
            this.btnHideTabs.Click += new System.EventHandler(this.btnHideTabs_Click);
            // 
            // stripDistance
            // 
            this.stripDistance.AutoSize = false;
            this.stripDistance.Font = new System.Drawing.Font("Tahoma", 18F);
            this.stripDistance.Margin = new System.Windows.Forms.Padding(-4, 0, 0, 0);
            this.stripDistance.Name = "stripDistance";
            this.stripDistance.Size = new System.Drawing.Size(102, 41);
            this.stripDistance.Text = "8888 ft";
            // 
            // toolStripDropDownBtnFuncs
            // 
            this.toolStripDropDownBtnFuncs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripDropDownBtnFuncs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripField,
            this.toolstripResetTrip});
            this.toolStripDropDownBtnFuncs.Font = new System.Drawing.Font("Tahoma", 18F);
            this.toolStripDropDownBtnFuncs.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownBtnFuncs.Image")));
            this.toolStripDropDownBtnFuncs.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripDropDownBtnFuncs.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDropDownBtnFuncs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownBtnFuncs.Name = "toolStripDropDownBtnFuncs";
            this.toolStripDropDownBtnFuncs.ShowDropDownArrow = false;
            this.toolStripDropDownBtnFuncs.Size = new System.Drawing.Size(82, 39);
            this.toolStripDropDownBtnFuncs.Text = "Start";
            this.toolStripDropDownBtnFuncs.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // toolstripField
            // 
            this.toolstripField.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolstripField.Font = new System.Drawing.Font("Tahoma", 22F);
            this.toolstripField.Image = ((System.Drawing.Image)(resources.GetObject("toolstripField.Image")));
            this.toolstripField.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolstripField.Name = "toolstripField";
            this.toolstripField.Size = new System.Drawing.Size(246, 70);
            this.toolstripField.Text = "Field";
            this.toolstripField.Click += new System.EventHandler(this.toolstripField_Click);
            // 
            // toolstripResetTrip
            // 
            this.toolstripResetTrip.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolstripResetTrip.Font = new System.Drawing.Font("Tahoma", 20F);
            this.toolstripResetTrip.Image = ((System.Drawing.Image)(resources.GetObject("toolstripResetTrip.Image")));
            this.toolstripResetTrip.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolstripResetTrip.Name = "toolstripResetTrip";
            this.toolstripResetTrip.Size = new System.Drawing.Size(246, 70);
            this.toolstripResetTrip.Text = ">0< Trip";
            this.toolstripResetTrip.Click += new System.EventHandler(this.toolstripResetTrip_Click_1);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripUDPConfig,
            this.toolstripUSBPortsConfig,
            this.toolstripNTRIPConfig,
            this.toolstripVehicleConfig,
            this.toolstripAutoSteerConfig});
            this.toolStripDropDownButton2.Font = new System.Drawing.Font("Tahoma", 18F);
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripDropDownButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.ShowDropDownArrow = false;
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(83, 39);
            this.toolStripDropDownButton2.Text = "Config";
            this.toolStripDropDownButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // toolstripUDPConfig
            // 
            this.toolstripUDPConfig.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolstripUDPConfig.Font = new System.Drawing.Font("Tahoma", 22F);
            this.toolstripUDPConfig.Image = ((System.Drawing.Image)(resources.GetObject("toolstripUDPConfig.Image")));
            this.toolstripUDPConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolstripUDPConfig.Name = "toolstripUDPConfig";
            this.toolstripUDPConfig.Size = new System.Drawing.Size(292, 86);
            this.toolstripUDPConfig.Text = "UDP";
            this.toolstripUDPConfig.Click += new System.EventHandler(this.toolstripUDPConfig_Click);
            // 
            // toolstripUSBPortsConfig
            // 
            this.toolstripUSBPortsConfig.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolstripUSBPortsConfig.Font = new System.Drawing.Font("Tahoma", 22F);
            this.toolstripUSBPortsConfig.Image = ((System.Drawing.Image)(resources.GetObject("toolstripUSBPortsConfig.Image")));
            this.toolstripUSBPortsConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolstripUSBPortsConfig.Name = "toolstripUSBPortsConfig";
            this.toolstripUSBPortsConfig.Size = new System.Drawing.Size(292, 86);
            this.toolstripUSBPortsConfig.Text = "Ports";
            this.toolstripUSBPortsConfig.Click += new System.EventHandler(this.toolstripUSBPortsConfig_Click);
            // 
            // toolstripNTRIPConfig
            // 
            this.toolstripNTRIPConfig.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolstripNTRIPConfig.Image = global::OpenGrade.Properties.Resources.Satellite64;
            this.toolstripNTRIPConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolstripNTRIPConfig.Name = "toolstripNTRIPConfig";
            this.toolstripNTRIPConfig.Size = new System.Drawing.Size(292, 86);
            this.toolstripNTRIPConfig.Text = "NTRIP";
            this.toolstripNTRIPConfig.Click += new System.EventHandler(this.toolstripNtripConfig_Click);
            // 
            // toolstripVehicleConfig
            // 
            this.toolstripVehicleConfig.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolstripVehicleConfig.Font = new System.Drawing.Font("Tahoma", 22F);
            this.toolstripVehicleConfig.Image = global::OpenGrade.Properties.Resources.Settings64;
            this.toolstripVehicleConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolstripVehicleConfig.Name = "toolstripVehicleConfig";
            this.toolstripVehicleConfig.Size = new System.Drawing.Size(292, 86);
            this.toolstripVehicleConfig.Text = "Vehicle";
            this.toolstripVehicleConfig.Click += new System.EventHandler(this.toolstripVehicleConfig_Click);
            // 
            // toolstripAutoSteerConfig
            // 
            this.toolstripAutoSteerConfig.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolstripAutoSteerConfig.Font = new System.Drawing.Font("Tahoma", 22F);
            this.toolstripAutoSteerConfig.Image = ((System.Drawing.Image)(resources.GetObject("toolstripAutoSteerConfig.Image")));
            this.toolstripAutoSteerConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolstripAutoSteerConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolstripAutoSteerConfig.Name = "toolstripAutoSteerConfig";
            this.toolstripAutoSteerConfig.Size = new System.Drawing.Size(292, 86);
            this.toolstripAutoSteerConfig.Text = "Auto Steer";
            this.toolstripAutoSteerConfig.Click += new System.EventHandler(this.toolstripAutoSteerConfig_Click);
            // 
            // stripSelectMode
            // 
            this.stripSelectMode.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.stripSelectMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.stripSelectMode.Font = new System.Drawing.Font("Tahoma", 18F);
            this.stripSelectMode.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stripSelectMode.Margin = new System.Windows.Forms.Padding(0);
            this.stripSelectMode.Name = "stripSelectMode";
            this.stripSelectMode.Size = new System.Drawing.Size(405, 41);
            this.stripSelectMode.Spring = true;
            this.stripSelectMode.Text = "Grade Mode";
            this.stripSelectMode.Click += new System.EventHandler(this.stripSelectMode_Click);
            // 
            // stripTopoLocation
            // 
            this.stripTopoLocation.AutoSize = false;
            this.stripTopoLocation.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stripTopoLocation.Margin = new System.Windows.Forms.Padding(-4, 0, 0, 0);
            this.stripTopoLocation.Name = "stripTopoLocation";
            this.stripTopoLocation.Size = new System.Drawing.Size(350, 41);
            this.stripTopoLocation.Text = "-- : ----- : -----";
            this.stripTopoLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // stripOnlineGPS
            // 
            this.stripOnlineGPS.AutoSize = false;
            this.stripOnlineGPS.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.stripOnlineGPS.Name = "stripOnlineGPS";
            this.stripOnlineGPS.Size = new System.Drawing.Size(16, 35);
            this.stripOnlineGPS.Value = 1;
            // 
            // stripOnlineAutoSteer
            // 
            this.stripOnlineAutoSteer.AutoToolTip = true;
            this.stripOnlineAutoSteer.ForeColor = System.Drawing.Color.Chartreuse;
            this.stripOnlineAutoSteer.Name = "stripOnlineAutoSteer";
            this.stripOnlineAutoSteer.Size = new System.Drawing.Size(16, 35);
            this.stripOnlineAutoSteer.ToolTipText = "Arduino";
            this.stripOnlineAutoSteer.Value = 1;
            // 
            // lblNorthing
            // 
            this.lblNorthing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNorthing.AutoSize = true;
            this.lblNorthing.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblNorthing.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblNorthing.Location = new System.Drawing.Point(21, 166);
            this.lblNorthing.Name = "lblNorthing";
            this.lblNorthing.Size = new System.Drawing.Size(59, 19);
            this.lblNorthing.TabIndex = 110;
            this.lblNorthing.Text = "label1";
            this.lblNorthing.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblEasting
            // 
            this.lblEasting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEasting.AutoSize = true;
            this.lblEasting.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblEasting.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblEasting.Location = new System.Drawing.Point(21, 143);
            this.lblEasting.Name = "lblEasting";
            this.lblEasting.Size = new System.Drawing.Size(59, 19);
            this.lblEasting.TabIndex = 111;
            this.lblEasting.Text = "label2";
            this.lblEasting.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblSpeed
            // 
            this.lblSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpeed.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblSpeed.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
            this.lblSpeed.Location = new System.Drawing.Point(931, -4);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(87, 41);
            this.lblSpeed.TabIndex = 116;
            this.lblSpeed.Text = "88.8";
            this.lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // contextMenuStripFlag
            // 
            this.contextMenuStripFlag.AutoSize = false;
            this.contextMenuStripFlag.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.contextMenuStripFlag.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFlagRed,
            this.toolStripMenuFlagGrn,
            this.toolStripMenuFlagYel,
            this.toolStripSeparator3,
            this.toolStripMenuFlagDelete,
            this.toolStripSeparator4,
            this.toolStripMenuFlagDeleteAll});
            this.contextMenuStripFlag.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.contextMenuStripFlag.Name = "contextMenuStripFlag";
            this.contextMenuStripFlag.Size = new System.Drawing.Size(72, 400);
            this.contextMenuStripFlag.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripFlag_Opening);
            // 
            // toolStripMenuItemFlagRed
            // 
            this.toolStripMenuItemFlagRed.AutoSize = false;
            this.toolStripMenuItemFlagRed.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuItemFlagRed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuItemFlagRed.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemFlagRed.Image")));
            this.toolStripMenuItemFlagRed.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItemFlagRed.Name = "toolStripMenuItemFlagRed";
            this.toolStripMenuItemFlagRed.Size = new System.Drawing.Size(70, 70);
            this.toolStripMenuItemFlagRed.Text = ".";
            this.toolStripMenuItemFlagRed.Click += new System.EventHandler(this.toolStripMenuItemFlagRed_Click);
            // 
            // toolStripMenuFlagGrn
            // 
            this.toolStripMenuFlagGrn.AutoSize = false;
            this.toolStripMenuFlagGrn.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuFlagGrn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuFlagGrn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuFlagGrn.Image")));
            this.toolStripMenuFlagGrn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuFlagGrn.Name = "toolStripMenuFlagGrn";
            this.toolStripMenuFlagGrn.Size = new System.Drawing.Size(70, 70);
            this.toolStripMenuFlagGrn.Text = ".";
            this.toolStripMenuFlagGrn.Click += new System.EventHandler(this.toolStripMenuGrn_Click);
            // 
            // toolStripMenuFlagYel
            // 
            this.toolStripMenuFlagYel.AutoSize = false;
            this.toolStripMenuFlagYel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuFlagYel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuFlagYel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuFlagYel.Image")));
            this.toolStripMenuFlagYel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuFlagYel.Name = "toolStripMenuFlagYel";
            this.toolStripMenuFlagYel.Size = new System.Drawing.Size(70, 70);
            this.toolStripMenuFlagYel.Text = ".";
            this.toolStripMenuFlagYel.Click += new System.EventHandler(this.toolStripMenuYel_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(68, 20);
            // 
            // toolStripMenuFlagDelete
            // 
            this.toolStripMenuFlagDelete.AutoSize = false;
            this.toolStripMenuFlagDelete.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripMenuFlagDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuFlagDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuFlagDelete.Image")));
            this.toolStripMenuFlagDelete.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuFlagDelete.Name = "toolStripMenuFlagDelete";
            this.toolStripMenuFlagDelete.Size = new System.Drawing.Size(70, 70);
            this.toolStripMenuFlagDelete.Text = ".";
            this.toolStripMenuFlagDelete.Click += new System.EventHandler(this.toolStripMenuFlagDelete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AutoSize = false;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(68, 20);
            // 
            // toolStripMenuFlagDeleteAll
            // 
            this.toolStripMenuFlagDeleteAll.AutoSize = false;
            this.toolStripMenuFlagDeleteAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuFlagDeleteAll.Image")));
            this.toolStripMenuFlagDeleteAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuFlagDeleteAll.Name = "toolStripMenuFlagDeleteAll";
            this.toolStripMenuFlagDeleteAll.Size = new System.Drawing.Size(70, 70);
            this.toolStripMenuFlagDeleteAll.Text = "toolStripMenuFlagDeleteAll";
            this.toolStripMenuFlagDeleteAll.Click += new System.EventHandler(this.toolStripMenuFlagDeleteAll_Click);
            // 
            // tboxSentence
            // 
            this.tboxSentence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxSentence.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tboxSentence.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tboxSentence.Location = new System.Drawing.Point(3, 0);
            this.tboxSentence.Multiline = true;
            this.tboxSentence.Name = "tboxSentence";
            this.tboxSentence.ReadOnly = true;
            this.tboxSentence.Size = new System.Drawing.Size(242, 80);
            this.tboxSentence.TabIndex = 134;
            // 
            // lblZone
            // 
            this.lblZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZone.AutoSize = true;
            this.lblZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblZone.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblZone.Location = new System.Drawing.Point(192, 143);
            this.lblZone.Name = "lblZone";
            this.lblZone.Size = new System.Drawing.Size(29, 19);
            this.lblZone.TabIndex = 135;
            this.lblZone.Text = "Zn";
            this.lblZone.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblSpeedUnits
            // 
            this.lblSpeedUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpeedUnits.AutoSize = true;
            this.lblSpeedUnits.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblSpeedUnits.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblSpeedUnits.Location = new System.Drawing.Point(1011, 14);
            this.lblSpeedUnits.Name = "lblSpeedUnits";
            this.lblSpeedUnits.Size = new System.Drawing.Size(38, 17);
            this.lblSpeedUnits.TabIndex = 139;
            this.lblSpeedUnits.Text = "kmh";
            this.lblSpeedUnits.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeading.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
            this.lblHeading.Location = new System.Drawing.Point(1060, -4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(130, 40);
            this.lblHeading.TabIndex = 117;
            this.lblHeading.Text = "359.8.";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "SettingsGear64.png");
            this.imageList1.Images.SetKeyName(1, "Satellite64.png");
            this.imageList1.Images.SetKeyName(2, "RunMan64.png");
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.DataPage);
            this.tabControl1.Controls.Add(this.configPage1);
            this.tabControl1.Controls.Add(this.controlPage2);
            this.tabControl1.Font = new System.Drawing.Font("Tahoma", 20.25F);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.ItemSize = new System.Drawing.Size(86, 85);
            this.tabControl1.Location = new System.Drawing.Point(965, 37);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(341, 263);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 150;
            // 
            // DataPage
            // 
            this.DataPage.BackColor = System.Drawing.SystemColors.ControlLight;
            this.DataPage.Controls.Add(this.label15);
            this.DataPage.Controls.Add(this.label14);
            this.DataPage.Controls.Add(this.lblSats);
            this.DataPage.Controls.Add(this.lblFixQuality);
            this.DataPage.Controls.Add(this.lblGPSHeading);
            this.DataPage.Controls.Add(this.lblZone);
            this.DataPage.Controls.Add(this.lblEasting);
            this.DataPage.Controls.Add(this.lblNorthing);
            this.DataPage.Controls.Add(this.label1);
            this.DataPage.Controls.Add(this.label7);
            this.DataPage.Controls.Add(this.label17);
            this.DataPage.Controls.Add(this.label19);
            this.DataPage.Controls.Add(this.label18);
            this.DataPage.Controls.Add(this.lblLongitude);
            this.DataPage.Controls.Add(this.lblLatitude);
            this.DataPage.Controls.Add(this.lblRoll);
            this.DataPage.Controls.Add(this.lblGyroHeading);
            this.DataPage.Controls.Add(this.tboxSentence);
            this.DataPage.Controls.Add(this.label10);
            this.DataPage.Controls.Add(this.label9);
            this.DataPage.Controls.Add(this.label8);
            this.DataPage.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.DataPage.ImageIndex = 1;
            this.DataPage.Location = new System.Drawing.Point(4, 4);
            this.DataPage.Name = "DataPage";
            this.DataPage.Size = new System.Drawing.Size(248, 255);
            this.DataPage.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(146, 163);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(44, 23);
            this.label15.TabIndex = 187;
            this.label15.Text = "Sat:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(146, 140);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 23);
            this.label14.TabIndex = 186;
            this.label14.Text = "Zn:";
            // 
            // lblSats
            // 
            this.lblSats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSats.AutoSize = true;
            this.lblSats.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblSats.Location = new System.Drawing.Point(192, 166);
            this.lblSats.Name = "lblSats";
            this.lblSats.Size = new System.Drawing.Size(19, 19);
            this.lblSats.TabIndex = 183;
            this.lblSats.Text = "S";
            // 
            // lblFixQuality
            // 
            this.lblFixQuality.AutoSize = true;
            this.lblFixQuality.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblFixQuality.Location = new System.Drawing.Point(157, 202);
            this.lblFixQuality.Name = "lblFixQuality";
            this.lblFixQuality.Size = new System.Drawing.Size(41, 19);
            this.lblFixQuality.TabIndex = 181;
            this.lblFixQuality.Text = "PPS";
            // 
            // lblGPSHeading
            // 
            this.lblGPSHeading.AutoSize = true;
            this.lblGPSHeading.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblGPSHeading.Location = new System.Drawing.Point(158, 229);
            this.lblGPSHeading.Name = "lblGPSHeading";
            this.lblGPSHeading.Size = new System.Drawing.Size(52, 23);
            this.lblGPSHeading.TabIndex = 136;
            this.lblGPSHeading.Text = "99.3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(403, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 19);
            this.label1.TabIndex = 185;
            this.label1.Text = "E:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(401, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 19);
            this.label7.TabIndex = 184;
            this.label7.Text = "N:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(123, 198);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(40, 23);
            this.label17.TabIndex = 182;
            this.label17.Text = "Fix:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label19.Location = new System.Drawing.Point(12, 83);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(42, 23);
            this.label19.TabIndex = 178;
            this.label19.Text = "Lat:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label18.Location = new System.Drawing.Point(7, 114);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 23);
            this.label18.TabIndex = 177;
            this.label18.Text = "Lon:";
            // 
            // lblLongitude
            // 
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblLongitude.Location = new System.Drawing.Point(51, 114);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(124, 23);
            this.lblLongitude.TabIndex = 176;
            this.lblLongitude.Text = "111.253475";
            // 
            // lblLatitude
            // 
            this.lblLatitude.AutoSize = true;
            this.lblLatitude.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblLatitude.Location = new System.Drawing.Point(51, 83);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(112, 23);
            this.lblLatitude.TabIndex = 175;
            this.lblLatitude.Text = "53.234455";
            // 
            // lblRoll
            // 
            this.lblRoll.AutoSize = true;
            this.lblRoll.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblRoll.Location = new System.Drawing.Point(54, 229);
            this.lblRoll.Name = "lblRoll";
            this.lblRoll.Size = new System.Drawing.Size(40, 23);
            this.lblRoll.TabIndex = 138;
            this.lblRoll.Text = "1.2";
            // 
            // lblGyroHeading
            // 
            this.lblGyroHeading.AutoSize = true;
            this.lblGyroHeading.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblGyroHeading.Location = new System.Drawing.Point(54, 198);
            this.lblGyroHeading.Name = "lblGyroHeading";
            this.lblGyroHeading.Size = new System.Drawing.Size(52, 23);
            this.lblGyroHeading.TabIndex = 135;
            this.lblGyroHeading.Text = "22.6";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label10.Location = new System.Drawing.Point(12, 229);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 23);
            this.label10.TabIndex = 4;
            this.label10.Text = "Roll:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label9.Location = new System.Drawing.Point(3, 198);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 23);
            this.label9.TabIndex = 3;
            this.label9.Text = "Pitch:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label8.Location = new System.Drawing.Point(113, 229);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 23);
            this.label8.TabIndex = 2;
            this.label8.Text = "GPS:";
            // 
            // configPage1
            // 
            this.configPage1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.configPage1.Controls.Add(this.btnABLine);
            this.configPage1.Controls.Add(this.btnUnits);
            this.configPage1.Controls.Add(this.btnFileExplorer);
            this.configPage1.Controls.Add(this.btnGPSData);
            this.configPage1.Controls.Add(this.btnFlag);
            this.configPage1.Controls.Add(this.btnSnap);
            this.configPage1.ImageIndex = 0;
            this.configPage1.Location = new System.Drawing.Point(4, 4);
            this.configPage1.Name = "configPage1";
            this.configPage1.Padding = new System.Windows.Forms.Padding(3);
            this.configPage1.Size = new System.Drawing.Size(248, 255);
            this.configPage1.TabIndex = 0;
            // 
            // btnABLine
            // 
            this.btnABLine.BackColor = System.Drawing.Color.Lavender;
            this.btnABLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnABLine.Enabled = false;
            this.btnABLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnABLine.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnABLine.Image = ((System.Drawing.Image)(resources.GetObject("btnABLine.Image")));
            this.btnABLine.Location = new System.Drawing.Point(3, 83);
            this.btnABLine.Name = "btnABLine";
            this.btnABLine.Size = new System.Drawing.Size(90, 90);
            this.btnABLine.TabIndex = 0;
            this.btnABLine.Text = "0";
            this.btnABLine.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnABLine.UseVisualStyleBackColor = false;
            this.btnABLine.Click += new System.EventHandler(this.btnABLine_Click);
            // 
            // btnUnits
            // 
            this.btnUnits.BackColor = System.Drawing.Color.AliceBlue;
            this.btnUnits.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnUnits.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnUnits.Image = ((System.Drawing.Image)(resources.GetObject("btnUnits.Image")));
            this.btnUnits.Location = new System.Drawing.Point(102, 0);
            this.btnUnits.Name = "btnUnits";
            this.btnUnits.Size = new System.Drawing.Size(100, 100);
            this.btnUnits.TabIndex = 142;
            this.btnUnits.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUnits.UseVisualStyleBackColor = false;
            this.btnUnits.Click += new System.EventHandler(this.btnUnits_Click);
            // 
            // btnFileExplorer
            // 
            this.btnFileExplorer.BackColor = System.Drawing.Color.AliceBlue;
            this.btnFileExplorer.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnFileExplorer.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnFileExplorer.Image = ((System.Drawing.Image)(resources.GetObject("btnFileExplorer.Image")));
            this.btnFileExplorer.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFileExplorer.Location = new System.Drawing.Point(3, 0);
            this.btnFileExplorer.Name = "btnFileExplorer";
            this.btnFileExplorer.Size = new System.Drawing.Size(100, 100);
            this.btnFileExplorer.TabIndex = 141;
            this.btnFileExplorer.Text = "Files";
            this.btnFileExplorer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnFileExplorer.UseVisualStyleBackColor = false;
            this.btnFileExplorer.Click += new System.EventHandler(this.btnFileExplorer_Click);
            // 
            // btnGPSData
            // 
            this.btnGPSData.BackColor = System.Drawing.Color.AliceBlue;
            this.btnGPSData.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnGPSData.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnGPSData.Image = ((System.Drawing.Image)(resources.GetObject("btnGPSData.Image")));
            this.btnGPSData.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGPSData.Location = new System.Drawing.Point(99, 83);
            this.btnGPSData.Name = "btnGPSData";
            this.btnGPSData.Size = new System.Drawing.Size(100, 100);
            this.btnGPSData.TabIndex = 138;
            this.btnGPSData.Text = "GPS";
            this.btnGPSData.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGPSData.UseVisualStyleBackColor = false;
            this.btnGPSData.Click += new System.EventHandler(this.btnGPSData_Click);
            // 
            // btnFlag
            // 
            this.btnFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlag.BackColor = System.Drawing.Color.Lavender;
            this.btnFlag.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnFlag.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnFlag.Image = ((System.Drawing.Image)(resources.GetObject("btnFlag.Image")));
            this.btnFlag.Location = new System.Drawing.Point(6, 165);
            this.btnFlag.Name = "btnFlag";
            this.btnFlag.Size = new System.Drawing.Size(90, 90);
            this.btnFlag.TabIndex = 121;
            this.btnFlag.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnFlag.UseVisualStyleBackColor = false;
            this.btnFlag.Click += new System.EventHandler(this.btnFlag_Click);
            // 
            // btnSnap
            // 
            this.btnSnap.BackColor = System.Drawing.Color.Lavender;
            this.btnSnap.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnSnap.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnSnap.Image = ((System.Drawing.Image)(resources.GetObject("btnSnap.Image")));
            this.btnSnap.Location = new System.Drawing.Point(131, 170);
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(90, 90);
            this.btnSnap.TabIndex = 132;
            this.btnSnap.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSnap.UseVisualStyleBackColor = false;
            this.btnSnap.Click += new System.EventHandler(this.btnSnap_Click);
            // 
            // controlPage2
            // 
            this.controlPage2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.controlPage2.Controls.Add(this.lblCutDelta);
            this.controlPage2.Controls.Add(this.btnZeroAltitude);
            this.controlPage2.Controls.Add(this.cboxLastPass);
            this.controlPage2.Controls.Add(this.cboxLaserModeOnOff);
            this.controlPage2.ImageIndex = 2;
            this.controlPage2.Location = new System.Drawing.Point(4, 4);
            this.controlPage2.Name = "controlPage2";
            this.controlPage2.Size = new System.Drawing.Size(248, 255);
            this.controlPage2.TabIndex = 4;
            // 
            // lblCutDelta
            // 
            this.lblCutDelta.BackColor = System.Drawing.Color.Lavender;
            this.lblCutDelta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCutDelta.Font = new System.Drawing.Font("Tahoma", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCutDelta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCutDelta.Location = new System.Drawing.Point(86, 178);
            this.lblCutDelta.Name = "lblCutDelta";
            this.lblCutDelta.Size = new System.Drawing.Size(160, 77);
            this.lblCutDelta.TabIndex = 220;
            this.lblCutDelta.Text = "93";
            this.lblCutDelta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnZeroAltitude
            // 
            this.btnZeroAltitude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZeroAltitude.BackColor = System.Drawing.Color.Lavender;
            this.btnZeroAltitude.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZeroAltitude.Font = new System.Drawing.Font("Tahoma", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZeroAltitude.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnZeroAltitude.Location = new System.Drawing.Point(5, 92);
            this.btnZeroAltitude.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnZeroAltitude.Name = "btnZeroAltitude";
            this.btnZeroAltitude.Size = new System.Drawing.Size(243, 81);
            this.btnZeroAltitude.TabIndex = 221;
            this.btnZeroAltitude.Text = "0.55";
            this.btnZeroAltitude.UseVisualStyleBackColor = false;
            this.btnZeroAltitude.Click += new System.EventHandler(this.btnZeroAltitude_Click);
            // 
            // cboxLastPass
            // 
            this.cboxLastPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboxLastPass.Appearance = System.Windows.Forms.Appearance.Button;
            this.cboxLastPass.BackColor = System.Drawing.Color.Transparent;
            this.cboxLastPass.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxLastPass.Image = global::OpenGrade.Properties.Resources.LastPassOnOff;
            this.cboxLastPass.Location = new System.Drawing.Point(137, 3);
            this.cboxLastPass.Name = "cboxLastPass";
            this.cboxLastPass.Size = new System.Drawing.Size(90, 90);
            this.cboxLastPass.TabIndex = 221;
            this.cboxLastPass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cboxLastPass.UseVisualStyleBackColor = false;
            // 
            // cboxLaserModeOnOff
            // 
            this.cboxLaserModeOnOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboxLaserModeOnOff.Appearance = System.Windows.Forms.Appearance.Button;
            this.cboxLaserModeOnOff.BackColor = System.Drawing.Color.Transparent;
            this.cboxLaserModeOnOff.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxLaserModeOnOff.Image = global::OpenGrade.Properties.Resources.LaserMode;
            this.cboxLaserModeOnOff.Location = new System.Drawing.Point(5, 3);
            this.cboxLaserModeOnOff.Name = "cboxLaserModeOnOff";
            this.cboxLaserModeOnOff.Size = new System.Drawing.Size(126, 90);
            this.cboxLaserModeOnOff.TabIndex = 242;
            this.cboxLaserModeOnOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cboxLaserModeOnOff.UseVisualStyleBackColor = false;
            // 
            // lblCut
            // 
            this.lblCut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCut.AutoSize = true;
            this.lblCut.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblCut.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCut.Location = new System.Drawing.Point(1008, 375);
            this.lblCut.Name = "lblCut";
            this.lblCut.Size = new System.Drawing.Size(55, 29);
            this.lblCut.TabIndex = 217;
            this.lblCut.Text = "C/Al";
            // 
            // label27
            // 
            this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label27.AutoSize = true;
            this.label27.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label27.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(969, 341);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(151, 13);
            this.label27.TabIndex = 202;
            this.label27.Text = "in cm, positive to lift the blade";
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label25.AutoSize = true;
            this.label25.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label25.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(967, 312);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(152, 29);
            this.label25.TabIndex = 200;
            this.label25.Text = "Blade Offset:";
            // 
            // lblAltitude
            // 
            this.lblAltitude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAltitude.AutoSize = true;
            this.lblAltitude.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblAltitude.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAltitude.Location = new System.Drawing.Point(769, -4);
            this.lblAltitude.Name = "lblAltitude";
            this.lblAltitude.Size = new System.Drawing.Size(108, 39);
            this.lblAltitude.TabIndex = 179;
            this.lblAltitude.Text = "356m";
            // 
            // panelSimControls
            // 
            this.panelSimControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelSimControls.Controls.Add(this.nudLongitude);
            this.panelSimControls.Controls.Add(this.label13);
            this.panelSimControls.Controls.Add(this.label6);
            this.panelSimControls.Controls.Add(this.nudLatitude);
            this.panelSimControls.Controls.Add(this.btnSimGoTo);
            this.panelSimControls.Controls.Add(this.label3);
            this.panelSimControls.Controls.Add(this.btnResetSteerAngle);
            this.panelSimControls.Controls.Add(this.btnResetSim);
            this.panelSimControls.Controls.Add(this.lblSteerAngle);
            this.panelSimControls.Controls.Add(this.label11);
            this.panelSimControls.Controls.Add(this.label12);
            this.panelSimControls.Controls.Add(this.tbarSteerAngle);
            this.panelSimControls.Controls.Add(this.tbarStepDistance);
            this.panelSimControls.Controls.Add(this.nudElevation);
            this.panelSimControls.Location = new System.Drawing.Point(3, 550);
            this.panelSimControls.Name = "panelSimControls";
            this.panelSimControls.Size = new System.Drawing.Size(544, 90);
            this.panelSimControls.TabIndex = 172;
            this.panelSimControls.Visible = false;
            // 
            // nudLongitude
            // 
            this.nudLongitude.DecimalPlaces = 7;
            this.nudLongitude.Location = new System.Drawing.Point(214, 58);
            this.nudLongitude.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudLongitude.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.nudLongitude.Name = "nudLongitude";
            this.nudLongitude.Size = new System.Drawing.Size(120, 27);
            this.nudLongitude.TabIndex = 190;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(165, 60);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 19);
            this.label13.TabIndex = 189;
            this.label13.Text = "Long";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 19);
            this.label6.TabIndex = 188;
            this.label6.Text = "Lat";
            // 
            // nudLatitude
            // 
            this.nudLatitude.DecimalPlaces = 7;
            this.nudLatitude.Location = new System.Drawing.Point(39, 58);
            this.nudLatitude.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudLatitude.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.nudLatitude.Name = "nudLatitude";
            this.nudLatitude.Size = new System.Drawing.Size(120, 27);
            this.nudLatitude.TabIndex = 187;
            // 
            // btnSimGoTo
            // 
            this.btnSimGoTo.Location = new System.Drawing.Point(421, 47);
            this.btnSimGoTo.Name = "btnSimGoTo";
            this.btnSimGoTo.Size = new System.Drawing.Size(79, 40);
            this.btnSimGoTo.TabIndex = 186;
            this.btnSimGoTo.Text = "Go To";
            this.btnSimGoTo.UseVisualStyleBackColor = true;
            this.btnSimGoTo.Click += new System.EventHandler(this.btnSimGoTo_click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label3.Location = new System.Drawing.Point(304, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 16);
            this.label3.TabIndex = 172;
            this.label3.Text = "L";
            // 
            // btnResetSteerAngle
            // 
            this.btnResetSteerAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetSteerAngle.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnResetSteerAngle.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnResetSteerAngle.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.btnResetSteerAngle.Location = new System.Drawing.Point(182, 7);
            this.btnResetSteerAngle.Name = "btnResetSteerAngle";
            this.btnResetSteerAngle.Size = new System.Drawing.Size(52, 29);
            this.btnResetSteerAngle.TabIndex = 162;
            this.btnResetSteerAngle.Text = ">0<";
            this.btnResetSteerAngle.UseVisualStyleBackColor = false;
            this.btnResetSteerAngle.Click += new System.EventHandler(this.btnResetSteerAngle_Click);
            // 
            // btnResetSim
            // 
            this.btnResetSim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetSim.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.btnResetSim.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnResetSim.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.btnResetSim.Location = new System.Drawing.Point(246, 8);
            this.btnResetSim.Name = "btnResetSim";
            this.btnResetSim.Size = new System.Drawing.Size(52, 26);
            this.btnResetSim.TabIndex = 164;
            this.btnResetSim.Text = "Reset";
            this.btnResetSim.UseVisualStyleBackColor = false;
            this.btnResetSim.Click += new System.EventHandler(this.btnResetSim_Click);
            // 
            // lblSteerAngle
            // 
            this.lblSteerAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSteerAngle.AutoSize = true;
            this.lblSteerAngle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblSteerAngle.Location = new System.Drawing.Point(140, 24);
            this.lblSteerAngle.Name = "lblSteerAngle";
            this.lblSteerAngle.Size = new System.Drawing.Size(19, 19);
            this.lblSteerAngle.TabIndex = 163;
            this.lblSteerAngle.Text = "0";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label11.Location = new System.Drawing.Point(4, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(40, 16);
            this.label11.TabIndex = 170;
            this.label11.Text = "B N M";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label12.Location = new System.Drawing.Point(367, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 16);
            this.label12.TabIndex = 171;
            this.label12.Text = "H J K";
            // 
            // tbarSteerAngle
            // 
            this.tbarSteerAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbarSteerAngle.LargeChange = 10;
            this.tbarSteerAngle.Location = new System.Drawing.Point(3, 5);
            this.tbarSteerAngle.Maximum = 300;
            this.tbarSteerAngle.Minimum = -300;
            this.tbarSteerAngle.Name = "tbarSteerAngle";
            this.tbarSteerAngle.RightToLeftLayout = true;
            this.tbarSteerAngle.Size = new System.Drawing.Size(181, 45);
            this.tbarSteerAngle.TabIndex = 161;
            this.tbarSteerAngle.TickFrequency = 30;
            this.tbarSteerAngle.Scroll += new System.EventHandler(this.tbarSteerAngle_Scroll);
            // 
            // tbarStepDistance
            // 
            this.tbarStepDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbarStepDistance.LargeChange = 10;
            this.tbarStepDistance.Location = new System.Drawing.Point(307, 6);
            this.tbarStepDistance.Maximum = 300;
            this.tbarStepDistance.Name = "tbarStepDistance";
            this.tbarStepDistance.Size = new System.Drawing.Size(117, 45);
            this.tbarStepDistance.TabIndex = 160;
            this.tbarStepDistance.TickFrequency = 10;
            this.tbarStepDistance.Value = 20;
            this.tbarStepDistance.Scroll += new System.EventHandler(this.tbarStepDistance_Scroll);
            // 
            // nudElevation
            // 
            this.nudElevation.DecimalPlaces = 3;
            this.nudElevation.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudElevation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudElevation.Location = new System.Drawing.Point(421, 4);
            this.nudElevation.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudElevation.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudElevation.Name = "nudElevation";
            this.nudElevation.Size = new System.Drawing.Size(123, 36);
            this.nudElevation.TabIndex = 185;
            this.nudElevation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudElevation.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudElevation.ValueChanged += new System.EventHandler(this.nudElevation_ValueChanged);
            // 
            // lblPureSteerAngle
            // 
            this.lblPureSteerAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPureSteerAngle.AutoSize = true;
            this.lblPureSteerAngle.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblPureSteerAngle.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
            this.lblPureSteerAngle.Location = new System.Drawing.Point(1194, -4);
            this.lblPureSteerAngle.Name = "lblPureSteerAngle";
            this.lblPureSteerAngle.Size = new System.Drawing.Size(107, 39);
            this.lblPureSteerAngle.TabIndex = 137;
            this.lblPureSteerAngle.Text = "88.88";
            // 
            // timerSim
            // 
            this.timerSim.Enabled = true;
            this.timerSim.Interval = 200;
            this.timerSim.Tick += new System.EventHandler(this.timerSim_Tick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(334, 62);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // lblScale
            // 
            this.lblScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScale.AutoSize = true;
            this.lblScale.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblScale.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScale.Location = new System.Drawing.Point(1146, 483);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(71, 29);
            this.lblScale.TabIndex = 218;
            this.lblScale.Text = "--- %";
            // 
            // pbarCutBelow
            // 
            this.pbarCutBelow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbarCutBelow.BarBackColor = System.Drawing.SystemColors.ControlLight;
            cBlendItems1.iColor = new System.Drawing.Color[] {
        System.Drawing.Color.Navy,
        System.Drawing.Color.Blue};
            cBlendItems1.iPoint = new float[] {
        0F,
        1F};
            this.pbarCutBelow.BarColorBlend = cBlendItems1;
            this.pbarCutBelow.BarColorSolid = System.Drawing.Color.Black;
            this.pbarCutBelow.BarColorSolidB = System.Drawing.Color.Red;
            this.pbarCutBelow.BarLengthValue = ((short)(40));
            this.pbarCutBelow.BarPadding = new System.Windows.Forms.Padding(0);
            this.pbarCutBelow.BarStyleFill = ProgBar.ProgBarPlus.eBarStyle.Hatch;
            this.pbarCutBelow.BarStyleHatch = System.Drawing.Drawing2D.HatchStyle.Horizontal;
            this.pbarCutBelow.BarStyleLinear = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.pbarCutBelow.BarStyleTexture = null;
            this.pbarCutBelow.BorderWidth = ((short)(1));
            this.pbarCutBelow.Corners.All = ((short)(0));
            this.pbarCutBelow.Corners.LowerLeft = ((short)(0));
            this.pbarCutBelow.Corners.LowerRight = ((short)(0));
            this.pbarCutBelow.Corners.UpperLeft = ((short)(0));
            this.pbarCutBelow.Corners.UpperRight = ((short)(0));
            this.pbarCutBelow.CylonInterval = ((short)(1));
            this.pbarCutBelow.CylonMove = 5F;
            this.pbarCutBelow.FillDirection = ProgBar.ProgBarPlus.eFillDirection.Down_Left;
            cFocalPoints1.CenterPoint = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints1.CenterPoint")));
            cFocalPoints1.FocusScales = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints1.FocusScales")));
            this.pbarCutBelow.FocalPoints = cFocalPoints1;
            this.pbarCutBelow.Location = new System.Drawing.Point(1231, 493);
            this.pbarCutBelow.Name = "pbarCutBelow";
            this.pbarCutBelow.Orientation = ProgBar.ProgBarPlus.eOrientation.Vertical;
            this.pbarCutBelow.ShapeTextFont = new System.Drawing.Font("Arial Black", 30F);
            this.pbarCutBelow.Size = new System.Drawing.Size(72, 150);
            this.pbarCutBelow.TabIndex = 223;
            this.pbarCutBelow.TextFormat = "Process {1}% Done";
            this.pbarCutBelow.Click += new System.EventHandler(this.pbarCutBelow_Click);
            // 
            // pbarCutAbove
            // 
            this.pbarCutAbove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbarCutAbove.BarBackColor = System.Drawing.SystemColors.ControlLight;
            cBlendItems2.iColor = new System.Drawing.Color[] {
        System.Drawing.Color.Navy,
        System.Drawing.Color.Blue};
            cBlendItems2.iPoint = new float[] {
        0F,
        1F};
            this.pbarCutAbove.BarColorBlend = cBlendItems2;
            this.pbarCutAbove.BarColorSolid = System.Drawing.Color.Black;
            this.pbarCutAbove.BarColorSolidB = System.Drawing.Color.LimeGreen;
            this.pbarCutAbove.BarLengthValue = ((short)(40));
            this.pbarCutAbove.BarPadding = new System.Windows.Forms.Padding(0);
            this.pbarCutAbove.BarStyleFill = ProgBar.ProgBarPlus.eBarStyle.Hatch;
            this.pbarCutAbove.BarStyleHatch = System.Drawing.Drawing2D.HatchStyle.Horizontal;
            this.pbarCutAbove.BarStyleLinear = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pbarCutAbove.BarStyleTexture = null;
            this.pbarCutAbove.BorderWidth = ((short)(1));
            this.pbarCutAbove.Corners.All = ((short)(0));
            this.pbarCutAbove.Corners.LowerLeft = ((short)(0));
            this.pbarCutAbove.Corners.LowerRight = ((short)(0));
            this.pbarCutAbove.Corners.UpperLeft = ((short)(0));
            this.pbarCutAbove.Corners.UpperRight = ((short)(0));
            this.pbarCutAbove.CylonInterval = ((short)(1));
            this.pbarCutAbove.CylonMove = 5F;
            cFocalPoints2.CenterPoint = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints2.CenterPoint")));
            cFocalPoints2.FocusScales = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints2.FocusScales")));
            this.pbarCutAbove.FocalPoints = cFocalPoints2;
            this.pbarCutAbove.Location = new System.Drawing.Point(1231, 343);
            this.pbarCutAbove.Name = "pbarCutAbove";
            this.pbarCutAbove.Orientation = ProgBar.ProgBarPlus.eOrientation.Vertical;
            this.pbarCutAbove.ShapeTextFont = new System.Drawing.Font("Arial Black", 30F);
            this.pbarCutAbove.Size = new System.Drawing.Size(72, 150);
            this.pbarCutAbove.TabIndex = 224;
            this.pbarCutAbove.TextFormat = "Process {1}% Done";
            this.pbarCutAbove.Click += new System.EventHandler(this.pbarCutAbove_Click);
            // 
            // lblBarGraphMax
            // 
            this.lblBarGraphMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBarGraphMax.AutoSize = true;
            this.lblBarGraphMax.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblBarGraphMax.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblBarGraphMax.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBarGraphMax.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblBarGraphMax.Location = new System.Drawing.Point(1261, 308);
            this.lblBarGraphMax.Name = "lblBarGraphMax";
            this.lblBarGraphMax.Size = new System.Drawing.Size(23, 29);
            this.lblBarGraphMax.TabIndex = 225;
            this.lblBarGraphMax.Text = "-";
            this.lblBarGraphMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.label5.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label5.Location = new System.Drawing.Point(1241, 308);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 29);
            this.label5.TabIndex = 226;
            this.label5.Text = "x";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnGoogleEarth
            // 
            this.btnGoogleEarth.BackColor = System.Drawing.Color.Lavender;
            this.btnGoogleEarth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnGoogleEarth.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGoogleEarth.Image = global::OpenGrade.Properties.Resources.GoogleEarth;
            this.btnGoogleEarth.Location = new System.Drawing.Point(6, 128);
            this.btnGoogleEarth.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnGoogleEarth.Name = "btnGoogleEarth";
            this.btnGoogleEarth.Size = new System.Drawing.Size(79, 79);
            this.btnGoogleEarth.TabIndex = 159;
            this.btnGoogleEarth.UseVisualStyleBackColor = false;
            this.btnGoogleEarth.Click += new System.EventHandler(this.btnGoogleEarth_Click);
            // 
            // btnAutoSteer
            // 
            this.btnAutoSteer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoSteer.BackColor = System.Drawing.Color.Lavender;
            this.btnAutoSteer.ContextMenuStrip = this.contextMenuStripFlag;
            this.btnAutoSteer.Enabled = false;
            this.btnAutoSteer.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnAutoSteer.Image = ((System.Drawing.Image)(resources.GetObject("btnAutoSteer.Image")));
            this.btnAutoSteer.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnAutoSteer.Location = new System.Drawing.Point(857, 247);
            this.btnAutoSteer.Name = "btnAutoSteer";
            this.btnAutoSteer.Size = new System.Drawing.Size(90, 90);
            this.btnAutoSteer.TabIndex = 128;
            this.btnAutoSteer.Text = "import agd file";
            this.btnAutoSteer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAutoSteer.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnAutoSteer.UseVisualStyleBackColor = false;
            this.btnAutoSteer.Visible = false;
            this.btnAutoSteer.Click += new System.EventHandler(this.btnAutoSteer_Click);
            // 
            // btnContour
            // 
            this.btnContour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContour.BackColor = System.Drawing.Color.Lavender;
            this.btnContour.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnContour.Enabled = false;
            this.btnContour.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnContour.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnContour.Image = ((System.Drawing.Image)(resources.GetObject("btnContour.Image")));
            this.btnContour.Location = new System.Drawing.Point(761, 128);
            this.btnContour.Name = "btnContour";
            this.btnContour.Size = new System.Drawing.Size(90, 90);
            this.btnContour.TabIndex = 105;
            this.btnContour.Text = "Altitude";
            this.btnContour.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnContour.UseVisualStyleBackColor = false;
            this.btnContour.Visible = false;
            this.btnContour.Click += new System.EventHandler(this.btnContour_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomIn.BackColor = System.Drawing.Color.Lavender;
            this.btnZoomIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.BackgroundImage")));
            this.btnZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomIn.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnZoomIn.Location = new System.Drawing.Point(793, 51);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(69, 52);
            this.btnZoomIn.TabIndex = 120;
            this.btnZoomIn.UseVisualStyleBackColor = false;
            this.btnZoomIn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnZoomIn_MouseDown);
            // 
            // btnTiltDown
            // 
            this.btnTiltDown.BackColor = System.Drawing.Color.Lavender;
            this.btnTiltDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTiltDown.BackgroundImage")));
            this.btnTiltDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnTiltDown.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnTiltDown.Location = new System.Drawing.Point(103, 51);
            this.btnTiltDown.Name = "btnTiltDown";
            this.btnTiltDown.Size = new System.Drawing.Size(69, 52);
            this.btnTiltDown.TabIndex = 122;
            this.btnTiltDown.UseVisualStyleBackColor = false;
            this.btnTiltDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTiltDown_MouseDown);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomOut.BackColor = System.Drawing.Color.Lavender;
            this.btnZoomOut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.BackgroundImage")));
            this.btnZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomOut.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnZoomOut.Location = new System.Drawing.Point(878, 51);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(69, 52);
            this.btnZoomOut.TabIndex = 119;
            this.btnZoomOut.UseVisualStyleBackColor = false;
            this.btnZoomOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnZoomOut_MouseDown);
            // 
            // btnTiltUp
            // 
            this.btnTiltUp.BackColor = System.Drawing.Color.Lavender;
            this.btnTiltUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTiltUp.BackgroundImage")));
            this.btnTiltUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnTiltUp.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnTiltUp.Location = new System.Drawing.Point(6, 51);
            this.btnTiltUp.Name = "btnTiltUp";
            this.btnTiltUp.Size = new System.Drawing.Size(69, 52);
            this.btnTiltUp.TabIndex = 123;
            this.btnTiltUp.UseVisualStyleBackColor = false;
            this.btnTiltUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTiltUp_MouseDown);
            // 
            // btnManualOffOn
            // 
            this.btnManualOffOn.BackColor = System.Drawing.Color.Lavender;
            this.btnManualOffOn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnManualOffOn.Enabled = false;
            this.btnManualOffOn.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.btnManualOffOn.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.btnManualOffOn.Image = ((System.Drawing.Image)(resources.GetObject("btnManualOffOn.Image")));
            this.btnManualOffOn.Location = new System.Drawing.Point(178, 51);
            this.btnManualOffOn.Name = "btnManualOffOn";
            this.btnManualOffOn.Size = new System.Drawing.Size(108, 90);
            this.btnManualOffOn.TabIndex = 98;
            this.btnManualOffOn.UseVisualStyleBackColor = false;
            this.btnManualOffOn.Visible = false;
            this.btnManualOffOn.Click += new System.EventHandler(this.btnManualOffOn_Click);
            // 
            // lblFixUpdateHz
            // 
            this.lblFixUpdateHz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFixUpdateHz.AutoSize = true;
            this.lblFixUpdateHz.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblFixUpdateHz.Location = new System.Drawing.Point(598, 9);
            this.lblFixUpdateHz.Name = "lblFixUpdateHz";
            this.lblFixUpdateHz.Size = new System.Drawing.Size(159, 19);
            this.lblFixUpdateHz.TabIndex = 227;
            this.lblFixUpdateHz.Text = "gps hz, fix, frame ms";
            this.lblFixUpdateHz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnStartPause
            // 
            this.btnStartPause.Font = new System.Drawing.Font("Tahoma", 22F, System.Drawing.FontStyle.Bold);
            this.btnStartPause.Location = new System.Drawing.Point(292, 63);
            this.btnStartPause.Name = "btnStartPause";
            this.btnStartPause.Size = new System.Drawing.Size(135, 58);
            this.btnStartPause.TabIndex = 228;
            this.btnStartPause.Text = "START";
            this.btnStartPause.UseVisualStyleBackColor = true;
            this.btnStartPause.Visible = false;
            this.btnStartPause.Click += new System.EventHandler(this.btnStartPause_Click);
            // 
            // btnBoundarySide
            // 
            this.btnBoundarySide.Location = new System.Drawing.Point(451, 63);
            this.btnBoundarySide.Name = "btnBoundarySide";
            this.btnBoundarySide.Size = new System.Drawing.Size(109, 58);
            this.btnBoundarySide.TabIndex = 229;
            this.btnBoundarySide.Text = "boundary Left";
            this.btnBoundarySide.UseVisualStyleBackColor = true;
            this.btnBoundarySide.Visible = false;
            this.btnBoundarySide.Click += new System.EventHandler(this.btnBoundarySide_Click);
            // 
            // numBladeOffset
            // 
            this.numBladeOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numBladeOffset.Font = new System.Drawing.Font("Tahoma", 30F);
            this.numBladeOffset.Location = new System.Drawing.Point(1126, 312);
            this.numBladeOffset.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numBladeOffset.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.numBladeOffset.Name = "numBladeOffset";
            this.numBladeOffset.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.numBladeOffset.Size = new System.Drawing.Size(91, 56);
            this.numBladeOffset.TabIndex = 230;
            this.numBladeOffset.ValueChanged += new System.EventHandler(this.numBladeOffset_ValueChanged);
            // 
            // btnCutFillElev
            // 
            this.btnCutFillElev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCutFillElev.Location = new System.Drawing.Point(857, 139);
            this.btnCutFillElev.Name = "btnCutFillElev";
            this.btnCutFillElev.Size = new System.Drawing.Size(84, 62);
            this.btnCutFillElev.TabIndex = 231;
            this.btnCutFillElev.Text = "Cut/Fill";
            this.btnCutFillElev.UseVisualStyleBackColor = true;
            this.btnCutFillElev.Click += new System.EventHandler(this.btnCutFillElev_Click);
            // 
            // btnPropExist
            // 
            this.btnPropExist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPropExist.Location = new System.Drawing.Point(857, 220);
            this.btnPropExist.Name = "btnPropExist";
            this.btnPropExist.Size = new System.Drawing.Size(84, 65);
            this.btnPropExist.TabIndex = 232;
            this.btnPropExist.Text = "Proposed";
            this.btnPropExist.UseVisualStyleBackColor = true;
            this.btnPropExist.Click += new System.EventHandler(this.btnPropExist_Click);
            // 
            // btnFixQuality
            // 
            this.btnFixQuality.Location = new System.Drawing.Point(589, 63);
            this.btnFixQuality.Name = "btnFixQuality";
            this.btnFixQuality.Size = new System.Drawing.Size(108, 58);
            this.btnFixQuality.TabIndex = 233;
            this.btnFixQuality.Text = "RTK fix only";
            this.btnFixQuality.UseVisualStyleBackColor = true;
            this.btnFixQuality.Visible = false;
            this.btnFixQuality.Click += new System.EventHandler(this.btnFixQuality_click);
            // 
            // btnColorCut
            // 
            this.btnColorCut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorCut.BackColor = System.Drawing.Color.Red;
            this.btnColorCut.Location = new System.Drawing.Point(972, 372);
            this.btnColorCut.Name = "btnColorCut";
            this.btnColorCut.Size = new System.Drawing.Size(32, 32);
            this.btnColorCut.TabIndex = 234;
            this.btnColorCut.UseVisualStyleBackColor = false;
            this.btnColorCut.Click += new System.EventHandler(this.btnColorCut_Click);
            // 
            // btnColorCenter
            // 
            this.btnColorCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorCenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnColorCenter.Location = new System.Drawing.Point(972, 480);
            this.btnColorCenter.Name = "btnColorCenter";
            this.btnColorCenter.Size = new System.Drawing.Size(32, 32);
            this.btnColorCenter.TabIndex = 235;
            this.btnColorCenter.UseVisualStyleBackColor = false;
            this.btnColorCenter.Click += new System.EventHandler(this.btnColorCenter_Click);
            // 
            // btnColorFill
            // 
            this.btnColorFill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorFill.BackColor = System.Drawing.Color.SteelBlue;
            this.btnColorFill.Location = new System.Drawing.Point(972, 588);
            this.btnColorFill.Name = "btnColorFill";
            this.btnColorFill.Size = new System.Drawing.Size(32, 32);
            this.btnColorFill.TabIndex = 236;
            this.btnColorFill.UseVisualStyleBackColor = false;
            this.btnColorFill.Click += new System.EventHandler(this.btnColorFill_Click);
            // 
            // lblFill
            // 
            this.lblFill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFill.AutoSize = true;
            this.lblFill.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblFill.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFill.Location = new System.Drawing.Point(1010, 591);
            this.lblFill.Name = "lblFill";
            this.lblFill.Size = new System.Drawing.Size(54, 29);
            this.lblFill.TabIndex = 237;
            this.lblFill.Text = "F/Al";
            // 
            // lblFillValue
            // 
            this.lblFillValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFillValue.AutoSize = true;
            this.lblFillValue.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblFillValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFillValue.Location = new System.Drawing.Point(1070, 591);
            this.lblFillValue.Name = "lblFillValue";
            this.lblFillValue.Size = new System.Drawing.Size(69, 29);
            this.lblFillValue.TabIndex = 238;
            this.lblFillValue.Text = "value";
            // 
            // lblCutValue
            // 
            this.lblCutValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCutValue.AutoSize = true;
            this.lblCutValue.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblCutValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCutValue.Location = new System.Drawing.Point(1078, 375);
            this.lblCutValue.Name = "lblCutValue";
            this.lblCutValue.Size = new System.Drawing.Size(69, 29);
            this.lblCutValue.TabIndex = 239;
            this.lblCutValue.Text = "value";
            // 
            // lblWatch
            // 
            this.lblWatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWatch.AutoSize = true;
            this.lblWatch.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lblWatch.Location = new System.Drawing.Point(491, 9);
            this.lblWatch.Name = "lblWatch";
            this.lblWatch.Size = new System.Drawing.Size(101, 19);
            this.lblWatch.TabIndex = 240;
            this.lblWatch.Text = "NTRIP status";
            this.lblWatch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnColorFillMid
            // 
            this.btnColorFillMid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorFillMid.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnColorFillMid.Location = new System.Drawing.Point(972, 552);
            this.btnColorFillMid.Name = "btnColorFillMid";
            this.btnColorFillMid.Size = new System.Drawing.Size(32, 32);
            this.btnColorFillMid.TabIndex = 241;
            this.btnColorFillMid.UseVisualStyleBackColor = false;
            this.btnColorFillMid.Click += new System.EventHandler(this.btnColorMidFill_Click);
            // 
            // btnColorFillMin
            // 
            this.btnColorFillMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorFillMin.BackColor = System.Drawing.Color.Lime;
            this.btnColorFillMin.Location = new System.Drawing.Point(972, 516);
            this.btnColorFillMin.Name = "btnColorFillMin";
            this.btnColorFillMin.Size = new System.Drawing.Size(32, 32);
            this.btnColorFillMin.TabIndex = 242;
            this.btnColorFillMin.UseVisualStyleBackColor = false;
            this.btnColorFillMin.Click += new System.EventHandler(this.btnColorMinFill_Click);
            // 
            // btnColorCutMid
            // 
            this.btnColorCutMid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorCutMid.BackColor = System.Drawing.Color.Orange;
            this.btnColorCutMid.Location = new System.Drawing.Point(972, 408);
            this.btnColorCutMid.Name = "btnColorCutMid";
            this.btnColorCutMid.Size = new System.Drawing.Size(32, 32);
            this.btnColorCutMid.TabIndex = 243;
            this.btnColorCutMid.UseVisualStyleBackColor = false;
            this.btnColorCutMid.Click += new System.EventHandler(this.btnColorMidCut_Click);
            // 
            // btnColorCutMin
            // 
            this.btnColorCutMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorCutMin.BackColor = System.Drawing.Color.Yellow;
            this.btnColorCutMin.Location = new System.Drawing.Point(972, 444);
            this.btnColorCutMin.Name = "btnColorCutMin";
            this.btnColorCutMin.Size = new System.Drawing.Size(32, 32);
            this.btnColorCutMin.TabIndex = 244;
            this.btnColorCutMin.UseVisualStyleBackColor = false;
            this.btnColorCutMin.Click += new System.EventHandler(this.btnColorMinCut_Click);
            // 
            // btnScalePlus
            // 
            this.btnScalePlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScalePlus.Location = new System.Drawing.Point(1151, 430);
            this.btnScalePlus.Name = "btnScalePlus";
            this.btnScalePlus.Size = new System.Drawing.Size(52, 46);
            this.btnScalePlus.TabIndex = 245;
            this.btnScalePlus.Text = "+";
            this.btnScalePlus.UseVisualStyleBackColor = true;
            this.btnScalePlus.Click += new System.EventHandler(this.btnScalePlus_Click);
            // 
            // btnScaleMinus
            // 
            this.btnScaleMinus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScaleMinus.Location = new System.Drawing.Point(1151, 516);
            this.btnScaleMinus.Name = "btnScaleMinus";
            this.btnScaleMinus.Size = new System.Drawing.Size(52, 46);
            this.btnScaleMinus.TabIndex = 246;
            this.btnScaleMinus.Text = "-";
            this.btnScaleMinus.UseVisualStyleBackColor = true;
            this.btnScaleMinus.Click += new System.EventHandler(this.btnScaleMinus_Click);
            // 
            // lblFillMinValue
            // 
            this.lblFillMinValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFillMinValue.AutoSize = true;
            this.lblFillMinValue.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblFillMinValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFillMinValue.Location = new System.Drawing.Point(1010, 516);
            this.lblFillMinValue.Name = "lblFillMinValue";
            this.lblFillMinValue.Size = new System.Drawing.Size(69, 29);
            this.lblFillMinValue.TabIndex = 247;
            this.lblFillMinValue.Text = "value";
            // 
            // lblCutMinValue
            // 
            this.lblCutMinValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCutMinValue.AutoSize = true;
            this.lblCutMinValue.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblCutMinValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCutMinValue.Location = new System.Drawing.Point(1010, 444);
            this.lblCutMinValue.Name = "lblCutMinValue";
            this.lblCutMinValue.Size = new System.Drawing.Size(69, 29);
            this.lblCutMinValue.TabIndex = 248;
            this.lblCutMinValue.Text = "value";
            // 
            // lblCutMidValue
            // 
            this.lblCutMidValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCutMidValue.AutoSize = true;
            this.lblCutMidValue.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblCutMidValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCutMidValue.Location = new System.Drawing.Point(1010, 411);
            this.lblCutMidValue.Name = "lblCutMidValue";
            this.lblCutMidValue.Size = new System.Drawing.Size(69, 29);
            this.lblCutMidValue.TabIndex = 249;
            this.lblCutMidValue.Text = "value";
            // 
            // lblFillMidValue
            // 
            this.lblFillMidValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFillMidValue.AutoSize = true;
            this.lblFillMidValue.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblFillMidValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFillMidValue.Location = new System.Drawing.Point(1010, 554);
            this.lblFillMidValue.Name = "lblFillMidValue";
            this.lblFillMidValue.Size = new System.Drawing.Size(69, 29);
            this.lblFillMidValue.TabIndex = 250;
            this.lblFillMidValue.Text = "value";
            // 
            // lblCentreValue
            // 
            this.lblCentreValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCentreValue.AutoSize = true;
            this.lblCentreValue.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblCentreValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCentreValue.Location = new System.Drawing.Point(1010, 479);
            this.lblCentreValue.Name = "lblCentreValue";
            this.lblCentreValue.Size = new System.Drawing.Size(69, 29);
            this.lblCentreValue.TabIndex = 251;
            this.lblCentreValue.Text = "value";
            // 
            // btnUseSavedAGS
            // 
            this.btnUseSavedAGS.Location = new System.Drawing.Point(185, 184);
            this.btnUseSavedAGS.Name = "btnUseSavedAGS";
            this.btnUseSavedAGS.Size = new System.Drawing.Size(97, 90);
            this.btnUseSavedAGS.TabIndex = 252;
            this.btnUseSavedAGS.Text = "Use Autosaved Survey";
            this.btnUseSavedAGS.UseVisualStyleBackColor = true;
            this.btnUseSavedAGS.Visible = false;
            this.btnUseSavedAGS.Click += new System.EventHandler(this.btnUseSavedAGS_Click);
            // 
            // FormGPS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1306, 684);
            this.Controls.Add(this.btnUseSavedAGS);
            this.Controls.Add(this.lblCentreValue);
            this.Controls.Add(this.lblFillMidValue);
            this.Controls.Add(this.lblCutMidValue);
            this.Controls.Add(this.lblCutMinValue);
            this.Controls.Add(this.lblFillMinValue);
            this.Controls.Add(this.btnScaleMinus);
            this.Controls.Add(this.btnScalePlus);
            this.Controls.Add(this.btnColorCutMin);
            this.Controls.Add(this.btnColorCutMid);
            this.Controls.Add(this.btnColorFillMin);
            this.Controls.Add(this.btnColorFillMid);
            this.Controls.Add(this.lblWatch);
            this.Controls.Add(this.lblCutValue);
            this.Controls.Add(this.lblFillValue);
            this.Controls.Add(this.lblFill);
            this.Controls.Add(this.btnColorFill);
            this.Controls.Add(this.btnColorCenter);
            this.Controls.Add(this.btnColorCut);
            this.Controls.Add(this.btnFixQuality);
            this.Controls.Add(this.btnPropExist);
            this.Controls.Add(this.btnCutFillElev);
            this.Controls.Add(this.numBladeOffset);
            this.Controls.Add(this.btnBoundarySide);
            this.Controls.Add(this.btnStartPause);
            this.Controls.Add(this.lblFixUpdateHz);
            this.Controls.Add(this.lblBarGraphMax);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pbarCutAbove);
            this.Controls.Add(this.pbarCutBelow);
            this.Controls.Add(this.btnGoogleEarth);
            this.Controls.Add(this.btnAutoSteer);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.btnContour);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.btnTiltDown);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnTiltUp);
            this.Controls.Add(this.lblCut);
            this.Controls.Add(this.lblAltitude);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.panelSimControls);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblPureSteerAngle);
            this.Controls.Add(this.btnManualOffOn);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.lblSpeedUnits);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.txtDistanceOffABLine);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.openGLControl);
            this.Controls.Add(this.openGLControlBack);
            this.Font = new System.Drawing.Font("Tahoma", 12F);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(520, 600);
            this.Name = "FormGPS";
            this.Text = "OpenGrade3D - Cuz Retail Sucks";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGPS_FormClosing);
            this.Load += new System.EventHandler(this.FormGPS_Load);
            this.Resize += new System.EventHandler(this.FormGPS_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl)).EndInit();
            this.contextMenuStripOpenGL.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.openGLControlBack)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStripFlag.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.DataPage.ResumeLayout(false);
            this.DataPage.PerformLayout();
            this.configPage1.ResumeLayout(false);
            this.controlPage2.ResumeLayout(false);
            this.panelSimControls.ResumeLayout(false);
            this.panelSimControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLatitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarSteerAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarStepDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBladeOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpGL.OpenGLControl openGLControl;
        private System.Windows.Forms.Button btnABLine;
        private System.Windows.Forms.TextBox txtDistanceOffABLine;
        private SharpGL.OpenGLControl openGLControlBack;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Timer tmrWatchdog;
        private System.Windows.Forms.ToolStripMenuItem polygonsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel stripDistance;
        private System.Windows.Forms.ToolStripMenuItem resetALLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadVehicleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveVehicleToolStripMenuItem;
        private System.Windows.Forms.Button btnManualOffOn;
        private System.Windows.Forms.Button btnContour;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem explorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webCamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fieldViewerToolStripMenuItem;
        private System.Windows.Forms.Label lblNorthing;
        private System.Windows.Forms.Label lblEasting;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.ToolStripMenuItem googleEarthToolStripMenuItem;
        private ProXoft.WinForms.RepeatButton btnZoomOut;
        private ProXoft.WinForms.RepeatButton btnZoomIn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFlag;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFlagRed;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuFlagGrn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuFlagYel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuFlagDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private ProXoft.WinForms.RepeatButton btnTiltUp;
        private ProXoft.WinForms.RepeatButton btnTiltDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuFlagDeleteAll;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOpenGL;
        private System.Windows.Forms.ToolStripMenuItem deleteFlagToolOpenGLContextMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem googleEarthOpenGLContextMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem gPSDataToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fieldToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar stripOnlineGPS;
        private System.Windows.Forms.ToolStripMenuItem colorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fieldToolStripMenuItem1;
        private System.Windows.Forms.ToolStripProgressBar stripOnlineAutoSteer;
        private System.Windows.Forms.ToolStripMenuItem logNMEAMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripUnitsMenu;
        private System.Windows.Forms.ToolStripMenuItem metricToolStrip;
        private System.Windows.Forms.ToolStripMenuItem imperialToolStrip;
        private System.Windows.Forms.ToolStripMenuItem skyToolStripMenu;
        private System.Windows.Forms.Button btnSnap;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.TextBox tboxSentence;
        private System.Windows.Forms.Label lblZone;
        private System.Windows.Forms.Button btnGPSData;
        private System.Windows.Forms.Button btnFileExplorer;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuHelpHelp;
        private System.Windows.Forms.Label lblSpeedUnits;
        private System.Windows.Forms.ToolStripMenuItem pursuitLineToolStripMenuItem;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnAutoSteer;
        private System.Windows.Forms.Button btnFlag;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage configPage1;
        private System.Windows.Forms.TabPage DataPage;
        private System.Windows.Forms.Button btnUnits;
        private System.Windows.Forms.Timer timerSim;
        private System.Windows.Forms.TrackBar tbarStepDistance;
        private System.Windows.Forms.TrackBar tbarSteerAngle;
        private System.Windows.Forms.Button btnResetSteerAngle;
        private System.Windows.Forms.Label lblSteerAngle;
        private System.Windows.Forms.Button btnResetSim;
        private System.Windows.Forms.ToolStripMenuItem simulatorOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownBtnFuncs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblRoll;
        private System.Windows.Forms.Label lblPureSteerAngle;
        private System.Windows.Forms.Label lblGPSHeading;
        private System.Windows.Forms.Label lblGyroHeading;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem toolstripUSBPortsConfig;
        private System.Windows.Forms.ToolStripMenuItem toolstripVehicleConfig;
        private System.Windows.Forms.ToolStripMenuItem toolstripAutoSteerConfig;
        private System.Windows.Forms.ToolStripMenuItem toolstripResetTrip;
        private System.Windows.Forms.ToolStripMenuItem toolstripField;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolStripMenuItem toolstripUDPConfig;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblLongitude;
        private System.Windows.Forms.Label lblLatitude;
        private System.Windows.Forms.Label lblAltitude;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblFixQuality;
        private System.Windows.Forms.Label lblSats;
        private System.Windows.Forms.Panel panelSimControls;
        private System.Windows.Forms.ToolStripDropDownButton btnHideTabs;
        private System.Windows.Forms.ToolStripStatusLabel stripSelectMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem setWorkingDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.NumericUpDown nudElevation;
        private System.Windows.Forms.ToolStripStatusLabel stripTopoLocation;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ToolStripStatusLabel stripMinMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCutDelta;
        private System.Windows.Forms.Label lblCut;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.CheckBox cboxLastPass;
        private System.Windows.Forms.Button btnZeroAltitude;
        private System.Windows.Forms.TabPage controlPage2;
        private System.Windows.Forms.CheckBox cboxLaserModeOnOff;
        private System.Windows.Forms.Button btnGoogleEarth;
        private ProgBar.ProgBarPlus pbarCutBelow;
        private ProgBar.ProgBarPlus pbarCutAbove;
        private System.Windows.Forms.Label lblBarGraphMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFixUpdateHz;
        private System.Windows.Forms.Button btnStartPause;
        private System.Windows.Forms.Button btnBoundarySide;
        private System.Windows.Forms.NumericUpDown numBladeOffset;
        private System.Windows.Forms.Button btnCutFillElev;
        private System.Windows.Forms.Button btnPropExist;
        private System.Windows.Forms.Button btnFixQuality;
        private System.Windows.Forms.NumericUpDown nudLongitude;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudLatitude;
        private System.Windows.Forms.Button btnSimGoTo;
        private System.Windows.Forms.Button btnColorCut;
        private System.Windows.Forms.Button btnColorCenter;
        private System.Windows.Forms.Button btnColorFill;
        private System.Windows.Forms.ToolStripMenuItem MapColorToolStripMenuItem;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblFill;
        private System.Windows.Forms.Label lblFillValue;
        private System.Windows.Forms.Label lblCutValue;
        private System.Windows.Forms.ToolStripMenuItem toolstripNTRIPConfig;
        private System.Windows.Forms.Label lblWatch;
        private System.Windows.Forms.Button btnColorFillMid;
        private System.Windows.Forms.Button btnColorFillMin;
        private System.Windows.Forms.Button btnColorCutMid;
        private System.Windows.Forms.Button btnColorCutMin;
        private System.Windows.Forms.ToolStripMenuItem rstColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GradToolStrip;
        private System.Windows.Forms.ToolStripMenuItem StepToolStrip;
        private System.Windows.Forms.ToolStripMenuItem GradMultiToolStrip;
        private System.Windows.Forms.Button btnScalePlus;
        private System.Windows.Forms.Button btnScaleMinus;
        private System.Windows.Forms.Label lblFillMinValue;
        private System.Windows.Forms.Label lblCutMinValue;
        private System.Windows.Forms.Label lblCutMidValue;
        private System.Windows.Forms.Label lblFillMidValue;
        private System.Windows.Forms.Label lblCentreValue;
        private System.Windows.Forms.Button btnUseSavedAGS;
    }
}

