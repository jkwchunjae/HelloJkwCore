namespace ProjectPingpong.Pages.Competition;

public partial class PpCompetitionSettings : JkwPageBase
{
    [Parameter] public string? CompetitionNameText { get; set; }

    [Inject] IPpService? Service { get; set; }

    private CompetitionData? CompetitionData;
    private CompetitionUpdator? CompetitionUpdator;

    protected override async Task OnPageInitializedAsync()
    {
        CompetitionData = await Service!.GetCompetitionDataAsync(new CompetitionName(CompetitionNameText ?? string.Empty));
        if (CompetitionData == null)
            return;
        CompetitionData.PlayerList ??= new();
        CompetitionUpdator = new CompetitionUpdator(CompetitionData!, Service);
    }

}
