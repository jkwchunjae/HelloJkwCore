using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Reflection;
using System.Text.Json;

namespace ProjectWorldCup.Pages.Wc2026;

public partial class Betting2026GroupStage : JkwPageBase
{
    private const string AiPromptResourceName = "ProjectWorldCup.Pages.Wc2026.HelpMeAI_prompt.txt";

    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] private IWorldCupService WcService { get; set; }
    [Inject] private IBettingService BettingService { get; set; }
    [Inject(Key = "2026")] private IBettingGroupStageService GroupStageService { get; set; }
    [Inject] private UserManager<AppUser> UserManager { get; set; }

    private List<WcGroup> Groups { get; set; } = new();

    private BettingUser BettingUser { get; set; }
    private WcBettingItem<GroupTeam> BettingItem { get; set; } = new();
    private List<WcBettingItem<GroupTeam>> BettingItems { get; set; }

    bool TimeOver { get; set; } = false;
    bool CheckRandom1 = false;
    bool CheckRandom2 = false;
    bool CheckRandom3 = false;

    private bool ShowAiHelperDialog { get; set; }
    private bool IsSubmittingAiResult { get; set; }
    private string AiHelpPrompt { get; set; } = "";
    private string AiResultInput { get; set; } = "";
    private string AiSubmitMessage { get; set; } = "";
    private Severity AiSubmitSeverity { get; set; } = Severity.Info;
    private DialogOptions AiHelperDialogOptions { get; } = new()
    {
        CloseButton = true,
        CloseOnEscapeKey = true,
        BackdropClick = false,
        FullWidth = true,
        MaxWidth = MaxWidth.Medium,
    };
    private IReadOnlyDictionary<string, AiPromptGroup> AiPromptGroupsByLetter { get; set; }
        = new Dictionary<string, AiPromptGroup>();

    protected override async Task OnPageInitializedAsync()
    {
        AiHelpPrompt = await LoadAiHelpPromptAsync();
        AiPromptGroupsByLetter = ParseAiPromptGroups(AiHelpPrompt);

        if (IsAuthenticated)
        {
            BettingUser = await BettingService.GetBettingUserAsync(User);

            if (BettingUser?.JoinedBetting?.Contains(BettingType.GroupStage) ?? false)
            {
                BettingItem = await GroupStageService.GetBettingAsync(BettingUser);
            }
        }

        Groups = await WcService.GetGroupsFromStandingAsync();
        var bettingItems = await GroupStageService.GetAllBettingsAsync();
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

    private async Task PickTeam(GroupTeam team)
    {
        if (TimeOver)
            return;
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.GroupStage))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.GroupStage);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.GroupStage))
            {
                return;
            }
            BettingUser = bettingUser;

            var buttonType = GetButtonType(team);

            if (buttonType == TeamButtonType.Pickable)
            {
                BettingItem = await GroupStageService.PickTeamAsync(bettingUser, team);
                StateHasChanged();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Configuration.ShowTransitionDuration = 100;
                Snackbar.Configuration.VisibleStateDuration = 3000;
                Snackbar.Configuration.HideTransitionDuration = 100;
                Snackbar.Configuration.MaxDisplayedSnackbars = 3;
                Snackbar.Add($"{team.Name} 팀을 선택하였습니다. (저장되었습니다)", Severity.Success);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private async Task UnpickTeam(GroupTeam team)
    {
        if (TimeOver)
            return;
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
            var buttonType = GetButtonType(team);

            if (buttonType == TeamButtonType.Picked)
            {
                BettingItem = await GroupStageService.UnpickTeamAsync(BettingUser, team);
                StateHasChanged();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add($"{team.Name} 팀을 제외했습니다. (저장되었습니다)", Severity.Success);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(ex.Message, Severity.Normal);
        }
    }

    private TeamButtonType GetButtonType(GroupTeam team)
    {
        if (BettingItem == null)
        {
            return TeamButtonType.Pickable;
        }

        if (BettingItem.Picked.Any(x => x == team))
        {
            return TeamButtonType.Picked;
        }

        var groupTeams = Groups.FirstOrDefault(g => g.Teams.Any(t => t == team));
        if (groupTeams == null)
            return TeamButtonType.Pickable;

        var groupPickCount = groupTeams.Teams.Count(t => BettingItem.Picked.Any(x => x == t));

        if (groupPickCount == 2)
        {
            return TeamButtonType.Disabled;
        }
        else
        {
            return TeamButtonType.Pickable;
        }
    }

    private void OnTimeOver()
    {
        TimeOver = true;
        StateHasChanged();
    }

    private async Task SelectFullRandom()
    {
        if (TimeOver)
            return;
        if (!IsAuthenticated)
        {
            ShowLoginRequireMessage();
            return;
        }
        if (BettingItem?.IsRandom ?? false)
            return;

        try
        {
            var bettingUser = await BettingService.GetBettingUserAsync(User);
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.GroupStage))
            {
                bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.GroupStage);
            }
            if (bettingUser.JoinedBetting.Empty(x => x == BettingType.GroupStage))
            {
                return;
            }
            BettingUser = bettingUser;

            BettingItem = await GroupStageService.PickRandomAsync(bettingUser);
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
        if (TimeOver)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("선택 시간이 끝났습니다.", Severity.Warning);
            return;
        }

        AiResultInput = "";
        AiSubmitMessage = "";
        ShowAiHelperDialog = true;
    }

    private void CloseAiHelperDialog()
    {
        if (IsSubmittingAiResult)
            return;

        ShowAiHelperDialog = false;
        AiSubmitMessage = "";
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
            var bettingUser = await EnsureGroupStageBettingJoinedAsync();
            BettingItem = await GroupStageService.PickTeamsWithAiAsync(bettingUser, pickedTeams);

            ShowAiHelperDialog = false;
            AiResultInput = "";
            AiSubmitMessage = "";

            StateHasChanged();
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("AI 결과로 24팀을 선택했습니다. (저장되었습니다)", Severity.Success);
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

    private async Task<BettingUser> EnsureGroupStageBettingJoinedAsync()
    {
        var bettingUser = await BettingService.GetBettingUserAsync(User);
        if (bettingUser == null)
        {
            throw new NotJoinedException();
        }

        if (!(bettingUser.JoinedBetting?.Contains(BettingType.GroupStage) ?? false))
        {
            bettingUser = await BettingService.JoinBettingAsync(bettingUser, BettingType.GroupStage);
        }
        if (!(bettingUser.JoinedBetting?.Contains(BettingType.GroupStage) ?? false))
        {
            throw new NotJoinedException();
        }

        BettingUser = bettingUser;
        return bettingUser;
    }

    private bool TryGetAiPickedTeams(out List<GroupTeam> pickedTeams, out string errorMessage)
    {
        pickedTeams = new List<GroupTeam>();
        errorMessage = "";

        if (string.IsNullOrWhiteSpace(AiResultInput))
        {
            errorMessage = "AI 결과를 입력해주세요.";
            return false;
        }

        if (!TryGetGroupsByLetter(out var groupsByLetter, out errorMessage))
        {
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

            var submittedLetters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in document.RootElement.EnumerateObject())
            {
                var groupLetter = GetGroupLetter(property.Name);
                if (groupLetter == null || !groupsByLetter.ContainsKey(groupLetter))
                {
                    errorMessage = $"알 수 없는 조가 있습니다: {property.Name}";
                    return false;
                }
                if (!submittedLetters.Add(groupLetter))
                {
                    errorMessage = $"{GetDisplayGroupName(groupLetter)}가 중복 제출되었습니다.";
                    return false;
                }
                if (!TryGetAiPickedTeamsForGroup(
                    property.Name,
                    groupLetter,
                    property.Value,
                    groupsByLetter[groupLetter],
                    out var groupPickedTeams,
                    out errorMessage))
                {
                    return false;
                }

                pickedTeams.AddRange(groupPickedTeams);
            }

            var missingGroups = groupsByLetter.Keys
                .Where(groupLetter => !submittedLetters.Contains(groupLetter))
                .OrderBy(groupLetter => groupLetter)
                .Select(GetDisplayGroupName)
                .ToList();
            if (missingGroups.Any())
            {
                errorMessage = $"누락된 조가 있습니다: {string.Join(", ", missingGroups)}";
                return false;
            }

            if (pickedTeams.Count != 24)
            {
                errorMessage = $"최종 선택은 24팀이어야 합니다. 현재 {pickedTeams.Count}팀입니다.";
                return false;
            }
        }

        return true;
    }

    private bool TryGetAiPickedTeamsForGroup(
        string submittedGroupName,
        string groupLetter,
        JsonElement value,
        WcGroup group,
        out List<GroupTeam> groupPickedTeams,
        out string errorMessage)
    {
        groupPickedTeams = new List<GroupTeam>();
        errorMessage = "";

        if (value.ValueKind != JsonValueKind.Array)
        {
            errorMessage = $"{submittedGroupName} 값은 팀 이름 배열이어야 합니다.";
            return false;
        }

        var submittedTeamElements = value.EnumerateArray().ToList();
        if (submittedTeamElements.Count != 2)
        {
            errorMessage = $"{submittedGroupName}에는 정확히 2팀을 제출해야 합니다. 현재 {submittedTeamElements.Count}팀입니다.";
            return false;
        }

        var teamLookup = GetTeamLookup(groupLetter, group);
        var normalizedSubmittedTeamNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var teamElement in submittedTeamElements)
        {
            if (teamElement.ValueKind != JsonValueKind.String)
            {
                errorMessage = $"{submittedGroupName}의 팀 값은 문자열이어야 합니다.";
                return false;
            }

            var teamName = teamElement.GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(teamName))
            {
                errorMessage = $"{submittedGroupName}에 빈 팀 이름이 있습니다.";
                return false;
            }
            if (!normalizedSubmittedTeamNames.Add(NormalizeTeamName(teamName)))
            {
                errorMessage = $"{submittedGroupName}에 중복된 팀이 있습니다: {teamName}";
                return false;
            }
            if (!teamLookup.TryGetValue(NormalizeTeamName(teamName), out var team))
            {
                errorMessage = $"{submittedGroupName}의 \"{teamName}\"은 해당 조 팀이 아닙니다. 가능한 팀: {GetCandidateTeamNames(groupLetter, group)}";
                return false;
            }
            if (groupPickedTeams.Any(pickedTeam => pickedTeam == team))
            {
                errorMessage = $"{submittedGroupName}에 중복된 팀이 있습니다: {teamName}";
                return false;
            }

            groupPickedTeams.Add(team);
        }

        return true;
    }

    private bool TryGetGroupsByLetter(out Dictionary<string, WcGroup> groupsByLetter, out string errorMessage)
    {
        groupsByLetter = new Dictionary<string, WcGroup>(StringComparer.OrdinalIgnoreCase);
        errorMessage = "";

        if (Groups.Count != 12)
        {
            errorMessage = $"현재 조 데이터가 12개가 아닙니다. 현재 {Groups.Count}개 조가 로드되었습니다.";
            return false;
        }

        foreach (var group in Groups)
        {
            var groupLetter = GetGroupLetter(group.Name);
            if (groupLetter == null)
            {
                errorMessage = $"조 이름을 해석할 수 없습니다: {group.Name}";
                return false;
            }
            if (groupsByLetter.ContainsKey(groupLetter))
            {
                errorMessage = $"{GetDisplayGroupName(groupLetter)} 데이터가 중복되어 있습니다.";
                return false;
            }

            groupsByLetter[groupLetter] = group;
        }

        return true;
    }

    private Dictionary<string, GroupTeam> GetTeamLookup(string groupLetter, WcGroup group)
    {
        var lookup = new Dictionary<string, GroupTeam>(StringComparer.OrdinalIgnoreCase);
        var orderedTeams = group.Teams
            .OrderBy(team => team.Placement)
            .ToList();

        for (var i = 0; i < orderedTeams.Count; i++)
        {
            AddTeamAlias(lookup, orderedTeams[i].Name, orderedTeams[i]);
            AddTeamAlias(lookup, orderedTeams[i].Id, orderedTeams[i]);
        }

        if (AiPromptGroupsByLetter.TryGetValue(groupLetter, out var promptGroup))
        {
            for (var i = 0; i < promptGroup.TeamNames.Count && i < orderedTeams.Count; i++)
            {
                AddTeamAlias(lookup, promptGroup.TeamNames[i], orderedTeams[i]);
            }
        }

        return lookup;
    }

    private string GetCandidateTeamNames(string groupLetter, WcGroup group)
    {
        if (AiPromptGroupsByLetter.TryGetValue(groupLetter, out var promptGroup))
        {
            return string.Join(", ", promptGroup.TeamNames);
        }

        return string.Join(", ", group.Teams.Select(team => team.Name));
    }

    private void SetAiSubmitMessage(string message, Severity severity)
    {
        AiSubmitMessage = message;
        AiSubmitSeverity = severity;
    }

    private static void AddTeamAlias(Dictionary<string, GroupTeam> lookup, string teamName, GroupTeam team)
    {
        if (string.IsNullOrWhiteSpace(teamName))
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

    private static string GetGroupLetter(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            return null;

        var match = Regex.Match(groupName.Trim(), @"^(?:Group|그룹)?\s*([A-L])\s*(?:조)?$", RegexOptions.IgnoreCase);
        if (!match.Success)
            return null;

        return match.Groups[1].Value.ToUpperInvariant();
    }

    private static string GetDisplayGroupName(string groupLetter)
    {
        return $"{groupLetter.ToUpperInvariant()}조";
    }

    private static async Task<string> LoadAiHelpPromptAsync()
    {
        var assembly = typeof(Betting2026GroupStage).GetTypeInfo().Assembly;
        await using var stream = assembly.GetManifestResourceStream(AiPromptResourceName);
        if (stream == null)
        {
            return "HelpMeAI_prompt.txt 파일을 불러오지 못했습니다.";
        }

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private static IReadOnlyDictionary<string, AiPromptGroup> ParseAiPromptGroups(string prompt)
    {
        var promptGroups = new Dictionary<string, AiPromptGroup>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in prompt.Split('\n'))
        {
            var groupMatch = Regex.Match(
                line,
                @"^\s*(?<group>(?:Group|그룹)?\s*[A-L]\s*조?)\s+(?<teams>.+)$",
                RegexOptions.IgnoreCase);
            if (!groupMatch.Success)
                continue;

            var groupName = groupMatch.Groups["group"].Value.Trim();
            var groupLetter = GetGroupLetter(groupName);
            if (groupLetter == null)
                continue;

            var teamNames = Regex.Matches(groupMatch.Groups["teams"].Value, @"""(?<team>[^""]+)""")
                .Select(match => match.Groups["team"].Value.Trim())
                .Where(teamName => !string.IsNullOrWhiteSpace(teamName))
                .ToList();
            if (teamNames.Any())
            {
                promptGroups[groupLetter] = new AiPromptGroup(groupName, groupLetter, teamNames);
            }
        }

        return promptGroups;
    }

    private sealed record AiPromptGroup(string GroupName, string GroupLetter, IReadOnlyList<string> TeamNames);
}
