namespace ProjectDiary;

public class DiaryView
{
    public DiaryInfo DiaryInfo { get; set; }
    public List<DiaryContent> DiaryContents { get; set; }
    public List<string> PicturesUrl { get; set; }
    public DiaryNavigationData DiaryNavigationData { get; set; }
}