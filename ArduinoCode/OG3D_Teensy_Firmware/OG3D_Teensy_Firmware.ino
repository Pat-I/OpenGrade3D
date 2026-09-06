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
//#define bladeOffsetPropLever
//#define bladeOffsetBtn
//#define useLEDs  // LEDs using four outputs
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

#define PWM_2 5       //onboard driver
#define PWM_1 6       //onboard driver
#define SLEEP_PIN 4   // DRV Sleep pin, LOCK output
#define WORKSW_PIN 2  //PD7 this pin must be low (to ground) to activate automode IMP on PCB --Steer -PIN19
#define LEVER_UP A15  // first axle -- WAS signal -PIN 32
#ifdef bladeOffsetBtn
#define BOFFUP_PIN 3   //signal (to GND) to move the blade offset up 1 cm?
#define BOFFDW_PIN 26  //offset down
#endif
#ifdef bladeOffsetPropLever
#define LEVER_SIDE A12  // second axle, if used for blade offset
#endif
//leds
#ifdef useLEDs
#define LED_DW 16    //led down (if used)
#define LED_UP 17    //led up (if used)
#define LED_AUTO 33  //led auto
#define LED_ON 37    //on led
#endif
#else  //AiO v4.5 // pin numbers not set yet

#define DIR_ENABLE 4  //PD4 cytron dir
#define PWM_OUT 3     //PD3  cytron pwm
#define WORKSW_PIN 7  //PD7 this pin must be low (to ground) to activate automode IMP on PCB --to AiOv4 which pin?
#define LEVER_UP A1   // first axle --to AiOv4 pressure pin?
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

// if not in eeprom, overwrite
#define EEP_Ident 0x56

#include <Wire.h>
#include <EEPROM.h>
#include "zNMEAParser.h"
#ifdef isAllInOneBoard
#include "LEDS.h"
LEDS LEDs = LEDS(1000, 255, 64, 127);  // 1000ms RGB update, 255/64/127 RGB brightness balance levels for v5.0a
#endif
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
int32_t dataGNSSrecieved = 180;
byte multipleValue = 0;
//pwm variables
byte integralMultiplier = 0;
int32_t pwmGainUp = 0, pwmMinUp = 0, pwmGainDw = 0, pwmMinDw = 0, pwmMaxUp = 0, pwmMaxDw = 0;
int32_t pwmDrive = 0, pwmValue = 0;
int32_t pwmValueCalc = 0;
int32_t lastCutValve = 100;
float pwmHist = 0;

//AutoControl switch button  ***********************************************************************************************************
byte currentState = 1;
byte reading;
byte previous = 0;

//BladeOffset stuff ************************************************************
int32_t bladeOffsetIn = 0, bladeOffsetOut = 0;

byte bOUprevious = 0;
byte bODprevious = 0;

int32_t leverUpValue = 0;
int32_t leverUpCenterValue = 512;
int32_t leverSideValue = 0;
int32_t LeverPushValue = 0;
int32_t onLedTime = 0;
int32_t autoLedTime = 0;

//*******************************************************************************

void setup() {
	delay(500);                //Small delay so serial can monitor start up
	set_arm_clock(150000000);  //Set CPU speed to 150mhz
	Serial.print("CPU speed set to: ");
	Serial.println(F_CPU_ACTUAL);

	pinMode(13, OUTPUT);
#ifdef isAllInOneBoard
	pinMode(PWM_1, OUTPUT);
	pinMode(PWM_2, OUTPUT);
	pinMode(SLEEP_PIN, OUTPUT);
	analogWriteFrequency(PWM_1, 100);  // 4482 hz max (FlexPWM)
	analogWriteFrequency(PWM_2, 100);  // 4482 hz max (FlexPWM)
#else
	pinMode(DIR_ENABLE, OUTPUT);
	pinMode(PWM_OUT, OUTPUT);
#endif
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

	analogWriteResolution(12);
	delay(500);
#ifdef isAllInOneBoard
	LEDs.init();
	LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::PWR_ON);
	LEDs.updateLoop();
#endif

	ReadFromEEPROM();

	//----Teensy 4.1 Ethernet--Start---------------------

	delay(500);

	Ethernet.begin(mac, 0);  // Start Ethernet with IP 0.0.0.0
	if (manualMovePropLever) {
		// calbrate to center
		int32_t centerCal = 0;
		leverUpCenterValue = 0;
		centerCal = analogRead(LEVER_UP);
		if (invertManMove) centerCal = map(centerCal, 0, 1023, 1023, 0);
		leverUpCenterValue += centerCal;
		delay(1);
		centerCal = analogRead(LEVER_UP);
		if (invertManMove) centerCal = map(centerCal, 0, 1023, 1023, 0);
		leverUpCenterValue += centerCal;
		delay(1);
		centerCal = analogRead(LEVER_UP);
		if (invertManMove) centerCal = map(centerCal, 0, 1023, 1023, 0);
		leverUpCenterValue += centerCal;
		delay(1);
		centerCal = analogRead(LEVER_UP);
		if (invertManMove) centerCal = map(centerCal, 0, 1023, 1023, 0);
		leverUpCenterValue += centerCal;

		leverUpCenterValue = (leverUpCenterValue >> 2);
	}
	delay(200);

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
	{
#ifdef isAllInOneBoard
		LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::ETH_READY);
#endif
	} else {
#ifdef isAllInOneBoard
		LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::NO_ETH);
#endif
	}
	GPS_setup();

	//----Teensy 4.1 Ethernet--End---------------------

#ifdef isAllInOneBoard
	LEDs.set(LED_ID::STEER, STEER_STATE::AUTOSTEER_READY);
	LEDs.updateLoop();
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

#ifdef isAllInOneBoard
		LEDs.updateLoop();
#endif
		if (dataGNSSrecieved++ >= 200) {
			dataGNSSrecieved = 180;
			altitudeOG = 22000000;
		}
		//If connection lost to AgOpenGPS, the watchdog will count up
		if (watchdogTimer++ > 250) watchdogTimer = 150;

		if (dataGNSSrecieved <= 95 || (loopTimer >= 40 && watchdogTimer >= 149) || (watchdogTimer == 2 && dataGNSSrecieved >= 170))
		//the loop trigger if a GNSS posititon is received, or blade data without GNSS, or 200ms Without any data
		{
			if (dataGNSSrecieved <= 100) dataGNSSrecieved = 100;
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

			} else leverUpValue = leverUpCenterValue;
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
			SendUDPbladeData();

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
		/*
		//send empty pgn to AgIO to show activity
		if (++helloCounter > 200) {
			Udp.beginPacket(ipDestination, AOGPort);
			Udp.write(helloAgIO, sizeof(helloAgIO));
			Udp.endPacket();
			helloCounter = 0;
		}
		*/
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
				LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::AGIO_CONNECTED);
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
			} else if (udpData[3] == 0xB8)  //settings from OG3D
			{
				digitalWrite(13, HIGH);
				pwmGainUp = (int32_t)udpData[5] << 2;
				pwmGainDw = (int32_t)udpData[6] << 2;
				pwmMinUp = (int32_t)udpData[7] << 4;
				pwmMinDw = (int32_t)udpData[8] << 4;
				pwmMaxUp = (int32_t)udpData[9] << 4;
				pwmMaxDw = (int32_t)udpData[10] << 4;
				integralMultiplier = udpData[11];
				deadband = udpData[12];

				SaveToEEPROM();
				settingsRecieved = true;
			}
		}
	}  //end if 80 81
}  //end udp callback

void SetPWM(void) {
	int32_t leverCenterDeadbandUnder = leverUpCenterValue - 15;
	int32_t leverCenterDeadbandAbove = leverUpCenterValue + 15;
	if (workSwitch) autoEnable = true;                                // if auto switch is tourned off turn on AutoEnable for the next time auto switch will be turned on
	if (leverUpValue < leverCenterDeadbandUnder) autoEnable = false;  //turn off automode when lifting the blade
	if (leverUpValue > 900) autoEnable = true;                        // tur on automode when lever is fully presed for lowering the blade

	pwmValue = 0;

	//Use the GNSS height is available
	if (targetAltitude < 20000000) {
		// reel minus target plus 100. 100 is on target, <100 is too low, lift, >100 is too high, lower
		cutValve = (uint8_t)constrain(altitudeOG - targetAltitude + 100, 0, 200);
	}

	if (!workSwitch && autoEnable)  // Auto mode
	{
		// Local processing optimization variables
		int32_t error = (int32_t)cutValve - 100;  // Positive if too high, Negative if too low
		int32_t absError = abs(error);

		// 1. Calculate raw target PWM based on deadbands
		if (absError <= deadband) {
			pwmValue = 0;
		} else if (error > 0)  // Lower the blade (pwmValue is negative)
		{
			pwmValue = -((error - deadband) * pwmGainDw + pwmMinDw);
		} else  // Lift the blade (pwmValue is positive)
		{
			pwmValue = -((error + deadband) * pwmGainUp - pwmMinUp);
		}

		// 2. DAMPING AND DERIVATIVE CALCULATIONS
		if (error != 0 && pwmValue != 0) {
			// If the blade approaches the center quickly, this drops PWM to zero instantly.
			// 2. PROXIMITY-BASED DAMPING AND DERIVATIVE CALCULATIONS

			// Calculate the real physical movement velocity of the valve/blade
			// (Positive if moving up, Negative if moving down)
			int32_t valveVelocity = (int32_t)cutValve - (int32_t)lastCutValve;

			// ONLY APPLY DAMPING IF: The blade is actively moving TOWARDS the center line (100)
			// - Error > 0 (too high) AND Velocity < 0 (moving down)
			// - Error < 0 (too low)  AND Velocity > 0 (moving up)
			if (integralMultiplier > 0 && ((error > 0 && valveVelocity < 0) || (error < 0 && valveVelocity > 0))) {
				// PROXIMITY BRAKING FORMULA (Pure Integer Math):
				// We square the velocity, multiply by your 0-255 byte factor, and divide by remaining error.
				int32_t proximityBrake = (valveVelocity * valveVelocity * (int32_t)integralMultiplier) / absError;

				// Oppose the ongoing movement to soften the landing near the deadband zone
				if (error > 0) {
					pwmValue += proximityBrake;      // Reduces negative downward PWM (brings it closer to 0)
					if (pwmValue > 0) pwmValue = 0;  // Prevent braking from reversing the valve direction
				} else {
					pwmValue -= proximityBrake;      // Reduces positive upward PWM
					if (pwmValue < 0) pwmValue = 0;  // Prevent braking from reversing the valve direction
				}
			}
		}

		// 3. Ultra-fast hardware boundary clamping using native constrain rules
		if (error > 0)  // Guarding downward movements
		{
			pwmValue = constrain(pwmValue, -pwmMaxDw, 0);
			if (pwmValue > -pwmMinDw) pwmValue = 0;  // Enforce minimum pressure floor
		} else                                     // Guarding upward movements
		{
			pwmValue = constrain(pwmValue, 0, pwmMaxUp);
			if (pwmValue < pwmMinUp) pwmValue = 0;  // Enforce minimum pressure floor
		}

		pwmDrive = abs(pwmValue);
		lastCutValve = cutValve;
	} else  // Manual Mode (Lever operations)
	{
		pwmDrive = 0;
		lastCutValve = 100;

		if (leverUpValue < leverCenterDeadbandUnder)  // Lifting the blade range 480 down to 0
		{
			pwmValueCalc = (int)(((float)(leverCenterDeadbandUnder - leverUpValue) / leverCenterDeadbandUnder) * (pwmMaxUp - pwmMinUp)) + pwmMinUp;
			pwmValue = constrain(pwmValueCalc, pwmMinUp, pwmMaxUp);
		} else if (leverUpValue > leverCenterDeadbandAbove)  // Lowering the blade range 540 up to 1024
		{
			pwmValueCalc = (int)(((float)(leverUpValue - leverCenterDeadbandAbove) / (1024.0f - leverCenterDeadbandAbove)) * -(pwmMaxDw - pwmMinDw)) - pwmMinDw;
			pwmValue = constrain(pwmValueCalc, -pwmMaxDw, -pwmMinDw);
		} else {
			pwmValue = 0;
		}

		pwmDrive = abs(pwmValue);
	}
#ifdef isAllInOneBoard
	if (pwmValue == 0)  // dont move
	{
		analogWrite(PWM_2, 0);
		analogWrite(PWM_1, 0);
	} else if (pwmValue > 0)  // lift
	{
		digitalWrite(SLEEP_PIN, HIGH);
		analogWrite(PWM_2, 0);
		analogWrite(PWM_1, pwmDrive);
	} else  //lower
	{
		digitalWrite(SLEEP_PIN, HIGH);
		analogWrite(PWM_2, pwmDrive);
		analogWrite(PWM_1, 0);
	}
#else  //AiO v4.5
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
		if (pwmDrive > 2) analogWrite(PWM_OUT, 4095);
		else analogWrite(PWM_OUT, 0);
	}
#endif
	//fill byte multipleValue,
	//bit 0 is 1 if pwmValue > 0
	//bit 1 is 1 if pwmValue < 0
	//bit 2 = bool workSwitch
	//bit 3 = bool autoEnable
	multipleValue = 0;
	if (pwmValue > 0) multipleValue |= (1 << 0);
	if (pwmValue < 0) multipleValue |= (1 << 1);
	if (workSwitch) multipleValue |= (1 << 2);
	if (autoEnable) multipleValue |= (1 << 3);
}

void SaveToEEPROM() {
	EEPROM.put(4, pwmGainUp);
	EEPROM.put(8, pwmMinUp);
	EEPROM.put(12, pwmGainDw);
	EEPROM.put(16, pwmMinDw);
	EEPROM.put(20, pwmMaxUp);
	EEPROM.put(24, pwmMaxDw);
	EEPROM.update(28, integralMultiplier);
	EEPROM.update(32, deadband);
	EEPROM.update(2, EEP_Ident);
	EEPROM.put(60, networkAddress);
}

void ReadFromEEPROM() {
	int checkValue;
	checkValue = EEPROM.read(2);
	if (checkValue == EEP_Ident) {
		EEPROM.get(4, pwmGainUp);
		EEPROM.get(8, pwmMinUp);
		EEPROM.get(12, pwmGainDw);
		EEPROM.get(16, pwmMinDw);
		EEPROM.get(20, pwmMaxUp);
		EEPROM.get(24, pwmMaxDw);
		integralMultiplier = EEPROM.read(28);
		deadband = EEPROM.read(32);
		EEPROM.get(60, networkAddress);
	} else {  // default values
		pwmGainUp = 40;
		pwmMinUp = 1100;
		pwmGainDw = 40;
		pwmMinDw = 1100;
		pwmMaxUp = 2880;
		pwmMaxDw = 2880;
		integralMultiplier = 20;
		deadband = 5;
		networkAddress = { 192, 168, 5 };
	}
}

void SendUDPbladeData() {
	//Send OG3D uint8_t OG3D[] = { 0x80, 0x81, 0x6a, 0xe1, 8, multipleStatus, pwmDrive, cutValve, bladeOffsetOut, leverUpValue, leverSideValue, not used, pwmHist, 0xCC };
	OG3D[0] = 0x80;  // Preamble 1
	OG3D[1] = 0x81;  // Preamble 2
	OG3D[2] = 0x6a;  // Source address
	OG3D[3] = 0xe1;  // Response PGN ID
	OG3D[4] = 8;     // Length of payload data blocks (Bytes 5 to 12)

	// Map status bytes directly into global buffer slots
	OG3D[5] = multipleValue;
	OG3D[6] = (uint8_t)(pwmDrive >> 4);  // Single byte approximation
	OG3D[7] = cutValve;
	OG3D[8] = bladeOffsetOut;
	OG3D[9] = (uint8_t)(leverUpValue >> 2);
	OG3D[10] = leverSideValue;
	OG3D[11] = 0;  // Not used / Padding slot
	OG3D[12] = pwmHist;

	// 3. Fast CRC checksum calculation over the global frame indexes
	uint8_t calculatedCrc = 0;
	for (int i = 2; i < 13; i++) {
		calculatedCrc += OG3D[i];
	}
	OG3D[13] = calculatedCrc;

	// 4. Send the updated global frame via UDP
	Udp.beginPacket(ipDestination, 9999);
	Udp.write(OG3D, 14);  // We pass 14 bytes explicitly
	Udp.endPacket();
}