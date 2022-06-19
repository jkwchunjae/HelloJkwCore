namespace Common;

public class StringId : IComparable<StringId>, IEquatable<StringId>
{
    public string Id { get; set; }

    public StringId() { }
    public StringId(string id)
    {
        Id = id;
    }

    public int Length => Id.Length;

    public bool Contains(string value) => Id.Contains(value);

    public override string ToString()
    {
        return Id;
    }
    public int CompareTo(StringId other)
    {
        return Id.CompareTo(other.Id);
    }
    public static bool operator ==(StringId obj1, StringId obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }
        if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return obj1.Equals(obj2);
    }
    public static bool operator !=(StringId obj1, StringId obj2)
    {
        return !(obj1 == obj2);
    }
    public static bool operator ==(StringId obj1, string obj2)
    {
        return obj1.Id == obj2;
    }
    public static bool operator !=(StringId obj1, string obj2)
    {
        return !(obj1 == obj2);
    }
    public bool Equals(StringId other)
    {
        if (ReferenceEquals(other, null))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as StringId);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public class StringName : StringId
{
    [JsonNetIgnore]
    [TextJsonIgnore]
    public string Name
    {
        get => Id;
        set => Id = value;
    }

    public StringName() { }

    public StringName(string name)
        : base(name)
    {
    }
}

public class StringIdJsonNetConverter<T> : Newtonsoft.Json.JsonConverter<T> where T : StringId, new()
{
    public override T ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader.Value is null)
            return default;

        if (reader.Value is string)
        {
            var stringId = new T();
            stringId.Id = reader.Value as string;
            return stringId;
        }

        return default;
    }

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, T value, Newtonsoft.Json.JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}

public class StringIdTextJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<T> where T : StringId, new()
{
    public override T Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        var stringId = new T();
        stringId.Id = reader.GetString();
        return stringId;
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
        return Path.GetInvalidFileNameChars().Any(chr => stringId.Id.Contains(chr));
    }

    public static bool HasInvalidPathChar(this StringId stringId)
    {
        return Path.GetInvalidPathChars().Any(chr => stringId.Id.Contains(chr));
    }
}
