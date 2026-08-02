using System.IO;
using System.Text.Json;
using Common;
using Microsoft.AspNetCore.Components;
using ProjectKidsnote.Client;
using ProjectKidsnote.Configuration;
using ProjectKidsnote.Models.Account;
using ProjectKidsnote.Models.Authentication;
using ProjectKidsnote.Models.Reports;
using ProjectKidsnote.Services;

namespace ProjectKidsnote.Pages;

public partial class KidsnoteHome : JkwPageBase
{
    [Inject] private IKidsnoteClient KidsnoteClient { get; set; } = null!;
    [Inject] private IKidsnoteService KidsnoteService { get; set; } = null!;
    [Inject] private KidsnoteOptions Options { get; set; } = null!;

    [Parameter] public long? ReportId { get; set; }

    private string? _errorMessage;
    private UserInfo? _myInfo;
    private Child? _selectedChild;
    private ReportsResponse? _reports;
    private KidsnoteReportIndex _reportIndex = new();
    private SingleReport? _selectedReport;
    private long? _appliedReportId;
    private int _reportLoadVersion;
    private bool _initialDataLoaded;
    private bool _isBusy;
    private bool _isLoadingAllReports;
    private bool _hasLoadedAllReports;
    private int _loadingReportsCount;

    private string LoadAllReportsButtonText => _isLoadingAllReports
        ? $"불러오는 중 ({_loadingReportsCount})"
        : _hasLoadedAllReports
            ? $"불러오기 완료 ({_loadingReportsCount})"
            : "전체불러오기";

    private int SelectedReportIndex =>
        _reportIndex.Reports.FindIndex(report =>
            report.ReportId == _selectedReport?.Id);

    private bool CanNavigatePrevious => SelectedReportIndex > 0;

    private bool CanNavigateNext =>
        SelectedReportIndex >= 0 &&
        SelectedReportIndex < _reportIndex.Reports.Count - 1;

    protected override async Task OnPageInitializedAsync()
    {
        if (!IsAuthenticated)
        {
            Navi.NavigateTo("/account/login");
            return;
        }

        if (User!.HasRole(UserRole.Kidsnote) is false)
        {
            Navi.NavigateTo("/account/login");
            return;
        }

        if (KidsnoteClient.IsLoggedIn)
        {
            await LoadInitialDataAsync();
            return;
        }

        if (Options.HasCredentials)
        {
            await LoginWithConfiguredAccountAsync();
        }
    }

    protected override async Task OnPageParametersSetAsync()
    {
        if (_initialDataLoaded && _appliedReportId != ReportId)
        {
            await LoadReportFromRouteAsync();
        }
    }

    private Task LoginAsync(KidsnoteLoginRequest request) =>
        ExecuteLoginAsync(request.UserId, request.Password);

    private Task LoginWithConfiguredAccountAsync()
    {
        if (!Options.HasCredentials)
        {
            return Task.CompletedTask;
        }

        return ExecuteLoginAsync(Options.UserId!, Options.Password!);
    }

    private async Task ExecuteLoginAsync(string userId, string password)
    {
        _isBusy = true;
        _errorMessage = null;

        try
        {
            await KidsnoteClient.LoginAsync(userId, password);
            await LoadInitialDataAsync();
        }
        catch (Exception exception) when (IsExpectedException(exception))
        {
            _errorMessage = exception.Message;
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async Task LoadInitialDataAsync()
    {
        _isBusy = true;
        _errorMessage = null;

        try
        {
            _myInfo = await KidsnoteClient.GetMyInfoAsync();
            _selectedChild = _myInfo.Children.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "키즈노트 계정에 등록된 자녀가 없습니다.");
            _hasLoadedAllReports = false;
            _reports = await KidsnoteClient.GetReportsAsync(_selectedChild.Id);
            _initialDataLoaded = true;
            await LoadReportFromRouteAsync();
        }
        catch (Exception exception) when (IsExpectedException(exception))
        {
            _errorMessage = exception.Message;
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async Task SelectChildAsync(long childId)
    {
        var child = _myInfo?.Children.FirstOrDefault(item => item.Id == childId);
        if (child is null)
        {
            return;
        }

        _selectedChild = child;
        _hasLoadedAllReports = false;
        await LoadPageAndSelectAsync();
    }

    private async Task LoadReportFromRouteAsync()
    {
        if (_selectedChild is null || _reports is null)
        {
            return;
        }

        _isBusy = true;
        _errorMessage = null;
        var requestedReportId = ReportId;
        var loadVersion = ++_reportLoadVersion;

        try
        {
            if (requestedReportId is null)
            {
                var firstReport = _reports.Results.FirstOrDefault();
                _selectedReport = firstReport;

                if (firstReport is not null)
                {
                    NavigateToReport(firstReport.Id, replaceHistoryEntry: true);
                }

                return;
            }

            if (requestedReportId <= 0)
            {
                throw new ArgumentException("올바르지 않은 알림장 번호입니다.");
            }

            var enrollment = _selectedChild.Enrollment.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    $"{_selectedChild.Name} 자녀의 소속 반 정보가 없습니다.");

            var report = await KidsnoteService.GetSingleReportAsync(
                requestedReportId.Value,
                _selectedChild.Id);
            var reportIndex = await KidsnoteService.GetReportIndexAsync(
                _selectedChild.Id);

            if (loadVersion != _reportLoadVersion ||
                requestedReportId != ReportId)
            {
                return;
            }

            _selectedReport = report;
            _reportIndex = reportIndex;
            _appliedReportId = requestedReportId;
        }
        catch (Exception exception) when (IsExpectedException(exception))
        {
            if (loadVersion != _reportLoadVersion ||
                requestedReportId != ReportId)
            {
                return;
            }

            _selectedReport = null;
            _appliedReportId = requestedReportId;
            _errorMessage = exception.Message;
        }
        finally
        {
            if (loadVersion == _reportLoadVersion)
            {
                _isBusy = false;
            }
        }
    }

    private Task ShowPreviousReportAsync() => ShowAdjacentReportAsync(-1);

    private Task ShowNextReportAsync() => ShowAdjacentReportAsync(1);

    private async Task LoadAllReportsAsync()
    {
        if (_selectedChild is null || _isLoadingAllReports)
        {
            return;
        }

        var childId = _selectedChild.Id;
        _isLoadingAllReports = true;
        _hasLoadedAllReports = false;
        _errorMessage = null;

        try
        {
            int count = 0;
            await foreach (var report in KidsnoteService.GetAllReportsAsync(childId))
            {
                count++;
                _ = InvokeAsync(() =>
                {
                    _loadingReportsCount = count;
                    StateHasChanged();
                });
            }

            var reportIndex = await KidsnoteService.GetReportIndexAsync(childId);
            if (_selectedChild?.Id == childId)
            {
                _reportIndex = reportIndex;
                _hasLoadedAllReports = true;
            }
        }
        catch (Exception exception) when (IsExpectedException(exception))
        {
            _errorMessage = exception.Message;
        }
        finally
        {
            _isLoadingAllReports = false;
        }
    }

    private Task ShowAdjacentReportAsync(int offset)
    {
        var currentIndex = SelectedReportIndex;
        var adjacentIndex = currentIndex + offset;

        if (currentIndex >= 0 &&
            adjacentIndex >= 0 &&
            adjacentIndex < _reportIndex.Reports.Count)
        {
            NavigateToReport(_reportIndex.Reports[adjacentIndex].ReportId);
        }

        return Task.CompletedTask;
    }

    private async Task LoadPageAndSelectAsync()
    {
        if (_selectedChild is null)
        {
            return;
        }

        _isBusy = true;
        _errorMessage = null;
        _reportLoadVersion++;

        try
        {
            _reports = await KidsnoteClient.GetReportsAsync(_selectedChild.Id);
            var report = _reports.Results.FirstOrDefault();

            _selectedReport = report;

            if (report is not null)
            {
                NavigateToReport(report.Id);
            }
        }
        catch (Exception exception) when (IsExpectedException(exception))
        {
            _errorMessage = exception.Message;
        }
        finally
        {
            _isBusy = false;
        }
    }

    private void SelectReportFromCalendar(long reportId)
    {
        NavigateToReport(reportId);
    }

    private void NavigateToReport(long reportId, bool replaceHistoryEntry = false)
    {
        Navi.NavigateTo(
            $"/kidsnote/{reportId}",
            forceLoad: false,
            replace: replaceHistoryEntry);
    }

    private static bool IsExpectedException(Exception exception) =>
        exception is HttpRequestException or
            IOException or
            InvalidOperationException or
            InvalidDataException or
            JsonException or
            ArgumentException;
}
