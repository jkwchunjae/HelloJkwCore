<Query Kind="Program">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"


void Main()
{
    var builder = new BoardBuilder()
        .SetNumbers("""
        ...79..23
        .278..95.
        .4925....
        .92......
        5......92
        ....2936.
        ....87249
        28.9.5137
        97..32.8.
        """);
        
    var board = builder.Build();
    
    IEnumerable<IValueStrategy> valueStrategies =
    [
        new FullHouseStrategy(),
        new NakedSingleStrategy(),
        new HiddenSingleStrategy(),
    ];
    IEnumerable<ICandidateStrategy> candidateStrategies =
    [
        new HiddenSubsetStrategy(),
        new NakedSubsetStrategy(),
        new IntersectionRowColumnStrategy(),
        new IntersectionBlockStrategy(),
        new XWingFishStrategy(),
        new SwordfishStrategy(),
        new JellyfishStrategy(),
        new SkyscraperStrategy(),
        new TurbotFishStrategy(),
        new TwoStringKiteStrategy(),
    ];

    foreach (var iteration in Enumerable.Range(1, 10))
    {
        iteration.Dump();
        foreach (var valueStrategy in valueStrategies)
        {
            if (valueStrategy.TryFind(board, out var result))
            {
                valueStrategy.Name.Dump();
                result.Value.Value.Dump("find");
                return;
            }
        }
        foreach (var candidateStrategy in candidateStrategies)
        {
            if (candidateStrategy.TryRemoveCandidate(board, out var result))
            {
                foreach (var c in result)
                {
                    c.Value.Dump("remove");
                }
                return;
            }
        }
    }
}


