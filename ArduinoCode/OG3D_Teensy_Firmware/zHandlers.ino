// Conversion to Hexidecimal
const char* asciiHex = "0123456789ABCDEF";

// the new PANDA sentence buffer
char nmea[100];

// GGA
char fixTime[12];
char latitude[15];
char latNS[3];
char longitude[15];
char lonEW[3];
char fixQuality[2];
char numSats[4];
char HDOP[5];
char altitude[12];
char geoid[12];
char ageDGPS[10];

// VTG
char vtgHeading[12] = {};
char speedKnots[10] = {};
char speedKPH[10] = {};

//HPR
bool useHPR = false;
bool dualReadyHPR = false;
float hprHeading = 0;
float hprRoll = 0;
char hprSolQuality[12] = {};

// IMU
char imuHeading[6];
char imuRoll[6];
char imuPitch[6];
char imuYawRate[6];

// If odd characters showed up.
void errorHandler() {
	//nothing at the moment
}

void GGA_Handler()  //Rec'd GGA
{
	// fix time
	parser.getArg(0, fixTime);

	String tempString = fixTime;
	utcTime = tempString.toFloat();

	// latitude
	parser.getArg(1, latitude);
	tempString = latitude;
	pivotLat = tempString.toFloat();

	parser.getArg(2, latNS);
	// Convert decimal latitude to int64_t with 10^9 scaling (Sub-millimeter)
	// If Southern hemisphere, make the coordinate negative
	latitudeOG = (int64_t)round(atof(tempString.c_str()) * 1000000000.0);
	if (latNS[0] == 'S') latitudeOG = -latitudeOG;

	// longitude
	parser.getArg(3, longitude);
	tempString = longitude;
	pivotLon = tempString.toFloat();
	pivotLon *= -1;

	parser.getArg(4, lonEW);
	// Convert decimal longitude to int64_t with 10^9 scaling
	// If Western hemisphere, make the coordinate negative
	longitudeOG = (int64_t)round(atof(tempString.c_str()) * 1000000000.0);
	if (lonEW[0] == 'W') longitudeOG = -longitudeOG;

	// fix quality
	parser.getArg(5, fixQuality);
	tempString = fixQuality;
	fixTypeGGA = tempString.toInt();
	fixQualityOG = fixTypeGGA;
	//LEDs.setGpsLED(fixTypeGGA);
	//LEDs.toggleTeensyLED();

	// satellite #
	parser.getArg(6, numSats);

	tempString = numSats;
	satsGGA = tempString.toInt();
	satNbrOG = satsGGA;

	// HDOP
	parser.getArg(7, HDOP);

	tempString = HDOP;
	hdopGGA = tempString.toFloat();
	// Convert HDOP float to int16_t scaled by 100
	hdopOG = (int16_t)round(tempString.toFloat() * 100.0f);

	// altitude
	parser.getArg(8, altitude);
	tempString = altitude;
	pivotAltitude = tempString.toFloat();
	// Convert altitude meters to int32_t millimeters (x1000)
	altitudeOG = (int32_t)round(tempString.toFloat() * 1000.0f);

	dataRecieved = true;
	// height of geoid
	parser.getArg(10, geoid);

	tempString = geoid;
	geoidalGGA = tempString.toFloat();

	// time of last DGPS update
	parser.getArg(12, ageDGPS);

	tempString = ageDGPS;
	rtkAgeGGA = tempString.toFloat();
	// Convert DGPS update correction age float to int16_t scaled by 100
	ageOG = (int16_t)round(tempString.toFloat() * 100.0f);

	BuildNmea();  //Build & send data GPS data to AgIO
}


void VTG_Handler() {
	//$GNVTG,123.119,T,130.046,M,0.00444,N,0.00822,K,A*38
	//      ,heading,T,headMag,M, knots ,N, kPH   ,K,mode *
	// vtg heading
	parser.getArg(0, vtgHeading);
	String tempString = vtgHeading;
	// Convert single heading to uint16_t scaled by 100
	singleHeadingOG = (uint16_t)round(tempString.toFloat() * 100.0f);

	// vtg Speed knots (Index 7 is KPH field, or index 4 for Knots multiplied to KPH)
	parser.getArg(7, speedKPH);  // Parsing the KPH index directly is safer
	tempString = speedKPH;
	// Convert speed KPH to int16_t scaled by 100
	speedOG = (int16_t)round(tempString.toFloat() * 100.0f);
}

void HPR_Handler() {
	//$GNHPR,074615.00,320.9610,-66.1712,000.0000,4,47,0.00,0999*45
	//      , time    , heading, pitch  , roll,qual,sat,age,stn *checksum

	char tempHPR[10] = {};
	parser.getArg(1, tempHPR);
	hprHeading = atof(tempHPR);

	String tempString = tempHPR;
	fixHeading = tempString.toFloat();
	// Convert dual receiver heading to uint16_t scaled by 100
	dualHeadingOG = (uint16_t)round(tempString.toFloat() * 100.0f);

	parser.getArg(2, tempHPR);
	hprRoll = atof(tempHPR);
	tempString = tempHPR;
	// Convert dual receiver pitch/roll to int16_t scaled by 100
	dualRollOG = (int16_t)round(tempString.toFloat() * 100.0f);

	parser.getArg(3, hprSolQuality);
}

void ZDA_Handler() {
}

void BuildNmea(void) {
	//GNSS sent to OG3D
	//uint8_t GNSS1[] = { 0x80, 0x81, 0x6a, 0xd1, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
	//int8_t GNSS1_Size = sizeof(GNSS1);
	// longitudeOG, latitudeOG, altitudeOG, dualHeadingOG, singleHeadingOG, dualRollOG, speedOG, imuHeadingOG, imuRollOG, imuPitchOG, imuYawRateOG, hdopOG, ageOG, fixQualityOG, satNbrOG;

	// We NO LONGER allocate 'uint8_t GNSS1[48]' here because it is already global!
	// This completely removes allocation overhead from the loop.

	// 1. Refresh the dynamic headers just in case
	GNSS1[0] = 0x80;
	GNSS1[1] = 0x81;
	GNSS1[2] = 0x6A;  // Message ID 1
	GNSS1[3] = 0xD1;  // Message ID 2
	GNSS1[4] = 42;    // Payload length

	// 2. Serialize your parameters directly into your pre-allocated global array
	memcpy(&GNSS1[5], &longitudeOG, sizeof(int64_t));
	memcpy(&GNSS1[13], &latitudeOG, sizeof(int64_t));
	memcpy(&GNSS1[21], &altitudeOG, sizeof(int32_t));
	memcpy(&GNSS1[25], &dualHeadingOG, sizeof(uint16_t));
	memcpy(&GNSS1[27], &singleHeadingOG, sizeof(uint16_t));
	memcpy(&GNSS1[29], &dualRollOG, sizeof(int16_t));
	memcpy(&GNSS1[31], &speedOG, sizeof(int16_t));

	memcpy(&GNSS1[33], &imuHeadingOG, sizeof(uint16_t));
	memcpy(&GNSS1[35], &imuRollOG, sizeof(int16_t));
	memcpy(&GNSS1[37], &imuPitchOG, sizeof(int16_t));
	memcpy(&GNSS1[39], &imuYawRateOG, sizeof(int16_t));
	memcpy(&GNSS1[41], &hdopOG, sizeof(int16_t));
	memcpy(&GNSS1[43], &ageOG, sizeof(int16_t));

	GNSS1[45] = fixQualityOG;
	GNSS1[46] = satNbrOG;

	// 3. Fast CRC checksum calculation over the global frame indexes
	uint8_t calculatedCrc = 0;
	for (int i = 2; i < 47; i++) {
		calculatedCrc += GNSS1[i];
	}
	GNSS1[47] = calculatedCrc;

	// 4. Send the updated global frame via UDP
	Udp.beginPacket(ipDestination, 9999);
	Udp.write(GNSS1, 48);  // We pass 48 bytes explicitly
	Udp.endPacket();
}

void CalculateChecksum(void) {
	int16_t sum = 0;
	int16_t inx = 0;
	char tmp;

	// The checksum calc starts after '$' and ends before '*'
	for (inx = 1; inx < 200; inx++) {
		tmp = nmea[inx];

		// * Indicates end of data and start of checksum
		if (tmp == '*') {
			break;
		}

		sum ^= tmp;  // Build checksum
	}

	byte chk = (sum >> 4);
	char hex[2] = { asciiHex[chk], 0 };
	strcat(nmea, hex);

	chk = (sum % 16);
	char hex2[2] = { asciiHex[chk], 0 };
	strcat(nmea, hex2);
}

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

  CHKSUM
*/

/*
  $GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M ,  ,*47
   0     1      2      3    4      5 6  7  8   9    10 11  12 13  14
		Time      Lat       Lon     FixSatsOP Alt
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
	  47          the checksum data, always begins with


  $GPRMC,123519,A,4807.038,N,01131.000,E,022.4,084.4,230394,003.1,W*6A
  0      1    2   3      4    5      6   7     8     9     10   11
		Time      Lat        Lon       knots  Ang   Date  MagV

  Where:
	 RMC          Recommended Minimum sentence C
	 123519       Fix taken at 12:35:19 UTC
	 A            Status A=active or V=Void.
	 4807.038,N   Latitude 48 deg 07.038' N
	 01131.000,E  Longitude 11 deg 31.000' E
	 022.4        Speed over the ground in knots
	 084.4        Track angle in degrees True
	 230394       Date - 23rd of March 1994
	 003.1,W      Magnetic Variation
	  6A          The checksum data, always begins with

  $GPVTG,054.7,T,034.4,M,005.5,N,010.2,K*48

	VTG          Track made good and ground speed
	054.7,T      True track made good (degrees)
	034.4,M      Magnetic track made good
	005.5,N      Ground speed, knots
	010.2,K      Ground speed, Kilometers per hour
	 48          Checksum
*/
