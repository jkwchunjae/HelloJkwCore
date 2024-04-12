namespace ProjectDiary;

public class DiaryContent
{
    public DateTime Date { get; set; }
    public DateTime RegDate { get; set; }
    public DateTime LastModifyDate { get; set; }
    public UserId WriterId { get; set; } = null;
    public string WirterName { get; set; } = null;
    public bool IsSecret { get; set; }
    public int Index { get; set; }
    public string Text { get; set; }
    public List<string> Pictures { get; set; } = null;

    public string GetFileName()
    {
        return $"{Date:yyyyMMdd}_{Index}.diary";
    }

    [TextJsonIgnore]
    public int PictureLastIndex
    {
        get
        {
            // picture = 19890201_001_FILE_NAME.jpg
            if (Pictures?.Any() ?? false)
            {
                var lastIndex = Pictures
                    .Select(pic => pic.Split(['_', '.']))
                    .Where(arr => arr.Length >= 3)
                    .Select(arr => int.Parse(arr[1]))
                    .Max();
                return lastIndex;
            }
            else
            {
                return 0;
            }
        }
    }
}