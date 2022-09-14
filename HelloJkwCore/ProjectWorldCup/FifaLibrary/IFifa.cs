namespace ProjectWorldCup.FifaLibrary;

public interface IFifa
{
    Task<List<OverviewGroup>> GetGroupOverview();
    Task<List<QualifiedTeam>> GetQualifiedTeamsAsync();
    Task<List<RankingTeamData>> GetLastRankingAsync(Gender gender);
    Task<List<FifaMatchData>> GetGroupStageMatchesAsync();
    Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync();
    Task<List<FifaMatchData>> GetRound16MatchesAsync();
    Task<List<FifaMatchData>> GetFinalMatchesAsync();
    Task<List<FifaStandingData>> GetStandingDataAsync();

    Task<IEnumerable<string>> GetFailoverList();
    Task<string> GetFailoverData(string filename);
    Task SaveFailoverData(string title, string value);
}