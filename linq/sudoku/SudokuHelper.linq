<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"

public enum CandidateHouseType
{
    Row,
    Column,
    Block,
}

public readonly struct CandidateStrongLink
{
    public CandidateStrongLink(CandidateHouseType houseType, ICell first, ICell second)
    {
        HouseType = houseType;
        First = first;
        Second = second;
    }

    public CandidateHouseType HouseType { get; }
    public ICell First { get; }
    public ICell Second { get; }
    public ICell[] Cells => [First, Second];

    public ICell GetOther(ICell cell)
    {
        if (ReferenceEquals(First, cell))
        {
            return Second;
        }

        return First;
    }
}

public static class SudokuHelper
{
    public static CandidateStrongLink[] GetStrongLinks(IBoard board, int digit, CandidateHouseType houseType)
    {
        IHouse[] houses = houseType switch
        {
            CandidateHouseType.Row => board.Rows,
            CandidateHouseType.Column => board.Columns,
            CandidateHouseType.Block => board.Blocks,
            _ => throw new ArgumentOutOfRangeException(nameof(houseType)),
        };

        return houses
            .Select(house => house.FilterByCandidate(digit).ToArray())
            .Where(cells => cells.Length == 2)
            .Select(cells => new CandidateStrongLink(houseType, cells[0], cells[1]))
            .ToArray();
    }

    public static bool Sees(ICell first, ICell second)
    {
        return first.Row == second.Row
            || first.Column == second.Column
            || first.Block == second.Block;
    }

    public static bool SharesRowOrColumn(ICell first, ICell second)
    {
        return first.Row == second.Row || first.Column == second.Column;
    }

    public static StrategyResult[] FindEliminations(IBoard board, int digit, ICell end1, ICell end2, params ICell[] patternCells)
    {
        return board.Houses
            .SelectMany(house => house.FilterByCandidate(digit))
            .Distinct()
            .Where(cell => !patternCells.Contains(cell))
            .Where(cell => Sees(cell, end1) && Sees(cell, end2))
            .OrderBy(cell => cell.Row)
            .ThenBy(cell => cell.Column)
            .Select(cell => new StrategyResult
            {
                Target = cell,
                Value = digit,
            })
            .ToArray();
    }
}
