namespace ProjectDiary;

public class DiaryInfo
{
    public UserId Id { get; set; }
    public string Owner { get; set; }
    public DiaryName DiaryName { get; set; }
    public bool IsSecret { get; set; }
    public List<string> Writers { get; set; } = new();
    public List<string> Viewers { get; set; } = new();

    public DiaryInfo()
    {
    }

    public DiaryInfo(UserId id, string owner, DiaryName diaryName, bool isSecret)
        : this()
    {
        Id = id;
        Owner = owner;
        DiaryName = diaryName;
        IsSecret = isSecret;
    }

    public DiaryInfo(DiaryInfo info)
    {
        Id = info.Id;
        Owner = info.Owner;
        DiaryName = info.DiaryName;
        IsSecret = info.IsSecret;
        Writers = info.Writers;
        Viewers = info.Viewers;
    }

    public bool CanManage(string userId)
    {
        if (Owner == userId)
            return true;

        return false;
    }

    public bool CanWrite(string userId)
    {
        if (Owner == userId)
            return true;
        if (Writers?.Contains(userId) ?? false)
            return true;

        return false;
    }

    public bool CanRead(string userId)
    {
        if (CanWrite(userId))
            return true;
        if (Viewers?.Contains(userId) ?? false)
            return true;

        return false;
    }
}