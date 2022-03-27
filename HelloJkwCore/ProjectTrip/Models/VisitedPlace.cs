namespace ProjectTrip.Models;

public class VisitedPlace : Place
{
    public DateTime Time { get; set; }
    public double? Expense { get; set; }
    public List<LatLng> Markers { get; set; }
    public string Review { get; set; }
    public string Others { get; set; }

    public VisitedPlace(string name, LatLng position, DateTime time)
        : base(name, position)
    {
        Time = time;
    }
}

