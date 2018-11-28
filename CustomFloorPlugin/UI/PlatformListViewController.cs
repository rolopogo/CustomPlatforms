using HMUI;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using VRUI;
using UnityEngine.Events;

namespace CustomFloorPlugin
{
    class PlatformListViewController : VRUIViewController, TableView.IDataSource
    {
        PlatformUI ui;

        public Button _pageUpButton;
        public Button _pageDownButton;
        public Button _backButton;
        public TextMeshProUGUI _versionNumber;

        public TableView _platformsTableView;
        LevelListTableCell _songListTableCellInstance;

        public Action platformListBackWasPressed;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    ui = PlatformUI._instance;
                    _songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));
                    
                    RectTransform container = new GameObject("PlatformsListContainer", typeof(RectTransform)).transform as RectTransform;
                    container.SetParent(rectTransform, false);
                    container.sizeDelta = new Vector2(60f, 0f);
                    
                    _platformsTableView = new GameObject("PlatformsListTableView").AddComponent<TableView>();
                    _platformsTableView.gameObject.AddComponent<RectMask2D>();
                    _platformsTableView.transform.SetParent(container, false);

                    (_platformsTableView.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
                    (_platformsTableView.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
                    (_platformsTableView.transform as RectTransform).sizeDelta = new Vector2(0f, 60f);
                    (_platformsTableView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);

                    _platformsTableView.SetPrivateField("_preallocatedCells", new TableView.CellsGroup[0]);
                    _platformsTableView.SetPrivateField("_isInitialized", false);
                    _platformsTableView.dataSource = this;

                    _platformsTableView.didSelectRowEvent += _PlatformTableView_DidSelectRowEvent;
                    
                    _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), container, false);
                    (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 30f);//-14
                    _pageUpButton.interactable = true;
                    _pageUpButton.onClick.AddListener(delegate ()
                    {
                        _platformsTableView.PageScrollUp();
                    });
                    
                    _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), container, false);
                    (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -30f);//8
                    _pageDownButton.interactable = true;
                    _pageDownButton.onClick.AddListener(delegate ()
                    {
                        _platformsTableView.PageScrollDown();
                    });
                    
                    _versionNumber = Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(x => (x.name == "Text")), rectTransform, false);

                    (_versionNumber.transform as RectTransform).anchoredPosition = new Vector2(-10f, 10f);
                    (_versionNumber.transform as RectTransform).anchorMax = new Vector2(1f, 0f);
                    (_versionNumber.transform as RectTransform).anchorMin = new Vector2(1f, 0f);

                    string versionNumber = (IllusionInjector.PluginManager.Plugins.Where(x => x.Name == "Custom Platforms").First()).Version;
                    _versionNumber.text = "v" + versionNumber;
                    _versionNumber.fontSize = 5;
                    _versionNumber.color = Color.white;

                    if (_backButton == null)
                    {
                        _backButton = BeatSaberUI.CreateBackButton(rectTransform as RectTransform);

                        _backButton.onClick.AddListener(delegate ()
                        {
                            if (platformListBackWasPressed != null) platformListBackWasPressed();
                        });
                    }
                }

                _platformsTableView.SelectRow(PlatformManager.Instance.currentPlatformIndex);
                _platformsTableView.ScrollToRow(PlatformManager.Instance.currentPlatformIndex, true);

            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION IN DidActivate: " + e);
            }
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            base.DidDeactivate(type);
        }

        public void RefreshScreen()
        {
            _platformsTableView.ReloadData();
        }
        
        private void _PlatformTableView_DidSelectRowEvent(TableView sender, int row)
        {
            PlatformManager.Instance.ChangeToPlatform(row);
        }

        public float RowHeight()
        {
            return 10f;
        }

        public int NumberOfRows()
        {
            return PlatformManager.Instance.GetPlatforms().Length;
        }

        public TableCell CellForRow(int row)
        {
            LevelListTableCell _tableCell = Instantiate(_songListTableCellInstance);

            CustomPlatform platform = PlatformManager.Instance.GetPlatform(row);

            _tableCell.songName = platform.platName;
            _tableCell.author = platform.platAuthor;
            _tableCell.coverImage = platform.icon;
            _tableCell.reuseIdentifier = "PlatformListCell";
            
            return _tableCell;
        }
    }
}