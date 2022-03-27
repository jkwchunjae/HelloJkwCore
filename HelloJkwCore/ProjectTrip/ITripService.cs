namespace ProjectTrip;

public interface ITripService
{
    Task<List<Trip>> GetTripsAsync(ITripUser user);
    Task<Trip> GetTripByUrlAsync(ITripUser user, StringId tripId);
    Task CreateOrUpdateTripAsync(ITripUser user, Trip trip);
    Task DeleteTripAsync(ITripUser user, Trip trip);
    Task DeleteTripByUrlAsync(ITripUser user, StringId tripId);
}
