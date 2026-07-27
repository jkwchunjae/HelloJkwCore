using System.IO;
using System.Text.Json;
using Common;
using Microsoft.AspNetCore.Components;
using ProjectKidsnote.Client;
using ProjectKidsnote.Configuration;
using ProjectKidsnote.Models.Account;
using ProjectKidsnote.Models.Reports;

namespace ProjectKidsnote.Pages;

public partial class KidsnoteHome : JkwPageBase
{
    [Inject] private IKidsnoteClient KidsnoteClient { get; set; } = null!;
    [Inject] private KidsnoteOptions Options { get; set; } = null!;

    private string _userId = string.Empty;
    private string _password = string.Empty;
    private string? _errorMessage;
    private UserInfo? _myInfo;
    private Child? _selectedChild;
    private ReportsResponse? _reports;
    private bool _isBusy;

    private bool CanLogin =>
        !_isBusy &&
        !string.IsNullOrWhiteSpace(_userId) &&
        !string.IsNullOrWhiteSpace(_password);

    protected override async Task OnPageInitializedAsync()
    {
        if (!IsAuthenticated)
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

    private Task LoginAsync() => ExecuteLoginAsync(_userId, _password);

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
            _password = string.Empty;
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
        await LoadPageAsync();
    }

    private async Task LoadPageAsync(string? page = null)
    {
        if (_selectedChild is null)
        {
            return;
        }

        _isBusy = true;
        _errorMessage = null;

        try
        {
            _reports = await KidsnoteClient.GetReportsAsync(_selectedChild, page);
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

    private static bool IsExpectedException(Exception exception) =>
        exception is HttpRequestException or
            InvalidOperationException or
            InvalidDataException or
            JsonException or
            ArgumentException;
}
