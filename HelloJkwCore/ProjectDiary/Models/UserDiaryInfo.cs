namespace ProjectDiary;

public class UserDiaryInfo
{
    public UserId UserId { get; set; }
    public List<DiaryName> MyDiaryList { get; set; } = new();
    public List<DiaryName> WriterList { get; set; } = new();
    public List<DiaryName> ViewList { get; set; } = new();

    public bool IsMine(DiaryName diaryName)
    {
        return MyDiaryList.Contains(diaryName);
    }

    public bool IsWritable(DiaryName diaryName)
    {
        return IsMine(diaryName) || WriterList.Contains(diaryName);
    }

    public bool IsViewable(DiaryName diaryName)
    {
        return IsWritable(diaryName) || ViewList.Contains(diaryName);
    }

    public void AddMyDiary(DiaryName diaryName)
    {
        MyDiaryList.Add(diaryName);
    }
}