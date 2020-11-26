# OpenGrade3D
 
Update November 27 2020 V1.0.6

OpenGrade3D is a software designed to make topographic surveys in Optisurface compatible .ags format and control a blade from an Optisurface .agd file.

Survey Mode:
To switch from grade mode to survey mode press the the bottom task bar
Only points with an RTK fix (or float if selected) will be added.

In survey mode a Survey.ags is created in the field folder at the end of the survey

Currently points with code (in this order) MB, 2SUBZONE1 (to9), 2PER, 3GRD the design file (.agd) will be added OpenGrade3D


Gradding Mode:

First you have to load an .agd file

Before openning the AGD file it is recommended to select the correct map display resolution (1m recommended unless fields over 20 Ha)
Go to Config/Vehicle/Display (in the bottom left)

If you press "Import AGD file" in "File" while no field open (recommended):
In Simulator Mode: the field zero easting/northing will be BenchMark position.
With real position: the field zero easting/northing will be the actual antenna position.

Position Correction in Config/Vehicle/Design Pt
Position can be corrected manually or automaticly by positionning the blade on the Master Benchmark and pressing "AUTO"








