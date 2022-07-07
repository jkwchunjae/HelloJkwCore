namespace ProjectPingpong.Pages.Match;

public partial class PpMatchCenter : JkwPageBase
{
    [Parameter] public string? MatchIdText { get; set; }
    private MatchId MatchId { get; set; } = MatchId.Default;

    [Inject] private IPpMatchService? MatchService { get; set; }
    [Inject] private IDialogService? DialogService { get; set; }

    private MatchData? Match { get; set; }
    private IPpNotifier<MatchData>? MatchNotify { get; set; }
    private string? LeagueId { get; set; } = null;

    protected override async Task OnPageParametersSetAsync()
    {
        if (MatchIdText != null)
        {
            MatchId = MatchId.FromUrl(MatchIdText);
            Match = await MatchService!.GetMatchDataAsync<MatchData>(MatchId);

            MatchNotify = MatchService.Watch(MatchId);
            MatchNotify.Updated += MatchUpdator_Updated;
        }

        if (Navi.TryGetQueryString("league", out string leagueId))
        {
            LeagueId = leagueId;
        }
    }

    private async void MatchUpdator_Updated(object? sender, MatchData data)
    {
        await InvokeAsync(() =>
        {
            Match = data;
            StateHasChanged();
        });
    }

    protected override void OnPageDispose()
    {
        if (MatchId != MatchId.Default && MatchNotify != null)
        {
            MatchNotify.Updated -= MatchUpdator_Updated;
            MatchService!.Unwatch(MatchId);
            MatchNotify = null;
        }
    }

    private async Task StartMatch()
    {
        await StartNewSet();
    }
    private async Task FinishMatch()
    {
        Match!.Finished = true;
        await MatchService!.UpdateMatchDataAsync<MatchData>(Match!.Id, data => Match);
    }
    private async Task RestartMatch()
    {
        Match!.Finished = false;
        await MatchService!.UpdateMatchDataAsync<MatchData>(Match!.Id, data => Match);
    }
    private async Task StartNewSet()
    {
        Match!.StartNewSet();
        await MatchService!.UpdateMatchDataAsync<MatchData>(Match!.Id, data => Match);
    }
    private async Task CancelSet()
    {
        var param = new DialogParameters
        {
            ["Content"] = $"게임을 취소하시겠습니까?",
            ["SubmitText"] = "게임 취소",
            ["SubmitColor"] = Color.Warning,
            ["SubmitVariant"] = Variant.Filled,
            ["CancelText"] = "닫기",
        };
        var dialog = DialogService!.Show<PpConfirmDialog>("경기 진행", param);
        var result = await dialog.Result;

        if (result.Data is bool confirm && confirm)
        {
            Match!.CancelSet();
            await MatchService!.UpdateMatchDataAsync<MatchData>(Match!.Id, data => Match);
        }
    }
    private async Task IncreaseScore(Player? player)
    {
        if (Match?.LeftPlayer == player)
        {
            Match?.IncreaseLeftScore();
        }
        else if (Match?.RightPlayer == player)
        {
            Match?.IncreaseRightScore();
        }

        var finishData = Match?.CheckGameFinish();
        if (finishData?.Finished ?? false && finishData?.Winner != null)
        {
            var winner = finishData?.Winner;
            var param = new DialogParameters
            {
                ["Content"] = $"{winner?.Name}님이 이번 세트에서 승리하셨습니다. 결과를 확정하시겠습니까?",
                ["SubmitText"] = "확정",
                ["SubmitColor"] = Color.Success,
            };
            var dialog = DialogService!.Show<PpConfirmDialog>("경기 진행", param);
            var result = await dialog.Result;

            if (result.Data is bool confirm && confirm)
            {
                Match?.ConfirmSetResult();
            }
        }
        await MatchService!.UpdateMatchDataAsync<MatchData>(Match!.Id, data => Match);
    }
}
