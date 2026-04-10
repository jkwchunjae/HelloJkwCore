namespace ProjectWorldCup;

// https://digitalhub.fifa.com/m/464f16f856f5ed05/original/FIFA-World-Cup-Qatar-2022-Match-Schedule.pdf
internal static class WorldCupConst
{
    /// <summary> 월드컵 첫 경기가 시작하는 시간 </summary>
    public static readonly DateTime WorldCupStartTime = DateTime.Parse("2026-06-12T04:00:00+09:00");
    public static readonly DateTime GroupStageStartTime = WorldCupStartTime;
    /// <summary> 조별리그 마지막 경기 시작 시간  </summary>
    public static readonly DateTime GroupStageLastTime = DateTime.Parse("2026-06-28T11:00:00+09:00");
    public static readonly DateTime Round32Match1StartTime = DateTime.Parse("2026-06-29T04:00:00+09:00");
    public static readonly DateTime Round32Match2StartTime = DateTime.Parse("2026-06-30T02:00:00+09:00");
    public static readonly DateTime Round32Match3StartTime = DateTime.Parse("2026-06-30T05:30:00+09:00");
    /// <summary> 32강 마지막 경기 시작 시간 </summary>
    public static readonly DateTime Round32LastTime = DateTime.Parse("2026-07-04T10:30:00+09:00");
    public static readonly DateTime Round16Match1StartTime = DateTime.Parse("2026-07-05T02:00:00+09:00");
    public static readonly DateTime Round16Match2StartTime = DateTime.Parse("2026-07-05T06:00:00+09:00");
    public static readonly DateTime Round16Match3StartTime = DateTime.Parse("2026-07-06T05:00:00+09:00");
    public static readonly DateTime Round8StartTime = DateTime.Parse("2026-07-10T05:00:00+09:00");
    public static readonly DateTime Round4StartTime = DateTime.Parse("2026-07-15T04:00:00+09:00");
    public static readonly DateTime FinalStartTime = DateTime.Parse("2026-07-20T04:00:00+09:00");
}
