using KakaoMapBlazor.Marker;
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
    IEnumerable<AppUser> Companions = new List<AppUser>();

    NewPlaceData NewPlaceData;

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

        this.Companions = await trip.Companions
            .Select(async userId => await UserManager.FindByIdAsync(userId.Id))
            .WhenAll();

        foreach (var place in trip.VisitedPlaces)
        {
            foreach (var position in place.Markers)
            {
                await KakaoMap.CreateMarker(new MarkerCreateOptionInMap
                {
                    Position = position,
                    Title = place.Name,
                });
            }
        }

        StateHasChanged();
    }

    protected override async Task HandleLocationChanged(LocationChangedEventArgs e)
    {
        await SetTripDetails(new TripId(tripId));
    }

    private async Task OpenCompanionsDialog()
    {
        await JS.InvokeVoidAsync("console.log", "open companion dialog");
        var users = await UserManager.GetUsersInRoleAsync("all");
        var options = new DialogOptions { CloseOnEscapeKey = false };
        var parameters = new DialogParameters
        {
            ["AllUsers"] = users.ToList(),
            ["Companions"] = new List<AppUser>(this.Companions),
        };
        var dialog = DialogService.Show<CompanionEditDialog>("", parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var companions = (List<AppUser>)result.Data;
            await TripService.UpdateTripAsync(trip.Id, trip =>
            {
                trip.Companions = companions.Select(x => x.Id).ToList();
                return ValueTask.FromResult(trip);
            });
            this.Companions = companions;
        }
    }

    private async Task OpenNewVisitedPlaceDialog()
    {
        await JS.InvokeVoidAsync("console.log", "장소 추가");

        NewPlaceData = new NewPlaceData();

        NewPlaceData.StartDate = trip.BeginTime.Date;
        NewPlaceData.StartTime = null;

        KakaoMap.Click += KakaoMap_Click_NewPlace;
    }

    private async void KakaoMap_Click_NewPlace(object sender, KakaoMapBlazor.Models.MouseEvent e)
    {
        var position = e.LatLng;
        var marker = await KakaoMap.CreateMarker(new MarkerCreateOptionInMap() { Position = position });
        marker.Click += async (s, e) =>
        {
            NewPlaceData.Markers.RemoveAll(x => x.Position == position);
            StateHasChanged();
            await marker.Close();
        };
        NewPlaceData.Markers.Add((position, marker));
    }

    private async Task AddNewPlace()
    {
        if (NewPlaceData?.Validate() ?? false)
        {
            var place = NewPlaceData?.ToVisitedPlace();

            if (place != null)
            {
                await TripService.UpdateTripAsync(trip.Id, trip =>
                {
                    trip.VisitedPlaces.Add(place);
                    return ValueTask.FromResult(trip);
                });

                NewPlaceData = null;
                KakaoMap.Click -= KakaoMap_Click_NewPlace;
            }
        }
    }

    private void CancelNewPlace()
    {
        NewPlaceData = null;
    }
}

class NewPlaceData
{
    public string PlaceName { get; set; }
    public double Expense { get; set; }
    public List<(LatLng Position, IKakaoMarker Marker)> Markers { get; set; } = new();
    public string Review { get; set; }
    public DateTime? StartDate { get; set; }
    public TimeSpan? StartTime { get; set; }

    public VisitedPlace ToVisitedPlace()
    {
        var position = Markers.First().Position;
        var newDateTime = StartDate.Value + StartTime.Value;
        var place = new VisitedPlace(PlaceName, position, newDateTime)
        {
            Expense = Expense,
            Review = Review,
            Markers = Markers.Select(x => x.Position).ToList(),
        };

        return place;
    }

    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(PlaceName))
            return false;
        if (Markers?.Empty() ?? true)
            return false;
        if (StartDate.HasValue == false)
            return false;
        if (StartTime.HasValue == false)
            return false;

        return true;
    }
}