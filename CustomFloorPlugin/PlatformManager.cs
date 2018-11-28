using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using IllusionPlugin;

namespace CustomFloorPlugin
{
    class PlatformManager : MonoBehaviour
    {
        public static PlatformManager Instance;
        
        public EnvironmentHider envHider;

        private CustomPlatform[] platforms;
        private int platformIndex = 0;

        public static void OnLoad()
        {
            if (Instance != null) return;
            GameObject go = new GameObject("Platform Manager");
            go.AddComponent<PlatformManager>();
        }

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;

            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            platforms = new PlatformLoader().CreateAllPlatforms(transform);
            
            // Retrieve saved path from player prefs if it exists
            if (ModPrefs.HasKey(CustomFloorPlugin.PluginName, "CustomPlatformPath"))
            {
                string savedPath = ModPrefs.GetString(CustomFloorPlugin.PluginName, "CustomPlatformPath");
                // Check if this path was loaded and update our platform index
                for (int i = 0; i < platforms.Length; i++)
                {
                    if (savedPath == platforms[i].platName + platforms[i].platAuthor)
                    {
                        platformIndex = i;
                        break;
                    }
                }
            }
            
            EnvironmentSceneOverrider.overrideMode = (EnvironmentSceneOverrider.EnvOverrideMode)ModPrefs.GetInt(CustomFloorPlugin.PluginName, "EnvironmentOverrideMode", 0, true);
            EnvironmentSceneOverrider.GetSceneInfos();
            EnvironmentSceneOverrider.OverrideEnvironmentScene();

            envHider = new EnvironmentHider();
            envHider.showFeetOverride = ModPrefs.GetBool(CustomFloorPlugin.PluginName, "AlwaysShowFeet", false, true);
            envHider.FindEnvironment();
            envHider.HideObjectsForPlatform(currentPlatform);

            currentPlatform.gameObject.SetActive(true);

            PlatformUI.OnLoad();
        }

        public void OnApplicationQuit()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }
        
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            envHider.FindEnvironment();
            envHider.HideObjectsForPlatform(currentPlatform);
        }
        
        private void Update()
        {
            

            if (Input.GetKeyDown(KeyCode.P))
            {
                NextPlatform();
            }
        }

        public int currentPlatformIndex { get { return platformIndex; } }

        public CustomPlatform currentPlatform { get { return platforms[platformIndex]; } }
        
        public CustomPlatform[] GetPlatforms()
        {
            return platforms;
        }

        public CustomPlatform GetPlatform(int i)
        {
            return platforms.ElementAt(i);
        }
        public void NextPlatform()
        {
            ChangeToPlatform(platformIndex + 1);
        }

        public void PrevPlatform()
        {
            ChangeToPlatform(platformIndex - 1);
        }

        public void ChangeToPlatform(int index)
        {
            // Hide current Platform
            currentPlatform.gameObject.SetActive(false);

            // Increment index
            platformIndex = index % platforms.Length;

            // Save path into ModPrefs
            ModPrefs.SetString(CustomFloorPlugin.PluginName, "CustomPlatformPath", currentPlatform.platName + currentPlatform.platAuthor);
            
            // Show new platform
            currentPlatform.gameObject.SetActive(true);

            // Hide environment for new platform
            envHider.HideObjectsForPlatform(currentPlatform);

            // Update lightSwitchEvent TubeLight references
            TubeLightManager.UpdateEventTubeLightList();
        }
    }
}