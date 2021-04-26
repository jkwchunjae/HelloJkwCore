using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcTeamMaker : JkwPageBase
    {
        [Inject]
        private ISuFcService SuFcService { get; set; }

        private readonly List<(TeamMakerStrategy Strategy, string Name)> TeamMakerStrategies = new List<(TeamMakerStrategy, string)>
        {
            (TeamMakerStrategy.FullRandom, "완전 랜덤"),
        };

        TeamResult TeamResult = null;
        int TeamCount = 3;
        TeamMakerStrategy TeamMakerStrategy;

        async Task MakeTeam()
        {
            var players = await SuFcService.GetMembers();
            TeamResult = await SuFcService.MakeTeamAsync(players, TeamCount, TeamMakerStrategy);
        }
    }
}
