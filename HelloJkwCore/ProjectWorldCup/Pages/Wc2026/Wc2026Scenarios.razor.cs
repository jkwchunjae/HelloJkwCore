using Microsoft.AspNetCore.Identity;

namespace ProjectWorldCup.Pages.Wc2026;

public partial class Wc2026Scenarios : JkwPageBase
{
    [Inject] private IFifa Fifa { get; set; }
    [Inject] private IWc2026ScenarioStorage ScenarioStorage { get; set; }
    [Inject(Key = "2026")] private IBettingGroupStageService GroupStageService { get; set; }
    [Inject] private UserManager<AppUser> UserManager { get; set; }

    private List<Wc2026ScenarioGroup> Groups { get; set; } = new();
    private List<WcBettingItem<GroupTeam>> BettingItems { get; set; } = new();
    private List<IWcBettingItem<ITeam>> SimulatedBettingRows { get; set; } = new();
    private Wc2026Round32SimulationResult Round32Simulation { get; set; } = Wc2026Round32SimulationResult.Blocked("");
    private Exception Error { get; set; }
    private Exception SaveError { get; set; }
    private Exception SimulationError { get; set; }
    private bool SimulationPanelOpen { get; set; } = false;
    private bool Round32SimulationPanelOpen { get; set; } = false;
    private bool SimulationLoaded { get; set; }

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

            await LoadBettingSimulationAsync();
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

        RefreshSimulation();
        await SaveScenarioAsync();
    }

    private async Task ResetScore(Wc2026ScenarioMatch match)
    {
        match.SetHomeScore(0);
        match.SetAwayScore(0);

        RefreshSimulation();
        await SaveScenarioAsync();
    }

    private async Task LoadBettingSimulationAsync()
    {
        try
        {
            SimulationLoaded = false;
            SimulationError = null;

            BettingItems = await GroupStageService.GetAllBettingsAsync();
            await FillBettingUsersAsync(BettingItems);
            RefreshSimulation();
        }
        catch (Exception ex)
        {
            SimulationError = ex;
            BettingItems = new();
            SimulatedBettingRows = new();
            RefreshRound32Simulation();
        }
        finally
        {
            SimulationLoaded = true;
        }
    }

    private async Task FillBettingUsersAsync(IEnumerable<WcBettingItem<GroupTeam>> bettingItems)
    {
        var users = (await UserManager.GetUsersInRoleAsync("all"))
            .ToDictionary(user => user.Id);

        foreach (var item in bettingItems.Where(item =>
            item?.User != null
            && users.ContainsKey(item.User.Id)))
        {
            item.User = users[item.User.Id];
        }
    }

    private void RefreshSimulation()
    {
        var simulationItems = Wc2026ScenarioBettingSimulator.CreateSimulationItems(BettingItems, Groups);
        SimulatedBettingRows = new BettingResultTable<IWcBettingItem<ITeam>>(simulationItems).ToList();
        RefreshRound32Simulation();
    }

    private void RefreshRound32Simulation()
    {
        Round32Simulation = Wc2026ScenarioRound32Simulator.CreateSimulation(Groups);
    }

    private void ToggleSimulationPanel()
    {
        SimulationPanelOpen = !SimulationPanelOpen;
        if (SimulationPanelOpen)
        {
            Round32SimulationPanelOpen = false;
        }
    }

    private void ToggleRound32SimulationPanel()
    {
        Round32SimulationPanelOpen = !Round32SimulationPanelOpen;
        if (Round32SimulationPanelOpen)
        {
            SimulationPanelOpen = false;
        }
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
