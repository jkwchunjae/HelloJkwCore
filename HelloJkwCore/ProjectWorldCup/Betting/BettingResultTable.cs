namespace ProjectWorldCup;

public interface IBettingResultTable<out T> : IEnumerable<T>
    where T : IBettingResultItem
{

}

public class BettingResultTable<T> : IBettingResultTable<T>
    where T : IBettingResultItem
{
    private IEnumerable<T> _items { get; set; }

    public BettingResultTable(IEnumerable<T> list, BettingTableOption bettingTableOption = null)
    {
        double totalScore = list.Sum(x => x.Score);
        var totalMoney = 10000 * list.Count(); // 항상 배팅금액은 1만원이다.

        foreach (var item in list)
        {
            if (totalScore == 0)
            {
                item.Reward = 10000;
            }
            else
            {
                var ratio = item.Score / totalScore;
                var reward = (int)(totalMoney * ratio);
                item.Reward = bettingTableOption?.RewardForUser?.Invoke(reward) ?? RewardForUser(reward);
            }
        }
        foreach (var item in list)
        {
            item.Rank = list.Count(x => x.Reward > item.Reward) + 1;
        }

        _items = list.OrderByDescending(x => x.Reward).ToList();
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
