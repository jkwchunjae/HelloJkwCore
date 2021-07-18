using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    public class BadukBoard
    {
        public int Size { get; init; }

        private List<StoneLogData> _stones = new();

        public BadukBoard(int size)
        {
            Size = size;
        }

        public bool SetStone(int row, int column, StoneColor color)
        {
            if (FindLastStone(row, column, out var _))
            {
                return false;
            }
            if (row.Between(0, Size) && column.Between(0, Size))
            {
                _stones.Add(new StoneLogData
                {
                    Row = row,
                    Column = column,
                    Action = StoneAction.Set,
                    Color = color,
                });
                return true;
            }
            return false;
        }

        public bool TryRemoveStone(int row, int column)
        {
            if (FindLastStone(row, column, out var lastStone))
            {
                _stones.Add(new StoneLogData
                {
                    Row = row,
                    Column = column,
                    Action = StoneAction.Remove,
                    Color = lastStone.Color,
                });
            }
            return false;
        }

        public bool FindLastStone(int row, int column, out StoneLogData stone)
        {
            var lastStone = _stones.LastOrDefault(x => x.Row == row && x.Column == column);
            if (lastStone?.Action == StoneAction.Set)
            {
                stone = lastStone;
                return true;
            }
            stone = null;
            return false;
        }
    }
}
