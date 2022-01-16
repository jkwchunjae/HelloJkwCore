namespace Common;

[JsonConverter(typeof(StringIdJsonConverter<UserId>))]
public class UserId : StringId
{
    public UserId() { }
    public UserId(string id)
        : base(id)
    {
    }
}
