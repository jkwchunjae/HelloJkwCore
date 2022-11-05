namespace ProjectWorldCup;

public interface IWorldCupService
{
    Task<List<SimpleGroup>> GetSimpleGroupsAsync();
    //Task<List<Team>> Get2022QualifiedTeamsAsync();
    //Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender);
    //Task<List<GroupMatch>> GetGroupStageMatchesAsync();
    Task<List<KnMatch>> GetKnockoutStageMatchesAsync();
    Task<List<KnMatch>> GetRound16MatchesAsync();
    /// <summary> 8강 경기 </summary>
    Task<List<KnMatch>> GetQuarterFinalMatchesAsync();
    /// <summary> 8강, 4강, 결승 경기 </summary>
    Task<List<KnMatch>> GetFinalMatchesAsync();
    Task<List<WcGroup>> GetGroupsAsync();
    //Task<KnockoutStageData> GetKnockoutStageDataAsync();
}