<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"

public class SkyscraperStrategy : ICandidateStrategy
{
    public string Name => "Skyscraper";
    public string Description =>
        "두 개의 행/열 conjugate pair 가 하나의 공통 열/행을 밑변으로 공유할 때, 나머지 두 끝점을 함께 보는 후보를 제거한다.";

    public bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        if (TryRemoveCandidate(board, CandidateHouseType.Row, out result))
        {
            return true;
        }

        if (TryRemoveCandidate(board, CandidateHouseType.Column, out result))
        {
            return true;
        }

        result = null;
        return false;
    }

    private bool TryRemoveCandidate(
        IBoard board,
        CandidateHouseType baseType,
        [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        for (var digit = 1; digit <= 9; digit++)
        {
            CandidateStrongLink[] strongLinks = SudokuHelper.GetStrongLinks(board, digit, baseType);

            foreach (var pair in strongLinks.Combinations(2))
            {
                CandidateStrongLink firstLink = pair[0];
                CandidateStrongLink secondLink = pair[1];

                var matchedPairs = firstLink.Cells
                    .SelectMany(
                        firstCell => secondLink.Cells,
                        (firstCell, secondCell) => new { FirstCell = firstCell, SecondCell = secondCell })
                    .Where(x => baseType == CandidateHouseType.Row
                        ? x.FirstCell.Column == x.SecondCell.Column
                        : x.FirstCell.Row == x.SecondCell.Row)
                    .ToArray();

                if (matchedPairs.Length != 1)
                {
                    continue;
                }

                ICell baseCell1 = matchedPairs[0].FirstCell;
                ICell baseCell2 = matchedPairs[0].SecondCell;
                ICell end1 = firstLink.GetOther(baseCell1);
                ICell end2 = secondLink.GetOther(baseCell2);

                if (ReferenceEquals(end1, end2))
                {
                    continue;
                }

                StrategyResult[] eliminations = SudokuHelper.FindEliminations(
                    board,
                    digit,
                    end1,
                    end2,
                    firstLink.First,
                    firstLink.Second,
                    secondLink.First,
                    secondLink.Second);

                if (eliminations.Any())
                {
                    result = eliminations;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}
