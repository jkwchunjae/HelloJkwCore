using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    interface ITeamMaker
    {
        Task<TeamResult> MakeTeamAsync(List<MemberName> members, int teamCount);
    }

    public abstract class TeamMaker : ITeamMaker
    {
        private ISuFcService SuFcService { get; set; }

        public abstract Task<TeamResult> MakeTeamAsync(List<MemberName> members, int teamCount);

        public TeamMaker(ISuFcService service)
        {
            SuFcService = service;
        }
    }
}
