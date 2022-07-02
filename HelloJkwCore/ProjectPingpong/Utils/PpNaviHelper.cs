namespace ProjectPingpong.Utils;

internal static class PpNaviHelper
{
    static readonly string jangtak9 = "jangtak9";

    public static string CompetitionPage(CompetitionName competitionName)
    {
        return $"/{jangtak9}/competition/{competitionName}";
    }
    public static void GotoCompetitionPage(this NavigationManager navi, CompetitionName competitionName)
    {
        navi.NavigateTo(CompetitionPage(competitionName));
    }
    public static string CompetitionSettingPage(CompetitionName competitionName)
    {
        return $"/{jangtak9}/competition/{competitionName}/settings";
    }
    public static void GotoCompetitionSettingPage(this NavigationManager navi, CompetitionName competitionName)
    {
        navi.NavigateTo(CompetitionPage(competitionName));
    }
}
