using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace ProjectDiary.Pages;

public partial class DiaryUser
{
    [Parameter] public UserId UserId { get; set; }
    [Parameter] public AppUser User { get; set; }
    [Parameter] public MudIcon Icon { get; set; } = null;

    [Inject] public UserManager<AppUser> UserManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (User == null)
        {
            if (UserId?.Id != null)
            {
                User = await UserManager.FindByIdAsync(UserId.Id);
            }
        }
    }
}
