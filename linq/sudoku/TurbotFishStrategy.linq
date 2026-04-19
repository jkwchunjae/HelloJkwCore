<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"

public class TurbotFishStrategy : ICandidateStrategy
{
    public string Name => "TurbotFish";
    public string Description =>
        "블록 conjugate pair 와 행/열 conjugate pair 를 strong-weak-strong 형태의 4개 후보 X-Chain 으로 연결해 끝점 둘을 함께 보는 후보를 제거한다.";

    public bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        for (var digit = 1; digit <= 9; digit++)
        {
            CandidateStrongLink[] blockLinks = SudokuHelper.GetStrongLinks(board, digit, CandidateHouseType.Block)
                .Where(link => link.First.Row != link.Second.Row && link.First.Column != link.Second.Column)
                .ToArray();

            CandidateStrongLink[] lineLinks = SudokuHelper
                .GetStrongLinks(board, digit, CandidateHouseType.Row)
                .Concat(SudokuHelper.GetStrongLinks(board, digit, CandidateHouseType.Column))
                .ToArray();

            foreach (var blockLink in blockLinks)
            {
                foreach (var lineLink in lineLinks)
                {
                    foreach (var blockCell in blockLink.Cells)
                    {
                        foreach (var lineCell in lineLink.Cells)
                        {
                            if (ReferenceEquals(blockCell, lineCell))
                            {
                                continue;
                            }

                            if (!SudokuHelper.SharesRowOrColumn(blockCell, lineCell))
                            {
                                continue;
                            }

                            ICell blockEnd = blockLink.GetOther(blockCell);
                            ICell lineEnd = lineLink.GetOther(lineCell);
                            if (ReferenceEquals(blockEnd, lineEnd))
                            {
                                continue;
                            }

                            StrategyResult[] eliminations = SudokuHelper.FindEliminations(
                                board,
                                digit,
                                blockEnd,
                                lineEnd,
                                blockLink.First,
                                blockLink.Second,
                                lineLink.First,
                                lineLink.Second);

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
