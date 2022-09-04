namespace ProjectWorldCup.FifaLibrary;

public interface IFifa
{
    Task<List<OverviewGroup>> GetGroupOverview();
    Task<List<QualifiedTeam>> GetQualifiedTeamsAsync();
    Task<List<RankingTeamData>> GetLastRankingAsync(Gender gender);
    Task<List<FifaMatchData>> GetGroupStageMatchesAsync();
    Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync();
    Task<List<FifaStandingData>> GetStandingDataAsync();
}