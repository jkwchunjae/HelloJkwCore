<Query Kind="Program" />

void Main()
{
    ComposeHangul(new Jaso("ㄱ", "ㅏ", "ㄴ")).Dump();
    char.ConvertToUtf32("가", 0).ToHex().Dump("가");
    char.ConvertToUtf32("힣", 0).ToString("x2").Dump("힣");
    char.ConvertToUtf32("ㄱ", 0).ToString("x2").Dump("ㄱ");
    var aa = Enumerable.Range(0x3130, 100)
        .Select(x => new { Index = x.ToHex(), Char = char.ConvertFromUtf32(x) })
        .Dump();
    //char.ConvertFromUtf32(0x3130).Dump("0x3130");
    //char.ConvertFromUtf32(0x3131).Dump("0x3131");
    //char.ConvertFromUtf32(0x3132).Dump("0x3132");
}

string[] L = ["ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ","ㅅ","ㅆ","ㅇ","ㅈ","ㅉ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ"];
string[] V = ["ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ"];
string[] T = ["", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"];

record Jaso(string Leading, string Vowel, string Tailing);

string ComposeHangul(Jaso jaso)
{
    var indexL = L.IndexOf(jaso.Leading);
    var indexV = V.IndexOf(jaso.Vowel);
    var indexT = T.IndexOf(jaso.Tailing);

    if (indexL == -1 || indexV == -1)
        return string.Empty;

    var code = 0xAC00 + (indexL * 21 + indexV) * 28 + indexT;
    return char.ConvertFromUtf32(code);
}

public static class Ex
{
    public static string ToHex(this int value)
    {
        return $"0x{value.ToString("X2")}";
    }
}

public enum JasoType { Leading, Vowel, Tailing }
public record Input3(string Text, JasoType Type);

public class HangulAutomata3
{
    static string[] leadings = ["ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ","ㅅ","ㅆ","ㅇ","ㅈ","ㅉ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ"];
    static string[] vowels = ["ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ"];
    static string[] tailings = ["", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"];

    private Jaso _currentState = new Jaso(string.Empty, string.Empty, string.Empty);

    public event EventHandler<string>? Composed;
    public event EventHandler<string>? CurrentChanged;
       
    private void CommitCurrent()
    {
        var hangul = Compose(_currentState);
        Composed?.Invoke(this, hangul);
        CurrentChanged?.Invoke(this, string.Empty);
        _currentState = new Jaso(string.Empty, string.Empty, string.Empty);
    }
    private static string Compose(Jaso jaso)
    {
        var indexLeading = leadings.IndexOf(jaso.Leading);
        var indexVowel = vowels.IndexOf(jaso.Vowel);
        var indexTailing = tailings.IndexOf(jaso.Tailing);
        
        var hasLeading = indexLeading >= 0;
        var hasVowel = indexLeading >= 0;
        var hasTailing = indexTailing >= 0 && jaso.Tailing != string.Empty;
        
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
            return jaso.Leading;
        }
        else if (hasLeading && !hasVowel && hasTailing)
        {
            throw new InvalidOperationException("초성, 종성 조합으로는 한글을 만들 수 없습니다.");
        }
        else if (!hasLeading && hasVowel && !hasTailing)
        {
            return jaso.Vowel;
        }
        else if (!hasLeading && hasVowel && hasTailing)
        {
            throw new InvalidOperationException("모음, 종성 조합으로는 한글을 만들 수 없습니다.");
        }
        else if (!hasLeading && !hasVowel && hasTailing)
        {
            return jaso.Tailing;
        }
        else
        {
            return string.Empty;
        }
    }
    /// <summary> 세벌식 입력 </summary>
    public void Handle3(Input3 input)
    {
        if (input.Text == "Enter" || input.Text == " ")
        {
            CommitCurrent();
            if (input.Text == " ")
            {
                Composed?.Invoke(this, " ");
            }
            return;
        }
        else if (input.Text == "Backspace")
        {
            
        }
    }
    
    /// <summary> 일반 키보드 입력 (두벌식, 영문) 모두 세벌식으로 처리 </summary>
    public void Handle2(string input, bool shift)
    {
        
    }
}