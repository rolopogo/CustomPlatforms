using HMUI;
using IllusionPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUI;
using Image = UnityEngine.UI.Image;

namespace CustomFloorPlugin
{
    class PlatformUI : MonoBehaviour
    {
        private RectTransform _mainMenuRectTransform;
        private MainMenuViewController _mainMenuViewController;
        private MainFlowCoordinator _mainFlowCoordinator;

        private Button _buttonInstance;
        private Button _backButtonInstance;

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
                _buttonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "QuitButton"));
                _backButtonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "BackArrowButton"));
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                _mainMenuRectTransform = _buttonInstance.transform.parent as RectTransform;
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

            Button _platformsButton = CreateUIButton(_mainMenuRectTransform, "QuitButton");

            try
            {
                (_platformsButton.transform as RectTransform).anchoredPosition += new Vector2(0f, 20f);
                (_platformsButton.transform as RectTransform).sizeDelta = new Vector2(28f, 10f);

                SetButtonText(ref _platformsButton, "Platforms");
                
                _platformsButton.onClick.AddListener(delegate () {

                    try
                    {
                        if (_platformListFlowCoordinator == null)
                        {
                            _platformListFlowCoordinator = new GameObject("PlatformListFlowCoordinator").AddComponent<PlatformListFlowCoordinator>();
                            _platformListFlowCoordinator.mainFlowCoordinator = _mainFlowCoordinator;
                        }
                        _mainFlowCoordinator.InvokePrivateMethod("PresentFlowCoordinator", new object[] { _platformListFlowCoordinator, null, false, false } );

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EXCETPION IN BUTTON: " + e.Message);
                    }

                });

            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION: " + e.Message);
            }

        }
        
        public Button CreateUIButton(RectTransform parent, string buttonTemplate)
        {
            if (_buttonInstance == null)
            {
                return null;
            }

            Button btn = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == buttonTemplate)), parent, false);
            DestroyImmediate(btn.GetComponent<SignalOnUIButtonClick>());
            btn.onClick = new Button.ButtonClickedEvent();

            return btn;
        }

        public Button CreateBackButton(RectTransform parent)
        {
            if (_backButtonInstance == null)
            {
                return null;
            }

            Button _button = Instantiate(_backButtonInstance, parent, false);
            DestroyImmediate(_button.GetComponent<SignalOnUIButtonClick>());
            _button.onClick = new Button.ButtonClickedEvent();
            (_button.transform as RectTransform).anchoredPosition = new Vector2(-48f, 0f);
            return _button;
        }

        public T CreateViewController<T>() where T : VRUIViewController
        {
            T vc = new GameObject("CreatedViewController").AddComponent<T>();

            vc.rectTransform.anchorMin = new Vector2(0f, 0f);
            vc.rectTransform.anchorMax = new Vector2(1f, 1f);
            vc.rectTransform.sizeDelta = new Vector2(0f, 0f);
            vc.rectTransform.anchoredPosition = new Vector2(0f, 0f);

            return vc;
        }
        

        public void SetButtonText(ref Button _button, string _text)
        {
            if (_button.GetComponentInChildren<TextMeshProUGUI>() != null)
            {

                _button.GetComponentInChildren<TextMeshProUGUI>().text = _text;
            }

        }
        


    }
}