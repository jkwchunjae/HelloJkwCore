namespace ProjectTrip.Models;

public class Place
{
    public string Name { get; set; }
    public LatLng Position { get; set; }

    public Place(string name, LatLng position)
    {
        Name = name;
        Position = position;
    }
}
