using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

public interface ISerializer
{
    T Deserialize<T>(string jsonText);
    string Serialize<T>(T value);
    string SerializeNoIndent<T>(T value);
}

public class Json : ISerializer
{
    private JsonSerializerOptions _options;
    private JsonSerializerOptions _optionsNoIndent;
    public Json(IEnumerable<JsonConverter> converters)
    {
        _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
        };
        converters.ForEach(_options.Converters.Add);

        _optionsNoIndent = new JsonSerializerOptions(_options)
        {
            WriteIndented = false,
        };
    }
    public T Deserialize<T>(string jsonText)
    {
        return JsonSerializer.Deserialize<T>(jsonText, _options)!;
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize<T>(value, _options);
    }

    public string SerializeNoIndent<T>(T value)
    {
        return JsonSerializer.Serialize<T>(value, _optionsNoIndent);
    }
}