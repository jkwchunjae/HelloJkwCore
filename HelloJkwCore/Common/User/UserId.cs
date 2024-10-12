namespace Common;

[TextJsonConverter(typeof(StringIdTextJsonConverter<UserId>))]
[TvString]
public record UserId : StringId
{
    public UserId(string id) : base(id)
    {
    }
}