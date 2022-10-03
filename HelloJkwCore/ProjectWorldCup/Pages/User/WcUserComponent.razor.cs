﻿using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.User;

public partial class WcUserComponent
{
    [Inject] public IJSRuntime Js { get; set; }
    [Inject] protected IUserStore<AppUser> UserStore { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public IBettingService BettingService { get; set; }
    [Parameter] public BettingUser TargetUser { get; set; }

    private string InputNickname { get; set; }
    TimeZoneInfo LocalTimeZone { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        InputNickname = TargetUser.AppUser.NickName ?? TargetUser.AppUser.UserName;
        var offset = await Js.InvokeAsync<int>("getTimezone");
        LocalTimeZone = TimeZoneInfo.CreateCustomTimeZone("UserLocalTimeZone", TimeSpan.FromMinutes(-offset), "UserLocalTimeZone", "UserLocalTimeZone");
        StateHasChanged();
    }

    private async Task ChangeNickname()
    {
        if (!CheckNickname(InputNickname))
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("적절하지 않은 이름입니다", Severity.Warning);
            return;
        }
        TargetUser.AppUser.NickName = InputNickname;
        await BettingService.AddHistoryAsync(TargetUser, new BettingHistory
        {
            Type = HistoryType.ChangeNickname,
            Value = 0,
            Comment = $"닉네임 변경 ({InputNickname})",
        });
        await UserStore.UpdateAsync(TargetUser.AppUser, default);
        StateHasChanged();
    }

    private async Task ChangeNickname500()
    {
        if (!CheckNickname(InputNickname))
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("적절하지 않은 이름입니다", Severity.Warning);
            return;
        }
        await BettingService.AddHistoryAsync(TargetUser, new BettingHistory
        {
            Type = HistoryType.ChangeNickname,
            Value = -500,
            Comment = $"닉네임 변경 ({InputNickname})",
        });
        TargetUser.AppUser.NickName = InputNickname;
        await UserStore.UpdateAsync(TargetUser.AppUser, default);
        await Task.Delay(TimeSpan.FromSeconds(1));
        StateHasChanged();
    }

    private static bool CheckNickname(string nickname)
    {
        nickname = nickname.Trim();
        if (string.IsNullOrWhiteSpace(nickname))
            return false;
        if (nickname.Length > 15)
            return false;
        return true;
    }

}
