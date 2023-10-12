namespace GameLibra;

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
                    .SelectMany(cube => Enumerable.Range(0, _state.Rule.CubePerPlayer).Select(_ => cube))
                    .ToArray(),
            })
            .ToList();
        _state.Scales = Enumerable.Range(0, _state.Rule.ScaleCount)
            .Select((x, i) => new DoubleScale
            {
                Id = i + 1,
            })
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
            var value3 = 10;
            var value4 = randomNums.Where(x => x >= 10).First();
            var value5 = randomNums.Where(x => x >= 10).Skip(1).First();
            return new[] { value1, value2, value3, value4, value5 }
                .RandomShuffle()
                .Select((value, i) => new Cube { Id = i + 1, Value = value })
                .ToList();
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