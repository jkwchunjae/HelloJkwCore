using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MudBlazor;

namespace ProjectTrip.Pages;

public partial class TripDetail : JkwPageBase
{
    [Parameter] public string tripId { get; set; }
    [Inject] public IJSRuntime JS { get; set; }
    [Inject] public ITripService TripService { get; set; }
    [Inject] public UserManager<AppUser> UserManager { get; set; }
    [Inject] public IDialogService DialogService { get; set; }

    IKakaoMap KakaoMap;
    Trip trip { get; set; }
    List<AppUser> partners = new();

    protected async Task OnMapCreated(IKakaoMap map)
    {
        KakaoMap = map;
        await SetTripDetails(new TripId(tripId));
    }

    private async Task SetTripDetails(TripId tripId)
    {
        var trip = await TripService.GetTripByUrlAsync(tripId);

        if (trip == null)
            return;

        this.trip = trip;

        await KakaoMap.SetCenter(trip.Positions.First());
        await KakaoMap.SetLevel(7);

        var partners = await trip.Users
            .Select(async userId => await UserManager.FindByIdAsync(userId.Id))
            .WhenAll();
        this.partners = partners.ToList();

        StateHasChanged();
    }

    protected override async Task HandleLocationChanged(LocationChangedEventArgs e)
    {
        await SetTripDetails(new TripId(tripId));
    }

    private async Task OpenPartnerDialog()
    {
        await JS.InvokeVoidAsync("console.log", "open partner dialog");
        var users = await UserManager.GetUsersInRoleAsync("all");
        var options = new DialogOptions { CloseOnEscapeKey = false };
        var parameters = new DialogParameters
        {
            ["AllUsers"] = users.ToList(),
            ["Partners"] = new List<AppUser>(this.partners),
        };
        var dialog = DialogService.Show<PartnerEditDialog>("", parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var partners = (List<AppUser>)result.Data;
            await TripService.UpdateTripAsync(trip.Id, trip =>
            {
                trip.Users = partners.Select(x => x.Id).ToList();
                return ValueTask.FromResult(trip);
            });
            this.partners = partners;
        }
    }
}
