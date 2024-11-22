
Update April 21 2022

Last ino is ValveControlPropLever.
It will save the setting to EEPROM

Check at the beginning of the ino for settings
You can set(true or false):
Proportional valve; if false Cytron will only output 0 or 255.

WorkButton --- the ino needs a signal to enter in automode, true momentory button, false continus btn

lever or swiches for manual blade control are optional. Set to true if needed.
None present:
bool manualMovePropLever = false;
bool invertManMove = false;
bool manualMoveBtn = false;

lever or swiches for manual offset control are optional. Set to true if needed.
None present:
bool bladeOffsetPropLever = false;
bool invertBladeOffset = false;
bool bladeOffsetBtn = false;




