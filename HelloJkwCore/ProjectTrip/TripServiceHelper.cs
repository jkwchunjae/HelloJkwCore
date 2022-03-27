using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectTrip;

public static partial class TripServiceHelper
{
    public static void AddTripService(this IServiceCollection services, IConfiguration configuration)
    {
        var tripOption= new TripServiceOption();
        configuration.GetSection("TripService").Bind(tripOption);

        services.AddSingleton(tripOption);
        services.AddSingleton<ITripService, TripService>();
    }
}