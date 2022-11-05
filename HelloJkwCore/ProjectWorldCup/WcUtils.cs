namespace ProjectWorldCup;

internal static class WcUtils
{
    public static RemainTime CalcRemainTime(DateTime goalTime, DateTime now)
    {
        if (goalTime < now)
        {
            return new RemainTime
            {
                HasRemainTime = false,
            };
        }
        else
        {
            return new RemainTime
            {
                HasRemainTime = true,
                Remain = goalTime - now,
            };
        }
    }
}

internal class RemainTime
{
    public bool HasRemainTime { get; set; }
    public TimeSpan Remain { get; set; }
}