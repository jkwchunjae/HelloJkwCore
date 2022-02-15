namespace ProjectWorldCup;

public class BettingResultTable<T> : IEnumerable<T>
    where T : IBettingResultItem, new()
{
    private IEnumerable<T> _items { get; set; }

    public BettingResultTable(IEnumerable<T> list, BettingTableOption bettingTableOption = null)
    {
        _items = list;

        double totalScore = list.Sum(x => x.Score);
        var listCount = list.Count();

        foreach (var item in _items)
        {
            var ratio = item.Score * listCount / totalScore;
            var reward = (int)(10000 * ratio); // 항상 배팅금액은 1만원이다.
            item.Reward = bettingTableOption?.RewardForUser?.Invoke(reward) ?? RewardForUser(reward);
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

    private static int RewardForUser(int reward)
    {
        return reward.RoundN(-3, 6); // 100의 자리에서 6기준으로 반올림한다.
    }
}

public class BettingTableOption
{
    public Func<int, int> RewardForUser { get; set; }
}
