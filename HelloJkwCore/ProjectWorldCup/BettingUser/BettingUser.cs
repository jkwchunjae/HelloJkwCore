namespace ProjectWorldCup;

public class BettingUser
{
    public AppUser AppUser { get; set; }
    public UserJoinStatus JoinStatus { get; set; }
    public List<BettingType> JoinedBetting { get; set; } = new();
    public List<BettingHistory> BettingHistories { get; set; } = new();

    public DateTime LastJoinRequestTime()
    {
        var data = BettingHistories.Where(x => x.Type == HistoryType.JoinRequest);

        if (data.Any())
        {
            return data.Last().Time;
        }
        return default;
    }
}

[TextJsonConverter(typeof(StringEnumConverter))]
public enum UserJoinStatus
{
    None,
    Requested,
    Joined,
    Rejected,
}

[TextJsonConverter(typeof(StringEnumConverter))]
public enum HistoryType
{
    None,
    JoinRequest,
    JoinApproved,
    JoinRejected,
    CancelJoinRequest,
    Betting,
    Reward1,
    Reward2,
    Reward3,
    ChangeNickname,
}

public class BettingHistory
{
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public HistoryType Type { get; set; }
    /// <summary> 첫 가입금 (양수), 내기 참가금 (음수), 상금 (양수) </summary>
    public long Value { get; set; }
    public string Comment { get; set; }
    /// <summary> 결과를 확인할 수 있는 페이지 주소 </summary>
    public string ResultUrl { get; set; }
}
