<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"

public abstract class BasicFishStrategyBase : ICandidateStrategy
{
    private readonly struct BasicFishOrientation
    {
        public BasicFishOrientation(IHouse[] baseSets, Func<ICell, IHouse> coverSelector)
        {
            BaseSets = baseSets;
            CoverSelector = coverSelector;
        }

        public IHouse[] BaseSets { get; }
        public Func<ICell, IHouse> CoverSelector { get; }
    }

    public abstract string Name { get; }
    public abstract string Description { get; }
    protected abstract int Size { get; }

    public bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        var orientations = new[]
        {
            new BasicFishOrientation(board.Rows, cell => board.GetColumn(cell.Column)),
            new BasicFishOrientation(board.Columns, cell => board.GetRow(cell.Row)),
        };

        foreach (var orientation in orientations)
        {
            for (var digit = 1; digit <= 9; digit++)
            {
                if (TryFindFish(digit, orientation, out result))
                {
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    private bool TryFindFish(
        int digit,
        BasicFishOrientation orientation,
        [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        IHouse[] eligibleBaseSets = orientation.BaseSets
            .Where(house => house.FilterByCandidate(digit).Any())
            .ToArray();

        foreach (var baseCombination in eligibleBaseSets.Combinations(Size))
        {
            if (TryCreateFish(digit, baseCombination, orientation.CoverSelector, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }

    private bool TryCreateFish(
        int digit,
        IHouse[] baseCombination,
        Func<ICell, IHouse> coverSelector,
        [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        ICell[] baseCells = baseCombination
            .SelectMany(house => house.FilterByCandidate(digit))
            .ToArray();

        IHouse[] coverSets = baseCells
            .Select(cell => coverSelector(cell))
            .Distinct()
            .ToArray();

        if (coverSets.Length != Size)
        {
            result = null;
            return false;
        }

        ICell[] eliminationsCell = coverSets
            .SelectMany(house => house.FilterByCandidate(digit))
            .Where(cell => !baseCells.Contains(cell))
            .Distinct()
            .ToArray();

        var eliminations = eliminationsCell
            .Select(cell => new StrategyResult
            {
                Target = cell,
                Value = digit,
            })
            .ToArray();

        if (eliminations.Any())
        {
            result = eliminations;
            return true;
        }

        result = null;
        return false;
    }
}

public class XWingFishStrategy : BasicFishStrategyBase
{
    protected override int Size => 2;
    public override string Name => "XWing";
    public override string Description =>
        "기본 2-집합 행/열 교차 패턴으로 후보를 제거하는 X-Wing (HoDoKu Basic Fish).";
}

public class SwordfishStrategy : BasicFishStrategyBase
{
    protected override int Size => 3;
    public override string Name => "Swordfish";
    public override string Description =>
        "세 개의 행/열을 엮어 동일 수 후보를 제한하는 Swordfish 패턴.";
}

public class JellyfishStrategy : BasicFishStrategyBase
{
    protected override int Size => 4;
    public override string Name => "Jellyfish";
    public override string Description =>
        "네 개의 행/열을 사용하는 Jellyfish 기본 피쉬 전략.";
}
