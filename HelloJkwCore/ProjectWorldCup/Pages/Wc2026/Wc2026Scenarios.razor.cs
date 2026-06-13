namespace ProjectWorldCup.Pages.Wc2026;

public partial class Wc2026Scenarios : JkwPageBase
{
    [Inject] private IFifa Fifa { get; set; }

    private List<Wc2026ScenarioGroup> Groups { get; set; } = new();
    private Exception Error { get; set; }

    protected override async Task OnPageInitializedAsync()
    {
        try
        {
            var matches = await Fifa.GetGroupStageMatchesAsync();
            Groups = Wc2026ScenarioGroup.CreateGroups(matches);
        }
        catch (Exception ex)
        {
            Error = ex;
        }
    }

    private static void OnScoreInput(Wc2026ScenarioMatch match, bool isHomeScore, string value)
    {
        var score = int.TryParse(value, out var parsedScore)
            ? parsedScore
            : 0;

        if (isHomeScore)
        {
            match.SetHomeScore(score);
        }
        else
        {
            match.SetAwayScore(score);
        }
    }
}
