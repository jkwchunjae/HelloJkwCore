﻿using Microsoft.AspNetCore.Components;

namespace ProjectTrip.Pages;

public partial class PartnerEditDialog
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Parameter] public List<AppUser> AllUsers { get; set; }
    [Parameter] public List<AppUser> Partners { get; set; }

    void Submit() => MudDialog.Close(DialogResult.Ok(Partners));
    void Cancel() => MudDialog.Cancel();

    AppUser searchPartner;

    private void RemovePartner(AppUser user)
    {
        // 파트너를 다 지울 수 없다. 누군가 한 명은 남겨야 한다.
        if (Partners.Any(x => x.Id != user.Id))
        {
            Partners.RemoveAll(x => x.Id == user.Id);
        }
    }

    private Task<IEnumerable<AppUser>> SearchPartner(string keyword)
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

    private void OnPartnerSelect()
    {
        if (searchPartner != null && !Partners.Contains(searchPartner))
        {
            Partners.Add(searchPartner);
            searchPartner = null;
        }
    }
}
