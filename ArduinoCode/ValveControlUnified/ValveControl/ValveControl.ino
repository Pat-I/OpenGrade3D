  /*
  * AOG Section Control
  */

//-------------------------------------------------------------------------
//to change the deadband go a the end a change
//the value in the cutvale digital.write
//the original code is BlinkTheRelay from BrianTee AGopenGPS
//-------------------------------------------------------------------------

//Output in ON/OFF MODE: BLADE_UP,BLADE_DOWN,CUTVALUE
//Output in PROPO MODE: BLADE_DIR,PWM_VALUE,CUTVALUE

     
  //loop time variables in milliseconds  
  const byte LOOP_TIME = 100; //10hz
  unsigned long lastTime = LOOP_TIME;
  unsigned long currentTime = LOOP_TIME;

  //Comm checks
  byte watchdogTimer = 0;      //make sure we are talking to AOG
  byte serialResetTimer = 0;   //if serial buffer is getting full, empty it

  //PWM or relay mode
  bool proportionalValve = true;
  //Deadband setting
  byte deadband = 5;
  
   //Communication with AgOpenGPS
  bool isDataFound = false, isSettingFound = false;
  int header = 0, tempHeader = 0, temp;

  //The variables used for storage
  byte relayHi=0, relayLo = 0, cutValve = 100;

  //workSwitch
  byte workSwitch = 0;

  //pwm variables
  byte pwmDrive = 0, pwmDisplay = 0, pwmGainUp = 5, pwmMinUp = 50, pwmGainDw = 5, pwmMinDw = 50, pwmMaxUp = 255, pwmMaxDw = 255, integralMultiplier = 20;
  int pwmValue = 0;
  //int cutValue = 0;
  int plannedValveValue = 0, pwmHist = 0, pwm1ago = 0, pwm2ago = 0, pwm3ago = 0, pwm4ago = 0, pwm5ago = 0;


  #define DIR_ENABLE 4 //PD4
  #define PWM_OUT 3 //PD3
  #define WORKSW_PIN 7  //PD7

  #define R1 4
  #define R2 5
 
  void setup()
  {
    if (proportionalValve){
      TCCR2B = TCCR2B & B11111000 | B00000110;    // set timer 2 to 256 for PWM frequency of   122.55 Hz
      TCCR1B = TCCR1B & B11111000 | B00000100;    // set timer 1 to 256 for PWM frequency of   122.55 Hz 
      
      
      //set the baud rate
      Serial.begin(38400);  
    
      //set pins to output
      pinMode(DIR_ENABLE, OUTPUT);
      pinMode(LED_BUILTIN, OUTPUT);
      
    } else {
      //set the baud rate
      Serial.begin(38400);  
  
      //set pins to output
      pinMode(R1, OUTPUT);
      pinMode(R2, OUTPUT);
      pinMode(LED_BUILTIN, OUTPUT);
    }

    //keep pulled high and drag low to activate, noise free safe   
    pinMode(WORKSW_PIN, INPUT_PULLUP);
 
  }

  void loop()
  {
    //Loop triggers every 100 msec and sends back gyro heading, and roll, steer angle etc
  
    currentTime = millis();
    unsigned int time = currentTime;
  
    if (currentTime - lastTime >= LOOP_TIME)
    {
      lastTime = currentTime;
  
      //If connection lost to AgOpenGPS, the watchdog will count up 
      if (watchdogTimer++ > 250) watchdogTimer = 12;
  
      //clean out serial buffer to prevent buffer overflow
      if (serialResetTimer++ > 20)
      {
        while (Serial.available() > 0) char t = Serial.read();
        serialResetTimer = 0;
      }
  
      //safety - turn off if confused
      if (watchdogTimer > 10) cutValve = 100;
      
      //section relays
      SetRelays();
      
      Serial.print(cutValve);
      Serial.print(",");
      Serial.println(deadband);
         
      Serial.flush();   // flush out buffer
     
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
    if (Serial.available() > 0 && isDataFound)
    {
      isDataFound = false;
      cutValve = Serial.read();
      
      //reset watchdog
      watchdogTimer = 0;
  
      //Reset serial Watchdog  
      serialResetTimer = 0;
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
    }

     //read all the switches
    workSwitch = digitalRead(WORKSW_PIN);  // read work switch
    
  }

  void SetRelays(void)
  {
    if (proportionalValve)
    {
     
  
      pwmValue = 0;
  
      if (workSwitch == 0)
    {
    

    if (cutValve >= (100 + deadband))
    {
      pwmValue = -((cutValve - 100 - deadband)*pwmGainDw + pwmMinDw);  
    }
    if  (cutValve <= (100 - deadband))
    {
      pwmValue = -((cutValve - 100 + deadband)*pwmGainUp - pwmMinUp);
    }

  if (cutValve != 100 && pwmValue != 0) //if cutValve
    {
      pwmHist = ((((pwm1ago*0) + pwm2ago + (pwm3ago) + (pwm4ago*0) + (pwm5ago/8))*(sq(integralMultiplier)/100))/sq(cutValve-100));

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
    }
    else 
    {
      pwmDrive = 0;
      plannedValveValue = 100;
    }

  if (pwmValue < 0)
      {
        digitalWrite(DIR_ENABLE, HIGH);
        Serial.print("1,");
      }
      else
      {
        digitalWrite(DIR_ENABLE, LOW); 
        Serial.print("0,");
      }


    
      analogWrite(PWM_OUT, pwmDrive);
      if (pwmDrive > 0) digitalWrite(LED_BUILTIN, HIGH);
      else digitalWrite(LED_BUILTIN, LOW);
      Serial.print(String((int)pwmDrive)+",");


      pwm5ago = pwm4ago;
      pwm4ago = pwm3ago;
      pwm3ago = pwm2ago;
      pwm2ago = pwm1ago;
      pwm1ago = pwmValue;







      
    } 
    else 
    {
      //Blade position vs deadband
      if (cutValve < (100-deadband)){
        digitalWrite(R1, HIGH);
        digitalWrite(LED_BUILTIN, HIGH);
        Serial.print("1,0,");
      } else if (cutValve > (100+deadband)) {
        digitalWrite(R2, HIGH);
        digitalWrite(LED_BUILTIN, HIGH);
        Serial.print("0,1,");
      } else {
        digitalWrite(R1, LOW);
        digitalWrite(R2, LOW);
        digitalWrite(LED_BUILTIN, LOW);
        Serial.print("0,0,");
      }
    }
  }
