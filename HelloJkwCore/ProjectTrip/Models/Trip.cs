namespace ProjectTrip.Models;

public class Trip
{
    /// <summary> Url에 사용될 문자열 </summary>
    public StringId Id { get; set; }
    public StringName Title { get; set; }
    public DateTime BeginTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<VisitedPlace> VisitedPlaces { get; set; }
    public List<UserId> Users { get; set; }
    /// <summary> 대표 위치, 요약 화면에서 사용함. </summary>
    public List<LatLng> Positions { get; set; }
}

