namespace ProjectDiary;

[TextJsonConverter(typeof(StringIdTextJsonConverter<DiaryName>))]
public record DiaryName : StringId
{
    public DiaryName(string id) : base(id)
    {
    }
}
