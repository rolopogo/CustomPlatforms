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

namespace CustomFloorPlugin
{
    class PlatformUI : MonoBehaviour
    {
        //private RectTransform _mainMenuRectTransform;
        //private MainMenuViewController _mainMenuViewController;
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
            try
            {
                //_buttonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "QuitButton"));
                //_backButtonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "BackArrowButton"));
                //_mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                //_mainMenuRectTransform = _buttonInstance.transform.parent as RectTransform;
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION ON AWAKE(TRY FIND BUTTONS): " + e);
            }

            CreatePlatformsButton();
            
            CreateSettingsUI();

        }

        private static void CreateSettingsUI()
        {
            
            var subMenu = SettingsUI.CreateSubMenu("Platforms");

            // Feet Icon Override
            var feetMenu = subMenu.AddBool("Always Show Feet");

            feetMenu.GetValue += delegate
            {
                return EnvironmentHider.showFeetOverride;
            };
            feetMenu.SetValue += delegate (bool value)
            {
                EnvironmentHider.showFeetOverride = value;
                PlatformLoader.Instance.HideEnvironmentForCurrentPlatform();
                ModPrefs.SetBool(CustomFloorPlugin.PluginName, "AlwaysShowFeet", EnvironmentHider.showFeetOverride);
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
            
        }

        private void CreatePlatformsButton()
        {
            MenuButtonUI.AddButton("Platforms", delegate ()
            {
                if (_platformListFlowCoordinator == null)
                {
                    _platformListFlowCoordinator = new GameObject("PlatformListFlowCoordinator").AddComponent<PlatformListFlowCoordinator>();
                    _platformListFlowCoordinator.mainFlowCoordinator = _mainFlowCoordinator;
                }
                _mainFlowCoordinator.InvokePrivateMethod("PresentFlowCoordinator", new object[] { _platformListFlowCoordinator, null, false, false });
            }
            );
        }
    }
}