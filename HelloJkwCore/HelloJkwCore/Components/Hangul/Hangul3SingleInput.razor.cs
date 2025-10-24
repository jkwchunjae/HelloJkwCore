using HelloJkwCore.Hangul3;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

    // 포커스/커서/선택 상태
    private bool isFocused = false;
    private int caretIndex = 0; // FinalText 기준 커서 위치
    private int? selectionAnchorIndex = null; // Shift로 선택 시작 위치(고정)
    private bool isShiftPressed = false;

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
            caretIndex = 0;
        }
    }

    private async Task OnFocus()
    {
        isFocused = true;
        if (objRef != null)
        {
            await Js.InvokeVoidAsync("keyListener.setActive", objRef, true);
        }
    }
    private async Task OnBlur()
    {
        isFocused = false;
        if (objRef != null)
        {
            await Js.InvokeVoidAsync("keyListener.setActive", objRef, false);
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
        if (!isFocused)
        {
            return;
        }

        // 수정키 상태 추적
        if (key == "Shift")
        {
            isShiftPressed = true;
            return;
        }

        // 이동/편집 키 처리
        switch (key)
        {
            case "ArrowLeft":
                CommitCompositionIfAny();
                MoveCaretBy(-1, isShiftPressed);
                StateHasChanged();
                return;
            case "ArrowRight":
                CommitCompositionIfAny();
                MoveCaretBy(1, isShiftPressed);
                StateHasChanged();
                return;
            case "Home":
                CommitCompositionIfAny();
                SetCaret(0, isShiftPressed);
                StateHasChanged();
                return;
            case "End":
                CommitCompositionIfAny();
                SetCaret(FinalText.Length, isShiftPressed);
                StateHasChanged();
                return;
            case "Delete":
                CommitCompositionIfAny();
                HandleDelete();
                StateHasChanged();
                return;
        }

        // 일반 입력은 오토마타로 처리
        automata.Handle2(key, false);
        StateHasChanged();
    }

    [JSInvokable]
    public void OnKeyUp(string key)
    {
        if (key == "Shift")
        {
            isShiftPressed = false;
        }
        StateHasChanged();
    }

    private void Composed(object? sender, string text)
    {
        // 선택이 있으면 먼저 삭제하고 삽입
        if (HasSelection())
        {
            var (start, end) = GetSelectionRange();
            FinalText = FinalText.Remove(start, end - start);
            caretIndex = start;
        }

        FinalText = FinalText.Insert(caretIndex, text);
        caretIndex += text.Length;
        ClearSelection();
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
            FinalText = FinalText.Remove(start, end - start);
            caretIndex = start;
            ClearSelection();
            StateHasChanged();
            return;
        }

        // 선택이 없으면 커서 앞 1자 삭제
        if (caretIndex > 0)
        {
            FinalText = FinalText.Remove(caretIndex - 1, 1);
            caretIndex--;
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
                if (HasSelection())
                {
                    var (start, end) = GetSelectionRange();
                    FinalText = FinalText.Remove(start, end - start);
                    caretIndex = start;
                    ClearSelection();
                }
                FinalText = FinalText.Insert(caretIndex, composed);
                caretIndex += composed.Length;
            }
            CurrentText = string.Empty;
        }
    }

    private void HandleDelete()
    {
        if (HasSelection())
        {
            var (start, end) = GetSelectionRange();
            FinalText = FinalText.Remove(start, end - start);
            caretIndex = start;
            ClearSelection();
            return;
        }
        if (caretIndex < FinalText.Length)
        {
            FinalText = FinalText.Remove(caretIndex, 1);
        }
    }

    private void MoveCaretBy(int delta, bool extendSelection)
    {
        var next = Math.Clamp(caretIndex + delta, 0, FinalText.Length);
        SetCaret(next, extendSelection);
    }

    private void SetCaret(int index, bool extendSelection)
    {
        index = Math.Clamp(index, 0, FinalText.Length);
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

    // 화면 렌더 조각
    private RenderFragment RenderSegments() => builder =>
    {
        var hasSelection = HasSelection();
        var (selectionStart, selectionEnd) = GetSelectionRange();
        var compositionIndex = hasSelection ? selectionStart : caretIndex;

        // 클릭으로 커서 배치할 수 있도록 1글자 단위로 span 구성
        for (int i = 0; i <= FinalText.Length; i++)
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

            if (i < FinalText.Length)
            {
                var ch = FinalText[i].ToString();
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
