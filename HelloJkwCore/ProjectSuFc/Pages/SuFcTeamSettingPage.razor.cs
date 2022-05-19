using Microsoft.JSInterop;

namespace ProjectSuFc.Pages;

public partial class SuFcTeamSettingPage : JkwPageBase
{
    [Inject] IJSRuntime JS { get; set; }
    [Inject] ISuFcService Service { get; set; }

    TeamSettingOption teamSettingOption = new();
    List<Member> Members = new();

    protected override async Task OnPageInitializedAsync()
    {
        teamSettingOption = await Service.GetTeamSettingOption();

        teamSettingOption.SplitOptions ??= new List<MergeSplitOption>();
        while (teamSettingOption.SplitOptions.Count < 3)
        {
            teamSettingOption.SplitOptions.Add(new MergeSplitOption());
        }

        Members = await Service.GetAllMember();
    }

    private async Task Check(MergeSplitOption option, Member member)
    {
        option.Names ??= new();
        option.Names.Add(member.Name);

        await Service.SaveTeamSettingOption(teamSettingOption);
    }

    private async Task Uncheck(MergeSplitOption option, Member member)
    {
        option.Names ??= new();
        option.Names.Remove(member.Name);

        await Service.SaveTeamSettingOption(teamSettingOption);
    }
}
