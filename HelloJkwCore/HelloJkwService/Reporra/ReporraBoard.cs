using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloJkwService.Reporra
{
    public class ReporraBoard
    {
        public int Size { get; }
        private ReporraCell[,] _cells;

        public bool IsEnded => Cells.Range(Size, _ => Size)
                            .Select(x => GetCell(x.Row, x.Column).Color)
                            .Distinct()
                            .Count() == 2;

        private static List<(int Dx, int Dy)> _cross = new List<(int Dx, int Dy)>
        {
            (0, 1), (0, -1), (1, 0), (-1, 0)
        };

        public ReporraBoard(int size, int colorCount = 6)
        {
            Size = size;
            Init(size, colorCount);
        }

        private void Init(int size, int colorCount)
        {
            _cells = new ReporraCell[size, size];
            Cells.Range(size, row => size)
                .ForEach(x =>
                {
                    _cells[x.Row, x.Column] = new ReporraCell(x.Row, x.Column);
                });

            var colors = Enum.GetValues(typeof(ReporraColor));
            Cells.Range(size, row => size - row)
                .ForEach(x =>
                {
                    var colorIndex = StaticRandom.Next(colorCount);
                    _cells[x.Row, x.Column].Color = (ReporraColor)colors.GetValue(colorIndex);
                    _cells[size - x.Row - 1, size - x.Column - 1].Color = (ReporraColor)colors.GetValue(colorCount - colorIndex - 1);
                });
        }

        public ReporraCell GetCell(int row, int column)
        {
            if (!(row >= 0 && row < Size))
                return null;
            if (!(column >= 0 && column < Size))
                return null;

            return _cells[row, column];
        }

        public void ChangeColor(int row, int column, ReporraColor color)
        {
            var initCell = GetCell(row, column);
            var initColor = initCell.Color;

            // BFS
            var queue = new Queue<ReporraCell>();
            queue.Enqueue(initCell);

            if (initColor == color)
                return;

            while (queue.Any())
            {
                var cell = queue.Dequeue();
                cell.Color = color;
                _cross.Select(x => GetCell(cell.Row + x.Dx, cell.Column + x.Dy))
                    .Where(cell => cell?.Color == initColor)
                    .ForEach(cell => queue.Enqueue(cell));
            }
        }

        public int CountArea(int row, int column)
        {
            var initCell = GetCell(row, column);
            var initColor = initCell.Color;

            // BFS
            var queue = new Queue<ReporraCell>();
            var visited = new HashSet<(int Row, int Column)>();

            queue.Enqueue(initCell);
            visited.Add((initCell.Row, initCell.Column));

            var count = 0;
            while (queue.Any())
            {
                var cell = queue.Dequeue();
                count++;

                _cross.Select(x => GetCell(cell.Row + x.Dx, cell.Column + x.Dy))
                    .Where(cell => cell?.Color == initColor)
                    .Where(cell => !visited.Contains((cell.Row, cell.Column)))
                    .ForEach(cell =>
                    {
                        queue.Enqueue(cell);
                        visited.Add((cell.Row, cell.Column));
                    });
            }

            return count;
        }

        public string GetColorString(bool reverse)
        {
            if (reverse)
            {
                return Cells.Range(Size, _ => Size)
                    .OrderByDescending(x => x.Row)
                    .ThenByDescending(x => x.Column)
                    .Select(x => GetCell(x.Row, x.Column).Color.ToString())
                    .StringJoin("");
            }
            else
            {
                return Cells.Range(Size, _ => Size)
                    .OrderBy(x => x.Row)
                    .ThenBy(x => x.Column)
                    .Select(x => GetCell(x.Row, x.Column).Color.ToString())
                    .StringJoin("");
            }
        }
    }

    public static class Cells
    {
        public static IEnumerable<(int Row, int Column)> Range(int size, Func<int, int> fnColumnSize)
        {
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < fnColumnSize(row); column++)
                {
                    yield return (row, column);
                }
            }
        }
    }
}
