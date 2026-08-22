using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace ProjectDiary.Pages;

public partial class DiaryText : JkwPageBase
{
    [Parameter] public string Text { get; set; }
    [Parameter] public EventCallback<string> TextChanged { get; set; }
    [Parameter] public EventCallback<bool> ErrorStateChanged { get; set; }
    [Parameter] public bool AutoFocus { get; set; } = false;

    private bool WarningTooLongWord { get; set; }
    private string LongWord { get; set; }

    private async Task OnBlur(FocusEventArgs args)
    {
        await OnTextChanged(Text);
    }

    private async Task OnTextChanged(string text)
    {
        text ??= string.Empty;

        await TextChanged.InvokeAsync(text);

        try
        {
            var tooLongWords = text.Split('\n')
                .SelectMany(line => line.Split(' '))
                .Where(word => word.Length > 20)
                .ToList();

            WarningTooLongWord = tooLongWords.Any();
            await ErrorStateChanged.InvokeAsync(WarningTooLongWord);

            if (WarningTooLongWord)
            {
                LongWord = tooLongWords.First();
            }
        }
        catch
        {
            await ErrorStateChanged.InvokeAsync(false);
        }
    }
}
