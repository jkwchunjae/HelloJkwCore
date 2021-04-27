﻿using JkwExtensions;
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
        string Title = string.Empty;
        TeamMakerStrategy TeamMakerStrategy;

        async Task MakeTeam()
        {
            var players = await SuFcService.GetAllMember();
            TeamResult = await SuFcService.MakeTeam(players, TeamCount, TeamMakerStrategy);
        }

        async Task SaveFile()
        {
            if (Title.HasInvalidFileNameChar() || Title.Empty())
            {
                return;
            }
            var saveFile = new TeamResultSaveFile
            {
                Title = Title,
                Result = TeamResult,
                CreateTime = DateTime.Now,
                Status = TeamResultStatus.Feature,
            };

            await SuFcService.SaveTeamResult(saveFile);

            Navi.NavigateTo("/sufc");
        }
    }
}