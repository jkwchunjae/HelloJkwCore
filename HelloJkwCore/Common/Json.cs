using System.Text.Json;

namespace Common;

public static class Json
{
    private static JsonSerializerOptions _options;
    private static JsonSerializerOptions _optionsNoIndent;
    static Json()
    {
        _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
        };
        _optionsNoIndent = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
        };
    }
    public static T Deserialize<T>(string jsonText)
    {
        //return JsonConvert.DeserializeObject<T>(jsonText);
        return JsonSerializer.Deserialize<T>(jsonText, _options);
    }

    public static string Serialize<T>(T value)
    {
        //return JsonConvert.SerializeObject(value, Formatting.Indented);
        return JsonSerializer.Serialize<T>(value, _options);
    }

    public static string SerializeNoIndent<T>(T value)
    {
        //return JsonConvert.SerializeObject(value);
        return JsonSerializer.Serialize<T>(value, _optionsNoIndent);
    }
}