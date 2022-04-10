using KakaoMapBlazor.InfoWindow;
using KakaoMapBlazor.Marker;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ProjectTrip.Pages;

public partial class TripMain : JkwPageBase
{
    [Inject]
    private IJSRuntime JS { get; set; }

    [Inject]
    private ITripService TripService { get; set; }

    IKakaoMap KakaoMap;
    MapCreateOption mapCreateOption = new MapCreateOption(new LatLng(36.55506321886859, 127.61013231891525))
    {
        MapTypeId = MapType.RoadMap,
        Level = 12,
    };

    protected async Task OnMapCreated(IKakaoMap map)
    {
        KakaoMap = map;
        if (IsAuthenticated)
        {
            await LoadUserTripListAsync();
        }
    }

    private async Task LoadUserTripListAsync()
    {
        var trips = await TripService.GetTripsAsync(User);
        await JS.InvokeVoidAsync("console.log", trips);
        foreach (Trip trip in trips)
        {
            foreach (var position in trip.Positions)
            {
                var marker = await KakaoMap.CreateMarker(new MarkerCreateOptionInMap
                {
                    Position = position,
                });
                var info = await KakaoMap.CreateInfoWindow(new InfoWindowCreateOption(trip.Title.Name));
                marker.MouseOver += async (s, e) =>
                {
                    await info.Open(marker);
                };
                marker.MouseOut += async (s, e) =>
                {
                    await info.Close();
                };
                marker.Click += (s, e) =>
                {
                    Navi.NavigateTo($"/trip/trip/{trip.Id}");
                };
            }
        }
    }
}
