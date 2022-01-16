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

[JsonConverter(typeof(StringIdJsonConverter<DiaryName>))]
public class DiaryName : StringName
{
    public DiaryName() { }
    public DiaryName(string name) : base(name) { }
}

public class BadukDiary
{
    public DiaryName Name { get; set; }
    public UserId OwnerUserId { get; set; }
    public List<UserId> ConnectUserIdList { get; set; }
    /// <summary> 최근것이 위에있음.  </summary>
    public List<string> GameDataList { get; set; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum StoneColor
{
    None, Black, White
}

[JsonConverter(typeof(StringEnumConverter))]
public enum StoneAction
{
    Set, Remove
}

[JsonConverter(typeof(StringEnumConverter))]
public enum StoneChangeMode
{
    Auto, Menual
}
