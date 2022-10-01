namespace ProjectWorldCup;

// https://digitalhub.fifa.com/m/464f16f856f5ed05/original/FIFA-World-Cup-Qatar-2022-Match-Schedule.pdf
internal static class WorldCupConst
{
    public static readonly DateTime WorldCupStartTime = DateTime.Parse("2022-11-20T19:00:00+03:00");
    public static readonly DateTime GroupStageStartTime = WorldCupStartTime;
    public static readonly DateTime Round16Match1StartTime = DateTime.Parse("2022-12-03T18:00:00+03:00");
    public static readonly DateTime Round16Match2StartTime = DateTime.Parse("2022-12-03T22:00:00+03:00");
    public static readonly DateTime Round16Match3StartTime = DateTime.Parse("2022-12-04T18:00:00+03:00");
    public static readonly DateTime Round8StartTime = DateTime.Parse("2022-12-09T18:00:00+03:00");
}
