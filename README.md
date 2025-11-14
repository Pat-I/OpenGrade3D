# OpenGrade3D
 
Update November 14 2025 V4.0.2

Discusions on Telegram: https://t.me/OpenGrade3D

OpenGrade3D is a software designed to make topographic surveys in Optisurface compatible .ags format and control a blade from an Optisurface .agd file.

Survey Mode:
To switch from grade mode to survey mode press the the bottom task bar
Only points with an RTK fix (or float if selected) will be added.

New:
Click on "Add pts from AOG elevation" and it will try to open the "elevation.txt" file from AOG that you have put in the OG field folder.
This will create an .Ags file (without boundary but still usable in Optisurface)

In survey mode a Survey.ags is created in the field folder at the end of the survey

Design Tips:

Currently points with code (in this order) MB, 2SUBZONE1 (to9)(name it SUBZONE1 (to 9) in optisurface), 2PER, 3GRD the design file (.agd) will be added OpenGrade3D

On the last version you should be able to use any heading for the "export grid heading" when creating the .agd file(in the last step after saving the file) 


Gradding Mode:

First you have to load an .agd file

Before openning the AGD file it is recommended to select the correct map display resolution (1m recommended unless fields over 20 Ha)
Go to Config/Vehicle/Display (in the bottom left)

If you press "Import AGD file" in "File" while no field open (recommended):
In Simulator Mode: the field zero easting/northing will be BenchMark position.
With real position: the field zero easting/northing will be the actual antenna position.

Position Correction in Config/Vehicle/Design Pt
Position can be corrected manually or automaticly by positionning the blade on the Master Benchmark and pressing "AUTO"








