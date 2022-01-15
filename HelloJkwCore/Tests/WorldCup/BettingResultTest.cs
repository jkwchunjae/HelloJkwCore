namespace Tests.WorldCup;

public class BettingResultTest
{
    [Fact]
    public void BettingResult_test()
    {
        var item1 = new BettingResultItem(null, 3);
        var item2 = new BettingResultItem(null, 5);
        var item3 = new BettingResultItem(null, 10);

        var items = new List<BettingResultItem>() { item1, item2, item3 };

        var table = new BettingResultTable<BettingResultItem>(items);
        foreach (var item in table)
        {
            if (item.Score == 3)
                Assert.Equal(5000, item.Reward);
            if (item.Score == 5)
                Assert.Equal(8000, item.Reward);
            if (item.Score == 10)
                Assert.Equal(16000, item.Reward);
        }
    }
}
