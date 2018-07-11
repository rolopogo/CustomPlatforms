using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;
using IllusionPlugin;

namespace CustomFloorPlugin
{
    class PlatformListViewController : VRUIViewController, TableView.IDataSource
    {
        PlatformMasterViewController _parentMasterViewController;
        PlatformUI ui;

        public Button _pageUpButton;
        public Button _pageDownButton;
        public TextMeshProUGUI _versionNumber;

        public TableView _platformsTableView;
        SongListTableCell _songListTableCellInstance;
        
        protected override void DidActivate()
        {
            ui = PlatformUI._instance;
            _parentMasterViewController = transform.parent.GetComponent<PlatformMasterViewController>();

            try
            {
                _songListTableCellInstance = Resources.FindObjectsOfTypeAll<SongListTableCell>().First(x => (x.name == "SongListTableCell"));

                if (_platformsTableView == null)
                {
                    _platformsTableView = new GameObject().AddComponent<TableView>();

                    _platformsTableView.transform.SetParent(rectTransform, false);

                    _platformsTableView.dataSource = this;

                    (_platformsTableView.transform as RectTransform).anchorMin = new Vector2(0f, 0.5f);
                    (_platformsTableView.transform as RectTransform).anchorMax = new Vector2(1f, 0.5f);
                    (_platformsTableView.transform as RectTransform).sizeDelta = new Vector2(0f, 60f);
                    (_platformsTableView.transform as RectTransform).position = new Vector3(0f, 0f, 2.4f);
                    (_platformsTableView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f); // -3

                    _platformsTableView.DidSelectRowEvent += _PlatformTableView_DidSelectRowEvent;

                }
                else
                {
                    _platformsTableView.ReloadData();
                }

                if (_pageUpButton == null)
                {
                    _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), rectTransform, false);
                    (_pageUpButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 1f);
                    (_pageUpButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 1f);
                    (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -10f);//-14
                    _pageUpButton.interactable = true;
                    _pageUpButton.onClick.AddListener(delegate ()
                    {
                        _platformsTableView.PageScrollUp();
                    });
                }

                if (_pageDownButton == null)
                {
                    _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), rectTransform, false);
                    (_pageDownButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0f);
                    (_pageDownButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0f);
                    (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 10);//8
                    _pageDownButton.interactable = true;
                    _pageDownButton.onClick.AddListener(delegate ()
                    {
                        _platformsTableView.PageScrollDown();
                    });
                }

                if(_versionNumber == null)
                {
                    _versionNumber = Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(x => (x.name == "BuildInfoText")), rectTransform, false);
                    DestroyImmediate(_versionNumber.GetComponent<BuildInfoText>());

                    (_versionNumber.transform as RectTransform).anchoredPosition = new Vector2(44f, 2f);

                    string versionNumber = (IllusionInjector.PluginManager.Plugins.Where(x => x.Name == "Custom Platforms").First()).Version;
                    _versionNumber.text = versionNumber;
                    _versionNumber.fontSize = 5;
                }

                _platformsTableView.SelectRow(PlatformLoader.Instance.GetPlatformIndex());
                _platformsTableView.ScrollToRow(PlatformLoader.Instance.GetPlatformIndex(), true);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION IN DidActivate: " + e);
            }
        }

        protected override void DidDeactivate()
        {
            base.DidDeactivate();
        }

        public void RefreshScreen()
        {
            _platformsTableView.ReloadData();
        }
        
        private void _PlatformTableView_DidSelectRowEvent(TableView sender, int row)
        {
            // swap platform
            PlatformLoader.Instance.ChangeToPlatform(row);
        }

        public float RowHeight()
        {
            return 10f;
        }

        public int NumberOfRows()
        {
            return PlatformLoader.Instance.GetPlatforms().Count;
        }

        public TableCell CellForRow(int row)
        {
            SongListTableCell _tableCell = Instantiate(_songListTableCellInstance);

            CustomPlatform platform = PlatformLoader.Instance.GetPlatform(row);

            _tableCell.songName = platform.platName;
            _tableCell.author = platform.platAuthor;
            _tableCell.coverImage = platform.icon;
            
            return _tableCell;
        }
    }
}