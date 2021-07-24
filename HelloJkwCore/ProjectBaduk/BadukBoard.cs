using Common;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    public class BadukBoard
    {
        public int Size { get; private set; }
        public StoneChangeMode ChangeMode { get; set; } = StoneChangeMode.Auto;
        public StoneColor CurrentColor { get; set; } = StoneColor.Black;
        public bool VisibleStoneIndex { get; set; } = true;

        private List<StoneLogData> _stones = new();

        public int CurrentIndex { get; set; } = 0;
        public int LastIndex => _stones.Count;

        public DefaultDictionary<StoneColor, int> RemovedCount => _stones.Take(CurrentIndex)
            .Where(x => x.Action == StoneAction.Remove)
            .GroupBy(x => x.Color)
            .ToDictionary(x => x.Key, x => x.Count())
            .ToDefaultDictionary();

        public BadukBoard(int size)
        {
            Size = size;
        }

        public BadukBoard(int size, List<StoneLogData> stoneLogs)
            : this(size)
        {
            _stones = stoneLogs;
        }

        public void ChangeSize(int size)
        {
            Size = size;
            _stones.Clear();
            CurrentIndex = 0;
        }

        public void ClickCell(int row, int column)
        {
            if (CurrentIndex < LastIndex)
            {
                _stones.RemoveRange(CurrentIndex, LastIndex - CurrentIndex);
            }
            if (TryRemoveStone(row, column))
            {
            }
            else
            {
                if (SetStone(row, column, CurrentColor))
                {
                    if (ChangeMode == StoneChangeMode.Auto)
                    {
                        CurrentColor = CurrentColor == StoneColor.Black ? StoneColor.White : StoneColor.Black;
                    }
                }
            }
        }

        public bool SetStone(int row, int column, StoneColor color)
        {
            if (FindLastStone(row, column, out var _, out var _))
            {
                return false;
            }
            if (row.Between(1, Size + 1) && column.Between(1, Size + 1))
            {
                _stones.Add(new StoneLogData
                {
                    Row = row,
                    Column = column,
                    Action = StoneAction.Set,
                    Color = color,
                });
                CurrentIndex++;
                return true;
            }
            return false;
        }

        public bool TryRemoveStone(int row, int column)
        {
            if (FindLastStone(row, column, out var lastStone, out var _))
            {
                _stones.Add(new StoneLogData
                {
                    Row = row,
                    Column = column,
                    Action = StoneAction.Remove,
                    Color = lastStone.Color,
                });
                CurrentIndex++;
                return true;
            }
            return false;
        }

        public bool FindLastStone(int row, int column, out StoneLogData stone, out int index)
        {
            stone = null;
            index = 0;
            var lastStone = _stones
                .Take(CurrentIndex)
                .LastOrDefault(x => x.Row == row && x.Column == column);
            if (lastStone?.Action == StoneAction.Set)
            {
                stone = lastStone;
                for (var i = 0; i < _stones.Count; i++)
                {
                    if (_stones[i] == lastStone)
                        index = i + 1;
                }
                return true;
            }
            return false;
        }

        public void ChangeCurrentIndex(int offset)
        {
            CurrentIndex = Math.Max(0, Math.Min(LastIndex, CurrentIndex + offset));
            if (ChangeMode == StoneChangeMode.Auto)
            {
                if (CurrentIndex == 0)
                {
                    CurrentColor = StoneColor.Black;
                }
                else
                {
                    var lastSet = _stones.Take(CurrentIndex)
                        .Where(x => x.Action == StoneAction.Set)
                        .LastOrDefault();
                    if (lastSet != null)
                    {
                        CurrentColor = lastSet.Color == StoneColor.Black ? StoneColor.White : StoneColor.Black;
                    }
                }
            }
        }
    }
}
