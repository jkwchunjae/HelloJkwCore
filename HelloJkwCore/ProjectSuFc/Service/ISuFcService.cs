﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public interface ISuFcService
    {
        #region Member
        Task<List<Member>> GetAllMember();
        Task<bool> SaveMember(Member player);
        #endregion

        #region Team
        Task<TeamResult> MakeTeam(List<Member> players, int teamCount, TeamMakerStrategy strategy);
        Task<List<TeamResultSaveFile>> GetAllTeamResult();
        Task<bool> SaveTeamResult(TeamResultSaveFile saveFile);
        Task<TeamResultSaveFile> FindTeamResult(string title);
        #endregion

        #region Schedule
        Task<List<ScheduleData>> GetAllSchedule();
        Task<bool> SaveSchedule(ScheduleData schedule);
        #endregion
    }
}