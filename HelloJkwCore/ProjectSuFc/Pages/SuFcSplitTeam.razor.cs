﻿using Microsoft.JSInterop;

namespace ProjectSuFc.Pages;

public partial class SuFcSplitTeam : JkwPageBase
{
    [Inject] ISuFcService Service { get; set; }

    List<Member> Members = new();
    List<MemberName> CheckedList = new();
    MemberName[][] Teams = default;
    TeamResult TeamResult = null;

    protected override async Task OnPageInitializedAsync()
    {
        Members = await Service.GetAllMember();
    }

    Task CheckMember(Member user)
    {
        CheckedList.Add(user.Name);
        return Task.CompletedTask;

    }

    Task UncheckMember(Member user)
    {
        CheckedList.RemoveAll(x => x == user.Name);
        return Task.CompletedTask;
    }

    async Task MakeTeam(int teamSize)
    {
        TeamResult = null;

        teamSize = Math.Min(teamSize, CheckedList.Count);

        if (teamSize <= 1)
            return;

        var teamSettingOption = await Service.GetTeamSettingOption();

        var result = await Service.MakeTeam(
            players: CheckedList,
            teamCount: teamSize,
            strategy: TeamMakerStrategy.TeamSetting,
            option: teamSettingOption);

        Teams = result.NamesForTable;
        TeamResult = result;
    }
}
