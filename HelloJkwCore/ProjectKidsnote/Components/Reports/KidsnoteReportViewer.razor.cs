using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ProjectKidsnote.Models.Reports;

namespace ProjectKidsnote.Components.Reports;

public partial class KidsnoteReportViewer
{
    private const int TransitionDurationMilliseconds = 160;

    [Parameter, EditorRequired] public SingleReport InitialReport { get; set; } = null!;
    [Parameter, EditorRequired] public int InitialImageIndex { get; set; }
    [Parameter, EditorRequired]
    public Func<long, int, CancellationToken, Task<SingleReport?>>
        LoadAdjacentReport { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback OnClosed { get; set; }

    private readonly CancellationTokenSource _disposeTokenSource = new();
    private ElementReference _viewerElement;
    private DotNetObjectReference<KidsnoteReportViewer>? _dotNetReference;
    private IJSObjectReference? _module;
    private IJSObjectReference? _viewerInterop;
    private SingleReport _currentReport = null!;
    private KidsnoteViewerItemKind _itemKind = KidsnoteViewerItemKind.Photo;
    private int _imageIndex;
    private bool _isTransitioning;
    private string? _transitionClass;
    private string? _pendingTextBoundary;
    private string? _notice;

    private string ItemKind =>
        _itemKind == KidsnoteViewerItemKind.Photo ? "photo" : "text";

    private string TransitionClass => _transitionClass ?? string.Empty;

    private string PositionText => _itemKind == KidsnoteViewerItemKind.Photo
        ? $"사진 {_imageIndex + 1} / {_currentReport.AttachedImages.Count}"
        : "알림장";

    protected override void OnInitialized()
    {
        _currentReport = InitialReport;
        _imageIndex = Math.Clamp(
            InitialImageIndex,
            0,
            Math.Max(0, InitialReport.AttachedImages.Count - 1));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await Js.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/ProjectKidsnote/js/kidsnoteReportViewer.js");
            _dotNetReference = DotNetObjectReference.Create(this);
            _viewerInterop = await _module.InvokeAsync<IJSObjectReference>(
                "initialize",
                _viewerElement,
                _dotNetReference);
        }

        if (_pendingTextBoundary is not null && _viewerInterop is not null)
        {
            await _viewerInterop.InvokeVoidAsync(
                "setTextBoundary",
                _pendingTextBoundary);
            _pendingTextBoundary = null;
        }
    }

    [JSInvokable]
    public Task HandleSwipe(int direction) => MoveAsync(direction);

    private async Task MoveAsync(int direction)
    {
        if (_isTransitioning || direction is not (-1 or 1))
        {
            return;
        }

        _isTransitioning = true;
        _notice = null;

        try
        {
            var destination = await ResolveDestinationAsync(
                direction,
                _disposeTokenSource.Token);
            if (destination is null)
            {
                _notice = "더 이상 불러올 데이터가 없습니다.";
                return;
            }

            _transitionClass = direction > 0
                ? "kidsnote-viewer-exit-up"
                : "kidsnote-viewer-exit-down";
            StateHasChanged();
            await Task.Delay(
                TransitionDurationMilliseconds,
                _disposeTokenSource.Token);

            _currentReport = destination.Report;
            _itemKind = destination.ItemKind;
            _imageIndex = destination.ImageIndex;
            _pendingTextBoundary = _itemKind == KidsnoteViewerItemKind.Text
                ? direction > 0 ? "start" : "end"
                : null;
            _transitionClass = direction > 0
                ? "kidsnote-viewer-enter-from-bottom"
                : "kidsnote-viewer-enter-from-top";
            StateHasChanged();
            await Task.Delay(
                TransitionDurationMilliseconds,
                _disposeTokenSource.Token);
            _transitionClass = null;
        }
        catch (OperationCanceledException)
            when (_disposeTokenSource.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _notice = exception.Message;
        }
        finally
        {
            _isTransitioning = false;
            if (!_disposeTokenSource.IsCancellationRequested)
            {
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task<KidsnoteViewerDestination?> ResolveDestinationAsync(
        int direction,
        CancellationToken cancellationToken)
    {
        if (direction > 0)
        {
            if (_itemKind == KidsnoteViewerItemKind.Photo &&
                _imageIndex < _currentReport.AttachedImages.Count - 1)
            {
                return new(
                    _currentReport,
                    KidsnoteViewerItemKind.Photo,
                    _imageIndex + 1);
            }

            if (_itemKind == KidsnoteViewerItemKind.Text &&
                _currentReport.AttachedImages.Count > 0)
            {
                return new(
                    _currentReport,
                    KidsnoteViewerItemKind.Photo,
                    0);
            }

            var olderReport = await LoadAdjacentReport(
                _currentReport.Id,
                1,
                cancellationToken);
            return olderReport is null
                ? null
                : new(
                    olderReport,
                    KidsnoteViewerItemKind.Text,
                    0);
        }

        if (_itemKind == KidsnoteViewerItemKind.Photo)
        {
            return _imageIndex > 0
                ? new(
                    _currentReport,
                    KidsnoteViewerItemKind.Photo,
                    _imageIndex - 1)
                : new(
                    _currentReport,
                    KidsnoteViewerItemKind.Text,
                    0);
        }

        var newerReport = await LoadAdjacentReport(
            _currentReport.Id,
            -1,
            cancellationToken);
        if (newerReport is null)
        {
            return null;
        }

        return newerReport.AttachedImages.Count > 0
            ? new(
                newerReport,
                KidsnoteViewerItemKind.Photo,
                newerReport.AttachedImages.Count - 1)
            : new(
                newerReport,
                KidsnoteViewerItemKind.Text,
                0);
    }

    private async Task HandleKeyDown(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == "Escape")
        {
            await CloseAsync();
            return;
        }

        if (_itemKind != KidsnoteViewerItemKind.Photo)
        {
            return;
        }

        switch (eventArgs.Key)
        {
            case "ArrowUp":
            case "PageDown":
                await MoveAsync(1);
                break;
            case "ArrowDown":
            case "PageUp":
                await MoveAsync(-1);
                break;
        }
    }

    private Task CloseAsync() => OnClosed.InvokeAsync();

    public async ValueTask DisposeAsync()
    {
        await _disposeTokenSource.CancelAsync();

        try
        {
            if (_viewerInterop is not null)
            {
                await _viewerInterop.InvokeVoidAsync("dispose");
                await _viewerInterop.DisposeAsync();
            }

            if (_module is not null)
            {
                await _module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
        }
        finally
        {
            _dotNetReference?.Dispose();
            _disposeTokenSource.Dispose();
        }
    }

    private enum KidsnoteViewerItemKind
    {
        Text,
        Photo,
    }

    private sealed record KidsnoteViewerDestination(
        SingleReport Report,
        KidsnoteViewerItemKind ItemKind,
        int ImageIndex);
}
