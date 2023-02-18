namespace Common;

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public class UserId : StringId
{
    public UserId() { }
    public UserId(string id)
        : base(id)
    {
    }
}
