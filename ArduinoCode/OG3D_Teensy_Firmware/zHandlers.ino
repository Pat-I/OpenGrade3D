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
char hprSolQuality[12] = {};

// If odd characters showed up.
void errorHandler() {
	//nothing at the moment
}

// Fast linear parser optimized for ARM processors
// targetDecimals: 9 for latitude/longitude (10^9), 3 for altitude (10^3)
int64_t fastParseNmeaToInt(const char* str, int targetDecimals) {
	if (!str || *str == '\0') return 0;

	int64_t result = 0;
	bool isNegative = false;
	int decimalCount = 0;
	bool countingDecimals = false;

	// Handle optional sign
	if (*str == '-') {
		isNegative = true;
		str++;
	} else if (*str == '+') {
		str++;
	}

	// Fast single-pass reading
	while (*str != '\0') {
		char c = *str;

		if (c == '.') {
			countingDecimals = true;
			str++;
			continue;
		}

		// Skip invalid characters or spaces
		if (c < '0' || c > '9') {
			str++;
			continue;
		}

		// Ignore remaining decimals if target scale is already reached
		if (countingDecimals && decimalCount >= targetDecimals) {
			str++;
			continue;
		}

		// Direct integer accumulation by base-10 shifting
		result = (result * 10) + (c - '0');

		if (countingDecimals) {
			decimalCount++;
		}
		str++;
	}

	// Add trailing zeros if string had fewer decimals than expected (Zero padding)
	while (decimalCount < targetDecimals) {
		result *= 10;
		decimalCount++;
	}

	return isNegative ? -result : result;
}

void GGA_Handler()  //Rec'd GGA
{
	// fix time
	parser.getArg(0, fixTime);

	// latitude
	parser.getArg(1, latitude);
	parser.getArg(2, latNS);
	// Convert decimal latitude to int64_t with 10^9 scaling (Sub-millimeter)
	// If Southern hemisphere, make the coordinate negative
	latitudeOG = fastParseNmeaToInt(latitude, 9);
	if (latNS[0] == 'S') latitudeOG = -latitudeOG;

	// longitude
	parser.getArg(3, longitude);
	parser.getArg(4, lonEW);
	// Convert decimal longitude to int64_t with 10^9 scaling
	// If Western hemisphere, make the coordinate negative
	longitudeOG = fastParseNmeaToInt(longitude, 9);
	if (lonEW[0] == 'W') longitudeOG = -longitudeOG;

	// fix quality
	parser.getArg(5, fixQuality);
	fixTypeOG = atoi(fixQuality);
#ifdef isAllInOneBoardProto
	LEDs.setGpsLED(fixTypeOG);
	//LEDs.toggleTeensyLED();
#endif
	// satellite #
	parser.getArg(6, numSats);
	satNbrOG = atoi(numSats);

	// HDOP
	parser.getArg(7, HDOP);
	// Convert HDOP float to int16_t scaled by 100
	hdopOG = (int16_t)fastParseNmeaToInt(HDOP, 2);

	// altitude
	parser.getArg(8, altitude);
	// Convert altitude meters to int32_t millimeters (x1000)
	altitudeOG = (int32_t)fastParseNmeaToInt(altitude, 3);

	// height of geoid
	parser.getArg(10, geoid);

	// time of last DGPS update
	parser.getArg(12, ageDGPS);
	// Convert DGPS update correction age float to int16_t scaled by 100
	ageOG = (int16_t)fastParseNmeaToInt(ageDGPS, 2);

	dataGNSSrecieved = 0;

	BuildNmea();  //Build & send data GPS data to AgIO
}


void VTG_Handler() {
	//$GNVTG,123.119,T,130.046,M,0.00444,N,0.00822,K,A*38
	//      ,heading,T,headMag,M, knots ,N, kPH   ,K,mode *
	// vtg heading
	parser.getArg(0, vtgHeading);
	// Convert single heading to uint16_t scaled by 100
	singleHeadingOG = (uint16_t)fastParseNmeaToInt(vtgHeading, 2);

	// vtg Speed knots (Index 7 is KPH field, or index 4 for Knots multiplied to KPH)
	parser.getArg(6, speedKPH);  // Parsing the KPH index directly is safer
	// Convert speed KPH to int16_t scaled by 100
	speedOG = (int16_t)fastParseNmeaToInt(speedKPH, 2);
}

void HPR_Handler() {
	//$GNHPR,074615.00,320.9610,-66.1712,000.0000,4,47,0.00,0999*45
	//      , time    , heading, pitch  , roll,qual,sat,age,stn *checksum

	char tempHPR[15] = {};
	parser.getArg(1, tempHPR);

	// Convert dual receiver heading to uint16_t scaled by 100
	dualHeadingOG = (uint16_t)fastParseNmeaToInt(tempHPR, 2);

	parser.getArg(2, tempHPR);
	// Convert dual receiver pitch/roll to int16_t scaled by 100
	dualRollOG = (int16_t)fastParseNmeaToInt(tempHPR, 2);

	parser.getArg(3, hprSolQuality);
}

void ZDA_Handler() {
}

void BuildNmea(void) {
	//GNSS sent to OG3D
	//Blink the Teensy LED to show an incomming GGA
	if (blink) digitalWrite(13, HIGH);
	else digitalWrite(13, LOW);
	blink = !blink;
	//uint8_t GNSS1[] = { 0x80, 0x81, 0x6a, 0xd1, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
	//int8_t GNSS1_Size = sizeof(GNSS1);
	// longitudeOG, latitudeOG, altitudeOG, dualHeadingOG, singleHeadingOG, dualRollOG, speedOG, imuHeadingOG, imuRollOG, imuPitchOG, imuYawRateOG, hdopOG, ageOG, fixTypeOG, satNbrOG;

	// We NO LONGER allocate 'uint8_t GNSS1[48]' here because it is already global!
	// This completely removes allocation overhead from the loop.

	// 1. Refresh the dynamic headers just in case
	GNSS1[0] = 0x80;
	GNSS1[1] = 0x81;
	GNSS1[2] = 0x6A;  // Message ID 1
	GNSS1[3] = 0xD1;  // Message ID 2
	GNSS1[4] = 42;    // Payload length

	if (fixTypeOG >= 1 && fixTypeOG <= 5) {
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

		GNSS1[45] = fixTypeOG;
		GNSS1[46] = satNbrOG;
#ifdef debugOG
		Serial.print("Latitude (String): ");
		Serial.println(latitude);
		Serial.printf("LatitudeOG (int64): %lld\n\n", latitudeOG);
		Serial.print("Longitude (String): ");
		Serial.println(longitude);
		Serial.printf("LongitudeOG (int64): %lld\n\n", longitudeOG);
#endif

	} else {
		memcpy(&GNSS1[5], 20000000000000, sizeof(int64_t));
		memcpy(&GNSS1[13], 20000000000000, sizeof(int64_t));
		memcpy(&GNSS1[21], 21000000, sizeof(int32_t));
		memcpy(&GNSS1[25], 0, sizeof(uint16_t));
		memcpy(&GNSS1[27], 0, sizeof(uint16_t));
		memcpy(&GNSS1[29], 0, sizeof(int16_t));
		memcpy(&GNSS1[31], 0, sizeof(int16_t));

		memcpy(&GNSS1[33], &imuHeadingOG, sizeof(uint16_t));
		memcpy(&GNSS1[35], &imuRollOG, sizeof(int16_t));
		memcpy(&GNSS1[37], &imuPitchOG, sizeof(int16_t));
		memcpy(&GNSS1[39], &imuYawRateOG, sizeof(int16_t));
		memcpy(&GNSS1[41], &hdopOG, sizeof(int16_t));
		memcpy(&GNSS1[43], &ageOG, sizeof(int16_t));

		GNSS1[45] = fixTypeOG;
		GNSS1[46] = 0;
//debug: not a good GNSS Fix to send
#ifdef debugOG
		Serial.print("No valid position, quality is: ");
		Serial.println(fixTypeOG);
#endif
	}

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


	fixTypeOG = 255;
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
