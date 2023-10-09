namespace GameLibra;

public class GameEngine
{
    public LibraGameState State { get; set; }
    public List<HistoryItem> History { get; set; } = new();
}

public class HistoryItem
{
    public string PlayerId { get; set; }
    public string TargetScaleIndex { get; set; }
    public List<Cube> Cubes { get; set; }
}

public class GameStateBuilder
{
    private bool _useDevilsPlanRule = false;
    private LibraGameState _state = new();
    public GameStateBuilder SetName(string name)
    {
        _state.Name = name;
        return this;
    }
    public GameStateBuilder SetRule(LibraGameRule rule)
    {
        _state.Rule = rule;
        return this;
    }
    public GameStateBuilder UseDevilsPlanRule()
    {
        _useDevilsPlanRule = true;
        _state.Rule = new LibraGameRule
        {
            CubeCount = 5,
            PlayerCount = 7,
            CubeMinValue = 1,
            CubeMaxValue = 20,
            CubePerPlayer = 2,
            OpenFirst = true,
            MinimumApplyCubeCount = 2,
            ScaleCount = 2,
        };
        return this;
    }
    public LibraGameState Build()
    {
        if (_state.Rule == null)
            throw new InvalidOperationException("Rule is not set");

        _state.CubeInfo = MakeCubeInfo();
        _state.Players = Enumerable.Range(0, _state.Rule.PlayerCount)
            .Select(x => new Player
            {
                Id = x + 1,
                Cubes = _state.CubeInfo
                    .Select(cube => new CubeCount
                    {
                        Cube = cube,
                        Count = _state.Rule.CubePerPlayer,
                    })
                    .ToList(),
            })
            .ToList();
        _state.Scales = Enumerable.Range(0, _state.Rule.ScaleCount)
            .Select(x => new DoubleScale())
            .ToList();
        _state.TurnPlayerIndex = 0;
        return _state;
    }

    private List<Cube> MakeCubeInfo()
    {
        if (_useDevilsPlanRule)
        {
            var rule = _state.Rule;
            var randomNums = Enumerable.Range(rule.CubeMinValue, rule.CubeMaxValue - rule.CubeMinValue + 1)
                .RandomShuffle()
                .ToArray();
            var value1 = randomNums.Where(x => x < 10).First();
            var value2 = randomNums.Where(x => x < 10).Skip(1).First();
            var value4 = randomNums.Where(x => x >= 10).First();
            var value5 = randomNums.Where(x => x >= 10).Skip(1).First();
            return new List<Cube>
            {
                new Cube { Id = 1, Value = Math.Min(value1, value2) },
                new Cube { Id = 2, Value = Math.Max(value1, value2) },
                new Cube { Id = 3, Value = 10 },
                new Cube { Id = 4, Value = Math.Min(value4, value5) },
                new Cube { Id = 5, Value = Math.Max(value4, value5) },
            };
        }
        else
        {
            var rule = _state.Rule;
            var cubeNumbers = Enumerable.Range(rule.CubeMinValue, rule.CubeMaxValue - rule.CubeMinValue + 1)
                .RandomShuffle()
                .Take(rule.CubeCount)
                .ToArray();
            return cubeNumbers
                .Select((x, i) => new Cube { Id = i + 1, Value = x })
                .ToList();
        }
    }
}