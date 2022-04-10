namespace ProjectTrip;

public static class TripFileSystemHelper
{
    private static Func<Paths, string> GetTripUserPath(UserId userId)
    {
        Func<Paths, string> tripUserPath = path => path["Users"] + $"/tripuser.{userId}.json";
        return tripUserPath;
    }

    private static Func<Paths, string> GetTripDataPath(TripId tripId)
    {
        Func<Paths, string> tripDataPath = path => path["Trips"] + $"/trip.{tripId}.json";
        return tripDataPath;
    }

    public static async Task<UserData> ReadUserDataAsync(this IFileSystem fs, AppUser user)
    {
        return await fs.ReadUserDataAsync(user.Id);
    }

    public static async Task<UserData> ReadUserDataAsync(this IFileSystem fs, UserId userId)
    {
        var tripUserPath = GetTripUserPath(userId);
        if (await fs.FileExistsAsync(tripUserPath))
        {
            var userData = await fs.ReadJsonAsync<UserData>(tripUserPath);
            return userData;
        }
        else
        {
            return new UserData
            {
                UserId = userId,
                TripList = new(),
            };
        }
    }

    public static async Task<bool> UpdateUserDataAsync(this IFileSystem fs, UserData tripUserData)
    {
        var tripUserPath = GetTripUserPath(tripUserData.UserId);

        return await fs.WriteJsonAsync(tripUserPath, tripUserData);
    }

    public static async Task<Trip> ReadTripAsync(this IFileSystem fs, TripId tripId)
    {
        var tripPath = GetTripDataPath(tripId);

        if (await fs.FileExistsAsync(tripPath))
        {
            var trip = await fs.ReadJsonAsync<Trip>(tripPath);
            return trip;
        }
        else
        {
            return null;
        }
    }

    public static async Task<bool> UpdateTripAsync(this IFileSystem fs, Trip trip)
    {
        var tripPath = GetTripDataPath(trip.Id);

        return await fs.WriteJsonAsync(tripPath, trip);
    }
}
