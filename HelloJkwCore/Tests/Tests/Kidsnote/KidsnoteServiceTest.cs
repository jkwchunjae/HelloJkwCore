using System.Text.Json;
using ProjectKidsnote;
using ProjectKidsnote.Client;
using ProjectKidsnote.Models.Reports;
using ProjectKidsnote.Services;

namespace Tests.Kidsnote;

public sealed class KidsnoteServiceTest
{
    private const long ReportId = 1234;
    private const long ClassId = 20;
    private const long ChildId = 30;
    private const long CenterId = 40;

    private readonly InMemoryFileSystem _fileSystem;
    private readonly Mock<IKidsnoteClient> _kidsnoteClient = new();
    private readonly KidsnoteService _service;

    public KidsnoteServiceTest()
    {
        var paths = new PathMap
        {
            InMemory = new Dictionary<string, string>
            {
                [KidsnotePathType.ReportPath] = "/kidsnote/reports",
            },
        };
        _fileSystem = new InMemoryFileSystem(
            new Paths(paths, FileSystemType.InMemory),
            new Json([]));
        _service = new KidsnoteService(_kidsnoteClient.Object, _fileSystem);
    }

    [Fact]
    public async Task GetSingleReportAsync_SavesApiResultAndIndex_WhenFileIsMissing()
    {
        var report = CreateReport();
        _kidsnoteClient
            .Setup(client => client.GetSingleReportAsync(
                ReportId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        var result = await _service.GetSingleReportAsync(
            ReportId,
            ChildId);

        result.Should().BeSameAs(report);
        var savedReport = await _fileSystem.ReadJsonAsync<SingleReport>(
            paths => paths.KidsReportFile(ChildId, ReportId));
        savedReport.Id.Should().Be(ReportId);
        savedReport.Content.Should().Be(report.Content);
        savedReport.AttachedImages.Should().ContainSingle();

        var index = await _service.GetReportIndexAsync(ChildId);
        index.Reports.Should().ContainSingle().Which.Should().BeEquivalentTo(
            new KidsnoteReportIndexItem
            {
                ReportId = ReportId,
                DateWritten = report.DateWritten,
            });

        var files = await _fileSystem.GetFilesAsync(
            paths => paths.KidsReportFolder(ChildId));
        files.Should().BeEquivalentTo($"{ReportId}.json", "_index.json");
    }

    [Fact]
    public async Task GetReportIndexAsync_ReturnsEmptyIndex_WhenFileIsMissing()
    {
        var index = await _service.GetReportIndexAsync(ChildId);

        index.Reports.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSingleReportAsync_ReadsFileBeforeCallingApi()
    {
        var savedReport = CreateReport();
        await _fileSystem.WriteJsonAsync(
            paths => paths.KidsReportFile(ChildId, ReportId),
            savedReport);

        var result = await _service.GetSingleReportAsync(
            ReportId,
            ChildId);

        result.Id.Should().Be(savedReport.Id);
        result.Content.Should().Be(savedReport.Content);
        _kidsnoteClient.Verify(
            client => client.GetSingleReportAsync(
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        var index = await _fileSystem.ReadJsonAsync<KidsnoteReportIndex>(
            paths => paths.KidsReportRootFile(ChildId));
        index.Reports.Should().ContainSingle(item =>
            item.ReportId == ReportId &&
            item.DateWritten == savedReport.DateWritten);
    }

    private static SingleReport CreateReport()
    {
        return new SingleReport
        {
            Id = ReportId,
            DateWritten = "2026-08-02",
            Author = new Author
            {
                Id = 1,
                Type = "teacher",
                Name = "선생님",
                Username = "teacher",
            },
            AuthorName = "선생님",
            Center = CenterId,
            Cls = ClassId,
            ClassName = "햇님반",
            Child = ChildId,
            ChildName = "어린이",
            Content = "오늘의 알림장",
            AttachedImages =
            [
                new AttachedImage
                {
                    Id = JsonSerializer.SerializeToElement(1),
                    AccessKey = "key",
                    OriginalFileName = "photo.jpg",
                    Original = "https://example.com/original.jpg",
                    Large = "https://example.com/large.jpg",
                    Small = "https://example.com/small.jpg",
                    SmallResize = "https://example.com/small-resize.jpg",
                    LargeResize = "https://example.com/large-resize.jpg",
                },
            ],
        };
    }
}
