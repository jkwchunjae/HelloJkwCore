namespace ProjectPingpong.Pages;

public partial class PpCompetition : JkwPageBase
{
    [Parameter] public string? CompetitionNameText { get; set; }

    [Inject] public IPpService? Service { get; set; }

    private CompetitionData? CompetitionData;

    protected override async Task OnPageInitializedAsync()
    {
        CompetitionData = await Service!.GetCompetitionDataAsync(new CompetitionName(CompetitionNameText ?? string.Empty));
    }
}
