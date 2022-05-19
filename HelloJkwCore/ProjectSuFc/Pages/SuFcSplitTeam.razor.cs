using Microsoft.JSInterop;

namespace ProjectSuFc.Pages;

public partial class SuFcSplitTeam : JkwPageBase
{
    [Inject] IJSRuntime JS { get; set; }
    [Inject] ISuFcService Service { get; set; }

    List<Member> Members = new();
    List<MemberName> CheckedList = new();
    MemberName[][] Teams = new MemberName[1][];

    TeamResult TeamResult { get; set; }

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
        CheckedList = CheckedList
            .Where(memberName => memberName != user.Name)
            .ToList();
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

