namespace ProjectPingpong.Pages;

public partial class PpCompetitionSettings : JkwPageBase
{
    [Parameter] public string? CompetitionNameText { get; set; }

    [Inject] IPpService? Service { get; set; }
    [Inject] IDialogService? DialogService { get; set; }

    private CompetitionData? CompetitionData;
    private CompetitionUpdator? CompetitionUpdator;
    private string _inputPlayerName = string.Empty;
    private int _inputPlayerClass = default;
    private MudTextField<string>? inputElement;

    protected override async Task OnPageInitializedAsync()
    {
        CompetitionData = await Service!.GetCompetitionDataAsync(new CompetitionName(CompetitionNameText ?? string.Empty));
        if (CompetitionData == null)
            return;
        CompetitionData.PlayerList ??= new();
        CompetitionUpdator = new CompetitionUpdator(CompetitionData!, Service);
    }

    private async Task AddPlayer(string playerName, int playerClass)
    {
        var player = new Player
        {
            Name = new PlayerName(playerName),
            Class = playerClass,
        };
        CompetitionData = await CompetitionUpdator!.AddPlayers(new[] { player });
        _inputPlayerName = string.Empty;
        await inputElement!.FocusAsync();
    }

    private async Task RemovePlayer(PlayerName playerName)
    {
        var param = new DialogParameters
        {
            ["Content"] = $"{playerName}님을 삭제하시겠습니까?",
            ["SubmitText"] = "삭제",
            ["SubmitColor"] = Color.Error,
        };
        var dialog = DialogService!.Show<PpConfirmDialog>("참가자 관리", param);
        var result = await dialog.Result;

        if (result.Data is bool deleteMember && deleteMember)
        {
            CompetitionData = await CompetitionUpdator!.RemovePlayer(playerName);
            _inputPlayerName = string.Empty;
            await inputElement!.FocusAsync();
        }
    }
}
