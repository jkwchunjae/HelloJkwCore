using Newtonsoft.Json;

namespace ProjectWorldCup;

public class BettingUser
{
    public AppUser AppUser { get; set; }
    public string Nickname { get; set; }
    public UserJoinStatus JoinStatus { get; set; }
    public List<BettingType> JoinedBetting { get; set; }
    public List<BettingHistory> BettingHistories { get; set; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum UserJoinStatus
{
    None,
    Requested,
    Canceled,
    Joined,
}

public class BettingHistory
{
    public DateTime Time { get; set; }
    /// <summary> 첫 가입금 (양수), 내기 참가금 (음수), 배당금 (양수) </summary>
    public long Value { get; set; }
    public string Comment { get; set; }
    /// <summary> 결과를 확인할 수 있는 페이지 주소 </summary>
    public string ResultUrl { get; set; }
}
