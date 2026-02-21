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
    /// <summary> 2026 전용: GetStandingDataAsync만 사용해 12개 그룹 48팀을 WcGroup 목록으로 변환 </summary>
    Task<List<WcGroup>> GetGroupsFromStandingAsync();
    /// <summary> 2026 전용: 32강 경기 목록 </summary>
    Task<List<KnMatch>> GetRound32MatchesAsync();
    //Task<KnockoutStageData> GetKnockoutStageDataAsync();
}