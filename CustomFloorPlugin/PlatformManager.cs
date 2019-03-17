using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using IllusionPlugin;
using CustomFloorPlugin.Util;

namespace CustomFloorPlugin
{
    public class PlatformManager : MonoBehaviour
    {
        public static PlatformManager Instance;
        
        private EnvironmentHider menuEnvHider;
        private EnvironmentHider gameEnvHider;

        private PlatformLoader platformLoader;

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
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            EnvironmentArranger.arrangement = (EnvironmentArranger.Arrangement)ModPrefs.GetInt(CustomFloorPlugin.PluginName, "EnvironmentArrangement", 0, true);
            EnvironmentSceneOverrider.overrideMode = (EnvironmentSceneOverrider.EnvOverrideMode)ModPrefs.GetInt(CustomFloorPlugin.PluginName, "EnvironmentOverrideMode", 0, true);
            EnvironmentSceneOverrider.GetSceneInfos();
            EnvironmentSceneOverrider.OverrideEnvironmentScene();

            menuEnvHider = new EnvironmentHider();
            gameEnvHider = new EnvironmentHider();
            platformLoader = new PlatformLoader();

            BSEvents.gameSceneLoaded += HandleGameSceneLoaded;
            BSEvents.menuSceneLoaded += HandleMenuSceneLoadedFresh;
            BSEvents.menuSceneLoaded += HandleMenuSceneLoaded;
            
            RefreshPlatforms();

            HandleMenuSceneLoadedFresh();
            
            PlatformUI.OnLoad();
        }

        public void RefreshPlatforms()
        {
            if (platforms != null)
            {
                foreach (CustomPlatform platform in platforms)
                {
                    Destroy(platform.gameObject);
                }
            }
            platforms = platformLoader.CreateAllPlatforms(transform);

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
            ChangeToPlatform(platformIndex);
        }

        private void HandleGameSceneLoaded()
        {
            gameEnvHider.FindEnvironment();
            gameEnvHider.HideObjectsForPlatform(currentPlatform);

            EnvironmentArranger.RearrangeEnvironment();
            TubeLightManager.CreateAdditionalLightSwitchControllers();
            TubeLightManager.UpdateEventTubeLightList();
            SetCameraMask();
        }

        private void HandleMenuSceneLoadedFresh()
        {
            menuEnvHider.FindEnvironment();
            HandleMenuSceneLoaded();
        }

        private void HandleMenuSceneLoaded()
        {
            menuEnvHider.HideObjectsForPlatform(currentPlatform);
            SetCameraMask();
        }

        private void SetCameraMask()
        {
            Camera.main.cullingMask &= ~(1 << CameraVisibilityManager.OnlyInThirdPerson);
            Camera.main.cullingMask |= 1 << CameraVisibilityManager.OnlyInHeadset;
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
            menuEnvHider.HideObjectsForPlatform(currentPlatform);

            // Update lightSwitchEvent TubeLight references
            TubeLightManager.UpdateEventTubeLightList();
        }
    }
}