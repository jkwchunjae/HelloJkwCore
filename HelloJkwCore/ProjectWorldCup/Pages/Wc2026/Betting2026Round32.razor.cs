using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace ProjectWorldCup.Pages.Wc2026;

public partial class Betting2026Round32 : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject(Key = "2026-round32")] private IBettingRound16Service BettingRound32Service { get; set; }
    [Inject] private UserManager<AppUser> UserManager { get; set; }

    List<KnMatch> Round32Matches = new();
    WcBettingItem<Team> BettingItem = new();
    List<WcBettingItem<Team>> BettingItems = null;

    bool AllMatchesAreSetted => Round32Matches.Count == 16
        && Round32Matches.SelectMany(m => m.Teams)
        .All(team => team?.Id != null && team.Id.Length == 3);

    bool[] TimeOver = new bool[16];
    bool CheckRandom1 = false;
    bool CheckRandom2 = false;
    bool CheckRandom3 = false;

    private bool ShowAiHelperDialog { get; set; }
    private bool IsSubmittingAiResult { get; set; }
    private MudDialog AiHelperDialog { get; set; }
    private string AiHelpPrompt => BuildAiHelpPrompt();
    private string AiResultInput { get; set; } = "";
    private string AiSubmitMessage { get; set; } = "";
    private Severity AiSubmitSeverity { get; set; } = Severity.Info;
    private bool CanUseAiHelper => IsAuthenticated
        && TimeOver.All(timeOver => timeOver == false)
        && AllMatchesAreSetted
        && !(BettingItem?.IsRandom ?? false);
    private DialogOptions AiHelperDialogOptions { get; } = new()
    {
        CloseButton = true,
        CloseOnEscapeKey = true,
        BackdropClick = false,
        FullWidth = true,
        MaxWidth = MaxWidth.Medium,
    };

    protected override async Task OnPageInitializedAsync()
    {
        Round32Matches = await WorldCupService.GetRound32MatchesAsync();
        if (IsAuthenticated)
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            BettingItem = await BettingRound32Service.GetBettingAsync(bettingUser);
        }
        var bettingItems = await BettingRound32Service.GetAllBettingsAsync();
        var users = (await UserManager.GetUsersInRoleAsync("all"))
            .ToDictionary(user => user.Id);
        foreach (var item in bettingItems.Where(item => users.ContainsKey(item.User.Id)))
        {
            item.User = users[item.User.Id];
        }
        BettingItems = bettingItems;
    }

    private void ShowLoginRequireMessage()
    {
        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add("로그인을 해주세요", Severity.Warning);
    }

    private async Task PickTeamAsync(int matchIndex, Team team)
    {
        if (TimeOver[matchIndex])
            return;

        if (team?.Id == null)
            return;

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Round32);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                return;
            }
            BettingItem = await BettingRound32Service.PickTeamAsync(bettingUser, team);
            if (BettingItem.Picked.Contains(team))
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("저장되었습니다", Severity.Success);
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private TeamButtonType GetButtonType(Team team)
    {
        if (team == null)
            return TeamButtonType.Disabled;

        if (BettingItem?.Picked?.Any(x => x == team) ?? false)
            return TeamButtonType.Picked;

        var match = Round32Matches.FirstOrDefault(m => m.HomeTeam == team || m.AwayTeam == team);

        if (match == null)
            return TeamButtonType.Disabled;

        if (match.Time < DateTime.UtcNow)
            return TeamButtonType.Disabled;

        return TeamButtonType.Pickable;
    }

    private void OnTimeOver(int index)
    {
        TimeOver[index] = true;
        if (index == 2)
        {
            for (var i = 2; i < 16; i++)
            {
                TimeOver[i] = true;
            }
        }
    }

    private async Task SelectFullRandom()
    {
        if (TimeOver.Any(x => x))
            return;
        if (!AllMatchesAreSetted)
            return;
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Round32);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Round32))
            {
                return;
            }

            BettingItem = await BettingRound32Service.PickRandomAsync(bettingUser);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private void OpenAiHelperDialog()
    {
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }

        if (TimeOver.Any(timeOver => timeOver))
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("선택 시간이 끝났습니다.", Severity.Warning);
            return;
        }

        if (!AllMatchesAreSetted)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("32강 진출팀이 모두 확정된 후에 사용할 수 있습니다.", Severity.Warning);
            return;
        }

        if (BettingItem?.IsRandom ?? false)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("랜덤 선택 이후에는 다시 선택 할 수 없습니다.", Severity.Warning);
            return;
        }

        AiResultInput = "";
        AiSubmitMessage = "";
        ShowAiHelperDialog = true;
    }

    private Task OnAiHelperDialogVisibleChanged(bool visible)
    {
        ShowAiHelperDialog = visible;
        if (!visible)
        {
            AiSubmitMessage = "";
        }

        return Task.CompletedTask;
    }

    private Task CloseAiHelperDialog()
    {
        if (IsSubmittingAiResult)
            return Task.CompletedTask;

        return HideAiHelperDialogAsync(clearInput: false);
    }

    private async Task HideAiHelperDialogAsync(bool clearInput)
    {
        var dialog = AiHelperDialog;
        ShowAiHelperDialog = false;
        AiSubmitMessage = "";
        if (clearInput)
        {
            AiResultInput = "";
        }

        if (dialog is not null)
        {
            await dialog.CloseAsync(DialogResult.Cancel());
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task CopyAiPromptAsync()
    {
        try
        {
            await Js.InvokeVoidAsync("navigator.clipboard.writeText", AiHelpPrompt);
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("프롬프트를 복사했습니다.", Severity.Success);
        }
        catch
        {
            SetAiSubmitMessage("클립보드 복사에 실패했습니다. 프롬프트를 직접 선택해서 복사해주세요.", Severity.Warning);
        }
    }

    private async Task SubmitAiResultAsync()
    {
        if (IsSubmittingAiResult)
            return;

        if (TimeOver.Any(timeOver => timeOver))
        {
            SetAiSubmitMessage("선택 시간이 끝났습니다.", Severity.Error);
            return;
        }

        if (!AllMatchesAreSetted)
        {
            SetAiSubmitMessage("32강 진출팀이 모두 확정된 후에 사용할 수 있습니다.", Severity.Error);
            return;
        }

        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            SetAiSubmitMessage("로그인을 해주세요.", Severity.Error);
            return;
        }

        if (!TryGetAiPickedTeams(out var pickedTeams, out var validationMessage))
        {
            SetAiSubmitMessage(validationMessage, Severity.Error);
            return;
        }

        if (BettingItem?.IsRandom ?? false)
        {
            SetAiSubmitMessage("랜덤 선택 이후에는 다시 선택 할 수 없습니다.", Severity.Error);
            return;
        }

        IsSubmittingAiResult = true;
        try
        {
            var bettingUser = await EnsureRound32BettingJoinedAsync();
            BettingItem = await BettingRound32Service.PickTeamsWithAiAsync(bettingUser, pickedTeams);

            await HideAiHelperDialogAsync(clearInput: true);
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("AI 결과로 16팀을 선택했습니다. (저장되었습니다)", Severity.Success);
        }
        catch (Exception ex)
        {
            SetAiSubmitMessage(ex.Message, Severity.Error);
        }
        finally
        {
            IsSubmittingAiResult = false;
        }
    }

    private async Task<BettingUser> EnsureRound32BettingJoinedAsync()
    {
        var bettingUser = await BettingService.GetBettingUserAsync(User);
        if (bettingUser == null)
        {
            throw new NotJoinedException();
        }

        if (!(bettingUser.JoinedBetting?.Contains(BettingType.Round32) ?? false))
        {
            bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Round32);
        }
        if (!(bettingUser.JoinedBetting?.Contains(BettingType.Round32) ?? false))
        {
            throw new NotJoinedException();
        }

        return bettingUser;
    }

    private bool TryGetAiPickedTeams(out List<Team> pickedTeams, out string errorMessage)
    {
        pickedTeams = new List<Team>();
        errorMessage = "";

        if (string.IsNullOrWhiteSpace(AiResultInput))
        {
            errorMessage = "AI 결과를 입력해주세요.";
            return false;
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(AiResultInput);
        }
        catch (JsonException ex)
        {
            errorMessage = $"JSON 형식이 올바르지 않습니다. {ex.Message}";
            return false;
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                errorMessage = "JSON 최상위 값은 객체여야 합니다.";
                return false;
            }

            var submittedMatchIndexes = new HashSet<int>();
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (!TryGetMatchIndex(property.Name, out var matchIndex))
                {
                    errorMessage = $"알 수 없는 경기가 있습니다: {property.Name}";
                    return false;
                }
                if (!submittedMatchIndexes.Add(matchIndex))
                {
                    errorMessage = $"{GetMatchLabel(matchIndex)}가 중복 제출되었습니다.";
                    return false;
                }
                if (!TryGetAiPickedTeamForMatch(
                    property.Name,
                    matchIndex,
                    property.Value,
                    out var pickedTeam,
                    out errorMessage))
                {
                    return false;
                }

                pickedTeams.Add(pickedTeam);
            }

            var missingMatches = Enumerable.Range(0, Round32Matches.Count)
                .Where(matchIndex => !submittedMatchIndexes.Contains(matchIndex))
                .Select(GetMatchLabel)
                .ToList();
            if (missingMatches.Any())
            {
                errorMessage = $"누락된 경기가 있습니다: {string.Join(", ", missingMatches)}";
                return false;
            }

            if (pickedTeams.Count != Round32Matches.Count)
            {
                errorMessage = $"최종 선택은 {Round32Matches.Count}팀이어야 합니다. 현재 {pickedTeams.Count}팀입니다.";
                return false;
            }
        }

        return true;
    }

    private bool TryGetAiPickedTeamForMatch(
        string submittedMatchName,
        int matchIndex,
        JsonElement value,
        out Team pickedTeam,
        out string errorMessage)
    {
        pickedTeam = null;
        errorMessage = "";

        if (value.ValueKind != JsonValueKind.String)
        {
            errorMessage = $"{submittedMatchName} 값은 팀 이름 문자열이어야 합니다.";
            return false;
        }

        var teamName = value.GetString()?.Trim();
        if (string.IsNullOrWhiteSpace(teamName))
        {
            errorMessage = $"{submittedMatchName}에 빈 팀 이름이 있습니다.";
            return false;
        }

        var match = Round32Matches[matchIndex];
        var teamLookup = GetTeamLookup(match);
        if (!teamLookup.TryGetValue(NormalizeTeamName(teamName), out pickedTeam))
        {
            errorMessage = $"{submittedMatchName}의 \"{teamName}\"은 해당 경기 팀이 아닙니다. 가능한 팀: {GetCandidateTeamNames(match)}";
            return false;
        }

        return true;
    }

    private bool TryGetMatchIndex(string submittedMatchName, out int matchIndex)
    {
        matchIndex = -1;
        if (string.IsNullOrWhiteSpace(submittedMatchName))
            return false;

        var matchNumberText = Regex.Match(submittedMatchName, @"\d+").Value;
        if (string.IsNullOrWhiteSpace(matchNumberText))
            return false;

        if (!int.TryParse(matchNumberText, out var matchNumber))
            return false;

        matchIndex = matchNumber - 1;
        return matchIndex >= 0 && matchIndex < Round32Matches.Count;
    }

    private Dictionary<string, Team> GetTeamLookup(KnMatch match)
    {
        var lookup = new Dictionary<string, Team>(StringComparer.OrdinalIgnoreCase);
        AddTeamAlias(lookup, match.HomeTeam?.Name, match.HomeTeam);
        AddTeamAlias(lookup, match.HomeTeam?.Id, match.HomeTeam);
        AddTeamAlias(lookup, match.HomeTeam?.FifaTeamId, match.HomeTeam);
        AddTeamAlias(lookup, match.AwayTeam?.Name, match.AwayTeam);
        AddTeamAlias(lookup, match.AwayTeam?.Id, match.AwayTeam);
        AddTeamAlias(lookup, match.AwayTeam?.FifaTeamId, match.AwayTeam);
        return lookup;
    }

    private string GetCandidateTeamNames(KnMatch match)
    {
        return $"{match.HomeTeam?.Name}, {match.AwayTeam?.Name}";
    }

    private string BuildAiHelpPrompt()
    {
        if (!AllMatchesAreSetted)
        {
            return "32강 진출팀이 모두 확정된 후에 프롬프트를 만들 수 있습니다.";
        }

        var prompt = new StringBuilder();
        prompt.AppendLine("AI야 월드컵 32강이야. 각 경기에서 16강에 올라갈 것 같은 팀을 한 팀씩 골라줘.");
        prompt.AppendLine("아래 경기 목록을 보고, 각 값에는 해당 경기의 두 팀 중 하나를 정확한 팀명으로 넣어줘.");
        prompt.AppendLine("JSON 형식 그대로만 답해줘. 설명은 쓰지 마.");
        prompt.AppendLine();
        prompt.AppendLine("경기 목록");

        for (var i = 0; i < Round32Matches.Count; i++)
        {
            var match = Round32Matches[i];
            prompt.AppendLine($"{GetMatchLabel(i)} \"{match.HomeTeam.Name}\" vs \"{match.AwayTeam.Name}\"");
        }

        prompt.AppendLine();
        prompt.AppendLine("응답 형식:");
        prompt.AppendLine("{");
        for (var i = 0; i < Round32Matches.Count; i++)
        {
            var comma = i == Round32Matches.Count - 1 ? "" : ",";
            prompt.AppendLine($"  \"{GetMatchLabel(i)}\": \"선택한 팀명\"{comma}");
        }
        prompt.AppendLine("}");

        return prompt.ToString();
    }

    private void SetAiSubmitMessage(string message, Severity severity)
    {
        AiSubmitMessage = message;
        AiSubmitSeverity = severity;
    }

    private static void AddTeamAlias(Dictionary<string, Team> lookup, string teamName, Team team)
    {
        if (string.IsNullOrWhiteSpace(teamName) || team == null)
            return;

        var key = NormalizeTeamName(teamName);
        if (!lookup.ContainsKey(key))
        {
            lookup[key] = team;
        }
    }

    private static string NormalizeTeamName(string teamName)
    {
        return Regex.Replace(teamName.Trim(), @"[\s\p{P}\p{S}]+", "");
    }

    private static string GetMatchLabel(int matchIndex)
    {
        return $"{matchIndex + 1}경기";
    }
}
