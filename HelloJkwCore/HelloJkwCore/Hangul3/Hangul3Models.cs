namespace HelloJkwCore.Hangul3;

internal enum JasoType { Leading, Vowel, Tailing, Util }
internal record struct Jaso(Input3 Leading, Input3 Vowel, Input3 Tailing)
{
    public Jaso(string l, string v, string t)
        : this(new Input3(l, JasoType.Leading), new Input3(v, JasoType.Vowel), new Input3(t, JasoType.Tailing))
    {
    }
    public bool HasLeading => !string.IsNullOrEmpty(Leading.Value);
    public bool HasVowel => !string.IsNullOrEmpty(Vowel.Value);
    public bool HasTailing => !string.IsNullOrEmpty(Tailing.Value);
    public bool HasLeadingOnly => HasLeading && !HasVowel && !HasTailing;
}
internal record struct Input2(string Value, bool Shift);
internal record struct Input3(string Value, JasoType Type)
{
    public bool IsLeading => Type == JasoType.Leading;
    public bool IsVowel => Type == JasoType.Vowel;
    public bool IsTailing => Type == JasoType.Tailing;
    public bool IsUtil => Type == JasoType.Util;

    public static Input3 Leading(string value) => new Input3(value, JasoType.Leading);
    public static Input3 Vowel(string value) => new Input3(value, JasoType.Vowel);
    public static Input3 Tailing(string value) => new Input3(value, JasoType.Tailing);
    public static Input3 Util(string value) => new Input3(value, JasoType.Util);

    public static Input3 Enter => new Input3("Enter", JasoType.Util);
    public static Input3 Backspace => new Input3("Backspace", JasoType.Util);
    public static Input3 Space => new Input3(" ", JasoType.Util);
}
internal record struct TableItem(Input3 Input1, Input3 Input2, Input3 Result)
{
    private static TableItem CreateItem(JasoType type, string input1, string input2, string result)
    {
        return new TableItem(new Input3(input1, type), new Input3(input2, type), new Input3(result, type));
    }
    public static TableItem CreateLeadingItem(string input1, string input2, string result)
    {
        return CreateItem(JasoType.Leading, input1, input2, result);
    }
    public static TableItem CreateVowelItem(string input1, string input2, string result)
    {
        return CreateItem(JasoType.Vowel, input1, input2, result);
    }
    public static TableItem CreateTailingItem(string input1, string input2, string result)
    {
        return CreateItem(JasoType.Tailing, input1, input2, result);
    }
}
