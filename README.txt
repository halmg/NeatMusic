1. To generate the ANN's of the modules, run Program.cs in the NeatMusic project folder. 

NOTE: After running NeatMusic the first time, it will create the bin/debug folder from where it is looking for the config.xml file. This file is situated in the NeatMusic folder and must be copied into the generated NeatMusic/bin/debug.

2. To make midi files from the generated ANN's run Program.cs in MakeMidi afterwards.

NOTE: To change song, alter CurrentSong function in MusicLibrary.cs. To change number of modules or fitness configurations, change variables in MusicEnvironment.cs. The Keyboard project folder has not been used in this project.