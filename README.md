# seeing-is-believing

Seeing is believing is a VR experience that allows users to view and manipulate the crystal latices of metal structures in a virtual reality scene.

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

Seeing is Believing  is designed for the Oculus Quest, and is best experienced with the Oculus Quest 2 headset and controllers.


## Authors
* Sayuri Magnabosco TH, PM
* Brendan Keane 2023, designer
* Macy Toppan 2022, designer+modeler
* Andy Kotz 2024, developer
* Siddharth Hathi 2024, developer
* Julian Wu 2022, mentor

## Acknowledgments 🤝



---
Designed and developed by [@DALI Lab](https://github.com/dali-lab)