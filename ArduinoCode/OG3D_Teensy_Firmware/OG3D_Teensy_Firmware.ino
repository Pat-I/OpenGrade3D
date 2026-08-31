/*
 * 
 * 
 * 4 Feb 2021, Brian Tischler
 * Like all Arduino code - copied from somewhere else :)
 * So don't claim it as your own

Changes by Pat
-Add HPR decoding for UM982
-add analog height sensor reading as current reading
		lower voltage is active
		also need the workswitch, high voltage mean active
		pressure seting is for CANBUS workswitch
- ISO GPS position can be send, it doesn't work, need to clean the variable put to gather the isobus position




 */
//----------------------------------------------------------

// it can also be used with the AIO
// uncomment the following line if you're using the All-In-One-Board
#define isAllInOneBoard

// GPS forwarding mode: (Serial Bynav etc)
// - GPS to Serial3, Forward to AgIO via UDP
// - Forward Ntrip from AgIO (Port 2233) to Serial3

// Panda Mode
// - GPS to Serial3, Forward to AgIO as Panda via UDP
// - Forward Ntrip from AgIO (Port 2233) to Serial3
// - BNO08x Data sent with Panda data

//This CAN setup is for CANBUS based steering controllers as below:
//Danfoss PVED-CL & PVED-CLS (Claas, JCB, Massey Fergerson, CaseIH, New Holland, Valtra, Deutz, Lindner)
//Fendt SCR, S4, Gen6, FendtOne Models need Part:ACP0595080 3rd Party Steering Unlock Installed
//Late model Valtra & Massey with PVED-CC valve (Steering controller in Main Tractor ECU)
//!!Model is selected via serial monitor service tool!! (One day we will will get a CANBUS setup page in AgOpen)

//For engage & disengage via CAN or Button on PCB, select "Button" as switch option in AgOpen
//For engage via AgOpen tablet & disengage via CAN, select "None" as switch option and make sure "Remote" is on the steering wheel icon
//For engage & disengage via PCB switch only select "Switch" as switch option

//PWM value drives set curve up & down, so you need to set the PWM settings in AgOpen
//Normal settings P=15, Max=254, Low=5, Min=1 - Note: New version of AgOpen "LowPWM" is removed and "MinPWM" is used as Low for CANBUS setups (MinPWM hardcoded in .ino coded to 1)
//Some tractors have very fast valves, this smooths out the setpoint from AgOpen

//Workswitch can be operated via PCB or CAN (Will need to setup CAN Messages in ISOBUS section)
//17.09.2021 - If Pressure Sensor selected, Work switch will be operated when hitch is less than pressure setting (0-250 x 0.4 = 0-100%)
//             Note: The above is temporary use of unused variable, as one day we will get hitch % added to AgOpen
//             Note: There is a AgOpenGPS on MechanicTony GitHub with these two labels & picture changed

//Fendt K-Bus - (Not FendtOne models) Note: This also works with Claas thanks to Ryan
//Big Go/End is operated via hitch control in AgOpen
//Arduino Hitch settings must be enableded and sent to module
//"Invert Relays" Uses section 1 to trigger hitch (Again temporary)

//----------------------------------------------------------

#ifdef isAllInOneBoard
String inoVersion = ("\r\nOG3D Ver 2026.08.30 (AIO v5 Proto PCB))");
#else
String inoVersion = ("\r\nOG3D Ver 2026.08.30 (AIO v4 PCB))");
#endif

////////////////// User Settings /////////////////////////

//How many degrees before decreasing Max PWM
#define LOW_HIGH_DEGREES 3.0

/*  PWM Frequency ->
 *   490hz (default) = 0
 *   122hz = 1
 *   3921hz = 2
 */
#define PWM_Frequency 0

/////////////////////////////////////////////

// if not in eeprom, overwrite
#define EEP_Ident 0x5422

//   ***********  Motor drive connections  **************888
//Connect ground only for cytron, Connect Ground and +5v for IBT2

//Dir1 for Cytron Dir, Both L and R enable for IBT2
#define DIR1_RL_ENABLE 6  //4  //PD4

//PWM1 for Cytron PWM, Left PWM for IBT2
#define PWM1_LPWM 9  //3  //PD3

//Not Connected for Cytron, Right PWM for IBT2
#define PWM2_RPWM 4  //9 //D9

//--------------------------- Switch Input Pins ------------------------

#define STEERSW_PIN 2   //32
#define WORKSW_PIN A17  //34
#define ANAKO_PIN A12   //26
#define REMOTE_PIN 3    //37 kickout d

#define CONST_180_DIVIDED_BY_PI 57.2957795130823
#define RAD_TO_DEG_X_10 572.95779513082320876798154814105

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

//----Teensy 4.1 CANBus--Start---------------------
//#include <FlexCAN_T4.h>
//FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_256> K_Bus;    //Tractor / Control Bus
//----Teensy 4.1 CANBus--End-----------------------

//Main loop time variables in microseconds
const uint16_t LOOP_TIME = 40;  //25Hz
uint32_t lastTime = LOOP_TIME;
uint32_t currentTime = LOOP_TIME;

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

const uint16_t WATCHDOG_THRESHOLD = 100;
const uint16_t WATCHDOG_FORCE_VALUE = WATCHDOG_THRESHOLD + 2;  // Should be greater than WATCHDOG_THRESHOLD
uint8_t watchdogTimer = WATCHDOG_FORCE_VALUE;

//Parsing PGN
bool isPGNFound = false, isHeaderFound = false;
uint8_t pgn = 0, dataLength = 0, idx = 0;
int16_t tempHeader = 0;

//show life in AgIO - v5.5
uint8_t helloAgIO[] = { 0x80, 0x81, 0x7f, 0xC7, 1, 0, 0x47 };
uint8_t helloCounter = 0;

//Heart beat hello AgIO - v5.6
uint8_t helloFromAutoSteer[] = { 128, 129, 126, 126, 5, 0, 0, 0, 0, 0, 71 };
int16_t helloSteerPosition = 0;




//from OG3D board to OG3D 6A E1 -
uint8_t OG3D[] = { 0x80, 0x81, 0x6a, 0xe1, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
int16_t OG3DSize = sizeof(OG3D);

//GNSS sent to OG3D
uint8_t GNSS1[] = { 0x80, 0x81, 0x6a, 0xd1, 42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
int8_t GNSS1_Size = sizeof(GNSS1);

int64_t longitudeOG=200000000, latitudeOG=200000000; // x 1000 000 000
int32_t altitudeOG=20000000; //mm
uint16_t dualHeadingOG= 40000, singleHeadingOG=40000, imuHeadingOG=40000;  // x 100
int16_t dualRollOG=32000, imuRollOG=32000, imuPitchOG=32000, imuYawRateOG=32000, speedOG=32000, hdopOG=0, ageOG=0;  // x100, km/h x 100,
uint8_t fixQualityOG=0, satNbrOG=0;
//EEPROM
int16_t EEread = 0;



//speed sent as *10
float gpsSpeed = 0;

//*******************************************************************************

void setup() {
	delay(500);                //Small delay so serial can monitor start up
	set_arm_clock(450000000);  //Set CPU speed to 450mhz
	Serial.print("CPU speed set to: ");
	Serial.println(F_CPU_ACTUAL);

	//keep pulled high and drag low to activate, noise free safe

	//pinMode(ANAKO_PIN, INPUT_DISABLE);
	//pinMode(WORKSW_PIN, INPUT_DISABLE);
	//pinMode(STEERSW_PIN, INPUT_PULLUP);
	//pinMode(REMOTE_PIN, INPUT_PULLUP);
	//pinMode(DIR1_RL_ENABLE, OUTPUT);
	pinMode(13, OUTPUT);

	//pinMode(PWM2_RPWM, OUTPUT);

	//set up communication
	//Wire.begin();
	Serial.begin(115200);

	delay(2000);
#ifdef isAllInOneBoard
	LEDs.init();
	LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::PWR_ON);
#endif

	EEPROM.get(0, EEread);  // read identifier

	if (EEread != EEP_Ident)  // check on first start and write EEPROM
	{
		EEPROM.put(0, EEP_Ident);
		EEPROM.put(60, networkAddress);
	} else {
		//EEPROM.get(6, aogConfig);       //Machine
		//EEPROM.get(10, steerSettings);  // read the Settings
		EEPROM.get(60, networkAddress);
	}

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

	//----Teensy 4.1 CANBus--Start---------------------
	LEDs.set(LED_ID::STEER, STEER_STATE::AUTOSTEER_READY);
	//pinMode(AUTOSTEER_STANDBY_LED, LOW);
	//pinMode(AUTOSTEER_ACTIVE_LED, LOW);
	//----Teensy 4.1 CANBus--End---------------------

	Serial.print(inoVersion);
	Serial.println("\r\nSetup complete, waiting for OG3D");
}
// End of Setup

void loop() {

	currentTime = millis();

	//--Main Timed Loop----------------------------------
	if (currentTime - lastTime >= LOOP_TIME) {
		lastTime = currentTime;

		//If connection lost to AgOpenGPS, the watchdog will count up and turn off steering
		if (watchdogTimer++ > 250) watchdogTimer = WATCHDOG_FORCE_VALUE;

		//read all the switches
		if (watchdogTimer < WATCHDOG_THRESHOLD) {
			//We are good to steer
			//digitalWrite(PWM2_RPWM, 1);
		}

		//send empty pgn to AgIO to show activity
		if (++helloCounter > 10) {
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
			if (udpData[3] == 0xFE)  //254
			{


				if (blink)
					digitalWrite(13, HIGH);
				else digitalWrite(13, LOW);
				blink = !blink;

				//Serial.println(steerAngleActual);
				//--------------------------------------------------------------------------
			}

			if (udpData[3] == 200)  // Hello from AgIO
			{
#ifdef isAllInOneBoard
				LEDs.set(LED_ID::PWR_ETH, PWR_ETH_STATE::AGIO_CONNECTED, true);
#endif
			} else if (udpData[3] == 201) {
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
			} // end 202
		}  //end 7F
		else if (udpData[2] == 0x61) {
			if (udpData[3] == 0xBA)  //data from OG3D
			{


				if (blink)
					digitalWrite(13, HIGH);
				else digitalWrite(13, LOW);
				blink = !blink;

				//Send OG3D uint8_t OG3D[] = { 0x80, 0x81, 0x6a, 0xe1, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
				//--------------------------------------------------------------------------
			}
		}
	}    //end if 80 81 
}  //end udp callback
