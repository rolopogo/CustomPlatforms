# CustomPlatforms
An IPA Plugin for BeatSaber to support custom platforms and environments

## Installation

Ensure you have the latest verion of the Beat Saber Mod Installer installed:
https://github.com/Umbranoxio/BeatSaberModInstaller/releases

Copy the contents of the "Plugin" Folderinto your "Steam\steamapps\common\Beat Saber" folder.

Your Beat Saber folder should then look like this:

```
| Beat Saber
  | Plugins
    | CustomFloorPlugin.dll     <-- 
  | CustomPlatforms		<--
    | <.plat files>		<--
  | IPA
  | Beat Saber.exe
  | (other files and folders)
```

## Controls

Press P to cycle through installed Platforms

## Adding More Platforms

Place platforms (.plat) files in the BeatSaber\CustomPlatforms folder.
Your installed platforms will be available upon relaunching the game.

## Creating New Platforms

1. Open the Unity project in the "Unity Project" folder.
The project was created and tested in version 2017.4.0, but it should work in other 2017 versions.

2. Create an empty GameObject and attach a "Custom Platform" component to it, fill out the fields
for your name and the name of the platform.

3. Create your custom platform as a child of the root object.
You can use most of the built in Unity components, custom shaders and materials, custom meshes, animators, etc.
You cannot attach custom scripts to these objects.

4. When you are finished, select the root object you attached the "Custom Platform" component to.
In the inspector, click "Export". Navigate to your CustomPlatforms folder, and press save.

5. Share your custom platform with other players by uploading the .plat file

## Support 

For questions and issues, post in the Beat Saber Mod Group discord's support channel:
https://discord.gg/UXpRn2n
Tag @Rolo for specific CustomPlatforms questions.