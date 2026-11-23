using System.Net;
using Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using ProjectKidsnote.Models.Account;
using ProjectKidsnote.Models.Authentication;
using ProjectKidsnote.Models.Parents;
using ProjectKidsnote.Services;

namespace ProjectKidsnote.Pages;

public partial class KidsnoteRegistration
{
    [Inject] private KidsnoteFamilies Families { get; set; } = null!;
    [Inject] private KidsnoteCredentials Credentials { get; set; } = null!;
    [Inject] private KidsnoteSessions Sessions { get; set; } = null!;
    [Inject] private AppUserManager Accounts { get; set; } = null!;
    [Inject] private KidsnoteMenuState Menu { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService Dialogs { get; set; } = null!;
    [Inject] private ILogger<KidsnoteRegistration> Logger { get; set; } = null!;

    private List<KidsParent> _families = [];
    private KidsParent? _family;
    private List<AppUser> _users = [];
    private List<Kid> _choices = [];
    private IEnumerable<Kid> VisibleChildren => _family!.Kids.Concat(
        _choices.Where(choice => !_family.Kids.Any(saved => saved.ChildId == choice.ChildId)));

    private string? _error;
    private string _accountId = "";
    private string _password = "";
    private long? _renamingChild;
    private string _nicknameDraft = "";
    private AccessTarget? _accessTarget;
    private HashSet<UserId> _originalAccess = [];
    private List<UserId> _accessDraft = [];
    private UserId? _selectedMember;
    private ElementReference _accessHeading;
    private bool _focusAccessHeading;
    private bool _ready;
    private bool _showConnectionForm;
    private bool _busy;

    private bool CanConnect => !_busy && !string.IsNullOrWhiteSpace(_accountId) && !string.IsNullOrWhiteSpace(_password);
    private bool HasAccessChanges => !_originalAccess.SetEquals(_accessDraft);
    private IEnumerable<AppUser> AvailableMembers => _users.Where(user => !_accessDraft.Contains(user.Id)
        && (_accessTarget?.Kind != AccessKind.Manager || user.Id != _family!.User));
    private string AccessChanges
    {
        get
        {
            var added = _accessDraft.Count(id => !_originalAccess.Contains(id));
            var removed = _originalAccess.Count(id => !_accessDraft.Contains(id));
            return string.Join(" · ", new[] { added > 0 ? $"{added}명 추가" : "", removed > 0 ? $"{removed}명 제거" : "" }
                .Where(text => text.Length > 0));
        }
    }

    protected override Task OnPageInitializedAsync()
    {
        if (User is null)
        {
            Navi.NavigateTo("/account/login");
            return Task.CompletedTask;
        }
        return RendererInfo.IsInteractive ? RunAsync(() => OpenFamilyAsync(User.Id.Id)) : Task.CompletedTask;
    }

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (!_focusAccessHeading || !_ready) return;
        _focusAccessHeading = false;
        await _accessHeading.FocusAsync();
    }

    private async Task OpenFamilyAsync(string? owner)
    {
        ResetAccessEditor();
        _focusAccessHeading = false;
        CancelRename();
        _accountId = _password = "";
        _ready = false;
        _showConnectionForm = false;
        _choices.Clear();
        _users.Clear();
        _family = null;
        _families = (await Families.ReadAsync()).Where(p => p.CanManage(User!.Id)).ToList();
        var savedOwners = _families.Select(p => p.User).ToHashSet();
        if (User!.HasRole(UserRole.Kidsnote) && !_families.Any(p => p.User == User.Id))
            _families.Add(new KidsParent { User = User.Id });
        _family = _families.FirstOrDefault(p => p.User.Id == owner) ?? _families.FirstOrDefault();
        if (_family is null) throw new InvalidOperationException("가족 설정을 관리할 권한이 없습니다.");
        _ready = savedOwners.Contains(_family.User);
        _showConnectionForm = _family.Auth?.EncryptedAuthInfo is null;
        _users = (await Accounts.GetUsersInRoleAsync("all")).OrderBy(u => u.DisplayName).ToList();
    }

    private string FamilyLabel(KidsParent family) => family.User == User!.Id ? "내 가족"
        : family.Kids.Count > 0 ? $"{string.Join(", ", family.Kids.Select(k => k.Name))} 가족"
        : $"{_families.IndexOf(family) + 1}번째 가족";

    private string FamilyName(string? owner) =>
        _families.FirstOrDefault(family => family.User.Id == owner) is { } family ? FamilyLabel(family) : "";

    private Task SwitchAsync(string? owner) => RunAsync(() => OpenFamilyAsync(owner));
    private Task RetryAsync() => RunAsync(() => LoginAsync());

    private async Task ConnectAsync()
    {
        if (!CanConnect) return;
        try
        {
            await RunAsync(() => LoginAsync(Credentials.Protect(_family!.User, new KidsnoteLoginRequest(_accountId.Trim(), _password))));
        }
        finally
        {
            _password = "";
        }
    }

    private async Task LoginAsync(string? newCredentials = null)
    {
        var owner = _family!.User;
        _showConnectionForm = true;
        var step = "Login";
        UserInfo info;
        try
        {
            info = newCredentials is null ? await RestoreConnectionAsync(ReadInfoAsync) : await ReadInfoAsync();
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning("Kidsnote connection failed at {Step}. HTTP status: {StatusCode}; error: {RequestError}.",
                step, (int?)ex.StatusCode, ex.HttpRequestError);
            _error = ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                    "키즈노트에서 접근을 허용하지 않았습니다. 키즈노트 로그인 정보를 확인해 주세요.",
                HttpStatusCode.TooManyRequests =>
                    "키즈노트 연결 요청이 너무 많습니다. 잠시 후 다시 연결해 주세요.",
                _ => "키즈노트에 연결하지 못했습니다. 잠시 후 다시 연결해 주세요."
            };
            return;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            Logger.LogWarning("Kidsnote connection timed out at {Step}.", step);
            _error = "키즈노트 응답이 지연되고 있습니다. 잠시 후 다시 연결해 주세요.";
            return;
        }

        if (newCredentials is not null)
        {
            await PersistAsync(p => p.Auth = new KidsnoteAuth { EncryptedAuthInfo = newCredentials });
        }

        _choices = info.Children.Select(child => new Kid
        {
            ChildId = child.Id, Name = child.Name, NicknameForUrl = "",
            CenterId = (child.Enrollment.FirstOrDefault(e => e.IsApproved && !e.RemovedChild)
                ?? child.Enrollment.FirstOrDefault())?.CenterId ?? 0
        }).ToList();
        _showConnectionForm = false;
        _ready = true;

        async Task<UserInfo> ReadInfoAsync()
        {
            step = "Login";
            var encrypted = newCredentials ?? await Families.GetManagedCredentialsAsync(User!.Id, owner);
            var client = await Sessions.GetAsync(owner, encrypted);
            step = "Profile";
            return await client.GetMyInfoAsync();
        }
    }

    internal static async Task<UserInfo> RestoreConnectionAsync(Func<Task<UserInfo>> readInfo)
    {
        try
        {
            return await readInfo();
        }
        catch (Exception ex) when (IsTemporaryConnectionFailure(ex))
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            return await readInfo();
        }
    }

    private static bool IsTemporaryConnectionFailure(Exception exception) => exception switch
    {
        HttpRequestException { StatusCode: HttpStatusCode.RequestTimeout } => true,
        HttpRequestException { StatusCode: { } status } => (int)status is >= 500 and <= 599,
        HttpRequestException { HttpRequestError: HttpRequestError.ConnectionError or
            HttpRequestError.NameResolutionError or HttpRequestError.ResponseEnded } => true,
        TaskCanceledException { InnerException: TimeoutException } => true,
        _ => false
    };

    private async Task PersistAsync(Action<KidsParent> change)
    {
        var current = _family!;
        var saved = await Families.UpdateAsync(User!, current, change);
        _families[_families.IndexOf(current)] = saved;
        _family = saved;
        Menu.NotifyChanged();
        Snackbar.Add("저장되었습니다.", Severity.Success);
    }

    private static string ReportUrl(Kid kid) => $"/kidsnote/{kid.NicknameForUrl}/";
    private static string Initial(string name) => string.IsNullOrWhiteSpace(name)
        ? "?" : System.Globalization.StringInfo.GetNextTextElement(name);

    private static bool IsValidNickname(string? nickname) =>
        !string.IsNullOrWhiteSpace(nickname) && nickname.Trim().All(c => c is >= 'a' and <= 'z' or >= '0' and <= '9');

    private void BeginRename(Kid kid)
    {
        _renamingChild = kid.ChildId;
        _nicknameDraft = kid.NicknameForUrl;
    }

    private void CancelRename()
    {
        _renamingChild = null;
        _nicknameDraft = "";
    }

    private void SetNickname(Kid kid, bool editing, string? value)
    {
        if (editing) _nicknameDraft = value ?? "";
        else kid.NicknameForUrl = value ?? "";
    }

    private Task SaveNicknameAsync(Kid kid, bool editing)
    {
        var nickname = (editing ? _nicknameDraft : kid.NicknameForUrl).Trim();
        if (!IsValidNickname(nickname)) return Task.CompletedTask;

        return RunAsync(async () =>
        {
            if (editing)
            {
                await PersistAsync(family => family.Kids.Single(saved => saved.ChildId == kid.ChildId).NicknameForUrl = nickname);
                CancelRename();
            }
            else
            {
                await PersistAsync(family => family.Kids.Add(new Kid
                {
                    ChildId = kid.ChildId, Name = kid.Name, CenterId = kid.CenterId, NicknameForUrl = nickname
                }));
            }
        });
    }

    private async Task ConfirmUnregisterAsync(Kid kid)
    {
        var familyOwner = _family!.User;
        var confirmed = await Dialogs.ShowMessageBoxAsync("아이 등록 해제",
            $"{kid.Name} 아이의 홈페이지 등록을 해제할까요? 알림장 주소와 아이별 열람 권한이 비활성화됩니다.",
            yesText: "등록 해제", cancelText: "취소");
        if (confirmed != true || _family?.User != familyOwner) return;

        await RunAsync(async () =>
        {
            await PersistAsync(family => family.Kids.RemoveAll(saved => saved.ChildId == kid.ChildId));
            if (_renamingChild == kid.ChildId) CancelRename();
            if (_accessTarget?.ChildId == kid.ChildId) CloseAccessEditor();
        });
    }

    private static IEnumerable<AccessTarget> AccessTargets(KidsParent family)
    {
        yield return new AccessTarget(AccessKind.Manager, "공동 관리자", "가족 설정 변경 · 모든 알림장 열람",
            "이 가족의 설정을 변경하고 모든 아이의 알림장을 볼 수 있어요.");
        yield return new AccessTarget(AccessKind.Family, "가족 열람자", "가족의 모든 알림장 열람",
            "이 가족의 모든 아이의 알림장을 볼 수 있어요.");
        foreach (var kid in family.Kids)
        {
            yield return new AccessTarget(AccessKind.Child, $"{kid.Name} 열람자", $"{kid.Name}의 알림장만 열람",
                $"{kid.Name}의 알림장만 볼 수 있어요.", kid.ChildId);
        }
    }

    private static List<UserId> AccessMembers(KidsParent family, AccessTarget target) => target.Kind switch
    {
        AccessKind.Manager => family.Partners,
        AccessKind.Family => family.AllowedList,
        _ => family.Kids.Single(kid => kid.ChildId == target.ChildId).AllowedList
    };

    private string MemberName(UserId id) =>
        _users.FirstOrDefault(user => user.Id == id)?.DisplayName ?? "알 수 없는 사용자";

    private string MemberChoiceLabel(UserId? id)
    {
        if (id is null) return "";
        if (_users.FirstOrDefault(user => user.Id == id) is not { } person) return MemberName(id);
        var providers = string.Join(", ", person.Logins.Select(login => login.Provider).Distinct());
        return providers.Length == 0 ? MemberName(id) : $"{MemberName(id)} · {providers}";
    }

    private void OpenAccessEditor(AccessTarget target)
    {
        _accessTarget = target;
        _accessDraft = AccessMembers(_family!, target).Distinct().ToList();
        _originalAccess = _accessDraft.ToHashSet();
        _selectedMember = null;
        _focusAccessHeading = true;
    }

    private void ResetAccessEditor()
    {
        _accessTarget = null;
        _accessDraft.Clear();
        _originalAccess.Clear();
        _selectedMember = null;
    }

    private void CloseAccessEditor()
    {
        ResetAccessEditor();
        _focusAccessHeading = true;
    }

    private void AddMember()
    {
        if (_selectedMember is { } member && AvailableMembers.Any(user => user.Id == member))
            _accessDraft.Add(member);
        _selectedMember = null;
    }

    private Task SaveAccessAsync()
    {
        if (_accessTarget is not { } target || !HasAccessChanges) return Task.CompletedTask;
        var members = _accessDraft.ToArray();
        return RunAsync(async () =>
        {
            await PersistAsync(family =>
            {
                var list = AccessMembers(family, target);
                list.Clear();
                list.AddRange(members);
            });
            CloseAccessEditor();
            if (!_family!.CanManage(User!.Id)) await OpenFamilyAsync(User.Id.Id);
        });
    }

    private enum AccessKind { Manager, Family, Child }

    private sealed record AccessTarget(AccessKind Kind, string Title, string Summary, string Description, long? ChildId = null)
    {
        public string EditorTitle => $"{Title} 편집";
        public string EmptyLabel => Kind == AccessKind.Manager ? "공동 관리자" : "열람자";
    }

    private async Task RunAsync(Func<Task> action)
    {
        if (_busy) return;
        _busy = true;
        _error = null;
        try { await action(); }
        catch (InvalidOperationException ex) { _error = ex.Message; }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning("Kidsnote registration storage request failed. HTTP status: {StatusCode}; error: {RequestError}.",
                (int?)ex.StatusCode, ex.HttpRequestError);
            _error = "가족 설정이나 계정 목록을 불러오거나 저장하지 못했습니다. 잠시 후 다시 시도해 주세요.";
        }
        catch (Exception) { _error = "키즈노트 계정 또는 저장소를 확인하고 다시 시도해 주세요."; }
        finally { _busy = false; }
    }
}
