
namespace GameLibra.Service;

public class LibraAssistor
{
    IReadOnlyList<IReadOnlyDictionary<string, int>> _sets;
    public IReadOnlyList<IReadOnlyDictionary<string, int>> Sets => _sets;

    List<Func<Dictionary<string, int>, bool>> _conditions = new();

    public LibraAssistor()
    {
    }

    public LibraAssistor(LibraGameState state)
    {
        string[] cubes = state.CubeInfo.Select(c => c.Name).Distinct().ToArray();
        Range range = new Range(state.Rule.CubeMinValue, state.Rule.CubeMaxValue);

        var values = Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value + 1).ToArray();
        var setssss = new List<IReadOnlyDictionary<string, int>>();
        Rec(cubes, values, new(), setssss);
        _sets = setssss.AsReadOnly();

        foreach (var hint in state.HintInfos)
        {
            SetValue(hint.CubeName, hint.Value);
            SetOrder(hint.CubeName, hint.Order);
        }
    }
    public void Init(LibraGameState state)
    {
        string[] cubes = state.CubeInfo.Select(c => c.Name).Distinct().ToArray();
        Range range = new Range(state.Rule.CubeMinValue, state.Rule.CubeMaxValue);
        var values = Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value + 1).ToArray();

        var sets = new List<IReadOnlyDictionary<string, int>>();
        RecWithConditions(cubes, values, new(), sets, _conditions);
        _sets = sets.AsReadOnly();

        foreach (var hint in state.HintInfos)
        {
            SetOrder(hint.CubeName, hint.Order);
        }
    }
    private void Rec(string[] cubes, int[] values, Dictionary<string, int> curr, List<IReadOnlyDictionary<string, int>> setssss)
    {
        var cube = cubes.First();
        foreach (var value in values)
        {
            curr[cube] = value;
            if (cubes.Count() == 1)
            {
                setssss.Add(curr.ToDictionary(x => x.Key, x => x.Value).AsReadOnly());
            }
            else
            {
                var nextCubes = cubes.Where(c => c != cube).ToArray();
                var nextValues = values.Where(v => v != value).ToArray();
                Rec(nextCubes, nextValues, curr, setssss);
            }
        }
    }
    private void RecWithConditions(string[] cubes, int[] values, Dictionary<string, int> curr, List<IReadOnlyDictionary<string, int>> setss,
        List<Func<Dictionary<string, int>, bool>> conditions)
    {
        var cube = cubes.First();
        foreach (var value in values)
        {
            curr[cube] = value;
            if (conditions.Any(condition => !condition(curr)))
            {
                continue;
            }
            if (cubes.Count() == 1)
            {
                setss.Add(curr.ToDictionary(x => x.Key, x => x.Value).AsReadOnly());
            }
            else
            {
                var nextCubes = cubes.Where(c => c != cube).ToArray();
                var nextValues = values.Where(v => v != value).ToArray();
                RecWithConditions(nextCubes, nextValues, curr, setss, conditions);
            }
        }
        curr.Remove(cube);
    }

    /// <summary>
    /// 특정 큐브의 값을 찾은 경우
    /// </summary>
    public void SetValue(string cubeName, int value)
    {
        _sets = _sets?
            .Where(set => Test(set))
            .ToList();

        _conditions.Add(Test);

        bool Test(IReadOnlyDictionary<string, int> set)
        {
            if (!set.ContainsKey(cubeName))
            {
                return true;
            }
            return set[cubeName] == value;
        }
    }
    public void SetOrder(string cubeName, int order)
    {
        _sets = _sets?
            .Where(set => Test(set))
            .ToList();

        _conditions.Add(Test);

        bool Test(IReadOnlyDictionary<string, int> set)
        {
            var cubeValue = set[cubeName];
            return order == set.Count(x => x.Value >= cubeValue);
        }
    }
    /// <summary>
    /// 왼쪽 저울이 작은 경우
    /// </summary>
    public void LessThan(string[] left, string[] right)
    {
        _sets = _sets?
            .Where(set => Test(set))
            .ToList();

        _conditions.Add(Test);

        bool Test(IReadOnlyDictionary<string, int> set)
        {
            if (!left.All(cube => set.ContainsKey(cube)))
            {
                return true;
            }
            if (!right.All(cube => set.ContainsKey(cube)))
            {
                return true;
            }
            var leftValue = left.Sum(cubeName => set[cubeName]);
            var rightValue = right.Sum(cubeName => set[cubeName]);
            return leftValue < rightValue;
        }
    }
    /// <summary>
    /// 왼쪽 저울이 큰 경우
    /// </summary>
    public void GreaterThan(string[] left, string[] right)
    {
        _sets = _sets?
            .Where(set => Test(set))
            .ToList();

        _conditions.Add(Test);

        bool Test(IReadOnlyDictionary<string, int> set)
        {
            if (!left.All(cube => set.ContainsKey(cube)))
            {
                return true;
            }
            if (!right.All(cube => set.ContainsKey(cube)))
            {
                return true;
            }
            var leftValue = left.Sum(cubeName => set[cubeName]);
            var rightValue = right.Sum(cubeName => set[cubeName]);
            return leftValue > rightValue;
        }
    }
    /// <summary>
    /// 저울이 평행을 이룬 경우
    /// </summary>
    public void SameValue(string[] left, string[] right)
    {
        _sets = _sets?
            .Where(set => Test(set))
            .ToList();

        _conditions.Add(Test);

        bool Test(IReadOnlyDictionary<string, int> set)
        {
            if (!left.All(cube => set.ContainsKey(cube)))
            {
                return true;
            }
            if (!right.All(cube => set.ContainsKey(cube)))
            {
                return true;
            }
            var leftValue = left.Sum(cubeName => set[cubeName]);
            var rightValue = right.Sum(cubeName => set[cubeName]);
            return leftValue == rightValue;
        }
    }
}
