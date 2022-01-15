namespace ProjectWorldCup;

public enum Gender
{
    Men,
    Women,
}

public class TeamFlag
{
    public string Src { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Title { get; set; }
    public string Alt { get; set; }
}

public class TeamTag
{
    public string Id { get; set; }
    public string Text { get; set; }
}