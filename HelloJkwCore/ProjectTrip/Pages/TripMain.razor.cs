using KakaoMapBlazor.Marker;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ProjectTrip.Pages;

public partial class TripMain : JkwPageBase
{
    [Inject]
    private IJSRuntime JS { get; set; }

    KakaoMapComponent kakaoMapComponent;
    IKakaoMap KakaoMap => kakaoMapComponent?.Instance;
    MapCreateOption mapCreateOption = new MapCreateOption(new LatLng(33.450701, 126.570667))
    {
        MapTypeId = MapType.RoadMap,
        Level = 3,
    };

    protected override Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender && KakaoMap != null)
        {
            KakaoMap.Click += async (s, e) =>
            {
                await JS.InvokeVoidAsync("console.log", e.LatLng);
                var marker = await KakaoMap.CreateMarker(new MarkerCreateOptionInMap()
                {
                    Position = e.LatLng,
                });
                marker.Click += async (s, _) =>
                {
                    await JS.InvokeVoidAsync("console.log", "marker clicked");
                };
            };
        }
        return Task.CompletedTask;
    }
}
