using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common
{
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public enum FileSystemType
    {
        InMemory,
        Local,
        Dropbox,
        Azure,
    }
}
