namespace ProjectWorldCup;

public interface IWorldCupService
{
    Task<List<SimpleGroup>> GetSimpleGroupsAsync();
    //Task<List<Team>> Get2022QualifiedTeamsAsync();
    //Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender);
    //Task<List<GroupMatch>> GetGroupStageMatchesAsync();
    //Task<List<KnMatch>> GetKnockOutStageMatchesAsync();
    Task<List<WcGroup>> GetGroupsAsync();
    //Task<KnockoutStageData> GetKnockoutStageDataAsync();
}