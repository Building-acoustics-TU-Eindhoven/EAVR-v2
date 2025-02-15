# EAVR-v2

The Eindhoven Acoustic Virtual Reality (EAVR) Toolkit is part of an ongoing research in the Building Acoustics Group at the Built Environment Department of Eindhoven University of Technology (TU/e). This project is based on previous work by A. Milo and M. Hornikx [1, 2], and the current project (EAVR-v2) has been published at the annual Inter-Noise conference in 2024 [3]. The goal of this project is to interactively teach students principles in the field of acoustics, by changing various aspects of the VR environment. These aspects include 
- Room size
- Source position
- Listener position (by walking around the room)
- Materials of the walls
- ..etc..

## Releases
Working applications for Windows and Mac can be found via https://github.com/SilvinWillemsen/EAVR-steam-audio/releases/. The model that is loaded is the Trappenzaal in the Vertigo building at Eindhoven Univeristy of Technology.

## Prerequisites
Before following the instructions below, make sure to have git installed on your machine (download the latest version of git here: https://git-scm.com/downloads).

## Controls

- `Right-click`: toggle the control panel

When control panel is disabled:
- `WASD`: move
- `Space bar`: jump
- `Left-click`: Select wall to change material of.
- `Shift + Left-click`: Select multiple walls.

When the control panel is enabled:
- `Choose material..`: change the material of the currently selected wall(s). If no wall is selected ("Selected Wall: None"), the change in material will not have any effect.
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
4. From within Unity Hub **'Add' the project** by selecting the folder you just cloned.
    - If you encounter an error related to **git** when trying to open the project, download git (https://git-scm.com/downloads) and restart your computer.
5. Once the project opened in the correct version of Unity, in the **Project tab** navigate to **"Assets/Scenes/"** and load the **SteamAudio** scene.
6. Try to **run it** (the play button in the top) to test whether it works.
7. If it doesn't work, refresh the 3D model by loading a new model following the steps below (Section 1.2).

### 1.2. Loading a new 3D model
1. Select the **GeometryManager** GameObject in the Hierarchy window.
2. Click on the **Load New Model** button in the Inspector window.
3. Navigate to **"Assets/Models/"** and select one of the \*.gltf files.
4. Press play and **enjoy** your freshly loaded model!
5. Please refer to the controls above for instructions on how to use the application.

![eavrInstructionsUnity](https://user-images.githubusercontent.com/32464520/235452874-126dcb11-d03a-4cb7-91bf-147377ab88e7.png)

## 2. Creating your own model
1. Download the latest version of **SketchUp**: https://www.sketchup.com/try-sketchup
2. Open the Extension Warehouse via **Extensions -> Extension Warehouse**.
3. Find the **glTF Export** extension by Centaur (Khronos) and install. (Can also be found here: https://extensions.sketchup.com/extension/052071e5-6c19-4f02-a7e8-fcfcc28a2fd8/gl-tf-export) 
4. **Create** a model following this video https://www.youtube.com/watch?v=plFHHBdqQYk.
5. Export the model to .gltf using the glTF export extension and save it to somewhere you can find.
6. **Load** the model following the steps in Section 1.2.

## 3 Switching to Virtual Reality
By default the application is set to screen-based. To switch to Virtual Reality, follow these steps.

### 3.1. Making the VR headset ready for linked use (specific to Meta Quest 2)
1. Turn on the headset and create a (stationary) boundary (if you haven't already).
2. Connect the VR headset using the USB-C cable to the laptop.
3. Click on the popup **Meta Quest Link**.
    - If the popup doesn't show:
        - Click on the button showing the current time in the bottom menu bar.
        - Click on the **Quest Link** button
4. Click on the **Launch** button.
5. You should now see a white environment with a menu in front of you. The headset is now in **Quest Link** mode.
    - If this takes longer than 10 seconds (or you get kicked back to the main menu) restart the headset.

### 3.2 Making EAVR-v2 ready for use with VR
1. Select the **Toggle VR** GameObject in the Unity hierarchy. 
2. In the inspector, set the dropdown menu to **VR**.
3. Follow the steps in Section 3.1 if you have not done so yet.
4. Run the application by clicking on the play button.
5. The scene should now be visible in the VR headset.
    - If it does not work somehow, go to **Edit -> Project settings... -> XR Plugin Management** and make sure that **Initialize XR on Startup** is active. Restart Unity and try again.

6. The user interface (with the buttons) is only visible on the screen; the person in VR will not see this.


## 4. Repository structure
The most important folders in this repository are shown below 
- root  
    - Assets
        - *Models*
            
            A collection of .gltf files (3D models) that can be loaded into the application according to the instructions in Section 1.2.
        - ⋮
        - *Resources/Audio*

            A collection of audio files that are included in the application. Your own custom audio files can be added here.
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

[3] S. Willemsen and M. Hornikx, "The Eindhoven Acoustic Virtual Reality (EAVR) toolkit in education and beyond," in *INTER-NOISE and NOISE-CON Congress and Conference Proceedings*, 2024.

# Appendix A: Material List

Below is a table of valid material names that you can use to tag groups in SketchUp. The values given are absorption coefficients (0.0 - 1.0) of three frequency bands (their center frequencies indicated).

| Material name | Low absorption (400 Hz) | Mid absorption (2500 Hz) | High absorption (15000 Hz) |
| --- | --- | --- | --- |
| Brick | 0.03 | 0.04 | 0.07 |
| Carpet | 0.24 | 0.69 | 0.73 |
| Ceramic | 0.01 | 0.01 | 0.02 |
| Concrete | 0.05 | 0.07 | 0.08 |
| Default | 0.1 | 0.2 | 0.3 |
| Glass | 0.06 | 0.03 | 0.02 |
| Gravel | 0.6 | 0.7 | 0.8 |
| Metal | 0.2 | 0.07 | 0.06 |
| NoAbsorption | 0.0 | 0.0 | 0.0 |
| Plaster | 0.12 | 0.06 | 0.04 |
| Rock | 0.13 | 0.2 | 0.24 |
| Transparent | 1.0 | 1.0 | 1.0 | 
| Wood | 0.11 | 0.07 | 0.06 |
