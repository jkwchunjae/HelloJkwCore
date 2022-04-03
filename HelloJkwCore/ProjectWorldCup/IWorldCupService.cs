namespace ProjectWorldCup;

public interface IWorldCupService
{
    Task<List<Team>> Get2022QualifiedTeamsAsync();
    Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender);
    Task<List<GroupMatch>> GetGroupStageMatchesAsync();
    Task<List<KnMatch>> GetKnockOutStageMatchesAsync();
    Task<List<WcGroup>> GetGroupsAsync();
    Task<KnockoutStageData> GetKnockoutStageDataAsync();

    #region Result 2018
    Task<List<WcBettingItem>> Get2018GroupStageBettingResult();
    Task<List<WcBettingItem>> Get2018Round16BettingResult();
    Task<List<WcFinalBettingItem>> Get2018FinalBettingResult();
    #endregion
}