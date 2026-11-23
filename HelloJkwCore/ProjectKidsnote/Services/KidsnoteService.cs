using Common;
using JkwExtensions;
using Microsoft.Extensions.DependencyInjection;
using ProjectKidsnote.Client;
using ProjectKidsnote.Models.Reports;
using System.Runtime.CompilerServices;

namespace ProjectKidsnote.Services;

public interface IKidsnoteService
{
    Task<KidsnoteReportIndex> GetReportIndexAsync(
        long childId,
        CancellationToken cancellationToken = default);

    Task<SingleReport> GetSingleReportAsync(
        long reportId,
        long childId,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<SingleReport> GetAllReportsAsync(
        long childId,
        CancellationToken cancellationToken = default);
}

public sealed class KidsnoteService : IKidsnoteService
{
    private readonly Func<CancellationToken, Task<IKidsnoteClient>> _getClient;
    private readonly IFileSystem _fileSystem;
    // Separate sessions can open the same child and report concurrently.
    private static readonly SemaphoreSlim[] ReportLocks = Enumerable.Range(0, 32)
        .Select(_ => new SemaphoreSlim(1, 1)).ToArray();
    private static readonly SemaphoreSlim _indexLock = new(1, 1);

    public KidsnoteService(
        IKidsnoteClient kidsnoteClient,
        [FromKeyedServices(nameof(KidsnoteService))] IFileSystem fileSystem)
        : this(_ => Task.FromResult(kidsnoteClient), fileSystem)
    {
    }

    internal KidsnoteService(Func<CancellationToken, Task<IKidsnoteClient>> getClient, IFileSystem fileSystem)
    {
        _getClient = getClient;
        _fileSystem = fileSystem;
    }

    public async Task<SingleReport> GetSingleReportAsync(
        long reportId,
        long childId,
        CancellationToken cancellationToken = default)
    {
        if (reportId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(reportId),
                "알림장 번호는 0보다 커야 합니다.");
        }

        var gate = ReportLocks[reportId % ReportLocks.Length];
        await gate.WaitAsync(cancellationToken);
        try
        {
            return await ReadOrFetchReportAsync(reportId, childId, cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<SingleReport> ReadOrFetchReportAsync(long reportId, long childId, CancellationToken cancellationToken)
    {
        Func<Paths, string> reportPath =
            paths => paths.KidsReportFile(childId, reportId);

        if (await _fileSystem.FileExistsAsync(reportPath, cancellationToken))
        {
            var savedReport = await _fileSystem.ReadJsonAsync<SingleReport>(
                reportPath,
                cancellationToken);
            ValidateReport(savedReport, childId, reportId);
            await UpsertIndexAsync(childId, savedReport, cancellationToken);
            return savedReport;
        }

        var client = await _getClient(cancellationToken);
        var report = await client.GetSingleReportAsync(
            reportId,
            cancellationToken);

        ValidateReport(report, childId, reportId);
        await _fileSystem.CreateDirectoryAsync(
            paths => paths.KidsReportFolder(childId),
            cancellationToken);

        var saved = await _fileSystem.WriteJsonAsync(
            reportPath,
            report,
            cancellationToken);
        if (!saved)
        {
            throw new IOException($"알림장 {reportId} 저장에 실패했습니다.");
        }

        await UpsertIndexAsync(childId, report, cancellationToken);
        return report;
    }

    public async Task<KidsnoteReportIndex> GetReportIndexAsync(
        long childId,
        CancellationToken cancellationToken = default)
    {
        await _indexLock.WaitAsync(cancellationToken);

        try
        {
            return await ReadIndexAsync(childId, cancellationToken);
        }
        finally
        {
            _indexLock.Release();
        }
    }

    private static void ValidateReport(SingleReport report, long childId, long reportId)
    {
        if (report.Child != childId || report.Id != reportId)
            throw new InvalidDataException("요청한 아이의 알림장이 아닙니다.");
    }

    private async Task UpsertIndexAsync(
        long childId,
        SingleReport report,
        CancellationToken cancellationToken)
    {
        await _indexLock.WaitAsync(cancellationToken);

        try
        {
            var index = await ReadIndexAsync(childId, cancellationToken);

            var savedItem = index.Reports.FirstOrDefault(
                item => item.ReportId == report.Id);
            if (savedItem?.DateWritten == report.DateWritten)
            {
                return;
            }

            var indexItem = new KidsnoteReportIndexItem
            {
                ReportId = report.Id,
                DateWritten = report.DateWritten,
            };

            if (savedItem is null)
            {
                index.Reports.Add(indexItem);
            }
            else
            {
                index.Reports[index.Reports.IndexOf(savedItem)] = indexItem;
            }

            var saved = await _fileSystem.WriteJsonAsync(
                paths => paths.KidsReportRootFile(childId),
                index,
                cancellationToken);
            if (!saved)
            {
                throw new IOException("알림장 인덱스 저장에 실패했습니다.");
            }
        }
        finally
        {
            _indexLock.Release();
        }
    }

    private async Task<KidsnoteReportIndex> ReadIndexAsync(
        long childId,
        CancellationToken cancellationToken)
    {
        Func<Paths, string> indexPath =
            paths => paths.KidsReportRootFile(childId);

        return await _fileSystem.FileExistsAsync(indexPath, cancellationToken)
            ? await _fileSystem.ReadJsonAsync<KidsnoteReportIndex>(
                indexPath,
                cancellationToken)
            : new KidsnoteReportIndex();
    }

    public async IAsyncEnumerable<SingleReport> GetAllReportsAsync(long childId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? next = null;
        do
        {
            var reportsResponse = await GetPageAsync(childId, next, cancellationToken);

            var reports = await reportsResponse.Results
                .Select(x => GetSingleReportAsync(x.Id, childId, cancellationToken))
                .WhenAll();

            foreach (var report in reports)
            {
                yield return report;
            }
            next = reportsResponse.Next;
        } while (next != null);
    }

    internal async Task<ReportsResponse> GetPageAsync(long childId, string? next, CancellationToken token)
    {
        var client = await _getClient(token);
        return await client.GetReportsAsync(childId, next, token);
    }
}
