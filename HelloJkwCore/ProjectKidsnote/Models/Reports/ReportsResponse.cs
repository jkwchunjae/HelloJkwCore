namespace ProjectKidsnote.Models.Reports;

public sealed class ReportsResponse
{
    public int Count { get; init; }
    public string? Next { get; init; }
    public string? Previous { get; init; }
    public List<SingleReport> Results { get; init; } = [];
}
