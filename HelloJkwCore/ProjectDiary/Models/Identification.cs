namespace ProjectDiary;

[JsonConverter(typeof(StringIdJsonConverter<DiaryName>))]
public class DiaryName : StringName
{
    public DiaryName() { }
    public DiaryName(string diaryName)
        : base(diaryName) { }
}
