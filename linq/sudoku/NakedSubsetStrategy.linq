<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"

public class NakedSubsetStrategy : ICandidateStrategy
{
    public string Name => "NakedSubset";
    public string Description => "";

    public bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        foreach (var house in board.Houses)
        {
            if (TryRemoveCandidate(house, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }

    private bool TryRemoveCandidate(IHouse house, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        Dictionary<int, ICell[]> cellsByCandidate = house.RemainNumbers
            .ToDictionary(number => number, number => house.FilterByCandidate(number).ToArray());

        for (var combiCount = 2; combiCount < house.RemainNumbers.Count(); combiCount++)
        {
            foreach (var cells in house.EmptyCells.ToArray().Combinations(combiCount))
            {
                var containsCandidate = cells
                    .SelectMany(cell => cell.Candidate)
                    .Distinct()
                    .ToArray();
                    
                if (cells.Count() == containsCandidate.Count())
                {
                    var otherCells = house.EmptyCells.Except(cells);
                    result = otherCells
                        // 지워야 할 셀과 후보를 추출
                        .SelectMany(otherCell => otherCell.Candidate
                                    // 
                                    .Where(c => containsCandidate.Contains(c))
                                    .Select(c => new { Cell = otherCell, Candidate = c }))
                        .Select(x => new StrategyResult
                        {
                            Target = x.Cell,
                            Value = x.Candidate,
                        })
                        .ToArray();

                    if (result.Any())
                    {
                        return true;
                    }
                    else
                    {
                        result = null;
                    }
                }
            }
        }

        result = null;
        return false;
    }
}

