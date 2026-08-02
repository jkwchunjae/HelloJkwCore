namespace ProjectKidsnote.Models.Reports;

public sealed class KidsnoteReportIndex
{
    public List<KidsnoteReportIndexItem> Reports
    {
        get
        {
            field.Sort((x, y) => y.ReportId.CompareTo(x.ReportId));
            return field;
        }
        init;
    } = [];
}

public sealed class KidsnoteReportIndexItem
{
    public long ReportId { get; init; }
    public required string DateWritten { get; init; }
}
