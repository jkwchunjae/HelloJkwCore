
public class LibraCalculator
{
	IReadOnlyList<IReadOnlyDictionary<string, int>> _sets;
	
	public void Init(string[] cubes, Range range)
	{
		var values = Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value + 1).ToArray();
		var setssss = new List<IReadOnlyDictionary<string, int>>();
		Rec(cubes, values, new(), setssss);
		_sets = setssss.AsReadOnly();
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
	
	/// <summary>
	/// 특정 큐브의 값을 찾은 경우
	/// </summary>
	public void SetValue(string cubeId, int value)
	{
		_sets = _sets
			.Where(set => Test(set))
			.ToList();
			
		bool Test(IReadOnlyDictionary<string, int> set)
		{
			return set[cubeId] == value;
		}
	}
	/// <summary>
	/// 왼쪽 저울이 작은 경우
	/// </summary>
	public void LessThan(string[] left, string[] right)
	{
		_sets = _sets
			.Where(set => Test(set))
			.ToList();

		bool Test(IReadOnlyDictionary<string, int> set)
		{
			var leftValue = left.Sum(cubeId => set[cubeId]);
			var rightValue = right.Sum(cubeId => set[cubeId]);
			return leftValue < rightValue;
		}
	}
	/// <summary>
	/// 왼쪽 저울이 큰 경우
	/// </summary>
	public void GreaterThan(string[] left, string[] right)
	{
		_sets = _sets
			.Where(set => Test(set))
			.ToList();

		bool Test(IReadOnlyDictionary<string, int> set)
		{
			var leftValue = left.Sum(cubeId => set[cubeId]);
			var rightValue = right.Sum(cubeId => set[cubeId]);
			return leftValue > rightValue;
		}
	}
	/// <summary>
	/// 저울이 평행을 이룬 경우
	/// </summary>
	public void SameValue(string[] left, string[] right)
	{
		_sets = _sets
			.Where(set => Test(set))
			.ToList();

		bool Test(IReadOnlyDictionary<string, int> set)
		{
			var leftValue = left.Sum(cubeId => set[cubeId]);
			var rightValue = right.Sum(cubeId => set[cubeId]);
			return leftValue == rightValue;
		}
	}
}
