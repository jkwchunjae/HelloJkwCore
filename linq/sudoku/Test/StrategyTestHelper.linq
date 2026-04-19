<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"

void ExpectElimination(
    string name,
    ICandidateStrategy strategy,
    IBoard board,
    (int row, int column, int value)[] expected)
{
    if (strategy.TryRemoveCandidate(board, out var result))
    {
        StrategyResult[] actual = result
            .OrderBy(x => x.Target.Row)
            .ThenBy(x => x.Target.Column)
            .ThenBy(x => x.Value)
            .ToArray();

        var expectedText = expected
            .OrderBy(x => x.row)
            .ThenBy(x => x.column)
            .ThenBy(x => x.value)
            .Select(x => $"r{x.row}c{x.column}<> {x.value}")
            .ToArray();

        var actualText = actual
            .Select(x => $"r{x.Target.Row}c{x.Target.Column}<> {x.Value}")
            .ToArray();

        if (!expectedText.SequenceEqual(actualText))
        {
            throw new Exception($"{name}: expected [{string.Join(", ", expectedText)}], actual [{string.Join(", ", actualText)}]");
        }

        actual.Dump(name, 1);
    }
    else
    {
        throw new Exception($"{name}: nothing");
    }
}
