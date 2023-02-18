namespace ProjectBaduk;

/// <summary> 바둑 한 판의 정보를 담고있는 데이터 </summary>
public class BadukGameData
{
    public string Subject { get; set; }
    public bool Favorite { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime LastModifyTime { get; set; }
    public string OwnerEmail { get; set; }
    public int Size { get; set; }
    public StoneChangeMode ChangeMode { get; set; }
    public StoneColor CurrentColor { get; set; }
    public int CurrentIndex { get; set; }
    public bool VisibleStoneIndex { get; set; }
    public string Memo { get; set; }
    public List<StoneLogData> StoneLog { get; set; }
}

public class StoneLogData
{
    public int Row { get; set; }
    public int Column { get; set; }
    public StoneAction Action { get; set; }
    public StoneColor Color { get; set; }
}

[JsonNetConverter(typeof(StringIdJsonNetConverter<BadukDiaryName>))]
[TextJsonConverter(typeof(StringIdTextJsonConverter<BadukDiaryName>))]
public class BadukDiaryName : StringName
{
    public BadukDiaryName() { }
    public BadukDiaryName(string name) : base(name) { }
}

public class BadukDiary
{
    public BadukDiaryName Name { get; set; }
    public UserId OwnerUserId { get; set; }
    public List<UserId> ConnectUserIdList { get; set; }
    /// <summary> 최근것이 위에있음.  </summary>
    public List<string> GameDataList { get; set; }
}

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum StoneColor
{
    None, Black, White
}

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum StoneAction
{
    Set, Remove
}

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum StoneChangeMode
{
    Auto, Menual
}
