namespace Common;

[TextJsonConverter(typeof(StringIdTextJsonConverter<UserId>))]
public record UserId : StringId
{
    public UserId(string id) : base(id)
    {
    }
}