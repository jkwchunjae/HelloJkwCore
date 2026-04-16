<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"

public class NakedSingleStrategy : IValueStrategy
{
    public string Name => "NakedSingle";
    public string Description => "";

    public bool TryFind(IBoard board, [NotNullWhen(true)] out StrategyResult? result)
    {
        foreach (var house in board.Houses)
        {
            foreach (var cell in house.EmptyCells)
            {
                if (cell.Candidate.Count() == 1)
                {
                    result = new StrategyResult
                    {
                        Target = cell,
                        Value = cell.Candidate.First(),
                    };
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}

public class HiddenSingleStrategy : IValueStrategy
{
    public string Name => "HiddenSingle";
    public string Description => "";

    public bool TryFind(IBoard board, [NotNullWhen(true)] out StrategyResult? result)
    {
        foreach (var house in board.Houses)
        {
            var hiddenSingle = house.EmptyCells
                .SelectMany(cell => cell.Candidate.Select(c => new { Candidate = c, Cell = cell }))
                .GroupBy(x => x.Candidate)
                .Select(x => new { Candidate = x.Key, Cells = x.Select(e => e.Cell).ToArray() })
                .Where(x => x.Cells.Count() == 1)
                .ToArray();

            if (hiddenSingle.Any())
            {
                result = new StrategyResult
                {
                    Target = hiddenSingle.First().Cells.First(),
                    Value = hiddenSingle.First().Candidate,
                };
                return true;
            }
        }

        result = null;
        return false;
    }
}

public class FullHouseStrategy : IValueStrategy
{
    public string Name => "FullHouse";
    public string Description => "";

    public bool TryFind(IBoard board, [NotNullWhen(true)] out StrategyResult? result)
    {
        foreach (var house in board.Houses)
        {
            if (house.EmptyCells.Count() == 1)
            {
                result = new StrategyResult
                {
                    Target = house.EmptyCells.First(),
                    Value = house.RemainNumbers.First(),
                };
                return true;
            }
        }

        result = null;
        return false;
    }
}