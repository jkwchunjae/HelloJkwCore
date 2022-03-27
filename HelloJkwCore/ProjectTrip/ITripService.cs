namespace ProjectTrip;

public interface ITripService
{
    Task<List<Trip>> GetTripsAsync(AppUser user);
    Task<Trip> GetTripByUrlAsync(AppUser user, StringId tripId);
    Task CreateOrUpdateTripAsync(AppUser user, Trip trip);
    Task DeleteTripAsync(AppUser user, Trip trip);
    Task DeleteTripByUrlAsync(AppUser user, StringId tripId);
}
