namespace Common;

[JsonNetConverter(typeof(StringIdJsonNetConverter<UserId>))]
[TextJsonConverter(typeof(StringIdTextJsonConverter<UserId>))]
public class UserId : StringId, IEquatable<UserId>
{
    public UserId() { }
    public UserId(string id)
        : base(id)
    {
    }

    public bool Equals(UserId other)
    {
        return base.Equals(other);
    }
}
