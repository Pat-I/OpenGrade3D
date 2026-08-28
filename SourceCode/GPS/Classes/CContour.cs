using SharpGL;
using System;
using System.Collections.Generic;

namespace OpenGrade
{
    public class CContourPt
    {
        public double altitude { get; set; }
        public double easting { get; set; }
        public double northing { get; set; }
        public double heading { get; set; }
        public double cutAltitude { get; set; }
        public double lastPassAltitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double distance { get; set; }

        //constructor
        public CContourPt(double _easting, double _heading, double _northing,
                            double _altitude, double _lat, double _long,
                            double _cutAltitude = -999, double _lastPassAltitude = -999, double _distance = -1)
        {
            easting = _easting;
            northing = _northing;
            heading = _heading;
            altitude = _altitude;
            latitude = _lat;
            longitude = _long;

            //optional parameters
            cutAltitude = _cutAltitude;
            lastPassAltitude = _lastPassAltitude;
            distance = _distance;
        }
    }

    // A list for the boundary pts in the visual Map
    public class BoundaryPt
    {
        public double easting { get; set; }
        public double northing { get; set; }
        public double heading { get; set; }
        public double altitude { get; set; }
        public double cutAltitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double code { get; set; }

        //constructor
        public BoundaryPt(double _easting, double _heading, double _northing,
                            double _altitude, double _lat, double _long,
                            double _cutAltitude = -1, double _code = 0)
        {
            easting = _easting;
            northing = _northing;
            heading = _heading;
            altitude = _altitude;
            latitude = _lat;
            longitude = _long;

            //optional parameters
            cutAltitude = _cutAltitude;
            code = _code;
        }
    }
    // ViewPt is a Pt list for the side elevation view
    // pt 0 to 100 for the past points, pt 101 (eleViewListCount = 102) for the prestent, pt 102 to 299 for the look ahead, 50 cm apart, by Pat

    public class ViewPt
    {
        public double altitude { get; set; }
        public double easting { get; set; }
        public double northing { get; set; }
        public double heading { get; set; }
        public double cutAltitude { get; set; }
        public double lastPassAltitude { get; set; }
        //public double distance { get; set; }

        //constructor
        public ViewPt(double _easting = 0, double _northing = 0,
                            double _altitude = -999, double _heading = 0,
                            double _cutAltitude = -999, double _lastPassAltitude = -999) // , double _distance = -1
        {
            easting = _easting;
            northing = _northing;
            heading = _heading;
            altitude = _altitude;
            

            //optional parameters
            cutAltitude = _cutAltitude;
            lastPassAltitude = _lastPassAltitude;
            //distance = _distance;
        }
    }

    // A list to view the survey pt used for cut/fill calulation
    public class UsedPt
    {
        public double easting { get; set; }
        public double northing { get; set; }
        public double used { get; set; }

        //constructor
        public UsedPt(double _easting = 0, double _northing = 0, double _used = 0)
        {
            easting = _easting;
            northing = _northing;
            used = _used;
        }
    }

    //Survey list by Pat
    public class SurveyPt
    {
        public double easting { get; set; }
        public double northing { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double altitude { get; set; }
        public double code { get; set; }
        public int fixQuality { get; set; }


        //constructor
        public SurveyPt(double _easting, double _northing, double _lat, double _long, double _altitude = -999, double _code = -1, int _fixQuality = 0)
        {

            easting = _easting;
            northing = _northing;
            latitude = _lat;
            longitude = _long;
            altitude = _altitude;

            code = _code;
            fixQuality = _fixQuality;
            
        }
    }

    // by pat
    public class mapListPt
    {
        public double eastingMap { get; set; }
        public double northingMap { get; set; }
        public double drawPtWidthMap { get; set; }
        public double altitudeMap { get; set; }
        public double cutAltitudeMap { get; set; }
        public double cutDeltaMap { get; set; }
        public double lastPassAltitudeMap { get; set; }
        public double lastPassRealAltitudeMap { get; set; }



        //public double longitude { get; set; }
        //public double distance { get; set; }

        //constructor
        public mapListPt(double _eastingMap, double _northingMap, double _drawPtWidthMap,
                            double _altitudeMap,
                            double _cutAltitudeMap = -999, 
                            double _cutDeltaMap = 9999,
                            double _lastPassAltitudeMap = -999,
                            double _lastPassRealAltitudeMap = -999)
        {
            eastingMap = _eastingMap;
            northingMap = _northingMap;
            drawPtWidthMap = _drawPtWidthMap;
            altitudeMap = _altitudeMap;
            cutAltitudeMap = _cutAltitudeMap;
            cutDeltaMap = _cutDeltaMap;
            lastPassAltitudeMap = _lastPassAltitudeMap;
            lastPassRealAltitudeMap = _lastPassRealAltitudeMap;
            
        }
    }

    //  to import Optisurface design points by pat
    public class designPt
    {
        public double altitude { get; set; }
        public double easting { get; set; }
        public double northing { get; set; }
        
        public double cutAltitude { get; set; }
        public double cutfill { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double code { get; set; }

        //constructor
        public designPt(double _lat, double _long, double _altitude,
                            double _cutAltitude = -999, double _cutfill = 9999,
                              double _code = -1, double _easting = 0, double _northing = 0)
        {
            easting = _easting;
            northing = _northing;
            //heading = _heading;
            altitude = _altitude;
            latitude = _lat;
            longitude = _long;

            //optional parameters
            cutAltitude = _cutAltitude;
            cutfill = _cutfill;
            code = _code;
        }
    }

    public class CContour
    {
        //copy of the mainform address
        private readonly FormGPS mf;

        private readonly OpenGL gl;

        //Map rendering variables
        // 48MB of memory for the vertex and color arrays, enough for 1 million points
        private float[] _globalVertexArray = new float[1000000 * 12];
        private byte[] _globalColorArray = new byte[1000000 * 12];
        private int _totalVerticesToRender = 0;

        public bool needsRebuildTerrainBuffers = true;

        public bool isContourOn, isContourBtnOn;
        public bool surveyMode;
        public bool isSurveyOn;
        public bool markBM;
        public bool clearSurveyList;
        public bool recBoundary;
        public bool recSurveyPt;
        public bool isBtnStartPause;
        public bool isBoundarySideRight;
        public bool isOpenGLControlBackVisible = true;
        public bool FloatIsOK;
        public bool isOKtoSurvey;
        public bool drawTheMap = true;

        //for the diferent maps views
        public bool isElevation;
        public bool isExistingElevation;
        public bool isActualCut;
        public bool isActualFill;

        //
        public int eleViewListCount = 300;
        public int ScaleFactor = 100;

        public double maxAltitude = -9999, minAltitude = 9999, maxCut = 0, maxFill = 0, midAltitude;

        public double slope = 0.002;
        public double zeroAltitude = 0;

        public double nearestSurveyEasting;
        public double nearestSurveyNorthing;

        public List<CContourPt> ptList = new List<CContourPt>();

        public List<mapListPt> mapList = new List<mapListPt>();

        public List<SurveyPt> surveyList = new List<SurveyPt>();

        public List<designPt> designList = new List<designPt>();

        public List<ViewPt> eleViewList = new List<ViewPt>();

        public List<UsedPt> usedPtList = new List<UsedPt>();

        public List<BoundaryPt> boundaryList = new List<BoundaryPt>();

        public double distanceFromCurrentLine;

        public double abFixHeadingDelta, abHeading;

        public bool isABSameAsFixHeading = true;
        public bool isOnRightSideCurrentLine = true;

        //pure pursuit values
        public vec2 goalPointCT = new vec2(0, 0);
        public vec2 radiusPointCT = new vec2(0, 0);
        public double steerAngleCT;
        public double rEastCT, rNorthCT;
        public double ppRadiusCT;

        //constructor
        public CContour(OpenGL _gl, FormGPS _f)
        {
            mf = _f;
            if (mf != null)
            {
                gl = _gl;
            }
        }

        //start stop and add points to list
        public void StartContourLine()
        {
            isContourOn = true;
            //reuse ptList
            //surveyList.Clear();

            //SurveyPt point = new SurveyPt(mf.pn.latitude, mf.pn.longitude, mf.pn.altitude, 2);
            //surveyList.Add(point);
        }

        //Add current position to ptList
        public void AddPoint()
        {
            //SurveyPt point = new SurveyPt(mf.pn.latitude, mf.pn.longitude, mf.pn.altitude, 2);
            //surveyList.Add(point);
        }

        //End the strip
        public void StopContourLine()
        {
            //SurveyPt point = new SurveyPt(mf.pn.latitude, mf.pn.longitude, mf.pn.altitude, 2);
            //surveyList.Add(point);

            //turn it off
            isContourOn = false;
        }

        //draw the red follow me line
        public void DrawContourLine()
        {

            #region Survey ----------------------------------------------------------------------------------------------------------------

            if (isSurveyOn)
            {
                if (clearSurveyList)
                {
                    surveyList.Clear();
                    clearSurveyList = false;
                }

                // Check the fix Quality before saving the point            
                if (mf.pn.fixQuality == 4 | mf.pn.fixQuality == 8) isOKtoSurvey = true;
                else if (mf.pn.fixQuality == 5 && FloatIsOK) isOKtoSurvey = true;
                else isOKtoSurvey = false;

                if (isOKtoSurvey)
                {
                    if (markBM)
                    {
                        surveyList.Clear();
                        SurveyPt point = new SurveyPt(mf.pn.easting, mf.pn.northing, mf.pn.latitude, mf.pn.longitude, mf.pn.altitude, 0, mf.pn.fixQuality);
                        surveyList.Add(point);

                        nearestSurveyEasting = mf.pn.easting;
                        nearestSurveyNorthing = mf.pn.northing;

                        markBM = false;
                        recBoundary = true;

                    }

                    // Start recording contour

                    if (recBoundary)
                    {
                        double halfToolWidth = (Properties.Vehicle.Default.setVehicle_toolWidth) / 2;

                        if (isBtnStartPause)
                        {
                            // translate the survey pt to the side of the tool

                            double sideEasting;
                            double sideNorthing;

                            if (isBoundarySideRight)
                            {
                                sideEasting = mf.pn.easting + Math.Sin(mf.fixHeading - glm.PIBy2) * -halfToolWidth;
                                sideNorthing = mf.pn.northing + Math.Cos(mf.fixHeading - glm.PIBy2) * -halfToolWidth;
                            }
                            else
                            {
                                sideEasting = mf.pn.easting + Math.Sin(mf.fixHeading - glm.PIBy2) * halfToolWidth;
                                sideNorthing = mf.pn.northing + Math.Cos(mf.fixHeading - glm.PIBy2) * halfToolWidth;
                            }

                            //check dist from last point 

                            double surveyDistance = ((nearestSurveyEasting - sideEasting) * (nearestSurveyEasting - sideEasting) +
                            (nearestSurveyNorthing - sideNorthing) * (nearestSurveyNorthing - sideNorthing));

                            if (surveyDistance > 9)
                            {
                                mf.pn.ConvertLocalToWGS84(sideNorthing, sideEasting, out double Lat, out double Lon);

                                SurveyPt point = new SurveyPt(sideEasting, sideNorthing, Lat, Lon, mf.pn.altitude, 2, mf.pn.fixQuality) ;
                                surveyList.Add(point);

                                nearestSurveyEasting = mf.pn.easting;
                                nearestSurveyNorthing = mf.pn.northing;
                            }
                        }
                    }

                    if (recSurveyPt)
                    {
                        if (isBtnStartPause)
                        {
                            // check for the nearest point in the surveyList

                            int surveyCount = surveyList.Count;
                            double minSurveyDistance = 1000000;
                            double existingHeight = -999;

                            for (int i = 0; i < surveyCount; i++)
                            {
                                double surveyDistance = ((surveyList[i].easting - mf.pn.easting) * (surveyList[i].easting - mf.pn.easting) +
                                    (surveyList[i].northing - mf.pn.northing) * (surveyList[i].northing - mf.pn.northing));

                                if (surveyDistance < minSurveyDistance)
                                {
                                    minSurveyDistance = surveyDistance;
                                    existingHeight = surveyList[i].altitude;
                                }
                            }

                            existingHeight = Math.Abs(existingHeight - mf.pn.altitude);

                            // if there is no point 3 metre around or the height difference is significant, add a point
                            if ((minSurveyDistance > 1 && existingHeight > 0.1) || minSurveyDistance > 9)
                            {
                                SurveyPt point = new SurveyPt(mf.pn.easting, mf.pn.northing, mf.pn.latitude, mf.pn.longitude, mf.pn.altitude, 3, mf.pn.fixQuality);
                                surveyList.Add(point);

                                nearestSurveyEasting = mf.pn.easting;
                                nearestSurveyNorthing = mf.pn.northing;

                            }
                        }
                    }
                }
            }
            else
            {
                // finish the survey
                if (recSurveyPt)
                {
                    mf.FileSaveSurveyPt();

                    recSurveyPt = false;
                    isBtnStartPause = false;
                }
            }

            #endregion survey ----------------------------------------------------------------------------------------------

            if (surveyMode)
            {
                int ptCount = surveyList.Count;

                if (ptCount > 0)
                {
                    gl.PointSize(4.0f);
                    gl.Begin(OpenGL.GL_POINTS);

                    gl.Color(0.97f, 0.42f, 0.45f);
                    for (int h = 0; h < ptCount; h++)
                    {
                        gl.Color(0.97f, 0.42f, 0.45f);

                        if (surveyList[h].code == 0) gl.Color(0.97f, 0.82f, 0.05f);
                        if (surveyList[h].code == 2) gl.Color(0.5f, 0.82f, 0.55f);

                        gl.Vertex(surveyList[h].easting, surveyList[h].northing, 0);
                    }

                    gl.End();
                }

                // Draw a contour line
                bool firstPt = true;
                int firstContourPt = 0;
                int lastContourPt = 0;

                gl.LineWidth(2);
                gl.Color(0.98f, 0.2f, 0.0f); // Default Red

                gl.Begin(OpenGL.GL_LINE_STRIP);
                for (int h = 0; h < ptCount; h++)
                {
                    if (surveyList[h].code == 2)
                    {
                        if (firstPt)
                        {
                            firstPt = false;
                            firstContourPt = h;
                        }
                        gl.Vertex(surveyList[h].easting, surveyList[h].northing, 0);
                        lastContourPt = h;
                    }
                }

                if (!firstPt)
                {
                    if (recBoundary)//boundary is recording, not closed yet, so draw a dashed yellow line to close the loop
                    {
                        // 1. Properly CLOSE the ongoing Red Line Strip first!
                        gl.End();

                        // 2. Turn on the custom dashed styles and colors
                        gl.Enable(OpenGL.GL_LINE_STIPPLE);
                        gl.LineStipple(2, 0xAAAA);
                        gl.Color(1.0f, 0.9f, 0.0f); // Bright Yellow

                        // 3. Start a brand new, isolated segment to close the boundary loop
                        gl.Begin(OpenGL.GL_LINES);
                        gl.Vertex(surveyList[lastContourPt].easting, surveyList[lastContourPt].northing, 0);
                        gl.Vertex(surveyList[firstContourPt].easting, surveyList[firstContourPt].northing, 0);
                        gl.End(); // Properly CLOSE the yellow dashed line loop

                        // 4. Cleanup the state machine right away
                        gl.Disable(OpenGL.GL_LINE_STIPPLE);
                    }
                    else
                    {
                        // the boundary is closed, so draw a solid red line to close the loop
                        gl.Vertex(surveyList[firstContourPt].easting, surveyList[firstContourPt].northing, 0);
                        gl.End();
                    }
                }
                else
                {
                    // Safety guard: if no points matched, close the initial Begin() to prevent memory leaks
                    gl.End();
                }

            }
            else //Grading mode
            {
                //now paint the map, by Pat
                int ptCount = mapList.Count;
                if (maxAltitude == -9999 | minAltitude == 9999 | maxCut == -9999 | maxFill == 9999) drawTheMap = true;

                if (ptCount > 0)
                {        
                    if (drawTheMap) // only when there are changes in the map colors,
                    {
                        // Search for the max min painting values
                        drawTheMap = false;

                        maxAltitude = -9999; minAltitude = 9999; maxCut = 0; maxFill = 0;
                        for (int h = 0; h < ptCount; h++)
                        {
                            if (mapList[h].cutAltitudeMap > -998)
                            {
                                if (mapList[h].cutAltitudeMap < minAltitude) minAltitude = mapList[h].cutAltitudeMap;
                            }

                            if (mapList[h].cutAltitudeMap > maxAltitude) maxAltitude = mapList[h].cutAltitudeMap;

                            if (mapList[h].cutDeltaMap < maxCut) maxCut = mapList[h].cutDeltaMap;

                            if (mapList[h].cutDeltaMap != 9999)
                            {
                                if (mapList[h].cutDeltaMap > maxFill) maxFill = mapList[h].cutDeltaMap;
                            }
                        }

                        //maxCut is negative
                        //maxFill is positive
                        // equalize maxcut and maxfill
                        if (Math.Abs(maxCut) < maxFill) maxCut = -maxFill;
                        else if (maxFill < Math.Abs(maxCut)) maxFill = -maxCut;

                        midAltitude = ((maxAltitude + minAltitude) / 2);

                        // to not mess up with colors when min and max altutudes are to close
                        if ((maxAltitude - minAltitude) < 0.1)
                        {
                            maxAltitude = midAltitude + 0.05;
                            minAltitude = midAltitude - 0.05;
                        }
                       
                        mf.fillCutFillLbl();
                        needsRebuildTerrainBuffers = true;
                    }

                    if(needsRebuildTerrainBuffers)
                    {
                        RebuildTerrainBuffers();
                        needsRebuildTerrainBuffers = false;
                    }

                    DrawTerrain(gl);
                }

                // Paint the elevation view line
                if (isOpenGLControlBackVisible)
                {
                    if (eleViewList.Count > 10)
                    {
                        gl.LineWidth(2);
                        gl.Color(0.98f, 0.2f, 0.0f);
                        gl.Begin(OpenGL.GL_LINE_STRIP);


                        for (int h = 0; h < 300; h++)
                        {
                            if (eleViewList[h].easting != 0 && eleViewList[h].northing != 0)
                                gl.Vertex(eleViewList[h].easting, eleViewList[h].northing, 0);

                        }
                        gl.End();
                    }
                }

                // Paint the design pts
                if (mf.areDesignPtsOn)
                {
                    int count = ptList.Count;

                    if(count > 0)
                    {
                        gl.PointSize(3.0f);
                        gl.Begin(OpenGL.GL_POINTS);
                        gl.Color(1.0f, 0.5f, 0.0f);

                        for (int j = 0; j < count; j++) gl.Vertex(ptList[j].easting, ptList[j].northing, 0);

                        gl.End();
                    }
                }

                // Paint the dots for the contour pts used for cut fill calculation
                int usedPtcnt = usedPtList.Count;

                if (usedPtcnt > 0)
                {
                    gl.PointSize(4.0f);
                    gl.Begin(OpenGL.GL_POINTS);

                    if (usedPtcnt > 1)
                    {
                        
                        for (int h = 1; h < usedPtcnt; h++)
                        {
                            if (usedPtList[h].used == 1) gl.Color(1.0f, 0.5f, 0.0f);
                            else gl.Color(0.0f, 0.0f, 1.0f);
                            gl.Vertex(usedPtList[h].easting, usedPtList[h].northing, 0);

                        }                        
                    }
                    //PAINT the closeset pt
                    gl.Color(1.0f, 0.0f, 0.0f);
                    gl.Vertex(usedPtList[0].easting, usedPtList[0].northing, 0);

                    gl.End();
                }

                //Paint the boundary and subzones

                int boundaryPtCnt = boundaryList.Count;

                if (boundaryPtCnt > 0)
                {
                    if (boundaryList[0].code == 0)
                    {
                        gl.PointSize(6.0f);
                        gl.Begin(OpenGL.GL_POINTS);
                        gl.Color(0.0f, 0.0f, 1.0f);
                        gl.Vertex(boundaryList[0].easting, boundaryList[0].northing, 0);
                        gl.End();
                    }

                    double lastCode = 2;
                    int firstZonePt = (boundaryPtCnt - 1);

                    gl.LineWidth(2);
                    gl.Color(0.73f, 0.27f, 0.69f);
                    gl.Begin(OpenGL.GL_LINE_STRIP);

                    for (int t = (boundaryPtCnt - 1); t > 0; t--)
                    {                 
                        if (boundaryList[t].code == lastCode)
                        {
                            gl.Vertex(boundaryList[t].easting, boundaryList[t].northing, 0);
                        }
                        else 
                        {
                            gl.Vertex(boundaryList[firstZonePt].easting, boundaryList[firstZonePt].northing, 0);
                            gl.End();
                            gl.LineWidth(1);
                            gl.Color(0.0f, 0.0f, 0.0f);
                            gl.Begin(OpenGL.GL_LINE_STRIP);
                            gl.Vertex(boundaryList[t].easting, boundaryList[t].northing, 0);
                            firstZonePt = t;
                        }
                        
                        lastCode = boundaryList[t].code;
                    }
                    gl.Vertex(boundaryList[firstZonePt].easting, boundaryList[firstZonePt].northing, 0);
                    gl.End();
                }
            }

            if (mf.isPureDisplayOn)
            {
                const int numSegments = 100;
                {
                    gl.Color(0.95f, 0.30f, 0.950f);

                    double theta = glm.twoPI / (numSegments);
                    double c = Math.Cos(theta);//precalculate the sine and cosine
                    double s = Math.Sin(theta);

                    double x = ppRadiusCT;//we start at angle = 0
                    double y = 0;

                    gl.LineWidth(1);
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int ii = 0; ii < numSegments; ii++)
                    {
                        //glVertex2f(x + cx, y + cy);//output vertex
                        gl.Vertex(x + radiusPointCT.easting, y + radiusPointCT.northing);//output vertex

                        //apply the rotation matrix
                        double t = x;
                        x = (c * x) - (s * y);
                        y = (s * t) + (c * y);
                    }
                    gl.End();

                    //Draw lookahead Point
                    gl.PointSize(4.0f);
                    gl.Begin(OpenGL.GL_POINTS);

                    //gl.Color(1.0f, 1.0f, 0.25f);
                    //gl.Vertex(rEast, rNorth, 0.0);

                    gl.Color(1.0f, 0.5f, 0.95f);
                    gl.Vertex(goalPointCT.easting, goalPointCT.northing, 0.0);

                    gl.End();
                    gl.PointSize(1.0f);
                }
            }
        }

        public void RebuildTerrainBuffers()
        {
            _totalVerticesToRender = 0;
            int pointCount = mapList.Count;
            int vIndex = 0; int cIndex = 0;

            // begin painting
            // --- Cache external object properties to local variables BEFORE the loop ---
            bool isGradual = mf.isGradual;
            bool isGradualMulticolor = mf.isGradualMulticolor;

            // Step Mode Colors from main form (mf)
            int red, green, blue;
            int redCenterI = mf.redCenter, grnCenterI = mf.grnCenter, bluCenterI = mf.bluCenter;
            int redFillI = mf.redFill, grnFillI = mf.grnFill, bluFillI = mf.bluFill;
            int redCutI = mf.redCut, grnCutI = mf.grnCut, bluCutI = mf.bluCut;

            int redCutMidI = mf.redCutMid, grnCutMidI = mf.grnCutMid, bluCutMidI = mf.bluCutMid;
            int redCutMinI = mf.redCutMin, grnCutMinI = mf.grnCutMin, bluCutMinI = mf.bluCutMin;

            int redFillMidI = mf.redFillMid, grnFillMidI = mf.grnFillMid, bluFillMidI = mf.bluFillMid;
            int redFillMinI = mf.redFillMin, grnFillMinI = mf.grnFillMin, bluFillMinI = mf.bluFillMin;

            float drawPtWidth;
            float easting;
            float northing;

            // this is the value for color selection
            int fillIndex = 0; // from -1000 (max fill/ min alt)-- 0 (0 cut/average alt) to 1000 (max cut/ max alt), 99999 for black

            //set the width of painting

            double zoom = mf.zoomValue;
            double camPitch = mf.camera.camPitch;

            if (camPitch > -20) camPitch = -20;

            int paintEastingMax = (int)(mf.pn.easting + zoom * -camPitch / 2);
            int paintEastingMin = (int)(mf.pn.easting - zoom * -camPitch / 2);
            int paintNorthingMax = (int)(mf.pn.northing + zoom * -camPitch / 2);
            int paintNorthingMin = (int)(mf.pn.northing - zoom * -camPitch / 2);

            for (int h = 0; h < pointCount; h++)
            {
                if (mapList[h].eastingMap < paintEastingMax && mapList[h].eastingMap > paintEastingMin && mapList[h].northingMap < paintNorthingMax && mapList[h].northingMap > paintNorthingMin)
                {
                    // paint the cut fill value
                    if (!isElevation)
                    {
                        // Extract the main variables to fast CPU registers once
                        double cutDelta = mapList[h].cutDeltaMap;

                        if (cutDelta != 9999)
                        {
                            // 1. Cache remaining sub-properties locally to prevent redundant RAM polling
                            double lastPassAlt = mapList[h].lastPassRealAltitudeMap;
                            double targetCutAlt = mapList[h].cutAltitudeMap;
                            double designAlt = mapList[h].altitudeMap;

                            if (isActualCut && lastPassAlt > -998 && cutDelta < 0) // shows the cuts onnly
                            {
                                double actAlt = (lastPassAlt > designAlt) ? designAlt : lastPassAlt;
                                double toCut = actAlt - targetCutAlt;

                                fillIndex = (int)((toCut / maxCut) * -1000);
                            }
                            else if (isActualFill && lastPassAlt > -998) // shows the fills and cuts
                            {
                                double actAlt = lastPassAlt;
                                if (actAlt > designAlt && targetCutAlt <= designAlt) actAlt = designAlt;
                                if (actAlt > targetCutAlt && designAlt <= targetCutAlt) actAlt = targetCutAlt;

                                double toCut = actAlt - targetCutAlt;

                                if (toCut > 0) // still to cut
                                {
                                    fillIndex = (int)((toCut / maxCut) * -1000);
                                }
                                else
                                {
                                    fillIndex = (int)((toCut / maxFill) * 1000);
                                }
                            }
                            else //shows just the original cut fill value, not the actual cut fill
                            {
                                if (cutDelta > 0)
                                {
                                    fillIndex = (int)((cutDelta / maxFill) * -1000); // negative, to fill
                                }
                                else
                                {
                                    fillIndex = (int)((cutDelta / maxCut) * 1000);   // positive, to cut (and handles 0 safely)
                                }
                            }
                        }
                        else // cutDeltaMap = 9999, no cut fill value, paint black
                        {
                            fillIndex = 99999;
                        }
                    }
                    else
                    // paint the desired altutude
                    {
                        if (isExistingElevation)
                        {                 
                            if (mapList[h].altitudeMap > -998)
                            {
                                // High-performance optimization: Calculate subtraction once and cache it in a CPU register
                                double delta = mapList[h].altitudeMap - midAltitude;

                                if (delta < 0)
                                {
                                    fillIndex = (int)(delta / (minAltitude - midAltitude) * -1000); // Negative profile
                                }
                                else
                                {
                                    // If delta == 0, this naturally evaluates to 0 without any division risk
                                    fillIndex = (int)(delta / (maxAltitude - midAltitude) * 1000);  // Positive profile or equal
                                }
                            }
                            else // No existing elevation, paint black
                            {
                                fillIndex = 99999;
                            }
                        }
                        else // proposed elevation
                        {
                            if (mapList[h].cutAltitudeMap != -999)
                            {
                                double delta = mapList[h].cutAltitudeMap - midAltitude;

                                if (delta < 0)
                                {
                                    fillIndex = (int)(delta / (minAltitude - midAltitude) * -1000); // Negative profile
                                }
                                else 
                                {
                                    fillIndex = (int)(delta / (maxAltitude - midAltitude) * 1000);  // Positive profile
                                }    
                            }
                            else
                            {
                                fillIndex = 99999;
                            }
                        }
                    }

                    // fill the color with the index
                    if (fillIndex == 99999) // black
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                    else
                    {
                        fillIndex = fillIndex * ScaleFactor / 100;

                        if (fillIndex < -1000) fillIndex = -1000;
                        if (fillIndex > 1000) fillIndex = 1000;

                        if (isGradual)
                        {
                            if (fillIndex == 0) // center
                            {
                                red = redCenterI;
                                green = grnCenterI;
                                blue = bluCenterI;
                            }
                            else if (fillIndex > 0) // cut or abvove avg alt
                            {
                                red = (int)((1000 * redCenterI - (redCenterI - redCutI) * fillIndex) * 0.001);
                                green = (int)((1000 * grnCenterI - (grnCenterI - grnCutI) * fillIndex) * 0.001);
                                blue = (int)((1000 * bluCenterI - (bluCenterI - bluCutI) * fillIndex) * 0.001);
                            }
                            else // fill or below avg alt
                            {
                                red = (int)((1000 * redCenterI + (redCenterI - redFillI) * fillIndex) * 0.001);
                                green = (int)((1000 * grnCenterI + (grnCenterI - grnFillI) * fillIndex) * 0.001);
                                blue = (int)((1000 * bluCenterI + (bluCenterI - bluFillI) * fillIndex) * 0.001);
                            }
                        }
                        else if (isGradualMulticolor) // is gradual multicolor
                        {
                            if (fillIndex < -666) // from midfill to fill -1000 to -667
                            {
                                red = (int)((333 * redFillMidI + (redFillMidI - redFillI) * (fillIndex + 667)) * 0.003003003);
                                green = (int)((333 * grnFillMidI + (grnFillMidI - grnFillI) * (fillIndex + 667)) * 0.003003003);
                                blue = (int)((333 * bluFillMidI + (bluFillMidI - bluFillI) * (fillIndex + 667)) * 0.003003003);
                            }
                            else if (fillIndex < -332) // from minfill to mid fill -666 to -333
                            {
                                red = (int)((334 * redFillMinI + (redFillMinI - redFillMidI) * (fillIndex + 333)) * 0.002994012);
                                green = (int)((334 * grnFillMinI + (grnFillMinI - grnFillMidI) * (fillIndex + 333)) * 0.002994012);
                                blue = (int)((334 * bluFillMinI + (bluFillMinI - bluFillMidI) * (fillIndex + 333)) * 0.002994012);
                            }
                            else if (fillIndex < 1) // from centre to minfill -332 to 0
                            {
                                red = (int)((333 * redCenterI + (redCenterI - redFillMinI) * fillIndex) * 0.003003003);
                                green = (int)((333 * grnCenterI + (grnCenterI - grnFillMinI) * fillIndex) * 0.003003003);
                                blue = (int)((333 * bluCenterI + (bluCenterI - bluFillMinI) * fillIndex) * 0.003003003);
                            }
                            else if (fillIndex < 334) // from mincut to center from 1 to 333
                            {
                                red = (int)((333 * redCutMinI + (redCutMinI - redCenterI) * (fillIndex - 333)) * 0.003003003);
                                green = (int)((333 * grnCutMinI + (grnCutMinI - grnCenterI) * (fillIndex - 333)) * 0.003003003);
                                blue = (int)((333 * bluCutMinI + (bluCutMinI - bluCenterI) * (fillIndex - 333)) * 0.003003003);
                            }
                            else if (fillIndex < 668) // from midcut to mincut from 334 to 667
                            {
                                red = (int)((334 * redCutMidI + (redCutMidI - redCutMinI) * (fillIndex - 667)) * 0.002994012);
                                green = (int)((334 * grnCutMidI + (grnCutMidI - grnCutMinI) * (fillIndex - 667)) * 0.002994012);
                                blue = (int)((334 * bluCutMidI + (bluCutMidI - bluCutMinI) * (fillIndex - 667)) * 0.002994012);
                            }
                            else // from cut to midcut from 668 to 1000
                            {
                                red = (int)((333 * redCutI + (redCutI - redCutMidI) * (fillIndex - 1000)) * 0.003003003);
                                green = (int)((333 * grnCutI + (grnCutI - grnCutMidI) * (fillIndex - 1000)) * 0.003003003);
                                blue = (int)((333 * bluCutI + (bluCutI - bluCutMidI) * (fillIndex - 1000)) * 0.003003003);
                            }
                        }
                        else // is step
                        {
                            if (fillIndex > 710) { red = redCutI; green = grnCutI; blue = bluCutI; }
                            else if (fillIndex > 420) { red = redCutMidI; green = grnCutMidI; blue = bluCutMidI; }
                            else if (fillIndex > 130) { red = redCutMinI; green = grnCutMinI; blue = bluCutMinI; }
                            else if (fillIndex > -130) { red = redCenterI; green = grnCenterI; blue = bluCenterI; }
                            else if (fillIndex > -420) { red = redFillMinI; green = grnFillMinI; blue = bluFillMinI; }
                            else if (fillIndex > -710) { red = redFillMidI; green = grnFillMidI; blue = bluFillMidI; }
                            else { red = redFillI; green = grnFillI; blue = bluFillI; }
                        }
                    }

                    drawPtWidth = (float)(mapList[h].drawPtWidthMap / 2);
                    easting = (float)mapList[h].eastingMap;
                    northing = (float)mapList[h].northingMap;

                    // --- C. POPULATE BUFFERS FOR THE 4 CORNERS OF THE SQUARE (GL_QUADS) ---
                    // Vertex 1: Bottom-Left
                    _globalVertexArray[vIndex++] = easting - drawPtWidth; _globalVertexArray[vIndex++] = northing - drawPtWidth; _globalVertexArray[vIndex++] = 0.0f;
                    _globalColorArray[cIndex++] = (byte)red; _globalColorArray[cIndex++] = (byte)green; _globalColorArray[cIndex++] = (byte)blue;

                    // Vertex 2: Top-Left
                    _globalVertexArray[vIndex++] = easting - drawPtWidth; _globalVertexArray[vIndex++] = northing + drawPtWidth; _globalVertexArray[vIndex++] = 0.0f;
                    _globalColorArray[cIndex++] = (byte)red; _globalColorArray[cIndex++] = (byte)green; _globalColorArray[cIndex++] = (byte)blue;

                    // Vertex 3: Top-Right
                    _globalVertexArray[vIndex++] = easting + drawPtWidth; _globalVertexArray[vIndex++] = northing + drawPtWidth; _globalVertexArray[vIndex++] = 0.0f;
                    _globalColorArray[cIndex++] = (byte)red; _globalColorArray[cIndex++] = (byte)green; _globalColorArray[cIndex++] = (byte)blue;

                    // Vertex 4: Bottom-Right
                    _globalVertexArray[vIndex++] = easting + drawPtWidth; _globalVertexArray[vIndex++] = northing - drawPtWidth; _globalVertexArray[vIndex++] = 0.0f;
                    _globalColorArray[cIndex++] = (byte)red; _globalColorArray[cIndex++] = (byte)green; _globalColorArray[cIndex++] = (byte)blue;
                }
            }


            _totalVerticesToRender = vIndex / 3; // On stocke le compte global
        }

        public void DrawTerrain(OpenGL gl)
        {
            // If no points are loaded or calculated yet, abort render sequence immediately
            if (_totalVerticesToRender == 0) return;

            // Enable hardware pipeline states
            gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.EnableClientState(OpenGL.GL_COLOR_ARRAY);

            // Safely lock arrays in memory and send raw pointers to the graphics card
            unsafe
            {
                fixed (float* pVertex = _globalVertexArray)
                fixed (byte* pColor = _globalColorArray)
                {
                    // Bind structural vertex positions (3 coordinates: X, Y, Z)
                    gl.VertexPointer(3, OpenGL.GL_FLOAT, 0, (IntPtr)pVertex);

                    // Bind color values (3 channels: R, G, B using fast Unsigned Bytes)
                    gl.ColorPointer(3, OpenGL.GL_UNSIGNED_BYTE, 0, (IntPtr)pColor);

                    // Execute parallel GPU render call for valid items only
                    gl.DrawArrays(OpenGL.GL_QUADS, 0, _totalVerticesToRender);
                }
            }

            // Turn off states to prevent rendering bugs on other UI layers
            gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.DisableClientState(OpenGL.GL_COLOR_ARRAY);
        }

        #region Convert design pt to ptList
        //add the utm to the agd data and save to the ptList for code 3 pts, to boundaryList For the others , by Pat
        public void designList2ptList()
        {
            var form = new FormTimedMessage(3000, "Creating map", "Can take several seconds");
            form.Show();

            mf.stopTheProgram = true;
            //if (ptList != null) 
            ptList.Clear();

            //if (boundaryList != null) 
            boundaryList.Clear();

            if (designList != null)
            {
                int ptCount = designList.Count;
                for (int t = 0; t < ptCount; t++)
                {
                    double lat = designList[t].latitude;
                    double lon = designList[t].longitude;
                    //double eastingAgd;
                    //double northingAgd;

                    //mf.pn.ConvertAgd2Utm(lat * 0.01745329251994329576923690766743, lon * 0.01745329251994329576923690766743);
                    mf.pn.ConvertWGS84ToLocal(lat, lon, out double northingAgd, out double eastingAgd);

                    if (designList[t].code == 3)
                    {

                    
                        CContourPt point = new CContourPt(eastingAgd,
                                    0,
                                    northingAgd,
                                    designList[t].altitude,
                                    designList[t].latitude,
                                    designList[t].longitude,
                                    designList[t].cutAltitude,
                                    -999,
                                    -1);

                        ptList.Add(point);
                    }
                    else
                    {
                        BoundaryPt point = new BoundaryPt(eastingAgd,
                            0,
                            northingAgd,
                            designList[t].altitude,
                            designList[t].latitude,
                            designList[t].longitude,
                            designList[t].cutAltitude,
                            designList[t].code);

                        boundaryList.Add(point);
                    }
                }
                mf.FileSaveContour();
                mf.FileSaveBoundaryList();
                mapList.Clear();
                mf.CalculatingMinMaxEastNort = true;
                //mf.CalculateMinMaxEastNort();

            }
        }
        #endregion

        //Reset the contour to zip
        public void ResetContour()
        {
            if (ptList != null) ptList.Clear();
        }

        //by Pat
        public void Build_eleViewList()
        {
            if (eleViewList != null) eleViewList.Clear();

            for (int t = 0; t < eleViewListCount; t++)
            {
                ViewPt point = new ViewPt(0, 0, -999, 0, -999, -999);
                eleViewList.Add(point);
            }

        }
    }//class
}//namespace