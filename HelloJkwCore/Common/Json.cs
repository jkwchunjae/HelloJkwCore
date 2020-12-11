using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    public static class Json
    {
        private static JsonSerializerOptions _options;
        static Json()
        {
            _options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            };
        }
        public static T Deserialize<T>(string jsonText)
        {
            return JsonSerializer.Deserialize<T>(jsonText, _options);
        }

        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize<T>(value, _options);
        }
    }
}
