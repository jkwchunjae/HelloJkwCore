using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Extensions;

namespace ProjectDiary.Pages;

public partial class DiaryText : JkwPageBase
{
    [Parameter] public string Text { get; set; }
    [Parameter] public EventCallback<string> TextChanged { get; set; }
    [Parameter] public EventCallback<bool> ErrorStateChanged { get; set; }
    [Parameter] public bool AutoFocus { get; set; } = false;

    private bool WarningTooLongWord { get; set; }
    private string LongWord { get; set; }
    private string _currentText = string.Empty;
    private bool _hasCurrentText;

    MudTextField<string> _textField;

    private async Task OnBlur(FocusEventArgs args)
    {
        await FlushAsync();
    }

    private void OnInternalInputChanged()
    {
        _currentText = _textField.GetState(x => x.Text)
                       ?? _textField.GetState(x => x.Value)
                       ?? string.Empty;
        _hasCurrentText = true;
    }

    public async Task FlushAsync()
    {
        var text = _hasCurrentText ? _currentText : null;

        if (text == null)
        {
            text = _textField.GetState(x => x.Text)
                   ?? _textField.GetState(x => x.Value)
                   ?? string.Empty;
        }

        await OnTextChanged(text);
    }

    private async Task OnTextChanged(string text)
    {
        text ??= string.Empty;
        Text = text;
        _currentText = text;
        _hasCurrentText = true;

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
