using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StringId : IComparable<StringId>, IComparable<string>
    {
        public string Id { get; set; }

        public StringId() { }
        public StringId(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id;
        }

        public int CompareTo(StringId other)
        {
            return Id.CompareTo(other.Id);
        }

        public int CompareTo(string other)
        {
            return Id.CompareTo(other);
        }
    }

    public class StringIdJsonConverter<T> : JsonConverter<T> where T : StringId, new()
    {
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
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

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
