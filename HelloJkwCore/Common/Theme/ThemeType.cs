namespace Common;

[JsonConverter(typeof(StringEnumConverter))]
public enum ThemeType
{
    Default,
    Dark,
}