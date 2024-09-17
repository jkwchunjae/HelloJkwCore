using System.Text.Json.Serialization;

namespace Common;

public record StringId
{
    public string Id { get; init; }

    [JsonConstructor]
    public StringId(string id)
    {
        Id = id;
    }

    public int Length => Id.Length;

    public bool Contains(string value) => Id.Contains(value);
    public sealed override string ToString() => Id;
    public static implicit operator string(StringId id) => id.Id;
}

public class StringIdTextJsonConverter<T> : JsonConverter<T> where T : StringId
{
    private readonly Func<string, T> _create = id => default!;
    public StringIdTextJsonConverter(Func<string, T> create)
    {
        _create = create;
    }

    public override T Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        return _create(reader.GetString()!)
            ?? (T?)Activator.CreateInstance(typeof(T), reader.GetString()!)!;
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, T value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

public static class StringIdExtension
{
    public static bool HasInvalidFileNameChar(this StringId stringId)
    {
        return Path.GetInvalidFileNameChars().Any(stringId.Id.Contains);
    }

    public static bool HasInvalidPathChar(this StringId stringId)
    {
        return Path.GetInvalidPathChars().Any(stringId.Id.Contains);
    }
}
