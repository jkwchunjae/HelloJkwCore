using Newtonsoft.Json;

namespace ProjectTrip;

internal class LatLngJsonConverter : JsonConverter<LatLng>
{
    public override LatLng ReadJson(JsonReader reader, Type objectType, LatLng existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            var value = (string)reader.Value;

            var arr = value.Replace("(", "").Replace(")", "").Split(',');
            var lat = arr[0].ToDouble();
            var lng = arr[1].ToDouble();

            return new LatLng(lat, lng);
        }
        else if (reader.TokenType == JsonToken.StartObject)
        {
            double lat = 0;
            double lng = 0;

            string currentPropertyName = string.Empty;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.EndObject:
                        return new LatLng(lat, lng);
                    case JsonToken.PropertyName:
                        currentPropertyName = reader.Value.ToString().ToLower();
                        break;
                    case JsonToken.Float:
                    case JsonToken.Integer:
                        if (currentPropertyName.StartsWith("lat"))
                            lat = (double)reader.Value;
                        else if (currentPropertyName.StartsWith("long")
                            || currentPropertyName.StartsWith("lng"))
                            lng = (double)reader.Value;
                        currentPropertyName = string.Empty;
                        break;
                    default:
                        currentPropertyName = string.Empty;
                        break;
                }
            }
        }

        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, LatLng value, JsonSerializer serializer)
    {
        writer.WriteValue($"{value.Latitude},{value.Longitude}");
    }
}
