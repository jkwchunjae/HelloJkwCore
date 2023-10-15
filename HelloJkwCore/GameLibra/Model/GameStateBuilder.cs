namespace GameLibra;

public class GameStateBuilder
{
    private bool _useDevilsPlanRule = false;
    private LibraGameState _state = new();
    public GameStateBuilder SetId(string id)
    {
        _state.Id = id;
        return this;
    }
    public GameStateBuilder SetName(string name)
    {
        _state.Name = name;
        return this;
    }
    public GameStateBuilder SetOwner(AppUser owner)
    {
        _state.Owner = owner;
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
            VisibleOtherCube = false,
        };
        return this;
    }
    public LibraGameState Build()
    {
        if (_state.Rule == null)
            throw new InvalidOperationException("Rule is not set");

        _state.CurrentPlayerId = -1;
        _state.Status = LibraGameStatus.Ready;
        _state.CubeInfo = MakeCubeInfo();
        _state.Players = Enumerable.Range(0, _state.Rule.PlayerCount)
            .Select(x => new Player
            {
                Id = x + 1,
                Cubes = _state.CubeInfo
                    .SelectMany(cube => Enumerable.Range(0, _state.Rule.CubePerPlayer)
                        .Select(_ => new Cube(cube)))
                    .ToList(),
            })
            .ToList();
        _state.Scales = Enumerable.Range(0, _state.Rule.ScaleCount)
            .Select((x, i) => new DoubleScale
            {
                Id = i + 1,
            })
            .ToList();
        return _state;
    }

    private List<Cube> MakeCubeInfo()
    {
        Dictionary<int, string> _cubeName = new Dictionary<int, string>
        {
            [1] = "a",
            [2] = "b",
            [3] = "c",
            [4] = "d",
            [5] = "e",
            [6] = "f",
            [7] = "g",
            [8] = "h",
            [9] = "x",
            [10] = "z",
        };

        if (_useDevilsPlanRule)
        {
            var rule = _state.Rule;
            var randomNums = Enumerable.Range(rule.CubeMinValue, rule.CubeMaxValue - rule.CubeMinValue + 1)
                .Where(x => x != 10)
                .RandomShuffle()
                .ToArray();
            var value1 = randomNums.Where(x => x < 10).First();
            var value2 = randomNums.Where(x => x < 10).Skip(1).First();
            var value3 = 10;
            var value4 = randomNums.Where(x => x >= 10).First();
            var value5 = randomNums.Where(x => x >= 10).Skip(1).First();
            return new[] { value1, value2, value3, value4, value5 }
                .RandomShuffle()
                .Select((value, i) => new Cube
                {
                    Id = i + 1,
                    Name = _cubeName[i + 1],
                    Value = value,
                })
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
                .Select((value, i) => new Cube
                {
                    Id = i + 1,
                    Name = _cubeName[i + 1],
                    Value = value,
                })
                .ToList();
        }
    }
}