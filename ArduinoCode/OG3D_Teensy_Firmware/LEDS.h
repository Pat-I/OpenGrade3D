/*
  This is a library written for AgOpenGPS AIO PCBs with RGB faceplate LEDs
  - written by Matt Elias, 2024
  - edited by Matt Niekamp, 2025

  It uses the Adafruit_NeoPixel library to control (4) faceplate facing RGB LEDs via PCA9685

  The goal of this class is to retrieve states from other classes (rtkStatus, ethStatus, steerStatus, etc)
  and decode those values into RGB color/blink states. That way all the RGB/LED code stays in this class
  instead of strewn about all over the rest of the code. I hope this works! ;)

*/

#ifndef H_LED_H
#define H_LED_H

#include "Adafruit_PWMServoDriver.h" // https://github.com/adafruit/Adafruit-PWM-Servo-Driver-Library
//#include "core_pins.h"
//#include "elapsedMillis.h"
//#include <Wire.h>
//#include <stdint.h>
typedef enum {
  PWR_ETH,
  GPS,
  STEER,
  UNUSED,
  NUM_IDs
} LED_ID;

typedef enum {
  STAGE0_OFF,
  STAGE1_RED,
  STAGE2_RED_BLINK,
  STAGE3_GREEN_BLINK,
  STAGE4_GREEN,
  STAGE5_AMBER,
} LED_STAGE;

typedef enum {
  PWR_OFF,        // stage 0: LED OFF
  PWR_ON,         // stage 1: red solid
  NO_ETH,         // stage 2: red blinking
  ETH_READY,      // stage 3: green blinking
  AGIO_CONNECTED, // stage 4: green solid
} PWR_ETH_STATE;

typedef enum {
  ZERO,            // stage 0: LED OFF
  WAS_ERROR,       // stage 1: red solid
  WAS_READY,       // stage 2: red blinking
  AUTOSTEER_READY, // stage 3: green blinking
  AUTOSTEER_ACTIVE // stage 4: green solid
} STEER_STATE;

class LEDS {
//#define GGA_LED 13 // Teensy built-in LED
#define MAX_LED_BRIGHTNESS 255
#define MAX_LED_PWM 4095
#define LED_INVERT true

private:
  Adafruit_PWMServoDriver ledDriver = Adafruit_PWMServoDriver(0x70, Wire); // RGB instance is 0x70 unless A5 Low solder jumper is closed, then 0x50

  uint8_t mainBrightness = MAX_LED_BRIGHTNESS;     // main brightness level, 0-MAX_LED_BRIGHTNESS, controls all RGB LEDs
  uint8_t redBrightnessScale = MAX_LED_BRIGHTNESS; // sets brightness scaling, 0-MAX_LED_BRIGHTNESS, for balancing R G B elements
  uint8_t greenBrightnessScale = MAX_LED_BRIGHTNESS;
  uint8_t blueBrightnessScale = MAX_LED_BRIGHTNESS;

  // this is the structure that correlates LED ID to PCA9685 driver number
  const u_int8_t pinAssingments[LED_ID::NUM_IDs][3] = {{13, 14, 15}, // PWR_ETH (LED 1)
                                                       {5, 7, 12},   // GPS (LED 2)
                                                       {1, 0, 3},    // STEER (LED 3)
                                                       {6, 4, 2}};   // UNUSED (LED 4)

  elapsedMillis updateTimer, agioHelloTimeoutTimer, gpsUpdateTimeoutTimer;

  uint16_t updatePeriod = 500;
  bool blinkNextLoop;

  bool blueFlashState, blueFlashEnabled;
  uint16_t blueFlashPeriod = 100;
  uint32_t blueFlashStartTime;
  uint32_t blueFlashStopTime;

  const char *ledNames[4] = {
      "PWR_ETH",
      "GPS",
      "STEER",
      "UNUSED"};

public:
  struct LED_DATA {
    uint8_t stage;    // stores hierarchy of stages, bit0 - red solid, bit1 - red blink, bit2 - green blink, bit3 - green solid, highest ON bit determines color
    uint8_t redValue; // stores red value while LED is OFF during a blink stage
    uint8_t greenValue;
    uint8_t blueValue;
    // bool blinkState;
    bool blueFlash;
  };
  LED_DATA data[LED_ID::NUM_IDs];

  LEDS(void) {
  }
  LEDS(uint16_t _updatePeriod) {
    updatePeriod = _updatePeriod;
    //init();
  }

  LEDS(uint16_t _updatePeriod, uint8_t _redScale, uint8_t _greenScale, uint8_t _blueScale) {
    updatePeriod = _updatePeriod;
    redBrightnessScale = _redScale;
    greenBrightnessScale = _greenScale;
    blueBrightnessScale = _blueScale;
    //init();
  }
  ~LEDS(void) {
    allLedOff();
  }

  void init() {
    //pinMode(GGA_LED, OUTPUT);
    Serial.print("\r\nabout to Initializing RGB LED Outputs");
    delay(100);
    ledDriver.begin();

    Serial.print("\r\nInitializing RGB LED Outputs");
    Wire.beginTransmission(0x70);
    Serial.print("\r\n- RGB PCA9685 ");
    if (Wire.endTransmission() == 0)
      Serial.print("found");
    else
      Serial.print("*NOT found!*");

    Wire.setClock(1000000);
    ledDriver.setPWMFreq(120); // 120 hz should be enough to not notice, but increase if flickering
    ledDriver.setOutputMode(false); // false: open drain, true: totempole (push/pull)
    allLedOff();
    updateLoop();
  }

  void toggleTeensyLED() {
    //digitalWrite(GGA_LED, !digitalRead(GGA_LED));
  }

  void setGpsLED(uint8_t _fixState, bool _debug = false) {
    gpsUpdateTimeoutTimer = 0;

    switch (_fixState) {
      case 0: // 0: Fix not valid
        set(LED_ID::GPS, STAGE1_RED, _debug);
        return;
      case 1: // 1: GPS fix
        set(LED_ID::GPS, STAGE2_RED_BLINK, _debug);
        return;
      case 2: // 2: Differential GPS fix (DGNSS), SBAS, OmniSTAR VBS, Beacon, RTX in GVBS mode
        set(LED_ID::GPS, STAGE2_RED_BLINK, _debug);
        return;
      case 4: // 4: RTK Fixed, xFill
        set(LED_ID::GPS, STAGE4_GREEN, _debug);
        return;
      case 5:
      case 6: // 5: RTK Float, OmniSTAR XP/HP, Location RTK, RTX - 6: INS Dead reckoning
        set(LED_ID::GPS, STAGE3_GREEN_BLINK, _debug);
        return;
      case 8: // 8: Simulation mode
        set(LED_ID::GPS, STAGE4_GREEN, _debug);
        return;
      case 9:
        set(LED_ID::GPS, STAGE5_AMBER, _debug); // UDP pass through
        blueFlashEnabled = false;
        return;
      case 3:
      case 7:
      default: // 3: Not applicable, 7: Manual input mode
        set(LED_ID::GPS, STAGE0_OFF, true);
        return;
    }
  }

  void set(uint8_t _id, uint8_t _stage, bool _debug = false) {
    if (_id == LED_ID::PWR_ETH && _stage == AGIO_CONNECTED)
      agioHelloTimeoutTimer = 0;
    if (_stage == data[_id].stage)
      return;

    if (_stage == STAGE3_GREEN_BLINK || _stage == STAGE4_GREEN) {
      data[_id].redValue = 0;
      data[_id].greenValue = greenBrightnessScale;
    } else if (_stage == STAGE1_RED || _stage == STAGE2_RED_BLINK) {
      data[_id].redValue = redBrightnessScale;
      data[_id].greenValue = 0;
    } else if (_stage == STAGE5_AMBER){
      data[_id].redValue = redBrightnessScale * 0.75;
      data[_id].greenValue = greenBrightnessScale * 0.8;
    } else { // any other value, set OFF (stage: 0)
      _stage = 0;
      data[_id].redValue = 0;
      data[_id].greenValue = 0;
    }

    // if (_debug && data[_id].stage != _stage) Serial.printf("\nSetting LED:%1i from stage %1i to %1i", _id, data[_id].stage, _stage);
    if (_debug)
      Serial.printf("\nChanging %s LED from stage %1i to %1i", ledNames[_id], data[_id].stage, _stage);
    data[_id].stage = _stage;
  }

  /*!
      @brief Led update function. Contains internal timing; call as fast as possible
  */
  void updateLoop(void) {
    if (agioHelloTimeoutTimer > 5000)
      set(LED_ID::PWR_ETH, ETH_READY, true); // sets PWR__ETH LED to green_blink if AgIO Hello times out after 5s
    if (gpsUpdateTimeoutTimer > 3000)
      setGpsLED(3, true); // sets GPS LED OFF if no GPS update for 3s

    if (updateTimer > updatePeriod) {
      updateTimer = 0;

      for (byte i = 0; i < LED_ID::NUM_IDs; i++) {
        if ((data[i].stage == STAGE2_RED_BLINK || data[i].stage == STAGE3_GREEN_BLINK) && blinkNextLoop) {
          ledOff((LED_ID)i);
        } else {
          setLedColor((LED_ID)i, data[i].redValue, data[i].greenValue, 0);
        }
      }
      blinkNextLoop = !blinkNextLoop;
      if (!blinkNextLoop) { // remove "if" to additionally flash blue during ON stage of blink cycle
        blueFlashStartTime = millis() + updatePeriod / 2 - blueFlashPeriod / 2;
        blueFlashState = 0;
        blueFlashEnabled = 1;
      }
    }

    if (blueFlashEnabled) {
      if (!blueFlashState && millis() > blueFlashStartTime) {
        blueFlashState = 1;

        for (byte i = 0; i < LED_ID::NUM_IDs; i++) {
          if (data[i].blueFlash)
            setLedColor((LED_ID)i, 0, 0, blueBrightnessScale);
        }
      }

      if (blueFlashState && millis() > blueFlashStartTime + blueFlashPeriod) {
        // blueFlashState = 0;
        blueFlashEnabled = 0;

        for (byte i = 0; i < LED_ID::NUM_IDs; i++) {
          if (data[i].blueFlash) {
            // add "&& !blinkNextLoop" to additionally flash blue during ON stage of blink cycle
            if ((data[i].stage == STAGE2_RED_BLINK || data[i].stage == STAGE3_GREEN_BLINK)) { // && !blinkNextLoop) {
              ledOff((LED_ID)i);
            } else {
              setLedColor((LED_ID)i, data[i].redValue, data[i].greenValue, 0);
            }
            data[i].blueFlash = 0;
          }
        }
      }
    }
  }

  /*!
      @brief Set the brightness scale to be applied to all Leds
      @param brightness Brightness, 0-MAX_LED_BRIGHTNESS (255)
  */
  void setBrightness(uint8_t _brightness) {
    mainBrightness = _brightness;
  }

  // queue LED to flash blue at next timed cycle
  void queueBlueFlash(uint8_t _id) {
    data[_id].blueFlash = 1;
  }

  // instantly flash LED blue
  void activateBlueFlash(uint8_t _id) {
    // add code to instantly flash blue instead of waiting for next timed cycle
    data[_id].blueFlash = 1;
  }

  /*!
      @brief Turn off all Leds
  */
  void allLedOff(void) {
    // probably never deconstruct, but turn off the LEDs anyway
    for (u_int8_t outerLoop = 0; outerLoop < LED_ID::NUM_IDs; outerLoop++) {
      ledOff((LED_ID)outerLoop);
    }
  }

  /*!
      @brief Will turn off the LED specified
      @param led Led index, use LED_ID
  */
  void ledOff(LED_ID led) {
    // this function will turn off a specific LED based on argument
    for (u_int8_t innerLoop = 0; innerLoop < 3; innerLoop++) {
      // there are 3 parts to each LED R, G, and B
      ledDriver.setPin(pinAssingments[led][innerLoop], 0, LED_INVERT);
    }
  }

  /*!
   *  @brief   Set a LED's color using separate red, green and blue
   *           components.
   *  @param   led  Led index, use LED_ID
   *  @param   R  Red brightness, 0 = minimum (off), MAX_LED_PWM = maximum.
   *  @param   G  Green brightness, 0 = minimum (off), MAX_LED_PWM = maximum.
   *  @param   B  Blue brightness, 0 = minimum (off), MAX_LED_PWM = maximum.
   */
  void setLedColor(LED_ID led, u_int16_t R, u_int16_t G, u_int16_t B) {
    // to replicate the intensity scaler, we must do that before sending the PWM
    // this maintains parity with previous WS2811 functions
    u_int16_t intensity = R * MAX_LED_PWM / MAX_LED_BRIGHTNESS;
    intensity = (intensity * mainBrightness) / MAX_LED_BRIGHTNESS;
    ledDriver.setPin(pinAssingments[led][0], intensity, LED_INVERT);

    intensity = G * MAX_LED_PWM / MAX_LED_BRIGHTNESS;
    intensity = (intensity * mainBrightness) / MAX_LED_BRIGHTNESS;
    ledDriver.setPin(pinAssingments[led][1], intensity, LED_INVERT);

    intensity = B * MAX_LED_PWM / MAX_LED_BRIGHTNESS;
    intensity = (intensity * mainBrightness) / MAX_LED_BRIGHTNESS;
    ledDriver.setPin(pinAssingments[led][2], intensity, LED_INVERT);
  }

  /*  void printBinary(int var) {
      for (unsigned int mask = B1000; mask; mask >>= 1) {    // prints 4 bits/digits, set mask to the number of bits you want to print
        Serial.write(var & mask ? '1' : '0');
      }
    }*/
};
#endif