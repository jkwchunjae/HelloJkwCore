using HelloJkwCore.Tetration;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HelloJkwCore.Components.MyTetration;

public record Rectangle(float X, float Y, float Width, float Height);

public partial class ComplexPlaneComponent : JkwPageBase
{
    [Parameter] public EventCallback<Rectangle> RectangleUpdated { get; set; }
    [Parameter] public TetrationService? TetrationService { get; set; } = default!;
    private IJSObjectReference? module;

    protected override Task OnPageInitializedAsync()
    {
        if (TetrationService != null)
        {
            TetrationService.OnTetrationResult += async (sender, result) =>
            {
                await DrawBase64Image(result.Base64Image);
            };
        }
        return Task.CompletedTask;
    }

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var dotNetRef = DotNetObjectReference.Create(this);
            var jsPath = "/js/ComplexPlane.js";
            module = await Js.InvokeAsync<IJSObjectReference>("import", jsPath);

            await module.InvokeVoidAsync("canvas_register", dotNetRef, "complexPlane", 0, 0, 10, 10.0 / 1920 * 1080, 1920, 1080);
            await module.InvokeVoidAsync("initComplexPlane");
        }
    }

    public async Task DrawBase64Image(string base64Image)
    {
        if (module != null)
        {
            // await Js.InvokeVoidAsync("console.log", "DrawBase64Image");
            await module.InvokeVoidAsync("drawBase64Image", base64Image);
            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task OnRectangleUpdated(float x, float y, float width, float height)
    {
        await Js.InvokeVoidAsync("console.log", "OnRectangleUpdated: " + x + ", " + y + ", " + width + ", " + height);
        await RectangleUpdated.InvokeAsync(new Rectangle(x, y, width, height));
    }
}
