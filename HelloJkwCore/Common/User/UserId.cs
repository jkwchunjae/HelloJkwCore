namespace Common;

[JsonNetConverter(typeof(StringIdJsonNetConverter<UserId>))]
[TextJsonConverter(typeof(StringIdTextJsonConverter<UserId>))]
public class UserId : StringId
{
    public UserId() { }
    public UserId(string id)
        : base(id)
    {
    }
}
