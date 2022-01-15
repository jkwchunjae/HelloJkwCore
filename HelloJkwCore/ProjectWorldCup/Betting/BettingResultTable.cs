namespace ProjectWorldCup;

public class BettingResultTable<T> : IEnumerable<T>
    where T : IBettingResultItemResult, new()
{
    private IEnumerable<T> _items { get; set; }

    public BettingResultTable(IEnumerable<IBettingResultItem> list)
    {
        double totalScore = list.Sum(x => x.Score);
        var listCount = list.Count();

        _items = list.Select(x => new T() { Id = x.Id, Score = x.Score }).ToList();

        foreach (var item in _items)
        {
            var ratio = item.Score * listCount / totalScore;
            item.Reward = CalcReward(ratio);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    private static int CalcReward(double ratio)
    {
        var input = 10000; // 항상 배팅금액은 1만원이다.
        var reward = (int)(input * ratio);

        return reward.RoundN(-3, 7); // 100의 자리에서 7기준으로 반올림한다.
    }
}
