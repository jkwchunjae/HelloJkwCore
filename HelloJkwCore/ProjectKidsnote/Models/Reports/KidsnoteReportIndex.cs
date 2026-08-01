namespace ProjectKidsnote.Models.Reports;

public sealed class KidsnoteReportIndex
{
    public List<KidsnoteReportIndexItem> Reports { get; init; } = [];
}

public sealed class KidsnoteReportIndexItem
{
    public long ReportId { get; init; }
    public required string DateWritten { get; init; }
}
