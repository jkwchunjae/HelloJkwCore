<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\SudokuModel"
#load ".\IStrategy"
#load ".\Utils"

public class HiddenSubsetStrategy : ICandidateStrategy
{
    public string Name => "HiddenSubset";
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
            foreach (var combo in house.RemainNumbers.Combinations(combiCount))
            {
                var containsCell = combo
                    .SelectMany(number => cellsByCandidate[number])
                    .Distinct()
                    .ToArray();

                if (containsCell.Count() == combo.Count())
                {
                    result = containsCell
                        // 이번에 뽑은 숫자(combo)가 아닌 다른 후보가 있는 셀
                        .Where(cell => cell.Candidate.Any(c => !combo.Contains(c)))
                        // 지워야 할 셀과 후보를 추출
                        .SelectMany(cell => cell.Candidate
                                    .Where(c => !combo.Contains(c))
                                    .Select(c => new { Cell = cell, Candidate = c }))
                        .Select(x => new StrategyResult
                        {
                            Target = x.Cell,
                            Value = x.Candidate,
                        })
                        .ToArray();

                    // 지워야 하는 다른 후보가 없을 수도 있다.
                    // 없으면 그냥 넘어간다.
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