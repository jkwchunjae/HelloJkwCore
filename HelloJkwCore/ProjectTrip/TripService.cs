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

    public async Task CreateTripAsync(Trip trip)
    {
        var exists = await ExistsTripIdAsync(trip.Id);
        if (exists)
            return;

        foreach (var userId in trip.Users)
        {
            var userData = await _fs.ReadUserDataAsync(userId);

            if (!userData.TripList.Contains(trip.Id))
            {
                userData.TripList.Add(trip.Id);
                await _fs.UpdateUserDataAsync(userData);
            }
        }

        await _fs.UpdateTripAsync(trip);
    }

    public async Task<Trip> UpdateTripAsync(TripId tripId, Func<Trip, ValueTask<Trip>> updateTripFunc)
    {
        Trip trip = await _fs.ReadTripAsync(tripId);
        if (trip == null)
            return null;

        var updated = await updateTripFunc(trip);
        await _fs.UpdateTripAsync(updated);

        return updated;
    }

    public Task DeleteTripAsync(AppUser user, Trip trip)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTripByUrlAsync(AppUser user, TripId tripId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsTripIdAsync(TripId tripId)
    {
        var trip = await _fs.ReadTripAsync(tripId);

        return trip != null;
    }

    public async Task<Trip> GetTripByUrlAsync(AppUser user, TripId tripId)
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

    public async Task<List<Trip>> GetTripsAsync(AppUser user)
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
