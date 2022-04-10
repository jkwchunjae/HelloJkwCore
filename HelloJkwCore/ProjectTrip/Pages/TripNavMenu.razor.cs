using Microsoft.AspNetCore.Components;

namespace ProjectTrip.Pages;

public partial class TripNavMenu : JkwPageBase
{
    [Inject]
    public ITripService TripService { get; set; }

    private List<Trip> Trips { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            await LoadTripsAsync();
        }
    }

    private async Task LoadTripsAsync()
    {
        Trips = await TripService.GetTripsAsync(User);
    }
}
