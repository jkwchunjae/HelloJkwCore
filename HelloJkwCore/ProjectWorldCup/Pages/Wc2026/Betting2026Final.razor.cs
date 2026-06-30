using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace ProjectWorldCup.Pages.Wc2026;

public partial class Betting2026Final : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] IWorldCupService WorldCupService { get; set; }
    [Inject] IBettingService BettingService { get; set; }
    [Inject(Key = "2026")] private IBettingFinalService BettingFinalService { get; set; }
    [Inject] private UserManager<AppUser> UserManager { get; set; }

    private BettingUser BettingUser { get; set; }
    List<(string StageId, List<KnMatch> Matches)> StageMatches { get; set; } = new();

    List<KnMatch> Matches { get; set; } = new();
    WcFinalBettingItem<Team> BettingItem { get; set; }
    List<WcFinalBettingItem<Team>> BettingItems { get; set; }

    Dictionary<string, string> StageName = new Dictionary<string, string>
    {
        [Fifa.Round8StageId] = "8강전",
        [Fifa.Round4StageId] = "4강전",
        [Fifa.ThirdStageId] = "3,4위전",
        [Fifa.FinalStageId] = "결승전",
    };
    bool AllMatchesAreSetted => StageMatches.Any() && StageMatches[0].Matches
        .All(match => match.Teams.All(team => team?.Id?.Length == 3));
    bool TimeOver = false;
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
        && !TimeOver
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
        if (IsAuthenticated)
        {
            BettingUser = await BettingService.GetBettingUserAsync(User);
            BettingItem = await BettingFinalService.GetBettingAsync(BettingUser);
        }
        Matches = await WorldCupService.GetFinalMatchesAsync();

        var quarterFinalMatches = await WorldCupService.GetQuarterFinalMatchesAsync();
        StageMatches.Add((Fifa.Round8StageId, quarterFinalMatches));

        var bettingItems = await BettingFinalService.GetAllBettingsAsync();
        var users = (await UserManager.GetUsersInRoleAsync("all"))
          .ToDictionary(user => user.Id);
        foreach (var item in bettingItems.Where(item => users.ContainsKey(item.User.Id)))
        {
            item.User = users[item.User.Id];
        }
        BettingItems = bettingItems;
        EvaluateUserBetting();
    }

    private void EvaluateUserBetting()
    {
        var quarters = StageMatches.First().Matches;
        if (BettingItem?.Picked?.Count() == 4)
        {
            StageMatches = BettingFinalService.EvaluateUserBetting(quarters, BettingItem, Matches);
            StateHasChanged();
        }
    }

    private TeamButtonType GetButtonType(string stageId, Team team)
    {
        return BettingFinalService.GetButtonType(stageId, team, StageMatches, BettingItem);
    }

    private void ShowLoginRequireMessage()
    {
        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add("로그인을 해주세요", Severity.Warning);
    }

    private async Task PickTeamAsync(string stageId, string matchId, Team team)
    {
        if (TimeOver)
            return;
        if (BettingItem?.IsRandom ?? false)
            return;
        if (team?.Id == null)
            return;
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Final);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
            {
                return;
            }

            StageMatches = BettingFinalService.PickTeam(stageId, matchId, team, StageMatches, Matches);

            if (stageId == Fifa.Round8StageId)
            {
                BettingItem.Picked = new List<Team> { null, null, null, null };
            }
            if (stageId == Fifa.Round4StageId)
            {
                BettingItem.Picked = new List<Team> { null, null, null, null };
            }
            else if (stageId == Fifa.ThirdStageId)
            {
                var third = StageMatches.First(s => s.StageId == Fifa.ThirdStageId).Matches[0];
                var loser = third.HomeTeam == team ? third.AwayTeam : third.HomeTeam;
                BettingItem.Picked = new List<Team>
                {
                    BettingItem.Pick0,
                    BettingItem.Pick1,
                    team,
                    loser,
                };
            }
            else if (stageId == Fifa.FinalStageId)
            {
                var final = StageMatches.First(s => s.StageId == Fifa.FinalStageId).Matches[0];
                var loser = final.HomeTeam == team ? final.AwayTeam : final.HomeTeam;
                BettingItem.Picked = new List<Team>
                {
                    team,
                    loser,
                    BettingItem.Pick2,
                    BettingItem.Pick3,
                };
            }

            if (BettingItem.Picked?.Count(x => x != null) == 4)
            {
                BettingItem.IsAi = false;
                await BettingFinalService.SaveBettingItemAsync(BettingItem);
                BettingItems = await BettingFinalService.GetAllBettingsAsync();
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("저장되었습니다", Severity.Success);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private async Task SelectFullRandom()
    {
        if (TimeOver)
            return;
        if (BettingItem?.IsRandom ?? false)
            return;
        if (!AllMatchesAreSetted)
            return;

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Final);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.Final))
            {
                return;
            }

            (StageMatches, var pickTeams) = BettingFinalService.PickRandom(StageMatches, Matches);
            BettingItem.Picked = pickTeams;
            BettingItem.IsRandom = true;
            BettingItem.IsAi = false;

            await BettingFinalService.SaveBettingItemAsync(BettingItem);
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

        if (TimeOver)
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
            Snackbar.Add("8강 진출팀이 모두 확정된 후에 사용할 수 있습니다.", Severity.Warning);
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

        if (TimeOver)
        {
            SetAiSubmitMessage("선택 시간이 끝났습니다.", Severity.Error);
            return;
        }

        if (!AllMatchesAreSetted)
        {
            SetAiSubmitMessage("8강 진출팀이 모두 확정된 후에 사용할 수 있습니다.", Severity.Error);
            return;
        }

        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            SetAiSubmitMessage("로그인을 해주세요.", Severity.Error);
            return;
        }

        if (!TryGetAiPickedTeams(out var pickedTeams, out var aiStageMatches, out var validationMessage))
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
            var bettingUser = await EnsureFinalBettingJoinedAsync();
            BettingItem ??= new WcFinalBettingItem<Team> { User = bettingUser.AppUser };
            BettingItem.User = bettingUser.AppUser;
            BettingItem.Picked = pickedTeams;
            BettingItem.IsRandom = false;
            BettingItem.IsAi = true;
            StageMatches = aiStageMatches;

            await BettingFinalService.SaveBettingItemAsync(BettingItem);
            BettingItems = await BettingFinalService.GetAllBettingsAsync();

            await HideAiHelperDialogAsync(clearInput: true);
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("AI 결과로 1~4위 팀을 선택했습니다. (저장되었습니다)", Severity.Success);
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

    private async Task<BettingUser> EnsureFinalBettingJoinedAsync()
    {
        var bettingUser = await BettingService.GetBettingUserAsync(User);
        if (bettingUser == null)
        {
            throw new NotJoinedException();
        }

        if (!(bettingUser.JoinedBetting?.Contains(BettingType.Final) ?? false))
        {
            bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.Final);
        }
        if (!(bettingUser.JoinedBetting?.Contains(BettingType.Final) ?? false))
        {
            throw new NotJoinedException();
        }

        BettingUser = bettingUser;
        return bettingUser;
    }

    private bool TryGetAiPickedTeams(
        out List<Team> pickedTeams,
        out List<(string StageId, List<KnMatch> Matches)> aiStageMatches,
        out string errorMessage)
    {
        pickedTeams = new List<Team>();
        aiStageMatches = new List<(string StageId, List<KnMatch> Matches)>();
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

            var quarterMatches = GetQuarterFinalMatches()
                .Select(match => new KnMatch(match))
                .ToList();
            if (!TryValidateLoadedMatches("8강", quarterMatches, 4, out errorMessage))
            {
                return false;
            }

            aiStageMatches = new List<(string StageId, List<KnMatch> Matches)>
            {
                (Fifa.Round8StageId, quarterMatches),
            };

            if (!TryGetStageObject(document.RootElement, "8강", Round8StageAliases, out var round8Element, out errorMessage))
            {
                return false;
            }
            if (!TryApplyAiStageWinners("8강", round8Element, Fifa.Round8StageId, 4, ref aiStageMatches, out errorMessage))
            {
                return false;
            }

            if (!TryGetStageObject(document.RootElement, "4강", Round4StageAliases, out var round4Element, out errorMessage))
            {
                return false;
            }
            if (!TryApplyAiStageWinners("4강", round4Element, Fifa.Round4StageId, 2, ref aiStageMatches, out errorMessage))
            {
                return false;
            }

            if (!TryGetStageMatch(aiStageMatches, Fifa.ThirdStageId, "3,4위전", out var thirdMatch, out errorMessage)
                || !TryGetStageMatch(aiStageMatches, Fifa.FinalStageId, "결승전", out var finalMatch, out errorMessage))
            {
                return false;
            }

            if (!TryGetStageWinner(document.RootElement, "3,4위전", ThirdStageAliases, thirdMatch, out var thirdWinner, out errorMessage)
                || !TryGetStageWinner(document.RootElement, "결승전", FinalStageAliases, finalMatch, out var finalWinner, out errorMessage))
            {
                return false;
            }

            var thirdLoser = GetOpponentTeam(thirdMatch, thirdWinner);
            var finalLoser = GetOpponentTeam(finalMatch, finalWinner);
            pickedTeams = new List<Team>
            {
                finalWinner,
                finalLoser,
                thirdWinner,
                thirdLoser,
            };

            if (pickedTeams.Any(team => team == null || string.IsNullOrWhiteSpace(team.Id)))
            {
                errorMessage = "최종 순위에 빈 팀이 있습니다.";
                return false;
            }
            if (pickedTeams.Distinct().Count() != 4)
            {
                errorMessage = "최종 1~4위 팀에 중복이 있습니다.";
                return false;
            }
        }

        return true;
    }

    private bool TryApplyAiStageWinners(
        string stageDisplayName,
        JsonElement stageElement,
        string stageId,
        int expectedMatchCount,
        ref List<(string StageId, List<KnMatch> Matches)> aiStageMatches,
        out string errorMessage)
    {
        errorMessage = "";
        if (stageElement.ValueKind != JsonValueKind.Object)
        {
            errorMessage = $"{stageDisplayName} 값은 경기별 승자를 담은 객체여야 합니다.";
            return false;
        }

        if (!TryGetStageMatches(aiStageMatches, stageId, stageDisplayName, expectedMatchCount, out var matches, out errorMessage))
        {
            return false;
        }

        var submittedMatchIndexes = new HashSet<int>();
        foreach (var property in stageElement.EnumerateObject())
        {
            if (!TryGetMatchIndex(property.Name, expectedMatchCount, out var matchIndex))
            {
                errorMessage = $"{stageDisplayName}에 알 수 없는 경기가 있습니다: {property.Name}";
                return false;
            }
            if (!submittedMatchIndexes.Add(matchIndex))
            {
                errorMessage = $"{stageDisplayName} {GetMatchLabel(matchIndex)}가 중복 제출되었습니다.";
                return false;
            }

            var match = matches[matchIndex];
            if (!TryGetAiPickedTeamForMatch(
                $"{stageDisplayName} {GetMatchLabel(matchIndex)}",
                match,
                property.Value,
                out var pickedTeam,
                out errorMessage))
            {
                return false;
            }

            aiStageMatches = BettingFinalService.PickTeam(stageId, match.MatchId, pickedTeam, aiStageMatches, Matches);
        }

        var missingMatches = Enumerable.Range(0, expectedMatchCount)
            .Where(matchIndex => !submittedMatchIndexes.Contains(matchIndex))
            .Select(GetMatchLabel)
            .ToList();
        if (missingMatches.Any())
        {
            errorMessage = $"{stageDisplayName}에 누락된 경기가 있습니다: {string.Join(", ", missingMatches)}";
            return false;
        }

        return true;
    }

    private bool TryGetStageWinner(
        JsonElement rootElement,
        string stageDisplayName,
        IReadOnlyCollection<string> stageAliases,
        KnMatch match,
        out Team winner,
        out string errorMessage)
    {
        winner = null;
        if (!TryGetStageProperty(rootElement, stageAliases, out var property))
        {
            errorMessage = $"{stageDisplayName} 결과가 없습니다.";
            return false;
        }

        return TryGetAiPickedTeamForMatch(stageDisplayName, match, property.Value, out winner, out errorMessage);
    }

    private bool TryGetAiPickedTeamForMatch(
        string submittedMatchName,
        KnMatch match,
        JsonElement value,
        out Team pickedTeam,
        out string errorMessage)
    {
        pickedTeam = null;
        errorMessage = "";

        if (!TryGetTeamName(value, out var teamName))
        {
            errorMessage = $"{submittedMatchName} 값은 팀 이름 문자열이어야 합니다.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(teamName))
        {
            errorMessage = $"{submittedMatchName}에 빈 팀 이름이 있습니다.";
            return false;
        }

        var teamLookup = GetTeamLookup(match);
        if (!teamLookup.TryGetValue(NormalizeTeamName(teamName), out pickedTeam))
        {
            errorMessage = $"{submittedMatchName}의 \"{teamName}\"은 해당 경기 팀이 아닙니다. 가능한 팀: {GetCandidateTeamNames(match)}";
            return false;
        }

        return true;
    }

    private bool TryGetStageObject(
        JsonElement rootElement,
        string stageDisplayName,
        IReadOnlyCollection<string> stageAliases,
        out JsonElement stageElement,
        out string errorMessage)
    {
        stageElement = default;
        if (!TryGetStageProperty(rootElement, stageAliases, out var property))
        {
            errorMessage = $"{stageDisplayName} 결과가 없습니다.";
            return false;
        }

        stageElement = property.Value;
        errorMessage = "";
        return true;
    }

    private bool TryGetStageMatches(
        List<(string StageId, List<KnMatch> Matches)> stageMatches,
        string stageId,
        string stageDisplayName,
        int expectedMatchCount,
        out List<KnMatch> matches,
        out string errorMessage)
    {
        matches = stageMatches.FirstOrDefault(stage => stage.StageId == stageId).Matches;
        if (matches == null)
        {
            errorMessage = $"{stageDisplayName} 대진을 만들지 못했습니다.";
            return false;
        }

        return TryValidateLoadedMatches(stageDisplayName, matches, expectedMatchCount, out errorMessage);
    }

    private bool TryGetStageMatch(
        List<(string StageId, List<KnMatch> Matches)> stageMatches,
        string stageId,
        string stageDisplayName,
        out KnMatch match,
        out string errorMessage)
    {
        match = null;
        if (!TryGetStageMatches(stageMatches, stageId, stageDisplayName, 1, out var matches, out errorMessage))
        {
            return false;
        }

        match = matches[0];
        return true;
    }

    private bool TryValidateLoadedMatches(string stageDisplayName, List<KnMatch> matches, int expectedMatchCount, out string errorMessage)
    {
        errorMessage = "";
        if (matches?.Count != expectedMatchCount)
        {
            errorMessage = $"{stageDisplayName} 경기 데이터는 {expectedMatchCount}개여야 합니다. 현재 {matches?.Count ?? 0}개입니다.";
            return false;
        }
        if (matches.Any(match => match == null
            || match.HomeTeam == null
            || match.AwayTeam == null
            || string.IsNullOrWhiteSpace(match.HomeTeam.Id)
            || string.IsNullOrWhiteSpace(match.AwayTeam.Id)))
        {
            errorMessage = $"{stageDisplayName} 대진에 확정되지 않은 팀이 있습니다.";
            return false;
        }

        return true;
    }

    private List<KnMatch> GetQuarterFinalMatches()
    {
        return StageMatches.FirstOrDefault(stage => stage.StageId == Fifa.Round8StageId).Matches ?? new List<KnMatch>();
    }

    private string BuildAiHelpPrompt()
    {
        if (!AllMatchesAreSetted)
        {
            return "8강 진출팀이 모두 확정된 후에 프롬프트를 만들 수 있습니다.";
        }

        var quarterMatches = GetQuarterFinalMatches();
        var prompt = new StringBuilder();
        prompt.AppendLine("AI야 월드컵 8강부터 최종 1,2,3,4위를 예측해야 해.");
        prompt.AppendLine("최종 순위만 바로 쓰지 말고, 8강 4경기, 4강 2경기, 3,4위전, 결승전의 승자를 순서대로 골라줘.");
        prompt.AppendLine("4강 1경기는 8강 2경기 승자 vs 8강 1경기 승자이고, 4강 2경기는 8강 4경기 승자 vs 8강 3경기 승자야.");
        prompt.AppendLine("3,4위전은 4강 패자끼리, 결승전은 4강 승자끼리 진행돼.");
        prompt.AppendLine("결승전 승자가 1위, 결승전 패자가 2위, 3,4위전 승자가 3위, 3,4위전 패자가 4위로 저장돼.");
        prompt.AppendLine("각 값에는 해당 경기의 두 팀 중 하나를 정확한 팀명으로 넣어줘.");
        prompt.AppendLine("JSON 형식 그대로만 답해줘. 설명은 쓰지 마.");
        prompt.AppendLine();
        prompt.AppendLine("8강 경기 목록");

        for (var i = 0; i < quarterMatches.Count; i++)
        {
            var match = quarterMatches[i];
            prompt.AppendLine($"{GetMatchLabel(i)} \"{match.HomeTeam.Name}\" vs \"{match.AwayTeam.Name}\"");
        }

        prompt.AppendLine();
        prompt.AppendLine("응답 형식:");
        prompt.AppendLine("{");
        prompt.AppendLine("  \"8강\": {");
        for (var i = 0; i < 4; i++)
        {
            var comma = i == 3 ? "" : ",";
            prompt.AppendLine($"    \"{GetMatchLabel(i)}\": \"선택한 팀명\"{comma}");
        }
        prompt.AppendLine("  },");
        prompt.AppendLine("  \"4강\": {");
        for (var i = 0; i < 2; i++)
        {
            var comma = i == 1 ? "" : ",";
            prompt.AppendLine($"    \"{GetMatchLabel(i)}\": \"선택한 팀명\"{comma}");
        }
        prompt.AppendLine("  },");
        prompt.AppendLine("  \"3,4위전\": \"선택한 팀명\",");
        prompt.AppendLine("  \"결승전\": \"선택한 팀명\"");
        prompt.AppendLine("}");

        return prompt.ToString();
    }

    private void SetAiSubmitMessage(string message, Severity severity)
    {
        AiSubmitMessage = message;
        AiSubmitSeverity = severity;
    }

    private static bool TryGetStageProperty(JsonElement rootElement, IReadOnlyCollection<string> aliases, out JsonProperty property)
    {
        var normalizedAliases = aliases
            .Select(NormalizeTeamName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var candidate in rootElement.EnumerateObject())
        {
            if (normalizedAliases.Contains(NormalizeTeamName(candidate.Name)))
            {
                property = candidate;
                return true;
            }
        }

        property = default;
        return false;
    }

    private static bool TryGetTeamName(JsonElement value, out string teamName)
    {
        teamName = null;
        if (value.ValueKind == JsonValueKind.String)
        {
            teamName = value.GetString()?.Trim();
            return true;
        }

        if (value.ValueKind != JsonValueKind.Object)
            return false;

        foreach (var property in value.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.String)
                continue;

            var key = NormalizeTeamName(property.Name);
            if (key is "승자" or "winner" or "선택" or "팀")
            {
                teamName = property.Value.GetString()?.Trim();
                return true;
            }
        }

        return false;
    }

    private static bool TryGetMatchIndex(string submittedMatchName, int matchCount, out int matchIndex)
    {
        matchIndex = -1;
        if (string.IsNullOrWhiteSpace(submittedMatchName))
            return false;

        var matches = Regex.Matches(submittedMatchName, @"\d+");
        if (!matches.Any())
            return false;

        if (!int.TryParse(matches.Last().Value, out var matchNumber))
            return false;

        matchIndex = matchNumber - 1;
        return matchIndex >= 0 && matchIndex < matchCount;
    }

    private static Dictionary<string, Team> GetTeamLookup(KnMatch match)
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

    private static string GetCandidateTeamNames(KnMatch match)
    {
        return $"{match.HomeTeam?.Name}, {match.AwayTeam?.Name}";
    }

    private static Team GetOpponentTeam(KnMatch match, Team team)
    {
        return match.HomeTeam == team ? match.AwayTeam : match.HomeTeam;
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

    private static readonly string[] Round8StageAliases = ["8강", "8강전", "QuarterFinal", "QuarterFinals"];
    private static readonly string[] Round4StageAliases = ["4강", "4강전", "준결승", "준결승전", "SemiFinal", "SemiFinals"];
    private static readonly string[] ThirdStageAliases = ["3,4위전", "3-4위전", "34위전", "3위결정전", "ThirdPlace", "ThirdPlacePlayoff"];
    private static readonly string[] FinalStageAliases = ["결승", "결승전", "Final"];
}
