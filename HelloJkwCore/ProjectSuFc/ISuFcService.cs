using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public interface ISuFcService
    {
        #region Member
        Task<List<Member>> GetMembers();
        Task<Member> FindMember(string name);
        Task<Member> AddMember(Member player);
        Task<Member> UpdateMember(Member player);
        Task<bool> DeleteMember(Member player);
        #endregion

        Task<TeamResult> MakeTeam(List<Member> players, int teamCount, TeamMakerStrategy strategy);
        Task<List<TeamResultSaveFile>> GetAllTeamResult();
        Task<bool> SaveTeamResult(TeamResultSaveFile saveFile);
        Task<TeamResultSaveFile> FindTeamResult(string title);
    }
}
