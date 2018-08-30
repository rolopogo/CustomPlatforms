using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IllusionPlugin;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    /// <summary>
    /// An IPA IPlugin to create a PlatformLoader GameObject
    /// </summary>
    public class CustomFloorPlugin : IPlugin
    {
        public string Name => "Custom Platforms";
        public string Version => "2.2.0";
        public void OnApplicationStart()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
            // Load in the menu scene
            if (arg0.name == "Menu")
            {
                BSSceneManager.OnLoad();
                PlatformLoader.OnLoad();
                //Application.logMessageReceived += LogCallback;
            }
        }

        //Called when there is an exception
        void LogCallback(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Log) return;
            Console.WriteLine(stackTrace);
        }

        public void OnApplicationQuit()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }
    }
}