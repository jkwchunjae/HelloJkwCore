namespace ProjectDiary;

public class DiaryFileName : IComparable
{
    private static readonly char[] _separator = new char[] { '_', '.', };

    public string FileName { get; set; }
    public DateTime Date { get; set; }
    public int Index { get; set; }

    public DiaryFileName(string fileName)
    {
        var arr = fileName.Split(_separator);
        FileName = fileName;
        Date = arr[0].ToDate().Value;
        Index = arr[1].ToInt();
    }

    public override string ToString()
    {
        return FileName;
    }

    public override bool Equals(object obj)
    {
        var fileName = obj as DiaryFileName;
        if (fileName == null)
            return false;

        return FileName == fileName.FileName;
    }

    public override int GetHashCode()
    {
        return FileName.GetHashCode();
    }

    public int CompareTo(object obj)
    {
        var fileName = obj as DiaryFileName;

        if (Date != fileName.Date)
            return Date < fileName.Date ? -1 : +1;

        if (Index != fileName.Index)
            return Index < fileName.Index ? -1 : +1;

        return 0;
    }
}