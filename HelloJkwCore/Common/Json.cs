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
            WriteIndented = true,
        };
    }
    public static T Deserialize<T>(string jsonText, params JsonConverter[] converters)
    {
        return JsonConvert.DeserializeObject<T>(jsonText, converters);
        //return JsonSerializer.Deserialize<T>(jsonText, _options);
    }

    public static string Serialize<T>(T value, params JsonConverter[] converters)
    {
        return JsonConvert.SerializeObject(value, Formatting.Indented, converters);
        //return JsonSerializer.Serialize<T>(value, _options);
    }

    public static string SerializeNoIndent<T>(T value)
    {
        return JsonConvert.SerializeObject(value);
        //return JsonSerializer.Serialize<T>(value, _optionsNoIndent);
    }
}