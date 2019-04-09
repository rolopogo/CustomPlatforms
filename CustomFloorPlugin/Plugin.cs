using UnityEngine;
using IPA;
using UnityEngine.SceneManagement;
using CustomFloorPlugin.Util;

namespace CustomFloorPlugin
{
    public class Plugin : IBeatSaberPlugin
    {
        public static BS_Utils.Utilities.Config config;
        public static IPA.Logging.Logger logger;
        private bool init = false;

        public void Init(object thisWillBeNull, IPA.Logging.Logger logger)
        {
            Plugin.logger = logger;
        }

        public void OnApplicationStart()
        {
            //Instance = this;
            BSEvents.OnLoad();
            BSEvents.menuSceneLoadedFresh += OnMenuSceneLoadedFresh;
        }

        private void OnMenuSceneLoadedFresh()
        {
            if(!init){ 
                init = true;
                config = new BS_Utils.Utilities.Config("Custom Platforms");
                PlatformManager.OnLoad();
            }
        }

        public void OnApplicationQuit()
        {
            BSEvents.menuSceneLoadedFresh -= OnMenuSceneLoadedFresh;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }

        public void OnSceneUnloaded(Scene scene) { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) { }

        public void OnUpdate() { }

        public void OnFixedUpdate() { }
    }
}