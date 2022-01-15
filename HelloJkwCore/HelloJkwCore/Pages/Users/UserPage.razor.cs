using Microsoft.AspNetCore.Components.Routing;

namespace HelloJkwCore.Pages.Users;

public partial class UserPage : JkwPageBase
{
    private bool ChangeNickName = false;

    private string InputNickName = string.Empty;

    protected override void OnPageInitialized()
    {
        CheckPageChangeNickName(Navi.Uri);
    }

    protected override Task HandleLocationChanged(LocationChangedEventArgs e)
    {
        CheckPageChangeNickName(e.Location);

        return Task.CompletedTask;
    }

    private void CheckPageChangeNickName(string url)
    {
        if (url.Contains("change-nickname"))
        {
            ChangeNickName = true;
            InputNickName = User?.DisplayName;
        }
        else
        {
            ChangeNickName = false;
        }

        StateHasChanged();
    }

    private async Task ChangeNickNameAsync()
    {
        User.NickName = InputNickName?.Trim();
        if (string.IsNullOrWhiteSpace(User.NickName))
        {
            User.NickName = null;
        }
        await UserStore.UpdateAsync(User, default);

        Navi.NavigateTo("/user");
    }
}