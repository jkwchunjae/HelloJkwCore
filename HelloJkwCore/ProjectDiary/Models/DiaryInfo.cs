namespace ProjectDiary;

public class DiaryInfo
{
    //public UserId Id { get; set; }
    public UserId Owner { get; set; }
    public DiaryName DiaryName { get; set; }
    public bool IsSecret { get; set; }
    public bool AllowPicture { get; set; } = false;
    public List<UserId> Writers { get; set; } = new();
    public List<UserId> Viewers { get; set; } = new();

    public DiaryInfo()
    {
    }

    public DiaryInfo(UserId owner, DiaryName diaryName, bool isSecret)
        : this()
    {
        Owner = owner;
        DiaryName = diaryName;
        IsSecret = isSecret;
    }

    public DiaryInfo(DiaryInfo info)
    {
        Owner = info.Owner;
        DiaryName = info.DiaryName;
        IsSecret = info.IsSecret;
        Writers = info.Writers;
        Viewers = info.Viewers;
        AllowPicture = info.AllowPicture;
    }

    public bool CanManage(UserId userId)
    {
        if (Owner == userId)
            return true;

        return false;
    }

    public bool CanWrite(UserId userId)
    {
        if (Owner == userId)
            return true;
        if (Writers?.Contains(userId) ?? false)
            return true;

        return false;
    }

    public bool CanRead(UserId userId)
    {
        if (CanWrite(userId))
            return true;
        if (Viewers?.Contains(userId) ?? false)
            return true;

        return false;
    }
}