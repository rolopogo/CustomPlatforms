using HMUI;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using VRUI;
using UnityEngine.Events;
using CustomUI.Utilities;

namespace CustomFloorPlugin
{
    class PlatformListViewController : CustomListViewController
    {
        public TextMeshProUGUI _versionNumber;

        public TableView _platformsTableView;
        LevelListTableCell _songListTableCellInstance;
        
        public override int NumberOfCells()
        {
            return PlatformManager.Instance.GetPlatforms().Length;
        }

        public override TableCell CellForIdx(int idx)
        {
            CustomPlatform platform = PlatformManager.Instance.GetPlatform(idx);

            LevelListTableCell _tableCell = Instantiate(_songListTableCellInstance);
            _tableCell.SetPrivateField("_songNameText", platform.platName);
            _tableCell.SetPrivateField("_authorText", platform.platAuthor);
            _tableCell.SetPrivateField("_coverImage", platform.icon);
            _tableCell.reuseIdentifier = "PlatformListCell";
            return _tableCell;
        }
    }
}