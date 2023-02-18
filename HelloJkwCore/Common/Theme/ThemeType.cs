namespace Common;

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum ThemeType
{
    Default,
    Dark,
}