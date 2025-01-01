namespace ProjectDiary;

public class DiaryView
{
    public DiaryInfo DiaryInfo { get; set; }
    public List<DiaryContent> DiaryContents { get; set; }
    public IReadOnlyList<IDiaryPicture> Pictures { get; set; }
    public DiaryNavigationData DiaryNavigationData { get; set; }
}

public interface IDiaryPicture
{
}

public class DiarySasUrlPicture : IDiaryPicture
{
    public string SasUrl { get; set; }
}

public class DiaryBase64Picture : IDiaryPicture
{
    public string Base64 { get; set; }
}
