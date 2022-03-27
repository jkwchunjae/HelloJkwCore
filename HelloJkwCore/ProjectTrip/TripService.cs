namespace ProjectTrip;

public class TripService : ITripService
{
    private readonly IFileSystem _fs;

    public TripService(
        TripServiceOption tripOption,
        IFileSystemService fsService)
    {
        _fs = fsService.GetFileSystem(tripOption.FileSystemSelect, tripOption.Path);
    }

    public async Task CreateOrUpdateTripAsync(ITripUser user, Trip trip)
    {
        var userData = await _fs.ReadUserDataAsync(user);

        if (!userData.TripList.Contains(trip.Id))
        {
            userData.TripList.Add(trip.Id);
            await _fs.UpdateUserDataAsync(userData);
        }

        await _fs.UpdateUserDataAsync(userData);
    }

    public Task DeleteTripAsync(ITripUser user, Trip trip)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTripByUrlAsync(ITripUser user, StringId tripId)
    {
        throw new NotImplementedException();
    }

    public async Task<Trip> GetTripByUrlAsync(ITripUser user, StringId tripId)
    {
        var trip = await _fs.ReadTripAsync(tripId);

        if (trip?.Users?.Contains(user.Id) ?? false)
        {
            return trip;
        }
        else
        {
            return null;
        }
    }

    public async Task<List<Trip>> GetTripsAsync(ITripUser user)
    {
        var userData = await _fs.ReadUserDataAsync(user);

        var trips = await userData.TripList
            .Select(tripId => _fs.ReadTripAsync(tripId))
            .WhenAll();

        var myTrips = trips
            .Where(trip => trip?.Users?.Contains(user.Id) ?? false)
            .ToList();

        return myTrips;
    }
}
