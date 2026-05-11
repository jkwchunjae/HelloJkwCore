using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages;

public partial class WcRemainTime : JkwPageBase, IDisposable
{
    [Parameter] public string ClassName { get; set; }
    [Parameter] public int RemainSeconds { get; set; }
    [Parameter] public EventCallback TimeOver { get; set; }

    IJSObjectReference _interval;
    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _interval = await CreateRemainTimeInterval(ClassName, RemainSeconds);
        }
    }

    private async Task<IJSObjectReference> CreateRemainTimeInterval(string className, int remainSeconds)
    {
        var thisRef = DotNetObjectReference.Create(this);
        var jsPath = "./_content/ProjectWorldCup/js/remainTimeInterval.js";
        var module = await Js.InvokeAsync<IJSObjectReference>("import", jsPath);
        var remainTime = await module.InvokeAsync<IJSObjectReference>("createRemainTimeInterval", thisRef, className, remainSeconds);
        return remainTime;
    }

    [JSInvokable]
    public async Task OnTimeOver()
    {
        await TimeOver.InvokeAsync();
    }

    public void Dispose()
    {
        if (_interval != null)
        {
            try
            {
                _interval.InvokeVoidAsync("dispose");
            }
            catch
            {
            }
        }
    }
}
