/* 
 4 Feb 2021, Brian Tischler
 Like all Arduino code - copied from somewhere else :)
 So don't claim it as your own
Changes by Pat
 */
//----------------------------------------------------------

// it can also be used with the AIO
// uncomment the following line if you're using the All-In-One-Board (proto v5)
#define isAllInOneBoard
// uncomment the following line if you're using pins for blade offset
#define bladeOffsetPropLever
#define bladeOffsetBtn
#define useLEDs  // LEDs using four outputs
//User set variables
//PWM or relay mode
bool proportionalValve = true;
//workswitch or work button
bool workButton = true;  // true for momentary button, false for switch(continus)
//proportional lever
bool manualMovePropLever = true;  //if a lever for manual operation is installed
bool invertManMove = false;
// blade off set choose betwen lever or btn or none.
bool invertBladeOffset = false;
//PWM or relay mode


#ifdef isAllInOneBoard

#define DIR_ENABLE 4  //PD4 cytron dir
#define PWM_OUT 3     //PD3  cytron pwm
#define WORKSW_PIN 7  //PD7 this pin must be low (to ground) to activate automode IMP on PCB
#define LEVER_UP A1   // first axle
#ifdef bladeOffsetBtn
#define BOFFUP_PIN 8  //signal (to GND) to move the blade offset up 1 cm?
#define BOFFDW_PIN 6  //offset down
#endif
#ifdef bladeOffsetPropLever
#define LEVER_SIDE A2  // second axle, if used for blade offset
#endif
//leds
#ifdef useLEDs
#define LED_DW 2    //DO2 led down (if used)
#define LED_UP 5    //DO5 led up (if used)
#define LED_AUTO 9  //DO9 led auto
#define LED_ON A0   //A0 on led
#endif
#else  //AiO v4.5 // pin numbers not set yet

#define DIR_ENABLE 4  //PD4 cytron dir
#define PWM_OUT 3     //PD3  cytron pwm
#define WORKSW_PIN 7  //PD7 this pin must be low (to ground) to activate automode IMP on PCB
#define LEVER_UP A1   // first axle
#ifdef bladeOffsetBtn
#define BOFFUP_PIN 8  //signal (to GND) to move the blade offset up 1 cm?
#define BOFFDW_PIN 6  //offset down
#endif
#ifdef bladeOffsetPropLever
#define LEVER_SIDE A2  // second axle, if used for blade offset
#endif
//leds
#ifdef useLEDs
#define LED_DW 2    //DO2 led down (if used)
#define LED_UP 5    //DO5 led up (if used)
#define LED_AUTO 9  //DO9 led auto
#define LED_ON A0   //A0 on led
#endif
#endif
//----------------------------------------------------------

#ifdef isAllInOneBoard
String inoVersion = ("\r\nOG3D Ver 2026.08.30 (AIO v5 Proto PCB))");
#else  //AiO v4.5
String inoVersion = ("\r\nOG3D Ver 2026.08.30 (AIO v4 PCB))");
#endif

////////////////// User Settings /////////////////////////
/*  PWM Frequency ->
 *   490hz (default) = 0
 *   122hz = 1
 *   3921hz = 2
 */
#define PWM_Frequency 1

/////////////////////////////////////////////

// if not in eeprom, overwrite
#define EEP_Ident 0x54

#include <Wire.h>
#include <EEPROM.h>
#include "zNMEAParser.h"
#include "LEDS.h"
LEDS LEDs = LEDS(1000, 255, 64, 127);  // 1000ms RGB update, 255/64/127 RGB brightness balance levels for v5.0a

/* A parser is declared with 3 handlers at most */
NMEAParser<3> parser;

//Used to set CPU speed
extern "C" uint32_t set_arm_clock(uint32_t frequency);  // required prototype
extern float tempmonGetTemp(void);
elapsedMillis tempChecker;

//----Teensy 4.1 Ethernet--Start---------------------
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>

struct ConfigIP {
	uint8_t ipOne = 192;
	uint8_t ipTwo = 168;
	uint8_t ipThree = 5;
};
ConfigIP networkAddress;  //3 bytes

// Module IP Address / Port
IPAddress ip = { 0, 0, 0, 155 };
unsigned int localPort = 8888;
unsigned int NtripPort = 2233;

// AOG IP Address / Port
static uint8_t ipDestination[] = { 0, 0, 0, 255 };
unsigned int AOGPort = 9999;

//MAC address
byte mac[] = { 0x00, 0x00, 0x58, 0x00, 0x00, 0x6E };

// Buffer For Receiving UDP Data
byte udpData[128];  // Incomming Buffer
byte NtripData[512];

// An EthernetUDP instance to let us send and receive packets over UDP
EthernetUDP Udp;
EthernetUDP NtripUdp;

//----Teensy 4.1 Ethernet--End---------------------

//Main loop time variables in microseconds
const uint16_t LOOP_TIME = 5;  //200Hz
uint32_t lastTime = LOOP_TIME;
uint32_t currentTime = LOOP_TIME;
uint32_t watchdogTimer = 0;
uint32_t loopTimer = 0;

//IMU data
bool blink;

//IMU data
float roll = 0;
float pitch = 0;
float yaw = 0;

//Dual data
double baseline = 0;
double rollDual = 0;
double relPosD = 0;
double heading = 0;

//GPS Data
double pivotLat = 0, pivotLon = 0, fixHeading = 0, pivotAltitude = 0;
float utcTime, geoidalGGA;
uint8_t fixTypeGGA, satsGGA;
float hdopGGA, rtkAgeGGA;

//show life in AgIO - v5.5
uint8_t helloAgIO[] = { 0x80, 0x81, 0x7f, 0xC7, 1, 0, 0x47 };
uint32_t helloCounter = 0;

//Heart beat hello AgIO - v5.6
uint8_t helloFromAutoSteer[] = { 128, 129, 126, 126, 5, 0, 0, 0, 0, 0, 71 };
int16_t helloSteerPosition = 0;

//from OG3D board to OG3D 6A E1 -
uint8_t OG3D[] = { 0x80, 0x81, 0x6a, 0xe1, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
int16_t OG3DSize = sizeof(OG3D);

//GNSS sent to OG3D
uint8_t GNSS1[] = { 0x80, 0x81, 0x6a, 0xd1, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
int8_t GNSS1_Size = sizeof(GNSS1);

int64_t longitudeOG = 200000000, latitudeOG = 200000000;                                                                          // x 1000 000 000
int32_t altitudeOG = 20000000;                                                                                                    //mm
uint16_t dualHeadingOG = 40000, singleHeadingOG = 40000, imuHeadingOG = 40000;                                                    // x 100
int16_t dualRollOG = 32000, imuRollOG = 32000, imuPitchOG = 32000, imuYawRateOG = 32000, speedOG = 32000, hdopOG = 0, ageOG = 0;  // x100, km/h x 100,
uint8_t fixQualityOG = 0, satNbrOG = 0;

//Blade control variables
byte deadband = 5;
byte cutValve = 100, cutValveReceived = 100;
int32_t targetAltitude = 20000000;
//workSwitch
bool workSwitch = true;  //high is circuit open, low is switch grounded
bool autoEnable = false;
bool settingsRecieved = false;
bool dataRecieved = false;
byte multipleValue = 0;
//pwm variables
byte pwmDrive = 0, pwmDisplay = 0, pwmGainUp = 0, pwmMinUp = 0, pwmGainDw = 0, pwmMinDw = 0, pwmMaxUp = 0, pwmMaxDw = 0, integralMultiplier = 0;
int pwmValue = 0;
float pwmValueCalc = 0;
int plannedValveValue = 0, pwm1ago = 0, pwm2ago = 0, pwm3ago = 0, pwm4ago = 0, pwm5ago = 0;
float pwmHist = 0;

//AutoControl switch button  ***********************************************************************************************************
byte currentState = 1;
byte reading;
byte previous = 0;

//BladeOffset stuff ************************************************************
int bladeOffsetIn = 0, bladeOffsetOut = 0;

byte bOUprevious = 0;
byte bODprevious = 0;

int leverUpValue = 0;
int leverSideValue = 0;
int LeverPushValue = 0;
int onLedTime = 0;
int autoLedTime = 0;

//*******************************************************************************

void setup() {
	delay(500);                //Small delay so serial can monitor start up
	set_arm_clock(450000000);  //Set CPU speed to 450mhz
	Serial.print("CPU speed set to: ");
	Serial.println(F_CPU_ACTUAL);

	pinMode(13, OUTPUT);

	pinMode(DIR_ENABLE, OUTPUT);
	pinMode(PWM_OUT, OUTPUT);
	//keep pulled high and drag low to activate, noise free safe
	pinMode(WORKSW_PIN, INPUT_PULLUP);
	pinMode(LEVER_UP, INPUT_DISABLE);
#ifdef bladeOffsetBtn
	pinMode(BOFFUP_PIN, INPUT_PULLUP);
	pinMode(BOFFDW_PIN, INPUT_PULLUP);
#endif
#ifdef bladeOffsetPropLever
	pinMode(LEVER_SIDE, INPUT_DISABLE);
#endif
#ifdef useLEDs
	pinMode(LED_DW, OUTPUT);
	pinMode(LED_UP, OUTPUT);
	pinMode(LED_AUTO, OUTPUT);
	pinMode(LED_ON, OUTPUT);
#endif
	//set up communication
	//Wire.begin();
	Serial.begin(115200);

	delay(500);
#ifdef isAllInOneBoard
	LEDs.init();
	LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::PWR_ON);
#endif

	ReadFromEEPROM();

	//----Teensy 4.1 Ethernet--Start---------------------

	delay(500);

	Ethernet.begin(mac, 0);  // Start Ethernet with IP 0.0.0.0

	delay(500);

	//grab the ip from EEPROM
	ip[0] = networkAddress.ipOne;
	ip[1] = networkAddress.ipTwo;
	ip[2] = networkAddress.ipThree;

	ipDestination[0] = networkAddress.ipOne;
	ipDestination[1] = networkAddress.ipTwo;
	ipDestination[2] = networkAddress.ipThree;

	Ethernet.setLocalIP(ip);  // Change IP address to IP set by user
	Serial.println("\r\nEthernet status OK");
	Serial.print("IP set Manually: ");
	Serial.println(Ethernet.localIP());

	// Udp.begin(localPort);
	NtripUdp.begin(NtripPort);

	if (Udp.begin(localPort))  // Eth_UDP.h
		LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::ETH_READY);
	else
		LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::NO_ETH);

	GPS_setup();

	//----Teensy 4.1 Ethernet--End---------------------

#ifdef isAllInOneBoard
	LEDs.set(LED_ID::STEER, STEER_STATE::AUTOSTEER_READY);
#else  //v4.5
#endif

	Serial.print(inoVersion);
	Serial.println("\r\nSetup complete, waiting for OG3D");
}
// End of Setup

void loop() {

	currentTime = millis();

	//--Main Timed Loop----------------------------------
	if (currentTime - lastTime >= LOOP_TIME) {  //200Hz
		lastTime = currentTime;
		loopTimer++;
		//If connection lost to AgOpenGPS, the watchdog will count up
		if (watchdogTimer++ > 250) watchdogTimer = 150;

		if (dataRecieved || loopTimer > 26)  //as soon as GNSS data is recieved or each 130 ms
		{
			dataRecieved = false;
			loopTimer = 0;

// On LED settings
#ifdef useLEDs
			if (settingsRecieved) {
				digitalWrite(LED_ON, HIGH);
				onLedTime = 0;
			} else {
				if (onLedTime > 19) onLedTime = 0;
				if (onLedTime < 11) digitalWrite(LED_ON, HIGH);
				else digitalWrite(LED_ON, LOW);
				onLedTime++;
			}

			// auto LED settings
			if (workSwitch == 0)  // Auto mode
			{

				if (autoEnable) {
					digitalWrite(LED_AUTO, HIGH);
					autoLedTime = 0;
				} else {
					if (autoLedTime > 7) autoLedTime = 0;
					if (autoLedTime > 3) digitalWrite(LED_AUTO, HIGH);
					else digitalWrite(LED_AUTO, LOW);
					autoLedTime++;
				}
			} else {
				digitalWrite(LED_AUTO, LOW);
				autoLedTime = 0;
			}
#endif
			//safety - turn off if confused
			if (watchdogTimer > 140) {
				workSwitch = 1;
				cutValve = 100;
				targetAltitude = 22000000;
			} else {
				//read the  work switch
				if (workButton) {
					//steer Button momentary

					reading = digitalRead(WORKSW_PIN);
					if (reading == LOW && previous == HIGH) {
						if (currentState == 1) {
							currentState = 0;
							workSwitch = 0;
						} else {
							currentState = 1;
							workSwitch = 1;
						}
					}
					previous = reading;

				} else workSwitch = digitalRead(WORKSW_PIN);  // read work switch
			}

			//read the inputs for manual blade controls
			if (manualMovePropLever) {
				//if a lever for manual operation is installed
				leverUpValue = analogRead(LEVER_UP);  //
				if (invertManMove) leverUpValue = map(leverUpValue, 0, 1023, 1023, 0);

			} else leverUpValue = 512;
				//0 lift -- 512 neutral-- 1023 lower

				//BladeOffset ************************************************
#ifdef bladeOffsetPropLever
			leverSideValue = analogRead(LEVER_SIDE);
			leverSideValue = map(leverSideValue, 0, 1023, 0, 5);
			if (invertBladeOffset) leverSideValue = map(leverSideValue, 0, 5, 5, 0);
			//0 offset down -- 2 neutral -- 4-5 offset up

			if (leverSideValue >= 4 && bOUprevious == HIGH) {
				bladeOffsetOut++;
			}
			if (leverSideValue == 0 && bOUprevious == HIGH) {
				bladeOffsetOut--;
			}
			if (leverSideValue >= 1 && leverSideValue <= 3) bOUprevious = HIGH;
			else bOUprevious = LOW;
#endif

#ifdef bladeOffsetBtn
			reading = digitalRead(BOFFUP_PIN);
			if (reading == LOW && bOUprevious == HIGH) {
				bladeOffsetOut++;
			}
			bOUprevious = reading;

			reading = digitalRead(BOFFDW_PIN);
			if (reading == LOW && bODprevious == HIGH) {
				bladeOffsetOut--;
			}
			bODprevious = reading;
#endif

			//section relays
			SetPWM();

#ifdef useLEDs
			if (pwmValue < 0) {
				digitalWrite(LED_DW, HIGH);  // lowering the blade
				digitalWrite(LED_UP, LOW);
			}
			if (pwmValue > 0) {
				digitalWrite(LED_UP, HIGH);  // lift the blade
				digitalWrite(LED_DW, LOW);
			}
			if (pwmValue == 0) {
				digitalWrite(LED_UP, LOW);
				digitalWrite(LED_DW, LOW);
			}
#endif
		}

		//send empty pgn to AgIO to show activity
		if (++helloCounter > 200) {
			Udp.beginPacket(ipDestination, AOGPort);
			Udp.write(helloAgIO, sizeof(helloAgIO));
			Udp.endPacket();
			helloCounter = 0;
		}
	}  //end of main timed loop

	//This runs continuously, outside of the timed loop, keeps checking for new udpData, etc
	//delay(1);

	//**GPS**
	Panda_GPS();
	Forward_Ntrip();

	//Check for UDP Packet
	int packetSize = Udp.parsePacket();
	if (packetSize) {
		//Serial.println("UDP Data Avalible");
		udpMessageRecv(packetSize);
	}
}  // end of main loop

//********************************************************************************

void udpMessageRecv(int sizeToRead) {
	if (sizeToRead > 128) sizeToRead = 128;
	IPAddress src_ip = Udp.remoteIP();
	Udp.read(udpData, sizeToRead);

	if (udpData[0] == 0x80 && udpData[1] == 0x81)  //Data
	{
		if (udpData[2] == 0x7F) {
			if (udpData[3] == 201) {
				//make really sure this is the subnet pgn
				if (udpData[4] == 5 && udpData[5] == 201 && udpData[6] == 201) {
					networkAddress.ipOne = udpData[7];
					networkAddress.ipTwo = udpData[8];
					networkAddress.ipThree = udpData[9];

					//save in EEPROM and restart
					EEPROM.put(60, networkAddress);
					SCB_AIRCR = 0x05FA0004;  //Teensy Reset
				}
			}  //end 201

			//Who Am I ?
			else if (udpData[3] == 202) {
				//make really sure this is the reply pgn
				if (udpData[4] == 3 && udpData[5] == 202 && udpData[6] == 202) {
					//hello from AgIO
					uint8_t scanReply[] = { 128, 129, 126, 203, 7,
						                      ip[0], ip[1], ip[2], 155, src_ip[0], src_ip[1], src_ip[2], 23 };

					//checksum
					int16_t CK_A = 0;
					for (uint8_t i = 2; i < sizeof(scanReply) - 1; i++) {
						CK_A = (CK_A + scanReply[i]);
					}
					scanReply[sizeof(scanReply) - 1] = CK_A;

					static uint8_t ipDest[] = { 255, 255, 255, 255 };
					uint16_t portDest = 9999;  //AOG port that listens

					//off to AOG
					Udp.beginPacket(ipDest, portDest);
					Udp.write(scanReply, sizeof(scanReply));
					Udp.endPacket();

					Serial.print("\r\nAdapter IP: ");
					Serial.print(src_ip[0]);
					Serial.print(" . ");
					Serial.print(src_ip[1]);
					Serial.print(" . ");
					Serial.print(src_ip[2]);
					Serial.print(" . ");
					Serial.print(src_ip[3]);

					Serial.print("\r\nModule  IP: ");
					Serial.print(ip[0]);
					Serial.print(" . ");
					Serial.print(ip[1]);
					Serial.print(" . ");
					Serial.print(ip[2]);
					Serial.print(" . ");
					Serial.print(ip[3]);
					Serial.println();

					Serial.println(inoVersion);
					Serial.println();
				}
			}  // end 202
		}    //end 7F
		else if (udpData[2] == 0x61) {
			if (udpData[3] == 0xBA)  //data from OG3D
			{
				if (blink)
					digitalWrite(13, HIGH);
				else digitalWrite(13, LOW);
				blink = !blink;
#ifdef isAllInOneBoard
				LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::AGIO_CONNECTED, true);
#endif
				//Data recieved, 0x61, 0xBA, 8, targetAltitude(32bits), cutValveReceived, bladeOffsetIn, not used, not used, CRC
				//reset watchdog
				watchdogTimer = 0;
				// Extract 32-bit signed target altitude starting at index 5 (Bytes 5, 6, 7, 8)
				memcpy(&targetAltitude, &udpData[5], sizeof(int32_t));

				// Extract remaining payload single-byte indices
				cutValveReceived = udpData[9];
				bladeOffsetIn = udpData[10];
				// udpData[11] -> not used
				// udpData[12] -> not used
				// udpData[13] -> Incoming CRC , not verified

				//Send OG3D uint8_t OG3D[] = { 0x80, 0x81, 0x6a, 0xe1, 8, multipleStatus, pwmDrive, cutValve, bladeOffsetOut, leverUpValue, leverSideValue, not used, pwmHist, 0xCC };
				OG3D[0] = 0x80;  // Preamble 1
				OG3D[1] = 0x81;  // Preamble 2
				OG3D[2] = 0x6a;  // Source address
				OG3D[3] = 0xe1;  // Response PGN ID
				OG3D[4] = 8;     // Length of payload data blocks (Bytes 5 to 12)

				// Map status bytes directly into global buffer slots
				OG3D[5] = multipleValue;
				OG3D[6] = pwmDrive;  // Single byte (int8_t or uint8_t)
				OG3D[7] = cutValve;
				OG3D[8] = bladeOffsetOut;
				OG3D[9] = leverUpValue;
				OG3D[10] = leverSideValue;
				OG3D[11] = 0;  // Not used / Padding slot
				OG3D[12] = pwmHist;

				// 3. Fast CRC checksum calculation over the global frame indexes
				uint8_t calculatedCrc = 0;
				for (int i = 2; i < 13; i++) {
					calculatedCrc += OG3D[i];
				}
				GNSS1[13] = calculatedCrc;

				// 4. Send the updated global frame via UDP
				Udp.beginPacket(ipDestination, 9999);
				Udp.write(OG3D, 14);  // We pass 14 bytes explicitly
				Udp.endPacket();
			} else if (udpData[3] == 0xB8)  //settings from OG3D
			{
				pwmGainUp = udpData[5];
				pwmGainDw = udpData[6];
				pwmMinUp = udpData[7];
				pwmMinDw = udpData[8];
				pwmMaxUp = udpData[9];
				pwmMaxDw = udpData[10];
				integralMultiplier = udpData[11];
				deadband = udpData[12];

				SaveToEEPROM();
				settingsRecieved = true;
			}
		}
	}  //end if 80 81
}  //end udp callback

void SetPWM(void) {
	if (workSwitch) autoEnable = true;           // if auto switch is tourned off turn on AutoEnable for the next time auto switch will be turned on
	if (leverUpValue < 480) autoEnable = false;  //turn off automode when lifting the blade
	if (leverUpValue > 1000) autoEnable = true;  // tur on automode when lever is fully presed for lowering the blade

	pwmValue = 0;

	//Use the GNSS height is available
	if (targetAltitude < 20000000) {
		// reel minus target plus 100. 100 is on target, <100 is too low, lift, >100 is too high, lower
		cutValve = (uint8_t)constrain(altitudeOG - targetAltitude + 100, 0, 200);
	}

	if (!workSwitch && autoEnable)  // Auto mode
	{
		if (cutValve >= (100 + deadband))  // then lower the blade
		{
			pwmValue = -((cutValve - 100 - deadband) * pwmGainDw + pwmMinDw);  //pwmValue is negative
		}
		if (cutValve <= (100 - deadband))  // then lift the blade
		{
			pwmValue = -((cutValve - 100 + deadband) * pwmGainUp - pwmMinUp);  //pwmValue is positive
		}

		if (cutValve != 100 && pwmValue != 0)  //calculate some sort of derivative
		{
			pwmHist = ((((pwm1ago) + pwm2ago + (pwm3ago) + (pwm4ago) + (pwm5ago / 2.000)) * (sq(integralMultiplier) / 100.0000)) / sq(cutValve - 100.0000));

			//put pwmHist to 0 when the blade cross the line.
			if (cutValve > 100 && (pwm1ago + pwm2ago + pwm3ago + pwm4ago + pwm5ago) > 0) pwmHist = 0;
			if (cutValve < 100 && (pwm1ago + pwm2ago + pwm3ago + pwm4ago + pwm5ago) < 0) pwmHist = 0;

			pwmValue = (pwmValue - pwmHist);
		}

		if (cutValve > 100 && pwmValue > 0) pwmValue = 0;

		if (cutValve > 100 && pwmValue < -pwmMaxDw) pwmValue = -pwmMaxDw;

		if (cutValve < 100 && pwmValue < 0) pwmValue = 0;

		if (cutValve < 100 && pwmValue > pwmMaxUp) pwmValue = pwmMaxUp;

		if (pwmValue > 0 && pwmValue < pwmMinUp) pwmValue = 0;

		if (pwmValue < 0 && pwmValue > -pwmMinDw) pwmValue = 0;

		pwmDrive = abs(pwmValue);
		plannedValveValue = cutValve;
	}     // end of automode
	else  // if manual mode
	{
		pwmDrive = 0;
		plannedValveValue = 100;

		// now give an output value by the lever
		if (leverUpValue < 480)  // lifting the blade range 480 to 0
		{
			pwmValueCalc = (((480 - leverUpValue) / 450.000 * (pwmMaxUp - pwmMinUp)) + pwmMinUp);  // (1 to 480)/450 *(pwmMaxUp-pwmMinUp)+ pwmMinUp
			pwmValue = pwmValueCalc;
			if (pwmValue > pwmMaxUp) pwmValue = pwmMaxUp;
		}
		if (leverUpValue > 540)  // lovering the blade range 540 to 1024
		{
			pwmValueCalc = ((leverUpValue - 540) / 450.000 * -(pwmMaxDw - pwmMinDw) - pwmMinDw);  // (1 to 484)/450*-(pwmMaxDw-pwmMinDw)- pwmMinDw
			pwmValue = pwmValueCalc;
			if (pwmValue < -pwmMaxDw) pwmValue = -pwmMaxDw;
		}

		pwmDrive = abs(pwmValue);
	}
	pwm5ago = pwm4ago;
	pwm4ago = pwm3ago;
	pwm3ago = pwm2ago;
	pwm2ago = pwm1ago;
	pwm1ago = pwmValue;

	if (pwmValue < 0)  // lowering the blade
	{
		digitalWrite(DIR_ENABLE, HIGH);
		//Serial.print("1,");
	} else {
		digitalWrite(DIR_ENABLE, LOW);
		//Serial.print("0,");
	}

	if (proportionalValve) analogWrite(PWM_OUT, pwmDrive);
	else {
		if (pwmDrive > 2) analogWrite(PWM_OUT, 255);
		else analogWrite(PWM_OUT, 0);
	}

	//fill byte multipleValue,
	//bit 0 is 1 if pwmValue > 0
	//bit 1 is 1 if pwmValue < 0
	//bit 2 = bool workSwitch
	//bit 3 = bool autoEnable
	multipleValue = 0;
	if (pwmDrive > 0) multipleValue |= (1 << 0);
	if (pwmDrive < 0) multipleValue |= (1 << 1);
	if (workSwitch) multipleValue |= (1 << 2);
	if (autoEnable) multipleValue |= (1 << 3);
}

void SaveToEEPROM() {
	EEPROM.update(1, pwmGainUp);
	EEPROM.update(3, pwmMinUp);
	EEPROM.update(5, pwmGainDw);
	EEPROM.update(7, pwmMinDw);
	EEPROM.update(9, pwmMaxUp);
	EEPROM.update(11, pwmMaxDw);
	EEPROM.update(13, integralMultiplier);
	EEPROM.update(15, deadband);
	EEPROM.update(17, EEP_Ident);
	EEPROM.put(60, networkAddress);
}

void ReadFromEEPROM() {
	int checkValue;
	checkValue = EEPROM.read(17);
	if (checkValue == EEP_Ident) {
		pwmGainUp = EEPROM.read(1);
		pwmMinUp = EEPROM.read(3);
		pwmGainDw = EEPROM.read(5);
		pwmMinDw = EEPROM.read(7);
		pwmMaxUp = EEPROM.read(9);
		pwmMaxDw = EEPROM.read(11);
		integralMultiplier = EEPROM.read(13);
		deadband = EEPROM.read(15);
		EEPROM.get(60, networkAddress);
	} else {  // default values
		pwmGainUp = 5;
		pwmMinUp = 70;
		pwmGainDw = 5;
		pwmMinDw = 70;
		pwmMaxUp = 180;
		pwmMaxDw = 180;
		integralMultiplier = 20;
		deadband = 5;
	}
}
