# veridium

Veridium is a VR experience that allows users to view and manipulate the crystal latices of metal structures in a virtual reality scene.

# Architecture

## Tech Stack 🥞
- Unity3D (2020.3.21f1)

## Setup steps

### Prerequisites

You will need Unity Hub installed, as well as Android Development Studio. The experience is designed to run on the Oculus Quest 2. Follow [these instructions](https://circuitstream.com/blog/oculus-quest-unity-setup/) as necessary to configure the Quest (if not already configured), Android Dev Studio, and Unity Hub. However, the article is a bit outdated so only follow the instructions up to "Install and run Unity". If you do not have an Oculus developer account, you will need to follow [these instructions](https://learn.adafruit.com/sideloading-on-oculus-quest/enable-developer-mode) to create one. For configuring the build, player, and XR settings, refer to [this](https://developer.oculus.com/documentation/unity/unity-conf-settings/) instead.

### Development Setup Instructions

1. Fork or clone this project repository using git to a local space.
2. Open Unity Hub and open the project that you have cloned, if prompted, install the correct version of Unity (2020.3.2f1) to use for development.
3. In `File>Build Settings`, if the development platform is not Android, click Android and then `Switch Platform`. Follow [these instructions](https://developer.oculus.com/documentation/unity/unity-conf-settings/) to configure your build settings.

The Oculus plugin is already in the repository, and therefore does not need to be specially installed.

### Building and Running

To build, connect the Oculus Quest to the computer via USB.

If ADB (Android Development Bridge, included in Android Studio) has been properly installed, you will be able to select the device in `File/Build Settings`. From there, you may Build and Run.

If you choose just the Build option, this will create an apk file that will be saved onto your computer. Using the command line, navigate to the location where the file has been saved and type `adb install -r [yourFilename].apk`. This will load the project onto your Quest, which can be found under Unknown Sources part of your Quest library.


### Gameplay

Look towards your left and reach out towards the periodic table. Use the grip button on your Oculus controler to select an element and place it into the podium in front of you. Reach out and grab the crystal structure floating above the podium to move, rotate, resize, and manipulate it in space.

Veridium is designed for the Oculus Quest, and is best experienced with the Oculus Quest 2 headset and controllers.


## Authors
* Sayuri Magnabosco 2021 + MEM 2023, PM
* Brendan Keane 2023, designer
* Macy Toppan 2022, designer+modeler
* Andy Kotz 2024, developer
* Siddharth Hathi 2024, developer
* Julian Wu 2022, mentor

## Acknowledgments 🤝


## New Member Onboarding instructions:

 1) create a new folder where your copy of veridium will live:

```bash
cd Desktop
```
*cd means change directory and Desktop navigates the terminal to your Desktop.

2) clone the Veridium repository so a local copy lives on your machine:

git clone https://github.com/dali-lab/veridium.git

3) Change directories to the new veridium folder and checkout your own branch (so you don't accidentally edit the main code):

```bash
cd veridium
git checkout -b FIRSTINITIAL_LASTINITIAL/short-description-of-intended-changes
```
example: git checkout -b "EK/Design-Mockups" 

4)  Now you have a local copy of the project and can run the command git pull origin main in order to pull in the latest changes.

5) If you don't already have it, download [Unity Hub](https://unity3d.com/get-unity/download) and open it. Make sure you have Unity 2021.3.26f. If you don't you can download it to unity hub [here](https://unity3d.com/get-unity/download/archive)

6) In Unity hub, click `open project` and find the folder that you cloned veridium into.

7) Unity should launch the veridium project.  In the objects heirarchy navigate to the veridium scene and double click it to open.  Congratulations, you now have access to the veridium projects.



Additional Notes: 

3D modelling and design for Veridium is best done in a program called Maya, which is worth downloading as well.

To make changes to the veridium project, edit the scene in Unity as you would normally. When you're done.  Run

```bash
git add .
git commit -m "description of changes made"
git push 
```

Then go to the github repository and open a Pull request of your branch against the main branch. 



---
Designed and developed by [@DALI Lab](https://github.com/dali-lab)
