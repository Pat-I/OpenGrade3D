# OpenGrade3D
 

OpenGrade3D is a software designed to make topographic surveys in Optisurface compatible .ags format and control a blade from an Optisurface .agd file.

To switch from grade mode to survey mode press the the bottom task bar

In survey mode a Survey.ags is created in the field folder at the end of the survey

Currently the design file (.agd) need to be edited to open in OpenGrade3D
There must be a title line and 0s instead nothing betwean comas

ex.:
Latitude (deg), Longitude (deg), Elevation Existing(m), Elevation Proposed(m), CutFill(m), Code, Comments
53.436221602, -111.260047, 100, 100, 0, 0mb_4g
53.436241837, -111.260077106, 100, 100, 0, 2PER
53.436262072, -111.260077106, 100, 100, 0, 3GRD

instead
40.83912017, -97.13020241, 426.825,,,MB,
40.84001800, -97.13040002, 428.827, , ,2PER,
40.83626861, -97.12860586, 415.955, 415.865, -0.091, 3GRD,

fasted is to erase all others lines than 3GRD

