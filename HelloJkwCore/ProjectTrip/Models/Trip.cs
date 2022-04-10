using Newtonsoft.Json;

namespace ProjectTrip.Models;

public class Trip
{
    /// <summary> Url에 사용될 문자열 </summary>
    public TripId Id { get; set; }
    public TripTitle Title { get; set; }
    public DateTime BeginTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<VisitedPlace> VisitedPlaces { get; set; } = new();
    public List<UserId> Users { get; set; } = new();
    /// <summary> 대표 위치, 요약 화면에서 사용함. </summary>
    public List<LatLng> Positions { get; set; } = new();

    public void AddUser(AppUser user)
    {
        Users ??= new();

        if (!Users.Contains(user.Id))
        {
            Users.Add(user.Id);
        }
    }

    public void AddUsers(IEnumerable<AppUser> users)
    {
        users.ForEach(user => AddUser(user));
    }
}

[JsonConverter(typeof(StringIdJsonConverter<TripId>))]
public class TripId : StringId
{
    public TripId() { }
    public TripId(string id)
        : base(id) { }
}

[JsonConverter(typeof(StringIdJsonConverter<TripTitle>))]
public class TripTitle : StringName
{
    public TripTitle() { }
    public TripTitle(string title)
        : base(title) { }
}
