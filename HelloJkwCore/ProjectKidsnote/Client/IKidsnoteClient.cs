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
        string? next = null,
        CancellationToken cancellationToken = default);

    Task<ReportsResponse> GetReportsAsync(
        Child child,
        string? next = null,
        CancellationToken cancellationToken = default);

    Task<ReportsResponse> GetReportsAsync(
        long classId,
        long childId,
        long centerId,
        string? next = null,
        CancellationToken cancellationToken = default);
}
