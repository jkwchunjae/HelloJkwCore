using Microsoft.AspNetCore.Components;

namespace ProjectTrip.Pages;

public partial class CompanionEditDialog
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Parameter] public List<AppUser> AllUsers { get; set; }
    [Parameter] public List<AppUser> Companions { get; set; }

    void Submit() => MudDialog.Close(DialogResult.Ok(Companions));
    void Cancel() => MudDialog.Cancel();

    AppUser searchCompanion;

    private void RemoveCompanion(AppUser user)
    {
        // 파트너를 다 지울 수 없다. 누군가 한 명은 남겨야 한다.
        if (Companions.Any(x => x.Id != user.Id))
        {
            Companions.RemoveAll(x => x.Id == user.Id);
        }
    }

    private Task<IEnumerable<AppUser>> SearchCompanion(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Task.FromResult<IEnumerable<AppUser>>(new List<AppUser>());
        }

        var filtered = AllUsers
            .Where(user => user.DisplayName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
                        || (user.Email?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ?? true))
            .Take(3);

        return Task.FromResult(filtered);
    }

    private void OnCompanionSelect()
    {
        if (searchCompanion != null && !Companions.Contains(searchCompanion))
        {
            Companions.Add(searchCompanion);
            searchCompanion = null;
        }
    }
}
