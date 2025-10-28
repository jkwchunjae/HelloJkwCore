using HelloJkwCore.Hangul3;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HelloJkwCore.Components.Hangul;

public partial class Hangul3SingleInput : JkwPageBase, IAsyncDisposable
{
    private readonly Hangul3Automata automata = new();
    private static readonly Hangul3Type[] Hangul3TypeOptions =
    [
        Hangul3Type.세벌식_390,
        Hangul3Type.세벌식_최종_391,
    ];

    /// <summary>
    /// 완성된 문자열
    /// </summary>
    private string _finalText = string.Empty;
    /// <summary>
    /// 작성중인 문자 (한글자)
    /// </summary>
    private string CurrentText = string.Empty;

    // 포커스/커서/선택 상태
    private bool isFocused = false;
    private int caretIndex = 0; // _finalText 기준 커서 위치
    private int? selectionAnchorIndex = null; // Shift로 선택 시작 위치(고정)
    private bool isShiftPressed = false;
    private bool isCtrlPressed = false;
    private bool isCommandPressed = false;
    private bool isAltPressed = false;
    private bool showTypeDropdown = false;

    private ElementReference containerRef;

    DotNetObjectReference<Hangul3SingleInput>? objRef;

    [Parameter] public Hangul3Type Hangul3Type { get; set; } = Hangul3Type.세벌식_390;
    [Parameter] public EventCallback<Hangul3Type> Hangul3TypeChanged { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string? FontFamily { get; set; }
    [Parameter] public string? FontSize { get; set; }
    [Parameter] public string? FontWeight { get; set; }
    [Parameter] public string? FontColor { get; set; }
    [Parameter] public string? BackgroundColor { get; set; }
    [Parameter] public string? BorderColor { get; set; }
    [Parameter] public string? BorderWidth { get; set; }
    [Parameter] public string? BorderRadius { get; set; }
    [Parameter] public string? Padding { get; set; }
    [Parameter] public string? FocusBorderColor { get; set; }
    [Parameter] public string? FocusShadowColor { get; set; }
    [Parameter] public string? CaretColor { get; set; }
    [Parameter] public string? CaretWidth { get; set; }
    [Parameter] public string? SelectionColor { get; set; }
    [Parameter] public bool DarkMode { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string ContainerClass
    {
        get
        {
            var classes = new List<string> { "hangul3-input" };
            if (DarkMode)
            {
                classes.Add("hangul3-input--dark");
            }
            if (!string.IsNullOrWhiteSpace(Class))
            {
                classes.Add(Class);
            }
            return string.Join(" ", classes);
        }
    }

    private string ContainerStyle
    {
        get
        {
            var styles = new List<string>();
            AppendCssVariable(styles, "--hangul3-font-family", FontFamily);
            AppendCssVariable(styles, "--hangul3-font-size", FontSize);
            AppendCssVariable(styles, "--hangul3-font-weight", FontWeight);
            AppendCssVariable(styles, "--hangul3-font-color", FontColor);
            AppendCssVariable(styles, "--hangul3-background-color", BackgroundColor);
            AppendCssVariable(styles, "--hangul3-border-color", BorderColor);
            AppendCssVariable(styles, "--hangul3-border-width", BorderWidth);
            AppendCssVariable(styles, "--hangul3-border-radius", BorderRadius);
            AppendCssVariable(styles, "--hangul3-padding", Padding);
            AppendCssVariable(styles, "--hangul3-border-focus-color", FocusBorderColor);
            AppendCssVariable(styles, "--hangul3-focus-shadow-color", FocusShadowColor);
            AppendCssVariable(styles, "--hangul3-caret-color", CaretColor);
            AppendCssVariable(styles, "--hangul3-caret-width", CaretWidth);
            AppendCssVariable(styles, "--hangul3-selection-color", SelectionColor);
            if (!string.IsNullOrWhiteSpace(Style))
            {
                styles.Add(Style.Trim());
            }
            return string.Join(";", styles);
        }
    }

    private static void AppendCssVariable(List<string> styles, string variableName, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            styles.Add($"{variableName}:{value}");
        }
    }

    private string CurrentHangul3TypeDisplayName => GetHangul3TypeDisplayName(Hangul3Type);

    private static string GetHangul3TypeDisplayName(Hangul3Type type)
    {
        return type.ToString().Replace('_', ' ');
    }

    private IReadOnlyDictionary<string, object> RootAttributes
    {
        get
        {
            var attributes = AdditionalAttributes != null
                ? new Dictionary<string, object>(AdditionalAttributes)
                : new Dictionary<string, object>();

            attributes.TryAdd("role", "textbox");
            attributes.TryAdd("aria-multiline", "false");
            attributes.TryAdd("aria-live", "polite");

            return attributes;
        }
    }

    protected override void OnPageParametersSet()
    {
        var incoming = Value ?? string.Empty;
        if (!string.Equals(incoming, _finalText, StringComparison.Ordinal))
        {
            _finalText = incoming;
            caretIndex = Math.Clamp(caretIndex, 0, _finalText.Length);
            ClearSelection();
        }
    }

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            objRef = DotNetObjectReference.Create(this);
            await Js.InvokeVoidAsync("keyListener.initialize", objRef);

            automata.Composed += Composed;
            automata.CurrentChanged += CurrentChanged;
            automata.OnBackspace += OnBackspace;
            caretIndex = 0;
        }
    }

    private void SetFinalText(string newValue)
    {
        var changed = !string.Equals(_finalText, newValue, StringComparison.Ordinal);
        _finalText = newValue;
        if (changed)
        {
            NotifyFinalTextChanged();
        }
    }

    private void NotifyFinalTextChanged()
    {
        if (ValueChanged.HasDelegate)
        {
            _ = InvokeAsync(() => ValueChanged.InvokeAsync(_finalText));
        }
    }

    private async Task OnFocus()
    {
        isFocused = true;
        ResetModifiers();
        if (objRef != null)
        {
            await Js.InvokeVoidAsync("keyListener.setActive", objRef, true);
        }
    }
    private async Task OnBlur()
    {
        isFocused = false;
        ResetModifiers();
        showTypeDropdown = false;
        if (objRef != null)
        {
            await Js.InvokeVoidAsync("keyListener.setActive", objRef, false);
        }
        StateHasChanged();
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
        // 이동/편집 키 처리
        switch (key)
        {
            case Keyboard.Shift:
                isShiftPressed = true;
                return;
            case Keyboard.Control:
                isCtrlPressed = true;
                return;
            case Keyboard.Command:
            case Keyboard.Meta:
                isCommandPressed = true;
                return;
            case Keyboard.Alt:
            case Keyboard.AltGraph:
            case Keyboard.Option:
                isAltPressed = true;
                return;
            case Keyboard.ArrowLeft:
                CommitCompositionIfAny();
                if (isAltPressed)
                {
                    MoveCaretByWord(-1, isShiftPressed);
                }
                else
                {
                    MoveCaretBy(-1, isShiftPressed);
                }
                StateHasChanged();
                return;
            case Keyboard.ArrowRight:
                CommitCompositionIfAny();
                if (isAltPressed)
                {
                    MoveCaretByWord(1, isShiftPressed);
                }
                else
                {
                    MoveCaretBy(1, isShiftPressed);
                }
                StateHasChanged();
                return;
            case Keyboard.Home:
                CommitCompositionIfAny();
                SetCaret(0, isShiftPressed);
                StateHasChanged();
                return;
            case Keyboard.End:
                CommitCompositionIfAny();
                SetCaret(_finalText.Length, isShiftPressed);
                StateHasChanged();
                return;
            case Keyboard.Delete:
                CommitCompositionIfAny();
                HandleDelete();
                StateHasChanged();
                return;
            case "a":
            case "A":
                if (isCtrlPressed || isCommandPressed)
                {
                    CommitCompositionIfAny();
                    SelectAll();
                    StateHasChanged();
                    return;
                }
                break;
        }

        // 일반 입력은 오토마타로 처리
        automata.Handle2(key, false, Hangul3Type);
        StateHasChanged();
    }

    [JSInvokable]
    public void OnKeyUp(string key)
    {
        switch (key)
        {
            case Keyboard.Shift:
                isShiftPressed = false;
                break;
            case Keyboard.Control:
                isCtrlPressed = false;
                break;
            case Keyboard.Command:
            case Keyboard.Meta:
                isCommandPressed = false;
                break;
            case Keyboard.Alt:
            case Keyboard.AltGraph:
            case Keyboard.Option:
                isAltPressed = false;
                break;
        }
    }

    private void Composed(object? sender, string text)
    {
        var nextText = _finalText;
        // 선택이 있으면 먼저 삭제하고 삽입
        if (HasSelection())
        {
            var (start, end) = GetSelectionRange();
            nextText = nextText.Remove(start, end - start);
            caretIndex = start;
        }

        nextText = nextText.Insert(caretIndex, text);
        caretIndex += text.Length;
        ClearSelection();
        SetFinalText(nextText);
        StateHasChanged();
    }

    private void CurrentChanged(object? sender, string text)
    {
        CurrentText = text;
        StateHasChanged();
    }

    private void OnBackspace(object? sender, EventArgs e)
    {
        // 선택 삭제 우선
        if (HasSelection())
        {
            var (start, end) = GetSelectionRange();
            var nextText = _finalText.Remove(start, end - start);
            caretIndex = start;
            ClearSelection();
            SetFinalText(nextText);
            StateHasChanged();
            return;
        }

        // 선택이 없으면 커서 앞 1자 삭제
        if (caretIndex > 0)
        {
            var nextText = _finalText.Remove(caretIndex - 1, 1);
            caretIndex--;
            SetFinalText(nextText);
        }
        StateHasChanged();
    }

    // 조합 중 문자가 있으면 커밋하고 본문에 삽입
    private void CommitCompositionIfAny()
    {
        if (!string.IsNullOrEmpty(CurrentText))
        {
            var composed = automata.Flush();
            if (!string.IsNullOrEmpty(composed))
            {
                var nextText = _finalText;
                if (HasSelection())
                {
                    var (start, end) = GetSelectionRange();
                    nextText = nextText.Remove(start, end - start);
                    caretIndex = start;
                    ClearSelection();
                }
                nextText = nextText.Insert(caretIndex, composed);
                caretIndex += composed.Length;
                SetFinalText(nextText);
            }
            CurrentText = string.Empty;
        }
    }

    private void HandleDelete()
    {
        if (HasSelection())
        {
            var (start, end) = GetSelectionRange();
            var nextText = _finalText.Remove(start, end - start);
            caretIndex = start;
            ClearSelection();
            SetFinalText(nextText);
            return;
        }
        if (caretIndex < _finalText.Length)
        {
            var nextText = _finalText.Remove(caretIndex, 1);
            SetFinalText(nextText);
        }
    }

    private void MoveCaretBy(int delta, bool extendSelection)
    {
        if (HasSelection() && !extendSelection)
        {
            // 선택이 있는 상태에서 확장 없이 이동하면 선택 해제 후 이동
            var (start, end) = GetSelectionRange();
            var next = delta < 0 ? start : end;
            SetCaret(next, false);
        }
        else
        {
            var next = Math.Clamp(caretIndex + delta, 0, _finalText.Length);
            SetCaret(next, extendSelection);
        }
    }

    private void MoveCaretByWord(int direction, bool extendSelection)
    {
        int next = direction < 0
            ? FindPreviousWordBoundary(caretIndex)
            : FindNextWordBoundary(caretIndex);
        SetCaret(next, extendSelection);
    }

    private void SetCaret(int index, bool extendSelection)
    {
        index = Math.Clamp(index, 0, _finalText.Length);
        if (extendSelection)
        {
            selectionAnchorIndex ??= caretIndex; // 최초 확장 시작점 고정
        }
        else
        {
            selectionAnchorIndex = null; // 선택 해제
        }
        caretIndex = index;
    }

    private bool HasSelection()
    {
        return selectionAnchorIndex.HasValue && selectionAnchorIndex.Value != caretIndex;
    }
    private (int start, int end) GetSelectionRange()
    {
        if (!selectionAnchorIndex.HasValue)
        {
            return (caretIndex, caretIndex);
        }
        var a = selectionAnchorIndex.Value;
        return (Math.Min(a, caretIndex), Math.Max(a, caretIndex));
    }
    private void ClearSelection()
    {
        selectionAnchorIndex = null;
    }

    private void ResetModifiers()
    {
        isShiftPressed = false;
        isCtrlPressed = false;
        isCommandPressed = false;
        isAltPressed = false;
    }

    private void SelectAll()
    {
        selectionAnchorIndex = 0;
        caretIndex = _finalText.Length;
    }

    private int FindPreviousWordBoundary(int index)
    {
        index = Math.Clamp(index, 0, _finalText.Length);
        if (index == 0)
        {
            return 0;
        }

        int i = index;

        while (i > 0 && char.IsWhiteSpace(_finalText[i - 1]))
        {
            i--;
        }

        if (i == 0)
        {
            return 0;
        }

        bool isWord = IsWordCharacter(_finalText[i - 1]);
        while (i > 0 && IsWordCharacter(_finalText[i - 1]) == isWord && !char.IsWhiteSpace(_finalText[i - 1]))
        {
            i--;
        }

        return i;
    }

    private int FindNextWordBoundary(int index)
    {
        index = Math.Clamp(index, 0, _finalText.Length);
        int len = _finalText.Length;
        int i = index;

        while (i < len && char.IsWhiteSpace(_finalText[i]))
        {
            i++;
        }

        if (i >= len)
        {
            return len;
        }

        bool isWord = IsWordCharacter(_finalText[i]);
        while (i < len && IsWordCharacter(_finalText[i]) == isWord && !char.IsWhiteSpace(_finalText[i]))
        {
            i++;
        }

        return i;
    }

    private static bool IsWordCharacter(char ch)
    {
        return char.IsLetterOrDigit(ch) || ch == '_';
    }

    private void ToggleTypeDropdown()
    {
        showTypeDropdown = !showTypeDropdown;
    }

    private async Task SelectHangul3Type(Hangul3Type type)
    {
        showTypeDropdown = false;
        if (Hangul3Type != type)
        {
            CommitCompositionIfAny();
            Hangul3Type = type;
            if (Hangul3TypeChanged.HasDelegate)
            {
                await Hangul3TypeChanged.InvokeAsync(type);
            }
        }
        await containerRef.FocusAsync();
    }

    // 화면 렌더 조각
    private RenderFragment RenderSegments() => builder =>
    {
        var hasSelection = HasSelection();
        var (selectionStart, selectionEnd) = GetSelectionRange();
        var compositionIndex = hasSelection ? selectionStart : caretIndex;

        // 클릭으로 커서 배치할 수 있도록 1글자 단위로 span 구성
        for (int i = 0; i <= _finalText.Length; i++)
        {
            // i 위치에 클릭 포인트
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "style", "display:inline-block;width:0;height:1em");
            builder.AddAttribute(2, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SetCaret(i, false)));
            if (isFocused && !hasSelection && i == caretIndex)
            {
                builder.OpenElement(3, "span");
                builder.AddAttribute(4, "class", "caret");
                builder.CloseElement();
            }
            builder.CloseElement();

            // 조합 중인 글자는 선택이 있으면 시작 위치, 아니면 캐럿 위치에 표시
            if (!string.IsNullOrEmpty(CurrentText) && i == compositionIndex)
            {
                builder.OpenElement(5, "span");
                builder.AddAttribute(6, "class", "composing");
                builder.AddContent(7, CurrentText);
                builder.CloseElement();
            }

            if (i < _finalText.Length)
            {
                var ch = _finalText[i].ToString();
                var selected = hasSelection && i >= selectionStart && i < selectionEnd;
                builder.OpenElement(8, "span");
                if (selected)
                {
                    builder.AddAttribute(9, "class", "selection");
                }
                builder.AddAttribute(10, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => SetCaret(i + 1, false)));
                builder.AddContent(11, ch);
                builder.CloseElement();
            }
        }
    };
}
