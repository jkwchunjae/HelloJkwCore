using ProjectKidsnote.Models.Account;
using ProjectKidsnote.Models.Reports;

namespace ProjectKidsnote.Client;

public interface IKidsnoteClient
{
    bool IsLoggedIn { get; }

    Task LoginAsync(
        string userId,
        string password,
        CancellationToken cancellationToken = default);

    Task<UserInfo> GetMyInfoAsync(CancellationToken cancellationToken = default);

    Task<ReportsResponse> GetReportsAsync(
        long childId,
        string? next = null,
        CancellationToken cancellationToken = default);

    Task<SingleReport> GetSingleReportAsync(
        long reportId,
        CancellationToken cancellationToken = default);
}
