
using System;
using SharpGL;

namespace OpenGrade
{
    public class CABLine
    {
        public double abHeading;
        public double abFixHeadingDelta;

        public bool isABSameAsFixHeading = true;
        public bool isOnRightSideCurrentLine = true;

        public double refLineSide = 1.0;

        public double distanceFromRefLine;
        public double distanceFromCurrentLine;
        public double snapDistance;

        public bool isABLineSet;
        public bool isABLineBeingSet;

        public double passNumber;

        public double howManyPathsAway;

        //tramlines
        //Color tramColor = Color.YellowGreen;
        public int tramPassEvery;
        public int passBasedOn;

        //pointers to mainform controls
        private readonly FormGPS mf;
        private readonly OpenGL gl;

        //the two inital A and B points
        public vec2 refPoint1 = new vec2(0.2, 0.2);
        public vec2 refPoint2 = new vec2(0.3, 0.3);

        //the reference line endpoints
        public vec2 refABLineP1 = new vec2(0.0, 0.0);
        public vec2 refABLineP2 = new vec2(0.0, 1.0);

        //the current AB guidance line
        public vec2 currentABLineP1 = new vec2(0.0, 0.0);
        public vec2 currentABLineP2 = new vec2(0.0, 1.0);

        //pure pursuit values
        public vec2 pivotAxlePosAB = new vec2(0, 0);
        public vec2 goalPointAB = new vec2(0, 0);
        public vec2 radiusPointAB = new vec2(0, 0);
        public double steerAngleAB;
        public double rEastAB, rNorthAB;
        public double ppRadiusAB;
        public double minLookAheadDistance = 8.0;

        public CABLine(OpenGL _gl, FormGPS _f)
        {
            //constructor
            gl = _gl;
            mf = _f;
        }

        public void DeleteAB()
        {
            refPoint1 = new vec2(0.0, 0.0);
            refPoint2 = new vec2(0.0, 1.0);

            refABLineP1 = new vec2(0.0, 0.0);
            refABLineP2 = new vec2(0.0, 1.0);

            currentABLineP1 = new vec2(0.0, 0.0);
            currentABLineP2 = new vec2(0.0, 1.0);

            abHeading = 0.0;

            passNumber = 0.0;

            howManyPathsAway = 0.0;

            isABLineSet = false;
        }

        public void SetABLineByBPoint()
        {
            refPoint2.easting = mf.pn.easting;
            refPoint2.northing = mf.pn.northing;

            //calculate the AB Heading
            abHeading = Math.Atan2(refPoint2.easting - refPoint1.easting, refPoint2.northing - refPoint1.northing);
            if (abHeading < 0) abHeading += glm.twoPI;

            //sin x cos z for endpoints, opposite for additional lines
            refABLineP1.easting = refPoint1.easting - (Math.Sin(abHeading) * 4000.0);
            refABLineP1.northing = refPoint1.northing - (Math.Cos(abHeading) * 4000.0);

            refABLineP2.easting = refPoint1.easting + (Math.Sin(abHeading) * 4000.0);
            refABLineP2.northing = refPoint1.northing + (Math.Cos(abHeading) * 4000.0);

            isABLineSet = true;
        }

        public void SetABLineByHeading()
        {
            //heading is set in the AB Form
            refABLineP1.easting = refPoint1.easting - (Math.Sin(abHeading) * 4000.0);
            refABLineP1.northing = refPoint1.northing - (Math.Cos(abHeading) * 4000.0);

            refABLineP2.easting = refPoint1.easting + (Math.Sin(abHeading) * 4000.0);
            refABLineP2.northing = refPoint1.northing + (Math.Cos(abHeading) * 4000.0);

            refPoint2.easting = refABLineP2.easting;
            refPoint2.northing = refABLineP2.northing;

            isABLineSet = true;
        }

             
    }
}
