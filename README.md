# CustomPlatforms
An IPA Plugin for BeatSaber to support custom platforms and environments

## Installation
### Easy Method

* Ensure you have the latest verion of the Beat Saber Mod Installer installed: https://github.com/Umbranoxio/BeatSaberModInstaller/releases
* Launch the mod installer
* Select the checkbox for Custom Platforms
* Click Install

### Manual Method

* Ensure your game is patched with IPA (The mod installer does this for you)
* Extract CustomPlatformsX.Y.zip (Not the Unity Project zip!) into your Beat Saber directory
* Add .plat files to the "CustomPlatforms" directory - A few are included 

Your Beat Saber folder should then look like this:

```
| Beat Saber
  | Plugins
    | CustomPlatforms.dll     <-- 
  | CustomPlatforms		<--
    | <.plat files>		<--
  | IPA
  | Beat Saber.exe
  | (other files and folders)
```

## Controls

Press P to cycle through installed Platforms, or use the main menu option

## Adding More Platforms

Place platforms (.plat) files in the "BeatSaber\CustomPlatforms" folder.
Your installed platforms will be available upon relaunching the game.

## Creating New Platforms

There's a comprehensive guide at https://bs.assistant.moe/Platforms/ written by Emma. The following are the basic steps:

1. Download the Unity project from the releases page, unzip it.

2. Open the Unity project
The project was created and tested in version 2018.1.6f1, other versions may not be supported.

3. Create an empty GameObject and attach a "Custom Platform" component to it
Fill out the fields for your name and the name of the platform.  You can also toggle the visibility of default environment parts if you need to make room for your platform.
Add an icon for your platform by importing an image, settings it to Sprite/UI in import settings, and dragging it into the icon field of your CustomPlatform

4. Create your custom platform as a child of this root object
You can use most of the built in Unity components, custom shaders and materials, custom meshes, animators, etc.
You cannot attach your own custom scripts to these objects. Only scripts from the CustomPlatforms dll will work.

5. When you are finished, select the root object you attached the "Custom Platform" component to.
In the inspector, click "Export". Navigate to your CustomPlatforms folder, and press save.

6. Share your custom platform with other players by uploading the .plat file
