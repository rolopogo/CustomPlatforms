using HMUI;
using IllusionPlugin;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;
using CustomUI.MenuButton;
using CustomUI.Settings;
using UnityEngine.SceneManagement;
using CustomUI.BeatSaber;

namespace CustomFloorPlugin
{
    class PlatformUI : MonoBehaviour
    {
        private MainFlowCoordinator _mainFlowCoordinator;
        
        public static PlatformUI _instance;
                
        public PlatformListFlowCoordinator _platformListFlowCoordinator;

        internal static void OnLoad()
        {
            if (_instance != null)
            {
                return;
            }
            new GameObject("PlatformUI").AddComponent<PlatformUI>();
        }

        private void Awake()
        {
            _instance = this;
            _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            DontDestroyOnLoad(gameObject);
            CreatePlatformsButton();
            CreateSettingsUI();
        }
        
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name != "Menu") return;

            _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();

            CreatePlatformsButton();
            CreateSettingsUI();
        }

        private static void CreateSettingsUI()
        {
            var subMenu = SettingsUI.CreateSubMenu("Platforms");
            
            var feetMenu = subMenu.AddBool("Always Show Feet");
            feetMenu.GetValue += delegate
            {
                return PlatformManager.Instance.envHider.showFeetOverride;
            };
            feetMenu.SetValue += delegate (bool value)
            {
                PlatformManager.Instance.envHider.showFeetOverride = value;
                ModPrefs.SetBool(CustomFloorPlugin.PluginName, "AlwaysShowFeet", PlatformManager.Instance.envHider.showFeetOverride);
            };
            
            var environment = subMenu.AddList("Environment Override", EnvironmentSceneOverrider.OverrideModes());
            environment.GetValue += delegate
            {
                return (float)EnvironmentSceneOverrider.overrideMode;
            };
            environment.SetValue += delegate (float value)
            {
                EnvironmentSceneOverrider.overrideMode = (EnvironmentSceneOverrider.EnvOverrideMode)value;
                EnvironmentSceneOverrider.OverrideEnvironmentScene();
                ModPrefs.SetInt(CustomFloorPlugin.PluginName, "EnvironmentOverrideMode", (int)EnvironmentSceneOverrider.overrideMode);
            };
            environment.FormatValue += delegate (float value) { return EnvironmentSceneOverrider.Name((EnvironmentSceneOverrider.EnvOverrideMode)value); };
            
            var arrangement = subMenu.AddList("Environment Arrangement", EnvironmentArranger.RepositionModes());
            arrangement.GetValue += delegate
            {
                return (float)EnvironmentArranger.arrangement;
            };
            arrangement.SetValue += delegate (float value)
            {
                EnvironmentArranger.arrangement = (EnvironmentArranger.Arrangement)value;
                ModPrefs.SetInt(CustomFloorPlugin.PluginName, "EnvironmentArrangement", (int)EnvironmentArranger.arrangement);
            };
            arrangement.FormatValue += delegate (float value) { return EnvironmentArranger.Name((EnvironmentArranger.Arrangement)value); };
            
        }

        private void CreatePlatformsButton()
        {
            MenuButtonUI.AddButton(
                "Platforms", 
                delegate ()
                {
                    if (_platformListFlowCoordinator == null)
                    {
                        _platformListFlowCoordinator = new GameObject("PlatformListFlowCoordinator").AddComponent<PlatformListFlowCoordinator>();
                        _platformListFlowCoordinator.mainFlowCoordinator = _mainFlowCoordinator;
                    }

                    _mainFlowCoordinator.InvokePrivateMethod( "PresentFlowCoordinator", new object[] { _platformListFlowCoordinator, null, false, false });
                }
            );
        }
    }
}