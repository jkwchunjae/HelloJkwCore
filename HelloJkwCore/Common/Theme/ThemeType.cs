namespace Common;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum ThemeType
{
    Default,
    Dark,
}