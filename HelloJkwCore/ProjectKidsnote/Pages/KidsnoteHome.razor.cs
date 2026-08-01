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
    private SingleReport? _selectedReport;
    private long? _appliedReportId;
    private int _reportLoadVersion;
    private bool _initialDataLoaded;
    private bool _isBusy;

    private int SelectedReportIndex =>
        _reports?.Results.FindIndex(report => report.Id == _selectedReport?.Id) ?? -1;

    private bool CanNavigatePrevious =>
        SelectedReportIndex > 0 ||
        (SelectedReportIndex == 0 && !string.IsNullOrWhiteSpace(_reports?.Previous));

    private bool CanNavigateNext =>
        SelectedReportIndex >= 0 &&
        (SelectedReportIndex < _reports!.Results.Count - 1 ||
         !string.IsNullOrWhiteSpace(_reports.Next));

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
            _reports = await KidsnoteClient.GetReportsAsync(_selectedChild);
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
                enrollment.BelongToClass,
                _selectedChild.Id,
                enrollment.CenterId);

            if (loadVersion != _reportLoadVersion ||
                requestedReportId != ReportId)
            {
                return;
            }

            _selectedReport = report;
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

    private async Task ShowAdjacentReportAsync(int offset)
    {
        if (_reports is null)
        {
            return;
        }

        var currentIndex = SelectedReportIndex;
        var adjacentIndex = currentIndex + offset;

        if (currentIndex >= 0 &&
            adjacentIndex >= 0 &&
            adjacentIndex < _reports.Results.Count)
        {
            NavigateToReport(_reports.Results[adjacentIndex].Id);
            return;
        }

        var page = offset < 0 ? _reports.Previous : _reports.Next;
        if (!string.IsNullOrWhiteSpace(page))
        {
            await LoadPageAndSelectAsync(page, selectLast: offset < 0);
        }
    }

    private async Task LoadPageAndSelectAsync(
        string? page = null,
        bool selectLast = false)
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
            _reports = await KidsnoteClient.GetReportsAsync(_selectedChild, page);
            var report = selectLast
                ? _reports.Results.LastOrDefault()
                : _reports.Results.FirstOrDefault();

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

    private void NavigateToReport(long reportId, bool replaceHistoryEntry = false)
    {
        Navi.NavigateTo(
            $"/kidsnote/{reportId}",
            forceLoad: false,
            replace: replaceHistoryEntry);
    }

    private static bool IsExpectedException(Exception exception) =>
        exception is HttpRequestException or
            InvalidOperationException or
            InvalidDataException or
            JsonException or
            ArgumentException;
}
