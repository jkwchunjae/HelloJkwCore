using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StringId : IComparable<StringId>, IEquatable<StringId>
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
        [JsonIgnore]
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
