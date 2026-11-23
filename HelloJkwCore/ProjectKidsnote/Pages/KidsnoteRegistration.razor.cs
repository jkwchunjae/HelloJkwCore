using Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProjectKidsnote.Client;
using ProjectKidsnote.Configuration;
using ProjectKidsnote.Models.Authentication;
using ProjectKidsnote.Models.Parents;
using ProjectKidsnote.Services;

namespace ProjectKidsnote.Pages;

public partial class KidsnoteRegistration
{
    [Inject] private KidsnoteFamilies Families { get; set; } = null!;
    [Inject] private KidsnoteCredentials Credentials { get; set; } = null!;
    [Inject] private KidsnoteOptions Options { get; set; } = null!;
    [Inject] private AppUserManager Accounts { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private List<KidsParent> _families = [];
    private KidsParent? _family;
    private List<AppUser> _users = [];
    private List<Kid> _choices = [];
    private string? _error;
    private bool _ready;
    private bool _busy;
    private int _inputVersion;

    protected override Task OnPageInitializedAsync()
    {
        if (User is null)
        {
            Navi.NavigateTo("/account/login");
            return Task.CompletedTask;
        }
        return RunAsync(() => OpenFamilyAsync(User.Id.Id));
    }

    private async Task OpenFamilyAsync(string? owner)
    {
        _ready = false;
        _choices.Clear();
        _users.Clear();
        _family = null;
        _families = (await Families.ReadAsync()).Where(p => p.CanManage(User!.Id)).ToList();
        if (User!.HasRole(UserRole.Kidsnote) && !_families.Any(p => p.User == User.Id))
            _families.Add(new KidsParent { User = User.Id });
        _family = _families.FirstOrDefault(p => p.User.Id == owner) ?? _families.FirstOrDefault();
        if (_family is null) throw new InvalidOperationException("가족 설정을 관리할 권한이 없습니다.");
        if (_family.Auth?.EncryptedAuthInfo is { } encrypted)
            await LoginAsync(Credentials.Open(_family.User, encrypted), saveCredentials: false);
    }

    private string FamilyLabel(KidsParent family) => family.User == User!.Id ? "내 가족"
        : family.Kids.Count > 0 ? $"{string.Join(", ", family.Kids.Select(k => k.Name))} 가족"
        : $"{_families.IndexOf(family) + 1}번째 가족";

    private Task SwitchAsync(string? owner) => RunAsync(() => OpenFamilyAsync(owner));
    private Task RetryAsync() => SwitchAsync(_family!.User.Id);
    private Task ConnectAsync(KidsnoteLoginRequest login) => RunAsync(() => LoginAsync(login, saveCredentials: true));

    private async Task LoginAsync(KidsnoteLoginRequest login, bool saveCredentials)
    {
        using var client = new KidsnoteClient(Options);
        await client.LoginAsync(login.UserId, login.Password);
        if (saveCredentials)
        {
            var encrypted = Credentials.Protect(_family!.User, login);
            await PersistAsync(p => p.Auth = new KidsnoteAuth { EncryptedAuthInfo = encrypted });
        }

        var info = await client.GetMyInfoAsync();
        _choices = info.Children.Select(child => new Kid
        {
            ChildId = child.Id, Name = child.Name, NicknameForUrl = "",
            CenterId = (child.Enrollment.FirstOrDefault(e => e.IsApproved && !e.RemovedChild)
                ?? child.Enrollment.FirstOrDefault())?.CenterId ?? 0
        }).ToList();
        // Keep previously registered children manageable if they disappear from the external listing.
        _choices.AddRange(_family!.Kids.Where(saved => !_choices.Any(k => k.ChildId == saved.ChildId)));
        _users = (await Accounts.GetUsersInRoleAsync("all")).OrderBy(u => u.DisplayName).ToList();
        _ready = true;
    }

    private async Task PersistAsync(Action<KidsParent> change)
    {
        var current = _family!;
        var saved = await Families.UpdateAsync(User!, current, change);
        _families[_families.IndexOf(current)] = saved;
        _family = saved;
        Snackbar.Add("저장되었습니다.", Severity.Success);
    }

    private Task RegisterAsync(Kid kid) => RunAsync(() => PersistAsync(p => p.Kids.Add(new Kid
    {
        ChildId = kid.ChildId, Name = kid.Name, CenterId = kid.CenterId, NicknameForUrl = kid.NicknameForUrl.Trim()
    })));

    private Task UnregisterAsync(long id) => RunAsync(() => PersistAsync(p => p.Kids.RemoveAll(k => k.ChildId == id)));

    private Task RenameAsync(long id, string? name) => RunAsync(() =>
        PersistAsync(p => p.Kids.Single(k => k.ChildId == id).NicknameForUrl = name?.Trim() ?? ""));

    private Task ChangeAccessAsync((UserId Id, bool Add) change, long? childId = null, bool partners = false) =>
        RunAsync(async () =>
        {
            await PersistAsync(p =>
            {
                var list = partners ? p.Partners : childId.HasValue
                    ? p.Kids.Single(k => k.ChildId == childId).AllowedList : p.AllowedList;
                list.Remove(change.Id);
                if (change.Add) list.Add(change.Id);
            });
            if (!_family!.CanManage(User!.Id)) await OpenFamilyAsync(User.Id.Id);
        });

    private static string ObjectName(string name)
    {
        var last = name.LastOrDefault();
        return name + (last is >= '가' and <= '힣' && (last - '가') % 28 != 0 ? "을" : "를");
    }

    private async Task RunAsync(Func<Task> action)
    {
        if (_busy) return;
        _busy = true;
        _error = null;
        try { await action(); }
        catch (InvalidOperationException ex) { _error = ex.Message; }
        catch (HttpRequestException) { _error = "키즈노트 연결에 실패했습니다. 로그인 정보와 연결 상태를 확인해 주세요."; }
        catch (Exception) { _error = "키즈노트 계정 또는 저장소를 확인하고 다시 시도해 주세요."; }
        finally { _inputVersion++; _busy = false; }
    }
}
