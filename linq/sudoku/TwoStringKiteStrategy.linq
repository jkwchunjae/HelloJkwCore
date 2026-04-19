<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"

public class TwoStringKiteStrategy : ICandidateStrategy
{
    public string Name => "TwoStringKite";
    public string Description =>
        "하나의 행 conjugate pair 와 하나의 열 conjugate pair 가 같은 블록 안에서 연결될 때, 두 끝점을 함께 보는 후보를 제거한다.";

    public bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        for (var digit = 1; digit <= 9; digit++)
        {
            CandidateStrongLink[] rowLinks = SudokuHelper.GetStrongLinks(board, digit, CandidateHouseType.Row);
            CandidateStrongLink[] columnLinks = SudokuHelper.GetStrongLinks(board, digit, CandidateHouseType.Column);

            foreach (var rowLink in rowLinks)
            {
                foreach (var columnLink in columnLinks)
                {
                    foreach (var rowCell in rowLink.Cells)
                    {
                        foreach (var columnCell in columnLink.Cells)
                        {
                            if (ReferenceEquals(rowCell, columnCell))
                            {
                                continue;
                            }

                            if (rowCell.Block != columnCell.Block)
                            {
                                continue;
                            }

                            ICell rowEnd = rowLink.GetOther(rowCell);
                            ICell columnEnd = columnLink.GetOther(columnCell);
                            if (ReferenceEquals(rowEnd, columnEnd))
                            {
                                continue;
                            }

                            StrategyResult[] eliminations = SudokuHelper.FindEliminations(
                                board,
                                digit,
                                rowEnd,
                                columnEnd,
                                rowLink.First,
                                rowLink.Second,
                                columnLink.First,
                                columnLink.Second);

                            if (eliminations.Any())
                            {
                                result = eliminations;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        result = null;
        return false;
    }
}
