namespace ProjectWorldCup;

public interface IFifa
{
    Task<List<QualifiedTeam>> GetQualifiedTeamsAsync();
    Task<List<RankingTeamData>> GetLastRankingAsync(Gender gender);
    Task<List<FifaMatchData>> GetGroupStageMatchesAsync();
    Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync();
}