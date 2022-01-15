using System;

namespace ProjectDiary;

public class DiaryContent
{
    public DateTime Date { get; set; }
    public DateTime RegDate { get; set; }
    public DateTime LastModifyDate { get; set; }
    public bool IsSecret { get; set; }
    public int Index { get; set; }
    public string Text { get; set; }

    public string GetFileName()
    {
        return $"{Date:yyyyMMdd}_{Index}.diary";
    }
}