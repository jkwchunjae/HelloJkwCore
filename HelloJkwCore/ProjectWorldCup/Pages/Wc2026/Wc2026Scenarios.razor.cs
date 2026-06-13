namespace ProjectWorldCup.Pages.Wc2026;

public partial class Wc2026Scenarios : JkwPageBase
{
    [Inject] private IFifa Fifa { get; set; }
    [Inject] private IWc2026ScenarioStorage ScenarioStorage { get; set; }

    private List<Wc2026ScenarioGroup> Groups { get; set; } = new();
    private Exception Error { get; set; }
    private Exception SaveError { get; set; }

    protected override async Task OnPageInitializedAsync()
    {
        try
        {
            var matches = await Fifa.GetGroupStageMatchesAsync();
            Groups = Wc2026ScenarioGroup.CreateGroups(matches);

            if (IsAuthenticated)
            {
                var savedScenario = await ScenarioStorage.LoadAsync(User);
                Wc2026ScenarioGroup.ApplySavedScenario(Groups, savedScenario);
            }
        }
        catch (Exception ex)
        {
            Error = ex;
        }
    }

    private async Task OnScoreIncrement(Wc2026ScenarioMatch match, bool isHomeScore)
    {
        if (isHomeScore)
        {
            match.SetHomeScore(match.HomeScore + 1);
        }
        else
        {
            match.SetAwayScore(match.AwayScore + 1);
        }

        await SaveScenarioAsync();
    }

    private async Task ResetScore(Wc2026ScenarioMatch match)
    {
        match.SetHomeScore(0);
        match.SetAwayScore(0);

        await SaveScenarioAsync();
    }

    private async Task SaveScenarioAsync()
    {
        if (!IsAuthenticated)
        {
            return;
        }

        try
        {
            SaveError = null;
            await ScenarioStorage.SaveAsync(User, Groups);
        }
        catch (Exception ex)
        {
            SaveError = ex;
        }
    }
}
