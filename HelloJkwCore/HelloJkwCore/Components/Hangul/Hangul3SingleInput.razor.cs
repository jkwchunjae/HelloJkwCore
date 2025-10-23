using HelloJkwCore.Hangul3;
using Microsoft.JSInterop;

namespace HelloJkwCore.Components.Hangul;

public partial class Hangul3SingleInput : JkwPageBase, IAsyncDisposable
{
    private Hangul3Automata automata = new Hangul3Automata();
    /// <summary>
    /// 완성된 문자열
    /// </summary>
    private string FinalText = string.Empty;
    /// <summary>
    /// 작성중인 문자 (한글자)
    /// </summary>
    private string CurrentText = string.Empty;

    DotNetObjectReference<Hangul3SingleInput>? objRef;

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            objRef = DotNetObjectReference.Create(this);
            await Js.InvokeVoidAsync("keyListener.initialize", objRef);

            automata.Composed += Composed;
            automata.CurrentChanged += CurrentChanged;
            automata.OnBackspace += OnBackspace;
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (objRef != null)
            {
                await Js.InvokeVoidAsync("keyListener.dispose", objRef);
                objRef.Dispose();
            }
        }
        catch
        {
            // 무시
        }
    }

    [JSInvokable]
    public void OnKeyDown(string key)
    {
        automata.Handle2(key, false);
        StateHasChanged();
    }

    [JSInvokable]
    public void OnKeyUp(string key)
    {
        StateHasChanged();
    }

    private void Composed(object? sender, string text)
    {
        FinalText += text;
        StateHasChanged();
    }

    private void CurrentChanged(object? sender, string text)
    {
        CurrentText = text;
        StateHasChanged();
    }

    private void OnBackspace(object? sender, EventArgs e)
    {
        if (FinalText.Length > 0)
        {
            FinalText = FinalText[..^1];
        }
        StateHasChanged();
    }
}
