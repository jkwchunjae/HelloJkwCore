namespace ProjectWorldCup;

public class BettingData2018
{
    public string BettingName { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime FreezeTime { get; set; }
    public List<Target2018> TargetList { get; set; }
    public Dictionary<string /* username */, UserBettingData2018> UserBettingList { get; set; }
    public int ScoreMinimum { get; set; } // ignore: -1
    public List<string> RandomSelectedUser { get; set; } = new List<string>();

    [JsonIgnore]
    public bool IsOpen => OpenTime >= DateTime.Now;
    [JsonIgnore]
    public bool IsFreeze => FreezeTime >= DateTime.Now;
    [JsonIgnore]
    public bool IsEditable => IsOpen && !IsFreeze;

    public BettingData2018()
    {
    }

    public BettingData2018(BettingData2018 bettingData)
    {
        BettingName = bettingData.BettingName;
        OpenTime = bettingData.OpenTime;
        FreezeTime = bettingData.FreezeTime;
        TargetList = bettingData.TargetList;
        UserBettingList = bettingData.UserBettingList;
    }
}

public interface I2018Team
{
    public string Id { get; set; }
    public string Value { get; set; }
}

public class Target2018 : I2018Team
{
    public string Id { get; set; }
    public string Value { get; set; }
    public double Weight { get; set; }
}

public class UserBettingData2018
{
    public string Username { get; set; }
    public int BettingAmount { get; set; }
    public int AllotmentAmount { get; set; }
    public string BettingGroup { get; set; }
    public List<UserBetting2018> BettingList { get; set; }

    public UserBettingData2018() { }

    public UserBettingData2018(string username, List<UserBetting2018> bettingList)
    {
        Username = username;
        BettingList = bettingList;
    }
}

public class UserBetting2018 : I2018Team
{
    public string Id { get; set; }
    public string Value { get; set; }
    public bool IsMatched { get; set; }
}
