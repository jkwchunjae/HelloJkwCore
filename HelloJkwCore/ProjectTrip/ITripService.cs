namespace ProjectTrip;

public interface ITripService
{
    Task<bool> ExistsTripIdAsync(TripId tripId);
    Task<List<Trip>> GetTripsAsync(AppUser user);
    Task<Trip> GetTripByUrlAsync(AppUser user, TripId tripId);
    Task CreateOrUpdateTripAsync(Trip trip);
    Task DeleteTripAsync(AppUser user, Trip trip);
    Task DeleteTripByUrlAsync(AppUser user, TripId tripId);
}
