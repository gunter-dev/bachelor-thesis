# Skinwalker

Skinwalker is a Unity made 2D platformer game. It is a bachelor thesis created to finish studies on FIT BUT.

The game is available for download on https://skin-walker.itch.io/skinwalker.

## Author

Name: Lukáš Vincenc
Login: xvince01

## Building the game

In order for you to be able to play the game you need to build it first. Here are the steps to do so:

1. Open the project in Unity. For development the `2021.3.4f1` version was used.
2. On the top bar of Unity navigate to *File -> Build Settings*. A window should pop up.
3. Select the target platform. It has to be Windows, Linux or Mac. Then select your architecture. If you can't select your platform, because you don't see it, refer to the question at the bottom of this section.
4. Press *Build* or *Build And Run*. *Build* will only build the application to your selected location. *Build And Run* will launch it immediately after the build is complete.

**Why am I not able to see my target platform?**

Most probably you haven't installed the support for building to your desired platform. To do so, open *Unity Hub*, select *Installs* and locate the version you are using to open this project. Then click the gear icon on the right side of that version card. Press *Add modules* and another window should open. Here you need to select build support for your platform and install it by pressing *Continue*. Restart Unity and now you should be able to see your platform.

## Launching the game

To launch the game, you have to build it first. If you did not do so yet, refer to the steps above.

Locate the folder of the built app and open `Skinwalker.exe`. The game will launch.

## Game controls

* `A or left arrow` - run left
* `D or right arrow` - run right
* `Spacebar` - jump
* `1` - switch to default abilities - all abilities disabed
* `2` - switch to high speed ability
* `3` - switch to high jump ability
* `4` - switch to invincibility ability
* `5` - switch to night vision ability
* `Esc` - pause the game

After you start the game, the main menu will appear. There you can select `Play game` which will transfer you to a selection of prepared official levels. Then you can select `Play local level`, which will open a file selection window. When you select an image, it will be generated as a level. The `Create level` button will transfer you to a level editor.

## Creating your own levels

To create your own level, you can draw an image following the predefined rules, which you then upload to the game. The rules are described in the game itself. To read them, open the game, press `Create level` and in the top right corner press `How to?`. There you can read about how to create your own level.

When you press `Create level` and select your input image, you are transferred to a level editor. There you can play your level. If you make change to the input file and save them, you can press reload and the level will be reloaded.

## Acknowledgements

Thank you to my supervisor, Ing. Tomáš Milet, Ph.D. for giving me a valuable feedback and helping me finish this game. A special thanks goes to Filip Žáček for creating most of the textures used in the game.

Some assets were downloaded from the Unity Asset Store:

* Player animations - https://assetstore.unity.com/packages/2d/characters/ethan-the-hero-237992
* Level background textures - https://assetstore.unity.com/packages/2d/environments/hand-painted-platformer-dungeon-240848
* Footsteps sounds - https://assetstore.unity.com/packages/audio/sound-fx/foley/footsteps-essentials-189879
* Keys animation - https://drxwat.itch.io/pixel-art-key

Packages, that were used in this project:

* LibTiff - https://bitmiracle.com/libtiff/
* StandaloneFileBrowser - https://github.com/gkngkc/UnityStandaloneFileBrowser
