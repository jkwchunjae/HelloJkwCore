namespace ProjectSuFc;

public interface ISuFcService
{
    #region Member
    Task<List<Member>> GetAllMember();
    Task<Member> FindMember(MemberName memberName);
    Task<bool> SaveMember(Member player);
    Task<bool> DeleteMember(MemberName memberName);
    #endregion

    #region Team
    Task<TeamResult> MakeTeam(List<MemberName> players, int teamCount, TeamMakerStrategy strategy, TeamSettingOption option);
    Task<List<TeamResult>> GetAllTeamResult();
    Task<bool> SaveTeamResult(TeamResult saveFile);
    Task<TeamResult> FindTeamResult(string title);
    #endregion

    #region Schedule
    Task<List<ScheduleData>> GetAllSchedule();
    Task<bool> SaveSchedule(ScheduleData schedule);
    Task<ScheduleData> GetSchedule(int scheduleId);
    Task<(bool Success, ScheduleData Result)> Vote(ScheduleData schedule, MemberName memberName, ScheduleMemberStatus memberStatus);
    #endregion

    #region TeamSettingOption
    Task<TeamSettingOption> GetTeamSettingOption();
    Task SaveTeamSettingOption(TeamSettingOption option);
    #endregion
}