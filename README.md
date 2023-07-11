# EAVR-steam-audio

The Eindhoven Acoustic Virtual Reality (EAVR) project is part of an ongoing research in the Building Acoustics Group at the Built Environment Department of Eindhoven University of Technology (TU/e). This project is based on previous work by A. Milo and M. Hornikx [1, 2]. The goal of this project is to ... and the students can learn ...

## Releases
Working applications for Windows and Mac can be found via https://github.com/SilvinWillemsen/EAVR-steam-audio/releases/. The model that is loaded is the Trappenzaal in the Vertigo building at Eindhoven Univeristy of Technology.

## Controls

- `Right-click`: toggle the control panel

When control panel is disabled:
- `WASD`: move
- `Space bar`: jump
- `Left-click`: Select wall to change material of.
- `Shift + Left-click`: Select multiple walls.

When the control panel is enabled:
- `Choose material..`: change the material of the currently selected wall(s).
- `Room`
    - `scaling`: change the scaling of the room in x, y and z direction individually (range: x0.5-x2.0).
- `Source`
    - `< 1 >`: Select source to edit.
    - `Add source`: Adds a source based on the currently selected source.
    - `Remove source`: Removes currently selected source.
    - `Position`: Change the position of the source as a ratio of the current room dimensions (range: 0-1).
    - `Select Audio..`: Select an audio file from the dropdown list.
    - `< Gain = 0 dB >`: Increase / decrease the gain of the currently selected source.
    - `Buttons`: In order of appearance:
        - `Play`: Plays the currently selected audio file through the currently selected source.
        - `Pause`: Pauses the currently selected source. 
        - `Stop`: Stops the currently selected source.
        - `Stop (Double click)`: Stops all sources.
        - `Loop`: Loops audio played by current source.
    - `Saved observations`: Shows a list of observations which are created after pressing the `Save` button.


## 1. Setup and Usage Instructions
### 1.1. Setting up Unity and opening the project
1. Download and install **Unity Hub**: https://unity.com/download
2. Download and install **Unity version 2021.3.21f1**: https://unity.com/releases/editor/qa/lts-releases
3. **Clone** this repository to a folder you can find. 
4. From within Unity Hub **'Open' the project** by selecting the folder you just cloned.
5. Once the project opened in the correct version of Unity, **run it** (the play button in the top) to test whether it works.

### 1.2. Loading a new 3D model
1. Select the **GeometryManager** GameObject in the Hierarchy window.
2. Click on the **Load New Model** button in the Inspector window.
3. Navigate to **"Assets/Models/"** and select one of the \*.gltf files.
4. Go to **"Assets/Scenes/"** and select and load the scene SteamAudio.
5. Press play and **enjoy** your freshly loaded model!

![eavrInstructionsUnity](https://user-images.githubusercontent.com/32464520/235452874-126dcb11-d03a-4cb7-91bf-147377ab88e7.png)

## 2. Creating your own model
1. Download the latest version of **SketchUp**: https://www.sketchup.com/try-sketchup
2. Open the Extension Warehouse via **Extensions -> Extension Warehouse**.
3. Find the **glTF Export** extension by Centaur (Khronos) and install. (Can also be found here: https://extensions.sketchup.com/extension/052071e5-6c19-4f02-a7e8-fcfcc28a2fd8/gl-tf-export) 
4. **Create** a model following this video https://surfdrive.surf.nl/files/index.php/s/67aTHu1AYFVeHT0#/files_mediaviewer/CreateModelExample.mp4.
5. Export the model to .gltf using the glTF export extension and save it to somewhere you can find.
6. **Load** the model following the steps in Section 1.2.


## 3. Repository structure
The most important folders in this repository are shown below 
- root  
    - Assets
        - *Audio*
            
            A collection of audio files that are included in the application. (*Please note that adding audio files here will not add them to the application.*)
        
        - ⋮
        - *Models*
            
            A collection of .gltf files (3D models) that can be loaded into the application according to the instructions in Section 1.2.
        - ⋮
        - *Scenes*
            
            Contains the Unity scenes.
        - *Scripts*
            
            The C# scripts that form the main functionality of the application.
        - ⋮
    - ⋮

# References
[1] A. Milo and M. Hornikx, "Acoustic Virtual Reality as a Learning Framework for Built Environment Students," *Euronoise*, 2021.

[2] A. Milo, "EAVR-EDU-release," GitLab Repository. Accessed 7 July 2023. [Online] Available: https://gitlab.tue.nl/building-acoustics-tue/eavr/eavr-edu-release/-/tree/v209
