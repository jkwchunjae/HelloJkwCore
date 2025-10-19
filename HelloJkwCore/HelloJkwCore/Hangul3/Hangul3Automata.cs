namespace HelloJkwCore.Hangul3;

internal class Hangul3Automata
{
    static readonly string[] leadings = ["ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"];
    static readonly string[] vowels = ["ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ"];
    static readonly string[] tailings = ["", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"];

    static TableItem[] table =
    [
        TableItem.CreateLeadingItem("ㄱ", "ㄱ", "ㄲ"),
        TableItem.CreateLeadingItem("ㄷ", "ㄷ", "ㄸ"),
        TableItem.CreateLeadingItem("ㅂ", "ㅂ", "ㅃ"),
        TableItem.CreateLeadingItem("ㅅ", "ㅅ", "ㅆ"),
        TableItem.CreateLeadingItem("ㅈ", "ㅈ", "ㅉ"),
        TableItem.CreateVowelItem("ㅗ", "ㅏ", "ㅘ"),
        TableItem.CreateVowelItem("ㅗ", "ㅐ", "ㅙ"),
        TableItem.CreateVowelItem("ㅗ", "ㅣ", "ㅚ"),
        TableItem.CreateVowelItem("ㅜ", "ㅓ", "ㅝ"),
        TableItem.CreateVowelItem("ㅜ", "ㅔ", "ㅞ"),
        TableItem.CreateVowelItem("ㅜ", "ㅣ", "ㅟ"),
        TableItem.CreateVowelItem("ㅡ", "ㅣ", "ㅢ"),
        TableItem.CreateTailingItem("ㄱ", "ㄱ", "ㄲ"),
        TableItem.CreateTailingItem("ㄱ", "ㅅ", "ㄳ"),
        TableItem.CreateTailingItem("ㄴ", "ㅈ", "ㄵ"),
        TableItem.CreateTailingItem("ㄴ", "ㅎ", "ㄶ"),
        TableItem.CreateTailingItem("ㄹ", "ㄱ", "ㄺ"),
        TableItem.CreateTailingItem("ㄹ", "ㅁ", "ㄻ"),
        TableItem.CreateTailingItem("ㄹ", "ㅂ", "ㄼ"),
        TableItem.CreateTailingItem("ㄹ", "ㅅ", "ㄽ"),
        TableItem.CreateTailingItem("ㄹ", "ㅌ", "ㄾ"),
        TableItem.CreateTailingItem("ㄹ", "ㅍ", "ㄿ"),
        TableItem.CreateTailingItem("ㄹ", "ㅎ", "ㅀ"),
        TableItem.CreateTailingItem("ㅂ", "ㅅ", "ㅄ"),
    ];

    private Jaso _currentState = default;
    private List<Input3> _history = new();

    public event EventHandler<string>? Composed;
    public event EventHandler<string>? CurrentChanged;
    public event EventHandler? OnEnter;
    public event EventHandler? OnBackspace;


    private void CommitCurrent()
    {
        var hangul = Compose(_currentState);
        Composed?.Invoke(this, hangul);
        CurrentChanged?.Invoke(this, string.Empty);
        _currentState = default;
        _history.Clear();
    }
    private static string Compose(Jaso jaso)
    {
        var indexLeading = leadings.IndexOf(jaso.Leading.Value);
        var indexVowel = vowels.IndexOf(jaso.Vowel.Value);
        var indexTailing = tailings.IndexOf(jaso.Tailing.Value ?? string.Empty);

        var hasLeading = jaso.HasLeading;
        var hasVowel = jaso.HasVowel;
        var hasTailing = jaso.HasTailing;

        if (hasLeading && hasVowel && hasTailing)
        {
            var code = 0xAC00 + (indexLeading * 21 + indexVowel) * 28 + indexTailing;
            return char.ConvertFromUtf32(code);
        }
        else if (hasLeading && hasVowel)
        {
            var code = 0xAC00 + (indexLeading * 21 + indexVowel) * 28;
            return char.ConvertFromUtf32(code);
        }
        else if (hasLeading && !hasVowel && !hasTailing)
        {
            return jaso.Leading.Value;
        }
        else if (hasLeading && !hasVowel && hasTailing)
        {
            throw new InvalidOperationException("초성, 종성 조합으로는 한글을 만들 수 없습니다.");
        }
        else if (!hasLeading && hasVowel && !hasTailing)
        {
            return jaso.Vowel.Value;
        }
        else if (!hasLeading && hasVowel && hasTailing)
        {
            throw new InvalidOperationException("모음, 종성 조합으로는 한글을 만들 수 없습니다.");
        }
        else if (!hasLeading && !hasVowel && hasTailing)
        {
            return jaso.Tailing.Value ?? string.Empty;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary> 세벌식 입력 </summary>
    public void Handle3(Input3 input)
    {
        if (input == Input3.Enter)
        {
            CommitCurrent();
            OnEnter?.Invoke(this, new());
        }
        else if (input == Input3.Backspace)
        {
            if (_history.Any())
            {
                HandleBackspace();
            }
            else
            {
                OnBackspace?.Invoke(this, new());
            }
        }
        else if (input.IsUtil)
        {
            CommitCurrent();
            Composed?.Invoke(this, input.Value);
        }
        else if (input.IsLeading)
        {
            HandleLeading(input);
        }
        else if (input.IsVowel)
        {
            HandleVowel(input);
        }
        else if (input.IsTailing)
        {
            HandleTailing(input);
        }
    }
    private void HandleLeading(Input3 input)
    {
        if (CanDoubleLeading(out var nextLeading))
        {
            // 기존 입력한 초성과 지금 입력한 초성을 합쳤을 때 새 초성을 만들 수 있는가?
            _currentState = _currentState with { Leading = nextLeading };
            _history.Add(input);
            CurrentChanged?.Invoke(this, Compose(_currentState));
            return;
        }
        if (_currentState != default)
        {
            // 앞에 뭔가 입력되고 있었다. 그것을 다 마치고, 새 입력을 시작
            CommitCurrent();
        }

        // 새 입력 시작
        _currentState = new Jaso(input, default, default);
        _history.Add(input);
        CurrentChanged?.Invoke(this, Compose(_currentState));

        bool CanDoubleLeading(out Input3 nextLeading)
        {
            if (_currentState.HasLeadingOnly)
            {
                // 기존 입력한 초성과 지금 입력한 초성을 합쳤을 때 새 초성을 만들 수 있는가?
                var hasNextLeading = table.Any(item => item.Input1 == _currentState.Leading && item.Input2 == input);
                nextLeading = table.FirstOrDefault(item => item.Input1 == _currentState.Leading && item.Input2 == input).Result;
                return hasNextLeading;
            }
            else
            {
                nextLeading = default;
                return false;
            }
        }
    }
    private void HandleVowel(Input3 input)
    {
        if (_currentState.HasLeadingOnly)
        {
            // 초성만 있는 경우. 이제 모음을 입력하자.
            _currentState = _currentState with { Vowel = input };
            _history.Add(input);
            CurrentChanged?.Invoke(this, Compose(_currentState));
        }
        else if (CanDoubleVowel(out var nextVowel))
        {
            // 기존 입력한 모음과 지금 입력한 모음을 합쳤을 때 새 모음을 만들 수 있는 경우
            _currentState = _currentState with { Vowel = nextVowel };
            _history.Add(input);
            CurrentChanged?.Invoke(this, Compose(_currentState));
        }
        else
        {
            CommitCurrent();
            _currentState = new Jaso(default, input, default);
            _history.Add(input);
            CurrentChanged?.Invoke(this, Compose(_currentState));
        }

        bool CanDoubleVowel(out Input3 nextVowel)
        {
            nextVowel = table.FirstOrDefault(item => item.Input1 == _currentState.Vowel && item.Input2 == input).Result;
            return table.Any(item => item.Input1 == _currentState.Vowel && item.Input2 == input);
        }
    }
    private void HandleTailing(Input3 input)
    {
        if (_currentState.HasLeading && _currentState.HasVowel)
        {
            // 초성과 모임이 입력되어 있는 경우
            if (!_currentState.HasTailing)
            {
                // 종성이 입력되어 있지 않은 경우. 가장 간단한 경우
                _currentState = _currentState with { Tailing = input };
                _history.Add(input);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
            else if (CanDoubleTailing(out var nextTailing))
            {
                // 기존 입력한 종성과 지금 입력한 종성을 합쳤을 때 새 종성을 만들 수 있는 경우
                _currentState = _currentState with { Tailing = nextTailing };
                _history.Add(input);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
            else
            {
                // 기존 입력한 종성과 연결되지 않는 경우.
                // 지금 것은 commit 시키고, 종성을 입력한다.
                CommitCurrent();
                _currentState = new Jaso(default, default, input);
                _history.Add(input);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
        }
        else
        {
            // 초성, 모임 중 하나가 입력되어 있지 않은 경우
            // 지금 것은 commit 시키고, 종성을 입력한다.
            CommitCurrent();
            _currentState = new Jaso(default, default, input);
            _history.Add(input);
            CurrentChanged?.Invoke(this, Compose(_currentState));
        }

        bool CanDoubleTailing(out Input3 nextTailing)
        {
            nextTailing = table.FirstOrDefault(item => item.Input1 == _currentState.Tailing && item.Input2 == input).Result;
            return nextTailing != default;
        }
    }

    private void HandleBackspace()
    {
        var lastInput = _history.Last();
        if (lastInput.IsLeading)
        {
            if (table.Any(item => item.Result == _currentState.Leading))
            {
                var doubleLeading = table.FirstOrDefault(item => item.Result == _currentState.Leading);
                _currentState = _currentState with { Leading = doubleLeading.Input1 };
                _history.RemoveAt(_history.Count - 1);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
            else
            {
                _currentState = _currentState with { Leading = default };
                _history.RemoveAt(_history.Count - 1);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
        }
        else if (lastInput.IsVowel)
        {
            if (table.Any(item => item.Result == _currentState.Vowel))
            {
                var doubleVowel = table.FirstOrDefault(item => item.Result == _currentState.Vowel);
                _currentState = _currentState with { Vowel = doubleVowel.Input1 };
                _history.RemoveAt(_history.Count - 1);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
            else
            {
                _currentState = _currentState with { Vowel = default };
                _history.RemoveAt(_history.Count - 1);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
        }
        else if (lastInput.IsTailing)
        {
            if (table.Any(item => item.Result == _currentState.Tailing))
            {
                var doubleTailing = table.FirstOrDefault(item => item.Result == _currentState.Tailing);
                _currentState = _currentState with { Tailing = doubleTailing.Input1 };
                _history.RemoveAt(_history.Count - 1);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
            else
            {
                _currentState = _currentState with { Tailing = default };
                _history.RemoveAt(_history.Count - 1);
                CurrentChanged?.Invoke(this, Compose(_currentState));
            }
        }
    }
    /// <summary> 일반 키보드 입력 (두벌식, 영문) 모두 세벌식으로 처리 </summary>
    public void Handle2(string input, bool shift)
    {
        var input3 = InputConverter.Input2ToInput3(new Input2(input, shift));
        if (input3 != default)
        {
            Handle3(input3);
        }
    }

}
