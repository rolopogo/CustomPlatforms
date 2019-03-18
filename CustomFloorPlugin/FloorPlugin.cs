using System;
using UnityEngine;
using IllusionPlugin;
using UnityEngine.SceneManagement;
using CustomFloorPlugin.Util;

namespace CustomFloorPlugin
{
    /// <summary>
    /// An IPA IPlugin to create a PlatformLoader GameObject
    /// </summary>
    public class CustomFloorPlugin : IPlugin
    {
        public string Name => "Custom Platforms";
        public string Version => "2.7.0";

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
            BSEvents.OnLoad();
            BSEvents.menuSceneLoadedFresh += OnMenuSceneLoadedFresh;

            Application.logMessageReceived += LogCallback;
        }

        private void OnMenuSceneLoadedFresh()
        {
            if(!init){ 
                init = true;
                PlatformManager.OnLoad();
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
            BSEvents.menuSceneLoadedFresh -= OnMenuSceneLoadedFresh;
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