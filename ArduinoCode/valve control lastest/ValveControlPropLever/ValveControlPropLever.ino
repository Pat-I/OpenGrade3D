/*
  AOG Section Control
*/
char arduinoDate[] = "2025-05-22";
char arduinoVersion[] = "v 2.0.0";

// by Pat
// to be used with OpenGrade3D v1.1.xx and OpenGrade v2.2.xx
// test with blade offset
//Only cytron output
//-------------------------------------------------------------------------
//
//the original code is BlinkTheRelay from BrianTee AGopenGPS
//-------------------------------------------------------------------------


//Output in PROPO MODE: BLADE_DIR,PWM_VALUE,CUTVALUE
/*Pinout
   TX1
   RX1
   D2 led down
   D3 Cytron PWM
   D4 Cytron DIR
   D5 led up
   D6 Pin for offset down (GND to activate)
   D7 Pin to enable the automatic mode (GND to activate)
   D8 Pin for offset up (GND to activate)
   D9 led for automatic status.
   D10 pin btn up
   D11 pin btn down
   D12

   A0 On led, blinking until the config data are recieved
   A1 input for prop lever to manually control the blade
   A2 input on the second  prop lever axle for blade offset(optional)
   A3
   A4
   A5
   A6
   A7
*/

#include "EEPROM.h"
//User set variables
//PWM or relay mode
bool proportionalValve = true;
//cytron
#define DIR_ENABLE 4  //PD4 cytron dir
#define PWM_OUT 3     //PD3  cytron pwm

//workswitch or work button
bool workButton = true;  // true for momentary button, false for switch(continus)
#define WORKSW_PIN 7     //PD7 this pin must be low (to ground) to activate automode IMP on PCB

//proportional lever
bool manualMovePropLever = true;  //if a lever for manual operation is installed
bool invertManMove = false;
bool manualMoveBtn = false;
#define LEVER_UP A1    // first axle
#define BMANUP_PIN 10  //signal (to GND) to move the blade up
#define BMANDW_PIN 11

// blade off set choose betwen lever or btn or none.
bool bladeOffsetPropLever = false;
bool invertBladeOffset = false;
bool bladeOffsetBtn = false;  // true if this fonctionality is used
#define BOFFUP_PIN 8          //signal (to GND) to move the blade offset up 1 cm?
#define BOFFDW_PIN 6          //offset down
#define LEVER_SIDE A2         // second axle, if used for blade offset

//leds
#define LED_DW 2    //DO2 led down (if used)
#define LED_UP 5    //DO5 led up (if used)
#define LED_AUTO 9  //DO9 led auto
#define LED_ON A0   //A0 on led




//end of user set variables

//loop time variables in milliseconds
const byte LOOP_TIME = 2;  //500hz
unsigned long lastTime = LOOP_TIME;
unsigned long currentTime = LOOP_TIME;

//Comm checks
byte watchdogTimer = 0;  //make sure we are talking to AOG
byte loopTimer = 0;      //if serial buffer is getting full, empty it
bool settingsRecieved = false;
bool dataRecieved = false;

//EEPROM identifier
byte EEP_Ident = 138;

byte deadband = 5;

//Communication with AgOpenGPS
bool isDataFound = false, isSettingFound = false;
int header = 0, tempHeader = 0, temp;

//The variables used for storage
byte relayHi = 0, relayLo = 0, cutValve = 100;

//workSwitch
byte workSwitch = 1;  //high is circuit open, low is switch grounded
byte autoEnable = 0;

//pwm variables
byte pwmDrive = 0, pwmDisplay = 0, pwmGainUp = 5, pwmMinUp = 50, pwmGainDw = 5, pwmMinDw = 50, pwmMaxUp = 255, pwmMaxDw = 255, integralMultiplier = 20;
int pwmValue = 0;
float pwmValueCalc = 0;
//int cutValue = 0;

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


int LeverUpValue = 0;
int LeverSideValue = 0;
int LeverPushValue = 0;
int onLedTime = 0;
int autoLedTime = 0;


void setup() {
  delay(100);
  TCCR2B = TCCR2B & B11111000 | B00000110;  // set timer 2 to 256 for PWM frequency of   122.55 Hz
  TCCR1B = TCCR1B & B11111000 | B00000100;  // set timer 1 to 256 for PWM frequency of   122.55 Hz


  //set the baud rate
  Serial.begin(115200);
  delay(50);
  //set pins to output
  pinMode(DIR_ENABLE, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(LED_DW, OUTPUT);
  pinMode(LED_UP, OUTPUT);
  pinMode(LED_AUTO, OUTPUT);
  pinMode(LED_ON, OUTPUT);



  //keep pulled high and drag low to activate, noise free safe
  pinMode(WORKSW_PIN, INPUT_PULLUP);

  if (manualMoveBtn) {
    pinMode(BMANUP_PIN, INPUT_PULLUP);
    pinMode(BMANDW_PIN, INPUT_PULLUP);
  }

  if (bladeOffsetBtn) {
    pinMode(BOFFUP_PIN, INPUT_PULLUP);
    pinMode(BOFFDW_PIN, INPUT_PULLUP);
  }

  ReadFromEEPROM();  // read saved settings

  Serial.println("OG machine controler");
  Serial.println(arduinoVersion);
  Serial.println(arduinoDate);
}

void loop() {
  //Loop triggers every 2 msec (500hz)

  currentTime = millis();
  unsigned int time = currentTime;

  if (currentTime - lastTime >= LOOP_TIME) {
    lastTime = currentTime;
    loopTimer++;
    //If connection lost to AgOpenGPS, the watchdog will count up
    if (watchdogTimer++ > 250) watchdogTimer = 150;

    if (dataRecieved || loopTimer > 105)  //as soon as data is recieved or each 210 ms
    {
      dataRecieved = false;
      loopTimer = 0;

      // On LED settings
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

        if (autoEnable == 1) {
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

      //safety - turn off if confused
      if (watchdogTimer > 140) 
      {
        workSwitch = 1;
        cutValve = 100;
      }
      else
      {
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
        LeverUpValue = analogRead(LEVER_UP);  //
        if (invertManMove) LeverUpValue = map(LeverUpValue, 0, 1023, 1023, 0);
      } else if (manualMoveBtn) {
        if (digitalRead(BMANUP_PIN) == LOW) LeverUpValue = 1;
        else if (digitalRead(BMANDW_PIN) == LOW) LeverUpValue = 1022;
        else LeverUpValue = 512;
      } else LeverUpValue = 512;
      //0 lift -- 512 neutral-- 1023 lower



      //BladeOffset ************************************************
      if (bladeOffsetPropLever) {
        LeverSideValue = analogRead(LEVER_SIDE);
        LeverSideValue = map(LeverSideValue, 0, 1023, 0, 5);
        if (invertBladeOffset) LeverSideValue = map(LeverSideValue, 0, 5, 5, 0);
        //0 offset down -- 2 neutral -- 4-5 offset up

        if (LeverSideValue >= 4 && bOUprevious == HIGH) {
          bladeOffsetOut++;
        }
        if (LeverSideValue == 0 && bOUprevious == HIGH) {
          bladeOffsetOut--;
        }
        if (LeverSideValue >= 1 && LeverSideValue <= 3) bOUprevious = HIGH;
        else bOUprevious = LOW;
      }

      else if (bladeOffsetBtn) {
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


      } else bladeOffsetOut = 0;  // 0 mean not activated

      //section relays
      SetPWM();

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
    }

  }  //end of timed loop

  //****************************************************************************************
  //This runs continuously, outside of the timed loop, keeps checking UART for new data
  // PGN - 32762 - 127.250 0x7FFA
  //public int mdHeaderHi, mdHeaderLo = 1, cutValve = 2
  //Settind PGN - 32760 - 127.248 0x7FF8
  if (Serial.available() > 0 && !isDataFound && !isSettingFound) {
    int temp = Serial.read();
    header = tempHeader << 8 | temp;             //high,low bytes to make int
    tempHeader = temp;                           //save for next time
    if (header == 32762) isDataFound = true;     //Do we have a match?
    if (header == 32760) isSettingFound = true;  //Do we have a match?
  }

  //Data Header has been found, so the next 6 bytes are the data
  if (Serial.available() > 5 && isDataFound) {
    dataRecieved = true;
    isDataFound = false;
    cutValve = Serial.read();
    bladeOffsetIn = Serial.read();  //bladeOffset value in opengrade 100 mean 0 offset.
    Serial.read();                  //optOut1
    Serial.read();                  //optOut2
    Serial.read();                  //optOut3
    Serial.read();                  //optOut4

    //reset watchdog
    watchdogTimer = 0;

    //Print data to openGrade, MUST send 8 bytes!
    //valve direction,pwm value,cutvalve,blade offset,opt,opt,opt,opt

    if (pwmValue < 0)  // lowering the blade
    {
      Serial.print("1,");
    } else Serial.print("0,");


    Serial.print(String((int)pwmDrive) + ",");

    Serial.print(cutValve);
    Serial.print(",");
    Serial.print(bladeOffsetOut);  // 100 mean no movement, 0 mean not active, in mm
    bladeOffsetOut = 100;
    Serial.print(",");
    Serial.print(LeverUpValue);  //just for info, not used
    Serial.print(",");
    Serial.print(LeverSideValue);  //just for info, not used
    Serial.print(",");
    Serial.print(bladeOffsetIn);  //just for info, not used //(LeverPushValue);
    Serial.print(",");
    Serial.println(pwmHist);  //just for info, not used

    Serial.flush();  // flush out buffer
  }

  //Setting Header has been found, so the next 8 bytes are the data
  if (Serial.available() > 7 && isSettingFound) {
    isSettingFound = false;
    pwmGainUp = Serial.read();
    pwmGainDw = Serial.read();
    pwmMinUp = Serial.read();
    pwmMinDw = Serial.read();
    pwmMaxUp = Serial.read();
    pwmMaxDw = Serial.read();
    integralMultiplier = Serial.read();
    deadband = Serial.read();

    //reset watchdog
    //watchdogTimer = 0;

    //Reset serial Watchdog
    //serialResetTimer = 0;
    SaveToEEPROM();
    settingsRecieved = true;
  }

  //Flush serial, something is wrong
  if (Serial.available() > 10) {
    isSettingFound = false;
    isDataFound = false;
    while (Serial.available() > 0) char t = Serial.read();
  }
}

void SetPWM(void) {
  if (workSwitch == 1) autoEnable = 1;      // if auto switch is tourned off turn on AutoEnable for the next time auto switch will be turned on
  if (LeverUpValue < 480) autoEnable = 0;   //turn off automode when lifting the blade
  if (LeverUpValue > 1000) autoEnable = 1;  // tur on automode when lever is fully presed for lowering the blade

  pwmValue = 0;

  if (workSwitch == 0 && autoEnable == 1)  // Auto mode
  {
    digitalWrite(LED_BUILTIN, HIGH);  // led on when automode

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
    digitalWrite(LED_BUILTIN, LOW);  // led low when manual mode
    pwmDrive = 0;
    plannedValveValue = 100;

    // now give an output value by the lever
    if (LeverUpValue < 480)  // lifting the blade range 480 to 0
    {
      pwmValueCalc = (((480 - LeverUpValue) / 450.000 * (pwmMaxUp - pwmMinUp)) + pwmMinUp);  // (1 to 480)/450 *(pwmMaxUp-pwmMinUp)+ pwmMinUp
      pwmValue = pwmValueCalc;
      if (pwmValue > pwmMaxUp) pwmValue = pwmMaxUp;
    }
    if (LeverUpValue > 540)  // lovering the blade range 540 to 1024
    {
      pwmValueCalc = ((LeverUpValue - 540) / 450.000 * -(pwmMaxDw - pwmMinDw) - pwmMinDw);  // (1 to 484)/450*-(pwmMaxDw-pwmMinDw)- pwmMinDw
      pwmValue = pwmValueCalc;
      if (pwmValue < -pwmMaxDw) pwmValue = -pwmMaxDw;
    }

    pwmDrive = abs(pwmValue);
  }

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
  //Serial.print(String((int)pwmDrive)+",");


  pwm5ago = pwm4ago;
  pwm4ago = pwm3ago;
  pwm3ago = pwm2ago;
  pwm2ago = pwm1ago;
  pwm1ago = pwmValue;
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
  }
}
