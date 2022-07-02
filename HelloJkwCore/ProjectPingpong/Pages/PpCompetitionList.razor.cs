namespace ProjectPingpong.Pages;

public partial class PpCompetitionList : JkwPageBase
{
    [Inject] IPpService? Service { get; set; }

    List<CompetitionName> CompetitionNameList = new();

    protected override async Task OnPageInitializedAsync()
    {
        CompetitionNameList = await Service!.GetAllCompetitionsAsync();
    }
}
