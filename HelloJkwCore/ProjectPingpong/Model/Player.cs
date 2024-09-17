using static Dropbox.Api.Files.ListRevisionsMode;

namespace ProjectPingpong;

[TextJsonConverter(typeof(StringIdTextJsonConverter<PlayerName>))]
public record PlayerName : StringId
{
    public static readonly PlayerName Default = new PlayerName(string.Empty);

    public PlayerName(string id) : base(id)
    {
    }
}

public class Player
{
    public PlayerName Name { get; set; } = PlayerName.Default;
    public int Class { get; set; } = default;

    public static bool operator ==(Player? obj1, Player? obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }
        if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return obj1.Equals(obj2);
    }
    public static bool operator !=(Player? obj1, Player? obj2)
    {
        return !(obj1 == obj2);
    }
    public bool Equals(Player other)
    {
        if (ReferenceEquals(other, null))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        else if (obj is Player other)
        {
            return Equals(other);
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
