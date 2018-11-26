using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VRUI;

namespace CustomFloorPlugin
{
    class PlatformListFlowCoordinator : FlowCoordinator
    {
        PlatformUI ui;

        public PlatformListViewController _platformListViewController;
        public MainFlowCoordinator mainFlowCoordinator;
        Button _backButton;
        
        public int _selectedRow = -1;
        
        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                
                title = "Platform Select";

                ui = PlatformUI._instance;
                _platformListViewController = ui.CreateViewController<PlatformListViewController>();
                _platformListViewController.rectTransform.anchorMin = new Vector2(0.3f, 0f);
                _platformListViewController.rectTransform.anchorMax = new Vector2(0.7f, 1f);
            }
            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(_platformListViewController, null, null);
            }
            
            if (_backButton == null)
            {
                _backButton = ui.CreateBackButton(_platformListViewController.rectTransform);

                _backButton.onClick.AddListener(delegate ()
                {
                    (mainFlowCoordinator as FlowCoordinator).InvokePrivateMethod("DismissFlowCoordinator", new object[] { this , null, false });
                });
            }
            
        }

        protected override void DidDeactivate(DeactivationType type)
        {

        }
    }
}