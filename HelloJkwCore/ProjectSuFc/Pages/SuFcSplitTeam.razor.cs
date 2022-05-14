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

        var result = await Service.MakeTeam(
            players: CheckedList,
            teamCount: teamSize,
            strategy: TeamMakerStrategy.FullRandom,
            option: null);

        var arrayLength = result.GroupByTeam.Max(team => team.Value.Count);

        Teams = new MemberName[arrayLength][];
        for (int i = 0; i < arrayLength; i++)
        {
            Teams[i] = new MemberName[result.TeamNames.Count];
            for (var j = 0; j < result.TeamNames.Count; j++)
            {
                var teamName = result.TeamNames[j];
                Teams[i][j] = result.GroupByTeam[teamName].GetMemberName(i);
            }
        }
        TeamResult = result;
    }
}

static class Extensions
{
    public static MemberName GetMemberName(this List<MemberName> members, int index)
    {
        if (index < members.Count)
        {
            return members[index];
        }
        else
        {
            return null;
        }
    }
}
