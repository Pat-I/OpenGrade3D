//Please, if you use this, share the improvements

using System;
using System.Globalization;
using System.Text;

namespace OpenGrade
{
    #region NMEA_Sentence_Guide

    //$GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M ,  ,*47
    //   0     1      2      3    4      5 6  7  8   9    10 11  12 13  14
    //        Time      Lat       Lon  

    /*
    GGA - essential fix data which provide 3D location and accuracy data.

     $GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M,,*47

    Where:
         GGA          Global Positioning System Fix Data
         123519       Fix taken at 12:35:19 UTC
         4807.038,N   Latitude 48 deg 07.038' N
         01131.000,E  Longitude 11 deg 31.000' E
         1            Fix quality: 0 = invalid
                                   1 = GPS fix (SPS)
                                   2 = DGPS fix
                                   3 = PPS fix
                                   4 = Real Time Kinematic
                                   5 = Float RTK
                                   6 = estimated (dead reckoning) (2.3 feature)
                                   7 = Manual input mode
                                   8 = Simulation mode
         08           Number of satellites being tracked
         0.9          Horizontal dilution of position
         545.4,M      Altitude, Meters, above mean sea level
         46.9,M       Height of geoid (mean sea level) above WGS84
                          ellipsoid
         (empty field) time in seconds since last DGPS update
         (empty field) DGPS station ID number
         *47          the checksum data, always begins with *
     *
     * 
   $GPVTG,054.7,T,034.4,M,005.5,N,010.2,K*48
     *   
        VTG          Track made good and ground speed
        054.7,T      True track made good (degrees)
        034.4,M      Magnetic track made good
        005.5,N      Ground speed, knots
        010.2,K      Ground speed, Kilometers per hour
        *48          Checksum
    */

    #endregion NMEA_Sentence_Guide

    public class CNMEA
    {
        //WGS84 Lat Long
        public double latitude, longitude;
        //local plane geometry
        public double latStart, lonStart;

        public double mPerDegreeLat, mPerDegreeLon;

        public bool updatedGGA, updatedVTG;

        public string rawBuffer = "";
        private string[] words;
        private string nextNMEASentence = "";

        //UTM coordinates
        public double northing, easting, northingAgd, eastingAgd;
        //public double actualEasting, actualNorthing;
        public double zone;

        //Position Offset corrections, by Pat
        public double northingOffset, eastingOffset, altitudeOffset;

        //other GIS Info
        public double altitude, speed;
        public double headingTrue, hdop, ageDiff;
        public double GPSroll, GPSpitch, GPSyawRate;

        public int fixQuality;
        public int satellitesTracked;
        public string status = "q";
        public DateTime utcDateTime;

        public char hemisphere = 'N';

        //UTM numbers are huge, these cut them way down.
        public int utmNorth = 0, utmEast = 0;
        public StringBuilder logNMEASentence = new StringBuilder();
        private readonly FormGPS mf;

        public CNMEA(FormGPS f)
        {
            //constructor, grab the main form reference
            mf = f;
        }

        //ParseNMEA
        public void ParseNMEA()
        {
            if (rawBuffer == null) return;

            //find end of a sentence
            int cr = rawBuffer.IndexOf("\r\n", StringComparison.Ordinal);
            if (cr == -1) return; // No end found, wait for more data

            // Find start of next sentence
            int dollar = rawBuffer.IndexOf("$", StringComparison.Ordinal);
            if (dollar == -1) return;

            //if the $ isn't first, get rid of the tail of corrupt sentence
            if (dollar >= cr) rawBuffer = rawBuffer.Substring(dollar);

            cr = rawBuffer.IndexOf("\r\n", StringComparison.Ordinal);
            if (cr == -1) return; // No end found, wait for more data
            dollar = rawBuffer.IndexOf("$", StringComparison.Ordinal);
            if (dollar == -1) return;

            //if the $ isn't first, get rid of the tail of corrupt sentence
            if (dollar >= cr) rawBuffer = rawBuffer.Substring(dollar);

            cr = rawBuffer.IndexOf("\r\n", StringComparison.Ordinal);
            dollar = rawBuffer.IndexOf("$", StringComparison.Ordinal);
            if (cr == -1 || dollar == -1) return;

            //now we have a complete sentence or more somewhere in the portData
            while (true)
            {
                //extract the next NMEA single sentence
                nextNMEASentence = Parse();
                if (nextNMEASentence == null) return;

                //parse them accordingly
                words = nextNMEASentence.Split(',');
                if (words.Length < 9) return;

                //if (words.Length < 3) break; from AgIO v5.6

                if ((words[0] == "$GPGGA" || words[0] == "$GNGGA") && words.Length > 13)
                {
                    ParseGGA();
                    //if (isGPSSentencesOn) ggaSentence = nextNMEASentence; from AgIO v5.6
                }

                else if ((words[0] == "$GPVTG" || words[0] == "$GNVTG") && words.Length > 7)
                {
                    ParseVTG();
                    //if (isGPSSentencesOn) vtgSentence = nextNMEASentence; from AgIO v5.6from 
                }

                else if (words[0] == "$PAOGI" && words.Length > 14)
                {
                    ParseOGI();
                    //if (isGPSSentencesOn) paogiSentence = nextNMEASentence; from AgIO v5.6
                }

                else if (words[0] == "$PANDA" && words.Length > 14)
                {
                    ParsePANDA();
                    //if (isGPSSentencesOn) pandaSentence = nextNMEASentence; from AgIO v5.6
                }
            }// while still data
        }

        public string currentNMEASentenceGGA = "";
        public string currentNMEASentenceVTG = "";

        // Returns a valid NMEA sentence from the pile from portData
        public string Parse()
        {
            string sentence;
            do
            {
                //double check for valid sentence
                // Find start of next sentence
                int start = rawBuffer.IndexOf("$", StringComparison.Ordinal);
                if (start == -1) return null;
                rawBuffer = rawBuffer.Substring(start);

                // Find end of sentence
                int end = rawBuffer.IndexOf("\r\n", StringComparison.Ordinal);
                if (end == -1) return null;

                //the NMEA sentence to be parsed
                sentence = rawBuffer.Substring(0, end + 2);

                //remove the processed sentence from the rawBuffer
                rawBuffer = rawBuffer.Substring(end + 2);
            }

            //if sentence has valid checksum, its all good
            while (!ValidateChecksum(sentence));

            // Remove trailing checksum and \r\n and return
            sentence = sentence.Substring(0, sentence.IndexOf("*", StringComparison.Ordinal));
            return sentence;
        }

        //The indivdual sentence parsing
        private void ParseGGA()
        {
            //$GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M ,  ,*47
            //   0     1      2      3    4      5 6  7  8   9    10 11  12 13  14
            //        Time      Lat       Lon  

            //is the sentence GGA
            if (!String.IsNullOrEmpty(words[2]) & !String.IsNullOrEmpty(words[3])
                & !String.IsNullOrEmpty(words[4]) & !String.IsNullOrEmpty(words[5]))
            {
                double temp;
                //get latitude and convert to decimal degrees
                double.TryParse(words[2].Substring(0, 2), NumberStyles.Float, CultureInfo.InvariantCulture, out latitude);
                double.TryParse(words[2].Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out  temp);
                temp *= 0.01666666666666666666666666666667;
                latitude += temp;
                if (words[3] == "S")
                {
                    latitude *= -1;
                    hemisphere = 'S';
                }
                else { hemisphere = 'N'; }

                //get longitude and convert to decimal degrees
                double.TryParse(words[4].Substring(0, 3), NumberStyles.Float, CultureInfo.InvariantCulture, out longitude);
                    double.TryParse(words[4].Substring(3), NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
                longitude += temp * 0.01666666666666666666666666666667;

                 { if (words[5] == "W") longitude *= -1; }

                //calculate zone and UTM coords
                //DecDeg2UTM();
                ConvertWGS84ToLocal(latitude, longitude, out northing, out easting);

                //fixQuality
                int.TryParse(words[6], NumberStyles.Float, CultureInfo.InvariantCulture, out fixQuality);

                //satellites tracked
                int.TryParse(words[7], NumberStyles.Float, CultureInfo.InvariantCulture, out satellitesTracked);

                //hdop
                double.TryParse(words[8], NumberStyles.Float, CultureInfo.InvariantCulture, out hdop);

                //altitude
                double.TryParse(words[9], NumberStyles.Float, CultureInfo.InvariantCulture, out altitude);
                //altitude -= mf.vehicle.antennaHeight;
                altitude -= (mf.vehicle.antennaHeight + mf.vehicle.bladeOffset - altitudeOffset);
                //altitude = altitude - mf.vehicle.antennaHeight + mf.vehicle.bladeOffset;

                //age of differential
                double.TryParse(words[12], NumberStyles.Float, CultureInfo.InvariantCulture, out ageDiff);

                updatedGGA = true;
                mf.recvCounter = 0;
            }
        }

        private void ParseVTG()
        {
            //$GPVTG,054.7,T,034.4,M,005.5,N,010.2,K*48
            //is the sentence GGA
            if (!String.IsNullOrEmpty(words[1]) & !String.IsNullOrEmpty(words[5]))
            {
                //kph for speed - knots read
                double.TryParse(words[5], NumberStyles.Float, CultureInfo.InvariantCulture, out speed);
                speed = Math.Round(speed * 1.852, 1);

                //True heading
                double.TryParse(words[1], NumberStyles.Float, CultureInfo.InvariantCulture, out headingTrue);

                //a valid VTG so set the flag
                updatedVTG = true;

                //average the speeds for display, not calcs
                mf.avgSpeed[mf.ringCounter] = speed;
                if (mf.ringCounter++ > 8) mf.ringCounter = 0;
            }
        }

        private void ParseOGI()
        {
            #region PAOGI Message
            /*
            $PAOGI
            (1) 123519 Fix taken at 1219 UTC

            Roll corrected position
            (2,3) 4807.038,N Latitude 48 deg 07.038' N
            (4,5) 01131.000,E Longitude 11 deg 31.000' E

            (6) 1 Fix quality: 
                0 = invalid
                1 = GPS fix(SPS)
                2 = DGPS fix
                3 = PPS fix
                4 = Real Time Kinematic
                5 = Float RTK
                6 = estimated(dead reckoning)(2.3 feature)
                7 = Manual input mode
                8 = Simulation mode
            (7) Number of satellites being tracked
            (8) 0.9 Horizontal dilution of position
            (9) 545.4 Altitude (ALWAYS in Meters, above mean sea level)
            (10) 1.2 time in seconds since last DGPS update

            (11) 022.4 Speed over the ground in knots - can be positive or negative

            FROM AHRS:
            (12) Heading in degrees
            (13) Roll angle in degrees(positive roll = right leaning - right down, left up)
            (14) Pitch angle in degrees(Positive pitch = nose up)
            (15) Yaw Rate in Degrees / second

            * CHKSUM
            */
            #endregion PAOGI Message

            if (!string.IsNullOrEmpty(words[1]) && !string.IsNullOrEmpty(words[2]) && !string.IsNullOrEmpty(words[3])
                && !string.IsNullOrEmpty(words[4]) && !string.IsNullOrEmpty(words[5]))
            {

                double temp;
                //get latitude and convert to decimal degrees
                double.TryParse(words[2].Substring(0, 2), NumberStyles.Float, CultureInfo.InvariantCulture, out latitude);
                double.TryParse(words[2].Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
                temp *= 0.01666666666666666666666666666667;
                latitude += temp;
                if (words[3] == "S")
                {
                    latitude *= -1;
                    hemisphere = 'S';
                }
                else { hemisphere = 'N'; }

                //get longitude and convert to decimal degrees
                double.TryParse(words[4].Substring(0, 3), NumberStyles.Float, CultureInfo.InvariantCulture, out longitude);
                double.TryParse(words[4].Substring(3), NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
                longitude += temp * 0.01666666666666666666666666666667;

                { if (words[5] == "W") longitude *= -1; }

                //calculate zone and UTM coords
                //DecDeg2UTM();
                ConvertWGS84ToLocal(latitude, longitude, out northing, out easting);

                //FixQuality
                int.TryParse(words[6], NumberStyles.Float, CultureInfo.InvariantCulture, out fixQuality);

                //satellites tracked
                int.TryParse(words[7], NumberStyles.Float, CultureInfo.InvariantCulture, out satellitesTracked);

                //hdop
                double.TryParse(words[8], NumberStyles.Float, CultureInfo.InvariantCulture, out hdop);

                //altitude
                double.TryParse(words[9], NumberStyles.Float, CultureInfo.InvariantCulture, out altitude);

                //kph for speed - knots read
                double.TryParse(words[11], NumberStyles.Float, CultureInfo.InvariantCulture, out speed);

                //Dual antenna derived heading
                double.TryParse(words[12], NumberStyles.Float, CultureInfo.InvariantCulture, out headingTrue);

                //roll
                double.TryParse(words[13], NumberStyles.Float, CultureInfo.InvariantCulture, out GPSroll);
                GPSroll *= 0.1;

                //age
                double.TryParse(words[10], NumberStyles.Float, CultureInfo.InvariantCulture, out ageDiff);

                //Pitch
                double.TryParse(words[14], NumberStyles.Float, CultureInfo.InvariantCulture, out GPSpitch);
                GPSpitch *= 0.1;

                //Yaw rate
                double.TryParse(words[15], NumberStyles.Float, CultureInfo.InvariantCulture, out GPSyawRate);

                //a valid VTG so set the flag
                updatedVTG = true;
                updatedGGA = true;
                mf.recvCounter = 0;

                //average the speeds for display, not calcs
                mf.avgSpeed[mf.ringCounter] = speed;
                if (mf.ringCounter++ > 8) mf.ringCounter = 0;
            }
        }

        private void ParsePANDA()
        {
            #region PANDA Message
            /*
            $PANDA
            (1) Time of fix

            position
            (2,3) 4807.038,N Latitude 48 deg 07.038' N
            (4,5) 01131.000,E Longitude 11 deg 31.000' E

            (6) 1 Fix quality: 
                0 = invalid
                1 = GPS fix(SPS)
                2 = DGPS fix
                3 = PPS fix
                4 = Real Time Kinematic
                5 = Float RTK
                6 = estimated(dead reckoning)(2.3 feature)
                7 = Manual input mode
                8 = Simulation mode
            (7) Number of satellites being tracked
            (8) 0.9 Horizontal dilution of position
            (9) 545.4 Altitude (ALWAYS in Meters, above mean sea level)
            (10) 1.2 time in seconds since last DGPS update
            (11) Speed in knots

            FROM IMU:
            (12) Heading in degrees
            (13) Roll angle in degrees(positive roll = right leaning - right down, left up)
            
            (14) Pitch angle in degrees(Positive pitch = nose up)
            (15) Yaw Rate in Degrees / second

            * CHKSUM
            */
            #endregion PANDA Message

            if (!string.IsNullOrEmpty(words[1]) && !string.IsNullOrEmpty(words[2]) && !string.IsNullOrEmpty(words[3])
                && !string.IsNullOrEmpty(words[3]) && !string.IsNullOrEmpty(words[0]))
            {


                double temp;
                //get latitude and convert to decimal degrees
                double.TryParse(words[2].Substring(0, 2), NumberStyles.Float, CultureInfo.InvariantCulture, out latitude);
                double.TryParse(words[2].Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
                temp *= 0.01666666666666666666666666666667;
                latitude += temp;
                if (words[3] == "S")
                {
                    latitude *= -1;
                    hemisphere = 'S';
                }
                else { hemisphere = 'N'; }

                //get longitude and convert to decimal degrees
                double.TryParse(words[4].Substring(0, 3), NumberStyles.Float, CultureInfo.InvariantCulture, out longitude);
                double.TryParse(words[4].Substring(3), NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
                longitude += temp * 0.01666666666666666666666666666667;

                { if (words[5] == "W") longitude *= -1; }

                //calculate zone and UTM coords
                //DecDeg2UTM();
                ConvertWGS84ToLocal(latitude, longitude, out northing, out easting);

                //FixQuality
                int.TryParse(words[6], NumberStyles.Float, CultureInfo.InvariantCulture, out fixQuality);


                //satellites tracked
                int.TryParse(words[7], NumberStyles.Float, CultureInfo.InvariantCulture, out satellitesTracked);

                //hdop
                double.TryParse(words[8], NumberStyles.Float, CultureInfo.InvariantCulture, out hdop);


                //altitude
                double.TryParse(words[9], NumberStyles.Float, CultureInfo.InvariantCulture, out altitude);

                //age
                double.TryParse(words[10], NumberStyles.Float, CultureInfo.InvariantCulture, out ageDiff);

                //kph for speed - knots read
                double.TryParse(words[11], NumberStyles.Float, CultureInfo.InvariantCulture, out speed);

                //imu heading
                double.TryParse(words[12], NumberStyles.Float, CultureInfo.InvariantCulture, out headingTrue);

                //roll
                double.TryParse(words[13], NumberStyles.Float, CultureInfo.InvariantCulture, out GPSroll);
                GPSroll *= 0.1;

                //Pitch
                double.TryParse(words[14], NumberStyles.Float, CultureInfo.InvariantCulture, out GPSpitch);
                GPSpitch *= 0.1;

                //YawRate
                double.TryParse(words[15], NumberStyles.Float, CultureInfo.InvariantCulture, out GPSyawRate);


                //a valid VTG so set the flag
                updatedVTG = true;
                updatedGGA = true;
                mf.recvCounter = 0;

                //average the speeds for display, not calcs
                mf.avgSpeed[mf.ringCounter] = speed;
                if (mf.ringCounter++ > 8) mf.ringCounter = 0;
            }
        }

        //checks the checksum against the string
        public bool ValidateChecksum(string Sentence)
        {
            int sum = 0;
            try
            {
                char[] sentenceChars = Sentence.ToCharArray();
                // All character xor:ed results in the trailing hex checksum
                // The checksum calc starts after '$' and ends before '*'
                int inx;
                for (inx = 1; ; inx++)
                {
                    if (inx >= sentenceChars.Length) // No checksum found
                        return false;
                    var tmp = sentenceChars[inx];
                    // Indicates end of data and start of checksum
                    if (tmp == '*') break;
                    sum ^= tmp;    // Build checksum
                }
                // Calculated checksum converted to a 2 digit hex string
                string sumStr = String.Format("{0:X2}", sum);

            // Compare to checksum in sentence
            return sumStr.Equals(Sentence.Substring(inx + 1, 2));
            }
            catch (Exception e)
            {
                mf.WriteErrorLog("Validate Checksum" + e);
                return false;
            }
         }

        public double Distance(double northing1, double easting1, double northing2, double easting2)
        {
            return Math.Sqrt(
                Math.Pow(easting1 - easting2, 2)
                + Math.Pow(northing1 - northing2, 2));
        }

        //not normalized distance, no square root
        public double DistanceSquared(double northing1, double easting1, double northing2, double easting2)
        {
            return  Math.Pow(easting1 - easting2, 2) + Math.Pow(northing1 - northing2, 2);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        





        //new easting northing calculation






        public void SetLocalMetersPerDegree()
        {
            mPerDegreeLat = 111132.92 - 559.82 * Math.Cos(2.0 * latStart * 0.01745329251994329576923690766743) + 1.175
            * Math.Cos(4.0 * latStart * 0.01745329251994329576923690766743) - 0.0023
            * Math.Cos(6.0 * latStart * 0.01745329251994329576923690766743);

            mPerDegreeLon = 111412.84 * Math.Cos(latStart * 0.01745329251994329576923690766743) - 93.5
            * Math.Cos(3.0 * latStart * 0.01745329251994329576923690766743) + 0.118
            * Math.Cos(5.0 * latStart * 0.01745329251994329576923690766743);

            ConvertWGS84ToLocal(latitude, longitude, out double northing, out double easting);
            mf.worldGrid.checkZoomWorldGrid(northing, easting);
        }

        

        public void ConvertWGS84ToLocal(double Lat, double Lon, out double Northing, out double Easting)
        {
            mPerDegreeLon = 111412.84 * Math.Cos(Lat * 0.01745329251994329576923690766743) - 93.5 * Math.Cos(3.0 * Lat * 0.01745329251994329576923690766743) + 0.118 * Math.Cos(5.0 * Lat * 0.01745329251994329576923690766743);

            Northing = (Lat - latStart) * mPerDegreeLat;
            Easting = (Lon - lonStart) * mPerDegreeLon;

            //Northing += mf.RandomNumber(-0.02, 0.02);
            //Easting += mf.RandomNumber(-0.02, 0.02);
        }

        public void ConvertLocalToWGS84(double Northing, double Easting, out double Lat, out double Lon)
        {
            Lat = (Northing / mPerDegreeLat) + latStart;
            mPerDegreeLon = 111412.84 * Math.Cos(Lat * 0.01745329251994329576923690766743) - 93.5 * Math.Cos(3.0 * Lat * 0.01745329251994329576923690766743) + 0.118 * Math.Cos(5.0 * Lat * 0.01745329251994329576923690766743);
            Lon = (Easting / mPerDegreeLon) + lonStart;
        }

        public string GetLocalToWSG84_KML(double Easting, double Northing)
        {
            double Lat = (Northing / mPerDegreeLat) + latStart;
            mPerDegreeLon = 111412.84 * Math.Cos(Lat * 0.01745329251994329576923690766743) - 93.5 * Math.Cos(3.0 * Lat * 0.01745329251994329576923690766743) + 0.118 * Math.Cos(5.0 * Lat * 0.01745329251994329576923690766743);
            double Lon = (Easting / mPerDegreeLon) + lonStart;

            return Lon.ToString("N7", CultureInfo.InvariantCulture) + ',' + Lat.ToString("N7", CultureInfo.InvariantCulture) + ",0 ";
        }
    }
}