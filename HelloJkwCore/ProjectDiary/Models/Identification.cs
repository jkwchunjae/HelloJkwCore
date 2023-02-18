namespace ProjectDiary;

[JsonNetConverter(typeof(StringIdJsonNetConverter<DiaryName>))]
[TextJsonConverter(typeof(StringIdTextJsonConverter<DiaryName>))]
public class DiaryName : StringName
{
    public DiaryName() { }
    public DiaryName(string diaryName)
        : base(diaryName) { }
}
