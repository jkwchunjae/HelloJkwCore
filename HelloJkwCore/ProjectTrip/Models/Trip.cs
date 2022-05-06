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
    public List<UserId> Companions { get; set; } = new();
    /// <summary> 대표 위치, 요약 화면에서 사용함. </summary>
    public List<LatLng> Positions { get; set; } = new();

    [JsonIgnore]
    public string TitleWithDate
    {
        get
        {
            if (string.IsNullOrEmpty(Title.Name))
                return string.Empty;

            if (BeginTime.Year == EndTime.Year
                && BeginTime.Month == EndTime.Month
                && BeginTime.Day == EndTime.Day)
            {
                return $"{Title} {BeginTime:yyyy.M.d}";
            }
            else if (BeginTime.Year == EndTime.Year
                && BeginTime.Month == EndTime.Month)
            {
                return $"{Title} {BeginTime:yyyy.M.d} - {EndTime.Day}";
            }
            else if (BeginTime.Year == EndTime.Year)
            {
                return $"{Title} {BeginTime:yyyy.M.d} - {EndTime:M.d}";
            }
            else
            {
                return $"{Title} {BeginTime:yyyy.M.d} - {EndTime:yyyy.M.d}";
            }
        }
    }

    public void AddUser(AppUser user)
    {
        Companions ??= new();

        if (!Companions.Contains(user.Id))
        {
            Companions.Add(user.Id);
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
