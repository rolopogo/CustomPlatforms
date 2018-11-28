using System;
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
        public string Version => "2.4.1";

        static CustomFloorPlugin Instance;
        private bool init = false;

        public static string PluginName
        {
            get
            {
                return Instance.Name;
            }
        }

        public void OnApplicationStart()
        {
            Instance = this;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
            // Load in the menu scene
            if (arg0.name == "Menu" && !init)
            {
                init = true;
                
                PlatformManager.OnLoad();
                BSSceneManager.OnLoad();
                
                Application.logMessageReceived += LogCallback;
            }
        }

        //Called when there is an exception
        void LogCallback(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Log) return;
            Console.WriteLine(condition);
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