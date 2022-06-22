namespace ProjectPingpong;

[JsonConverter(typeof(StringIdJsonConverter<PlayerName>))]
public class PlayerName : StringName
{
    public static readonly PlayerName Default = new PlayerName(string.Empty);
    public PlayerName() { }
    public PlayerName(string name)
        : base(name)
    {
    }
}

public class Player
{
    public PlayerName Name { get; set; } = PlayerName.Default;
    public int Class { get; set; } = default;
}
