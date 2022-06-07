using Microsoft.JSInterop;

namespace ProjectSuFc.Pages;

public partial class SuFcSplitTeam : JkwPageBase
{
    [Inject] ISuFcService Service { get; set; }

    List<Member> Members = new();
    List<MemberName> CheckedList = new();
    MemberName[][] Teams = default;
    TeamResult TeamResult = null;
    TeamSettingOption TeamSettingOption;

    protected override async Task OnPageInitializedAsync()
    {
        Members = await Service.GetAllMember();
        TeamSettingOption = await Service.GetTeamSettingOption();
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

        TeamSettingOption = await Service.GetTeamSettingOption();

        var result = await Service.MakeTeam(
            players: CheckedList,
            teamCount: teamSize,
            strategy: TeamMakerStrategy.TeamSettingAndClass,
            option: TeamSettingOption);

        Teams = result.NamesForTable;
        TeamResult = result;
    }

    bool VisibleMemberOption = false;
    Task ToggleMemberOption()
    {
        VisibleMemberOption = !VisibleMemberOption;
        return Task.CompletedTask;
    }

    string GetMemberOption(MemberName name)
    {
        string result = string.Empty;

        var splitData = TeamSettingOption.SplitOptions
            .Select((option, index) => (option, index))
            .FirstOrDefault(x => x.option?.Names?.Contains(name) ?? false);

        if (splitData != default)
        {
            result += $"{splitData.index + 1}";
        }

        var groupData = TeamSettingOption.ClassOptions
            .Select((option, index) => (option, index))
            .FirstOrDefault(x => x.option?.Names?.Contains(name) ?? false);

        if (groupData != default)
        {
            var groupName = "ABCDEFGHIJ".Substring(groupData.index, 1);
            result += groupName;
        }

        return result;
    }
}

