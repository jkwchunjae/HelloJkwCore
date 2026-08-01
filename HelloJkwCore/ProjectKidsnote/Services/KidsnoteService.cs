using Common;
using Microsoft.Extensions.DependencyInjection;
using ProjectKidsnote.Client;
using ProjectKidsnote.Models.Reports;

namespace ProjectKidsnote.Services;

public interface IKidsnoteService
{
    Task<SingleReport> GetSingleReportAsync(
        long reportId,
        long classId,
        long childId,
        long centerId,
        CancellationToken cancellationToken = default);
}

public sealed class KidsnoteService : IKidsnoteService, IDisposable
{
    private readonly IKidsnoteClient _kidsnoteClient;
    private readonly IFileSystem _fileSystem;
    private readonly SemaphoreSlim _indexLock = new(1, 1);

    public KidsnoteService(
        IKidsnoteClient kidsnoteClient,
        [FromKeyedServices(nameof(KidsnoteService))] IFileSystem fileSystem)
    {
        _kidsnoteClient = kidsnoteClient;
        _fileSystem = fileSystem;
    }

    public async Task<SingleReport> GetSingleReportAsync(
        long reportId,
        long classId,
        long childId,
        long centerId,
        CancellationToken cancellationToken = default)
    {
        if (reportId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(reportId),
                "알림장 번호는 0보다 커야 합니다.");
        }

        Func<Paths, string> reportPath =
            paths => paths.KidsReportFile(childId, reportId);

        if (await _fileSystem.FileExistsAsync(reportPath, cancellationToken))
        {
            var savedReport = await _fileSystem.ReadJsonAsync<SingleReport>(
                reportPath,
                cancellationToken);
            await UpsertIndexAsync(childId, savedReport, cancellationToken);
            return savedReport;
        }

        var report = await _kidsnoteClient.GetSingleReportAsync(
            reportId,
            classId,
            childId,
            centerId,
            cancellationToken);

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

    public void Dispose()
    {
        _indexLock.Dispose();
    }

    private async Task UpsertIndexAsync(
        long childId,
        SingleReport report,
        CancellationToken cancellationToken)
    {
        await _indexLock.WaitAsync(cancellationToken);

        try
        {
            Func<Paths, string> indexPath =
                paths => paths.KidsReportRootFile(childId);
            var index = await _fileSystem.FileExistsAsync(
                indexPath,
                cancellationToken)
                ? await _fileSystem.ReadJsonAsync<KidsnoteReportIndex>(
                    indexPath,
                    cancellationToken)
                : new KidsnoteReportIndex();

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
                indexPath,
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
}
