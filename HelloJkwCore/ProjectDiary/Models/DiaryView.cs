using System.Collections.Generic;

namespace ProjectDiary;

public class DiaryView
{
    public DiaryInfo DiaryInfo { get; set; }
    public List<DiaryContent> DiaryContents { get; set; }
    public DiaryNavigationData DiaryNavigationData { get; set; }
}