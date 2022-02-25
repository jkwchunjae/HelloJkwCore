namespace ProjectDiary.Pages;

public partial class DiaryText : JkwPageBase
{
    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    [Parameter]
    public bool AutoFocus { get; set; } = false;


    private async Task OnTextChanged(string text)
    {
        Text = text;

        await TextChanged.InvokeAsync(text);
    }
}
