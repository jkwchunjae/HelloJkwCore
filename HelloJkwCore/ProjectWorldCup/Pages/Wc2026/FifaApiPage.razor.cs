using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Wc2026;

public partial class FifaApiPage : JkwPageBase, IAsyncDisposable
{
    private const string InitialScrollTargetClass = "fifa-api-initial-scroll-target";

    [Inject] private IFifa Fifa { get; set; }

    private List<FifaApiStage> Stages { get; set; } = new();
    private int ActivePanelIndex { get; set; }
    private Exception Error { get; set; }
    private IJSObjectReference _jsModule;
    private FifaApiMatchRow _initialScrollTargetRow;
    private bool _initialScrollApplied;

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

            var stageRows = Stages.Select(stage => (IReadOnlyList<FifaApiMatchRow>)stage.Rows).ToList();
            var nowKst = GetNowKst();
            ActivePanelIndex = FifaApiTabSelector.GetActivePanelIndex(stageRows, nowKst);

            var scrollTarget = FifaApiTabSelector.GetInitialScrollTarget(stageRows, nowKst);
            if (scrollTarget is { } target)
            {
                ActivePanelIndex = target.StageIndex;
                _initialScrollTargetRow = Stages[target.StageIndex].Rows[target.RowIndex];
            }
        }
        catch (Exception ex)
        {
            Error = ex;
        }
    }

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (_initialScrollApplied || Error != null || _initialScrollTargetRow == null)
        {
            return;
        }

        _initialScrollApplied = true;
        _jsModule ??= await Js.InvokeAsync<IJSObjectReference>("import", "./_content/ProjectWorldCup/js/fifaApiPage.js");
        await _jsModule.InvokeVoidAsync("scrollToInitialMatch", $".{InitialScrollTargetClass}");
    }

    private string GetInitialScrollRowClass(FifaApiMatchRow row)
    {
        return ReferenceEquals(row, _initialScrollTargetRow)
            ? InitialScrollTargetClass
            : string.Empty;
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

    public async ValueTask DisposeAsync()
    {
        if (_jsModule == null)
        {
            return;
        }

        try
        {
            await _jsModule.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
