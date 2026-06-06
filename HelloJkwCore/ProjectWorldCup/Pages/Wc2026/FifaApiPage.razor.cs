namespace ProjectWorldCup.Pages.Wc2026;

public partial class FifaApiPage : JkwPageBase
{
    [Inject] private IFifa Fifa { get; set; }

    private List<FifaApiStage> Stages { get; set; } = new();
    private int ActivePanelIndex { get; set; }
    private Exception Error { get; set; }

    protected override async Task OnPageInitializedAsync()
    {
        try
        {
            var groupStageTask = Fifa.GetGroupStageMatchesAsync();
            var round32Task = Fifa.GetRound32MatchesAsync();
            var round16Task = Fifa.GetRound16MatchesAsync();
            var finalTask = Fifa.GetFinalMatchesAsync();

            await Task.WhenAll(groupStageTask, round32Task, round16Task, finalTask);

            Stages = new()
            {
                CreateStage("GroupStage", groupStageTask.Result),
                CreateStage("Knockouts (32)", round32Task.Result),
                CreateStage("Knockouts (16)", round16Task.Result),
                CreateStage("Knockouts (8 - final)", finalTask.Result),
            };
            ActivePanelIndex = FifaApiTabSelector.GetActivePanelIndex(Stages.Select(stage => stage.Rows).ToList(), GetNowKst());
        }
        catch (Exception ex)
        {
            Error = ex;
        }
    }

    private static FifaApiStage CreateStage(string title, IEnumerable<FifaMatchData> matches)
    {
        var rows = FifaApiMatchRow.CreateRows(matches);

        return new FifaApiStage(title, rows);
    }

    private record FifaApiStage(string Title, List<FifaApiMatchRow> Rows);

    private static DateTime GetNowKst()
    {
        return DateTime.UtcNow.AddHours(9);
    }
}
