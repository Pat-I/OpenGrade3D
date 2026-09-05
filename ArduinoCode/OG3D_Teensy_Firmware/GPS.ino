#ifdef isAllInOneBoard
#define GPS Serial5
#define GPS_Dual Serial8
#define GPS_RTK Serial3
#define RTK_Baud 115200
#else //v4.5 ----to set to correct values
#define GPS Serial5//Serial7
#define GPS_Dual Serial8
#define GPS_RTK Serial3
#define RTK_Baud 115200
#endif
char rxbuffer[512];         //Extra serial rx buffer
char txbuffer[1023];        //Extra serial tx buffer

char rxbuffer_GPS_Dual[512];   //Extra serial rx buffer
char rxbuffer_RTK[1023];       //Extra serial rx buffer

char nmeaBuffer[200];
int count=0;
bool stringComplete = false;

int test = 0;

byte CK_A = 0;
byte CK_B = 0;
byte ackPacket[72] = { 0xB5, 0x62, 0x01, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

int relposnedByteCount = 0;

//**************************************************************

void GPS_setup()
{
GPS.begin(460800);
  GPS.addMemoryForRead(rxbuffer, 512);
  GPS.addMemoryForWrite(txbuffer, 1023);

  GPS_RTK.begin(RTK_Baud);
  GPS_RTK.addMemoryForRead(rxbuffer_RTK, 1023);

  // the dash means wildcard
  parser.setErrorHandler(errorHandler);
  parser.addHandler("G-GGA", GGA_Handler);
  parser.addHandler("G-VTG", VTG_Handler);
  //parser.addHandler("G-ZDA", ZDA_Handler);
  parser.addHandler("G-HPR", HPR_Handler);
}

//**************************************************************



//**************************************************************

void Panda_GPS()
{
    while (GPS.available())
    {
        parser << GPS.read();
    }   
}

bool calcChecksum()
{
    CK_A = 0;
    CK_B = 0;

    for (int i = 2; i < 70; i++)
    {
        CK_A = CK_A + ackPacket[i];
        CK_B = CK_B + CK_A;
    }

    return (CK_A == ackPacket[70] && CK_B == ackPacket[71]);
}

//**************************************************************



//**************************************************************

void Forward_Ntrip()
{

//Check for UDP Packet (Ntrip 2233)
    int NtripSize = NtripUdp.parsePacket();
    
    if (NtripSize) 
    {
        NtripUdp.read(NtripData, NtripSize);
        //Serial.print("Ntrip Data ="); 
        //Serial.write(NtripData, sizeof(NtripData)); 
        //Serial.write(10);
        //Serial.println("Ntrip Forwarded");
        GPS.write(NtripData, NtripSize); 
        //LEDs.queueBlueFlash(LED_ID::GPS);
    }

//Check for Radio RTK
    if (GPS_RTK.available())
    {
        GPS.write(GPS_RTK.read());
    }
}
    
//-------------------------------------------------------------------------------------------------

void clearBufferArray()
{
  /*
  for (int i=0; i<count; i++)
  {
    nmeaBuffer[i]=NULL;
    stringComplete = false;
  }
  */
  
  strcpy(nmeaBuffer, "");
  stringComplete = false;

}
     