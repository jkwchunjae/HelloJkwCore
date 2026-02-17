namespace ProjectWorldCup;

// https://digitalhub.fifa.com/m/464f16f856f5ed05/original/FIFA-World-Cup-Qatar-2022-Match-Schedule.pdf
internal static class WorldCupConst
{
    public static readonly DateTime WorldCupStartTime = DateTime.Parse("2026-06-11T19:00:00Z");
    public static readonly DateTime GroupStageStartTime = WorldCupStartTime;
    public static readonly DateTime GroupStageLastTime = DateTime.Parse("2026-12-02T22:00:00+03:00");
    public static readonly DateTime Round32Match1StartTime = DateTime.Parse("2026-12-03T18:00:00+03:00");
    public static readonly DateTime Round32Match2StartTime = DateTime.Parse("2026-12-03T22:00:00+03:00");
    public static readonly DateTime Round32Match3StartTime = DateTime.Parse("2026-12-04T18:00:00+03:00");
    public static readonly DateTime Round16Match1StartTime = DateTime.Parse("2026-12-03T18:00:00+03:00");
    public static readonly DateTime Round16Match2StartTime = DateTime.Parse("2026-12-03T22:00:00+03:00");
    public static readonly DateTime Round16Match3StartTime = DateTime.Parse("2026-12-04T18:00:00+03:00");
    public static readonly DateTime Round8StartTime = DateTime.Parse("2026-12-09T18:00:00+03:00");
    public static readonly DateTime Round4StartTime = DateTime.Parse("2026-12-13T22:00:00+03:00");
    public static readonly DateTime FinalStartTime = DateTime.Parse("2026-12-18T18:00:00+03:00");
}
