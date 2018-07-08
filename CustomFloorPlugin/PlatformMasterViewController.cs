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
    class PlatformMasterViewController : VRUINavigationController
    {
        PlatformUI ui;

        public PlatformListViewController _platformListViewController;
        
        private List<LevelStaticData> _notUpdatedSongs = new List<LevelStaticData>();

        Button _backButton;
        
        public int _selectedRow = -1;
        
        protected override void DidActivate()
        {
            ui = PlatformUI._instance;

            if (_platformListViewController == null)
            {
                _platformListViewController = ui.CreateViewController<PlatformListViewController>();
                _platformListViewController.rectTransform.anchorMin = new Vector2(0.3f, 0f);
                _platformListViewController.rectTransform.anchorMax = new Vector2(0.7f, 1f);

                PushViewController(_platformListViewController, true);

            }
            else
            {
                if (_viewControllers.IndexOf(_platformListViewController) < 0)
                {
                    PushViewController(_platformListViewController, true);
                }

            }

            if (_backButton == null)
            {
                _backButton = ui.CreateBackButton(rectTransform);

                _backButton.onClick.AddListener(delegate ()
                {
                    
                    DismissModalViewController(null, false);
                    
                });
            }

            base.DidActivate();

        }

        protected override void DidDeactivate()
        {
            base.DidDeactivate();
        }
    }
}