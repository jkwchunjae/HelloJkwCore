namespace ProjectDiary;

public class DiaryInfo
{
    public UserId Id { get; set; }
    public string Owner { get; set; }
    public string DiaryName { get; set; }
    public bool IsSecret { get; set; }
    public List<string> Writers { get; set; }
    public List<string> Viewers { get; set; }

    public DiaryInfo()
    {
        Writers = new List<string>();
        Viewers = new List<string>();
    }

    public DiaryInfo(UserId id, string owner, string diaryName, bool isSecret)
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

    public bool CanManage(string email)
    {
        if (Owner == email)
            return true;

        return false;
    }

    public bool CanWrite(string email)
    {
        if (Owner == email)
            return true;
        if (Writers?.Contains(email) ?? false)
            return true;

        return false;
    }

    public bool CanRead(string email)
    {
        if (CanWrite(email))
            return true;
        if (Viewers?.Contains(email) ?? false)
            return true;

        return false;
    }
}