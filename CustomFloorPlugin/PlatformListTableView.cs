using System;
using System.Diagnostics;
using HMUI;
using UnityEngine;

namespace CustomFloorPlugin
{
    public class PlatformListTableView : MonoBehaviour, TableView.IDataSource
    {
        public event Action<PlatformListTableView, int> platformListTableViewDidSelectRow;

        public TableView _tableView;

        public SongListTableCell _cellPrefab;

        protected float _cellHeight = 12f;

        protected const string kCellIdentifier = "Cell";

        protected CustomPlatform[] _platforms;

        public virtual void Start()
        {
            this._tableView.DidSelectRowEvent += this.HandleDidSelectRowEvent;
        }

        public float RowHeight()
        {
            return this._cellHeight;
        }

        public int NumberOfRows()
        {
            if (this._platforms == null)
            {
                return 0;
            }
            return this._platforms.Length;
        }

        public TableCell CellForRow(int row)
        {
            Console.WriteLine("CellForRow " + row);
            SongListTableCell songListTableCell = this._tableView.DequeueReusableCellForIdentifier("PlatformCell") as SongListTableCell;
            try
            {
                if (songListTableCell == null)
                {
                    songListTableCell = UnityEngine.Object.Instantiate<SongListTableCell>(this._cellPrefab);
                    songListTableCell.reuseIdentifier = "PlatformCell";
                }

                CustomPlatform platform = _platforms[row];
                songListTableCell.songName = platform.platName;
                songListTableCell.author = platform.platAuthor;
                if (platform.icon != null) songListTableCell.coverImage = platform.icon;
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return songListTableCell;
        }

        public virtual void HandleDidSelectRowEvent(TableView tableView, int row)
        {
            if (platformListTableViewDidSelectRow != null)
            {
                platformListTableViewDidSelectRow(this, row);
            }
        }

        public virtual void SetPlatforms(CustomPlatform[] platforms)
        {
            _platforms = platforms;
            _tableView.dataSource = this;
            _tableView.ScrollToRow(0, false);
        }

        public virtual void SelectRow(int row)
        {
            _tableView.SelectRow(row);
            _tableView.ScrollToRow(row, false);
        }
        
        public virtual void ClearSelection()
        {
            _tableView.ClearSelection();
            _tableView.ScrollToRow(0, false);
        }
    }
}