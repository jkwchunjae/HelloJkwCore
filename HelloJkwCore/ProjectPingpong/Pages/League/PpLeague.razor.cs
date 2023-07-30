namespace ProjectPingpong.Pages.League;

public partial class PpLeague : JkwPageBase
{
    [Parameter] public string? LeagueIdText { get; set; }

    [Inject] public IPpService? Service { get; set; }

    private LeagueData? League { get; set; }

    protected override async Task OnPageParametersSetAsync()
    {
        League = await Service!.GetLeagueDataAsync(new LeagueId(LeagueIdText!.Replace("__", ".")));
    }
}
