namespace ProjectTrip;

public interface ITripService
{
    Task<bool> ExistsTripIdAsync(TripId tripId);
    Task<List<Trip>> GetTripsAsync(AppUser user);
    Task<Trip> GetTripByUrlAsync(TripId tripId);
    Task CreateTripAsync(Trip trip);
    Task<Trip> UpdateTripAsync(TripId tripId, Func<Trip, ValueTask<Trip>> updateTripFunc);
    Task DeleteTripAsync(AppUser user, Trip trip);
    Task DeleteTripByUrlAsync(AppUser user, TripId tripId);
}
