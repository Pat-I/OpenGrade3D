  /*
  * AOG Section Control
  */
// Last change: 2021-08-11 by Pat 12h30
// to be used with OpenGrade3D v1.1.xx
// test with blade offset
//Only cytron output
//-------------------------------------------------------------------------
//to change the deadband go a the end a change
//the value in the cutvale digital.write
//the original code is BlinkTheRelay from BrianTee AGopenGPS
//-------------------------------------------------------------------------


//Output in PROPO MODE: BLADE_DIR,PWM_VALUE,CUTVALUE


  #include "EEPROM.h"
     
  //loop time variables in milliseconds  
  const byte LOOP_TIME = 50; //20hz
  unsigned long lastTime = LOOP_TIME;
  unsigned long currentTime = LOOP_TIME;

  //Comm checks
  byte watchdogTimer = 0;      //make sure we are talking to AOG
  byte serialResetTimer = 0;   //if serial buffer is getting full, empty it
  bool settingsRecieved = false;
  //EEPROM identifier
  byte EEP_Ident = 138;

  //PWM or relay mode
  //bool proportionalValve = true; to erase
  //Deadband setting
  byte deadband = 5;
  
   //Communication with AgOpenGPS
  bool isDataFound = false, isSettingFound = false;
  int header = 0, tempHeader = 0, temp;

  //The variables used for storage
  byte relayHi=0, relayLo = 0, cutValve = 100;

  //workSwitch
  byte workSwitch = 1; //high is circuit open, low is switch grounded
  byte autoEnable = 0;

  //pwm variables
  byte pwmDrive = 0, pwmDisplay = 0, pwmGainUp = 5, pwmMinUp = 50, pwmGainDw = 5, pwmMinDw = 50, pwmMaxUp = 255, pwmMaxDw = 255, integralMultiplier = 20;
  int pwmValue = 0;
  float pwmValueCalc = 0;
  //int cutValue = 0;

  int plannedValveValue = 0, pwm1ago = 0, pwm2ago = 0, pwm3ago = 0, pwm4ago = 0, pwm5ago = 0;
  float pwmHist = 0;

  //cytron
  #define DIR_ENABLE 4 //PD4 cytron dir
  #define PWM_OUT 3 //PD3  cytron pwm

  //workswitch or work button
  #define WORKSW_PIN 7  //PD7 this pin must be low (to ground) to activate automode IMP on PCB
  bool workButton = true; // true for button, false for switch
  
  //AutoControl switch button  ***********************************************************************************************************
  byte currentState = 1;
  byte reading;
  byte previous = 0;

  //BladeOffset stuff ************************************************************
  int bladeOffsetIn = 0, bladeOffsetOut = 0;
  #define BOFFUP_PIN 8 //signal to move the blade offset up 1 mm
  #define BOFFDW_PIN 6
  bool bladeOffsetButton = true; // true if this fonctionality is used

  byte bOUprevious = 0;
  byte bODprevious = 0;

  #define LED_DW 2 //DO2 led down (if used)
  #define LED_UP 5 //DO5 led up (if used)
  #define LED_AUTO 9 //DO9 led auto
  #define LED_ON A0 //A0 on led 

  //proportional lever
  #define LEVER_UP A1 // first axle
  #define LEVER_SIDE A2 // second axle, not used
  
 int LeverUpValue = 0;
 int LeverSideValue = 0;
 int LeverPushValue = 0;
 int onLedTime = 0;
 int autoLedTime = 0;

 
  void setup()
  {
    
      TCCR2B = TCCR2B & B11111000 | B00000110;    // set timer 2 to 256 for PWM frequency of   122.55 Hz
      TCCR1B = TCCR1B & B11111000 | B00000100;    // set timer 1 to 256 for PWM frequency of   122.55 Hz 
      
      
      //set the baud rate
      Serial.begin(38400);  
    
      //set pins to output
      pinMode(DIR_ENABLE, OUTPUT);
      pinMode(LED_BUILTIN, OUTPUT);
      pinMode(LED_DW, OUTPUT);
      pinMode(LED_UP, OUTPUT);
      pinMode(LED_AUTO, OUTPUT);
      pinMode(LED_ON, OUTPUT);
      
    

    //keep pulled high and drag low to activate, noise free safe   
    pinMode(WORKSW_PIN, INPUT_PULLUP);
    pinMode(BOFFUP_PIN, INPUT_PULLUP);
    pinMode(BOFFDW_PIN, INPUT_PULLUP);

    ReadFromEEPROM();// read saved settings
 
  }

  void loop()
  {
    //Loop triggers every 50 msec (20hz) and sends back gyro heading, and roll, steer angle etc
  
    currentTime = millis();
    unsigned int time = currentTime;
  
    if (currentTime - lastTime >= LOOP_TIME)
    {
      lastTime = currentTime;
  
      //If connection lost to AgOpenGPS, the watchdog will count up 
      if (watchdogTimer++ > 250) watchdogTimer = 32;
  
      //clean out serial buffer to prevent buffer overflow
      if (serialResetTimer++ > 20)
      {
        while (Serial.available() > 0) char t = Serial.read();
        serialResetTimer = 0;
      }
      
      // On LED settings
      if (settingsRecieved) {
        digitalWrite(LED_ON, HIGH);
        onLedTime = 0;
      }
      else{
        if (onLedTime > 19) onLedTime = 0;
        if (onLedTime < 11) digitalWrite(LED_ON, HIGH);
        else digitalWrite(LED_ON, LOW);
        onLedTime++;
      }

      // auto LED settings
      if (workSwitch == 0)// Auto mode
      {
      
      if (autoEnable == 1) {
        digitalWrite(LED_AUTO, HIGH);
        autoLedTime = 0;
      }
      else{
        if (autoLedTime > 7) autoLedTime = 0;
        if (autoLedTime > 3) digitalWrite(LED_AUTO, HIGH);
        else digitalWrite(LED_AUTO, LOW);
        autoLedTime++;
      }
      }
      else {
        digitalWrite(LED_AUTO, LOW);
        autoLedTime = 0;
      }
  
      //safety - turn off if confused
      if (watchdogTimer > 30) cutValve = 100;

      if (watchdogTimer < 29){
        
      

       //read the  work switch
       if (workButton){
         //steer Button momentary
    
      reading = digitalRead(WORKSW_PIN);      
      if (reading == LOW && previous == HIGH) 
      {
        if (currentState == 1)
        {
          currentState = 0;
          workSwitch = 0;
        }
        else
        {
          currentState = 1;
          workSwitch = 1;
        }
      }      
      previous = reading;
    
       }
       else workSwitch = digitalRead(WORKSW_PIN);  // read work switch
      }
      else workSwitch = 1;

      LeverUpValue = analogRead(LEVER_UP);
      //LeverSideValue = analogRead(LEVER_SIDE); not reed
      
      //BladeOffset ************************************************
      if (bladeOffsetButton){
        reading = digitalRead(BOFFUP_PIN);      
        if (reading == LOW && bOUprevious == HIGH) 
          {
            bladeOffsetOut ++;
          }      
        bOUprevious = reading;

        reading = digitalRead(BOFFDW_PIN);      
        if (reading == LOW && bODprevious == HIGH) 
          {
            bladeOffsetOut --;
          }      
        bODprevious = reading;

        
      }
      else bladeOffsetOut = 0; // 0 mean not activated
      
      //section relays
      SetPWM();

      if (pwmValue < 0) {
        digitalWrite(LED_DW, HIGH); // lowering the blade
        digitalWrite(LED_UP, LOW);
      }
      if (pwmValue > 0) {
        digitalWrite(LED_UP, HIGH); // lift the blade
        digitalWrite(LED_DW, LOW);
      }
      if (pwmValue == 0) {
        digitalWrite(LED_UP, LOW);
        digitalWrite(LED_DW, LOW);
      }
     
    } //end of timed loop
  
    //****************************************************************************************
    //This runs continuously, outside of the timed loop, keeps checking UART for new data
          // PGN - 32762 - 127.250 0x7FFA
          //public int mdHeaderHi, mdHeaderLo = 1, cutValve = 2
          //Settind PGN - 32760 - 127.248 0x7FF8
    if (Serial.available() > 0 && !isDataFound && !isSettingFound)
    {
      int temp = Serial.read();
      header = tempHeader << 8 | temp;                //high,low bytes to make int
      tempHeader = temp;                              //save for next time
      if (header == 32762) isDataFound = true;        //Do we have a match?
      if (header == 32760) isSettingFound = true;        //Do we have a match?
    }
  
    //Data Header has been found, so the next 6 bytes are the data
    if (Serial.available() > 5 && isDataFound)
    {
      isDataFound = false;
      cutValve = Serial.read();
      bladeOffsetIn = Serial.read(); //bladeOffset value in opengrade 100 mean 0 offset.
      Serial.read(); //optOut1
      Serial.read(); //optOut2
      Serial.read(); //optOut3
      Serial.read(); //optOut4
      
      //reset watchdog
      watchdogTimer = 0;
  
      //Reset serial Watchdog  
      serialResetTimer = 0;

      //Print data to openGrade, MUST send 8 bytes!
      //valve direction,pwm value,cutvalve,blade offset,opt,opt,opt,opt

       if (pwmValue < 0) // lowering the blade
      {
      Serial.print("1,");
      }
      else Serial.print("0,");
      
      
      Serial.print(String((int)pwmDrive)+",");
      
      Serial.print(cutValve);
      Serial.print(",");
      Serial.print(bladeOffsetOut); // 100 mean no movement, 0 mean not active, in mm
      bladeOffsetOut = 100;
      Serial.print(",");
      Serial.print(LeverUpValue); //just for info, not used
      Serial.print(",");
      Serial.print(LeverSideValue); //just for info, not used
      Serial.print(",");
      Serial.print(bladeOffsetIn); //just for info, not used //(LeverPushValue);
      Serial.print(",");
      Serial.println(pwmHist); //just for info, not used
         
      Serial.flush();   // flush out buffer

      
    }

     //Setting Header has been found, so the next 8 bytes are the data
    if (Serial.available() > 7 && isSettingFound)
    {
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

    
  }

  void SetPWM(void)
  {
      if (workSwitch == 1) autoEnable = 1; // if auto switch is tourned off turn on AutoEnable for the next time auto switch will be turned on
      if (LeverUpValue < 480) autoEnable = 0; //turn off automode when lifting the blade
      if (LeverUpValue > 1000) autoEnable = 1; // tur on automode when lever is fully presed for lowering the blade
  
      pwmValue = 0;
  
      if (workSwitch == 0 && autoEnable == 1)// Auto mode
      {
        digitalWrite(LED_BUILTIN, HIGH);// led on when automode

        if (cutValve >= (100 + deadband))// then lower the blade
        {
          pwmValue = -((cutValve - 100 - deadband)*pwmGainDw + pwmMinDw);  //pwmValue is negative
        }
        if  (cutValve <= (100 - deadband)) // then lift the blade
        {
         pwmValue = -((cutValve - 100 + deadband)*pwmGainUp - pwmMinUp);  //pwmValue is positive
        }

      if (cutValve != 100 && pwmValue != 0) //calculate some sort of derivative
        {
          pwmHist = ((((pwm1ago) + pwm2ago + (pwm3ago) + (pwm4ago) + (pwm5ago/2.000))*(sq(integralMultiplier)/100.0000))/sq(cutValve-100.0000));

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
      } // end of automode
      else // if manual mode
      {
      digitalWrite(LED_BUILTIN, LOW);// led low when manual mode
      pwmDrive = 0;
      plannedValveValue = 100;

      // now give an output value by the lever
      if (LeverUpValue < 480)// lifting the blade range 480 to 0
        {
         pwmValueCalc = (((480 - LeverUpValue)/450.000 * (pwmMaxUp - pwmMinUp)) + pwmMinUp); // (1 to 480)/450 *(pwmMaxUp-pwmMinUp)+ pwmMinUp
         pwmValue = pwmValueCalc;
         if (pwmValue > pwmMaxUp) pwmValue = pwmMaxUp;
        }
      if (LeverUpValue > 540) // lovering the blade range 540 to 1024
        {
          pwmValueCalc = ((LeverUpValue - 540)/450.000*-(pwmMaxDw - pwmMinDw) - pwmMinDw); // (1 to 484)/450*-(pwmMaxDw-pwmMinDw)- pwmMinDw
          pwmValue = pwmValueCalc;
          if (pwmValue < -pwmMaxDw) pwmValue = -pwmMaxDw;
        }

      pwmDrive = abs(pwmValue);

      
      }

    if (pwmValue < 0) // lowering the blade
      {
        digitalWrite(DIR_ENABLE, HIGH);
        //Serial.print("1,");
      }
      else
      {
        digitalWrite(DIR_ENABLE, LOW); 
        //Serial.print("0,");
      }


    
      analogWrite(PWM_OUT, pwmDrive);
      
      //Serial.print(String((int)pwmDrive)+",");


      pwm5ago = pwm4ago;
      pwm4ago = pwm3ago;
      pwm3ago = pwm2ago;
      pwm2ago = pwm1ago;
      pwm1ago = pwmValue;


 
    
  }

  void SaveToEEPROM(){
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

  void ReadFromEEPROM(){
    int checkValue;
    checkValue = EEPROM.read(17);
    if(checkValue == EEP_Ident){
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
