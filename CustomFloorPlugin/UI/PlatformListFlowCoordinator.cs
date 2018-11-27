using HMUI;
using System;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using VRUI;

namespace CustomFloorPlugin
{
    class PlatformListFlowCoordinator : FlowCoordinator
    {
        PlatformUI ui;

        public PlatformListViewController _platformListViewController;
        public MainFlowCoordinator mainFlowCoordinator;
        
        
        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                
                title = "Platform Select";

                ui = PlatformUI._instance;
                _platformListViewController = BeatSaberUI.CreateViewController<PlatformListViewController>();
                _platformListViewController.platformListBackWasPressed += Dismiss;

            }
            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(_platformListViewController, null, null);
            }
            
        }

        void Dismiss()
        {
            (mainFlowCoordinator as FlowCoordinator).InvokePrivateMethod("DismissFlowCoordinator", new object[] { this, null, false });
        }

        protected override void DidDeactivate(DeactivationType type)
        {

        }
    }
}