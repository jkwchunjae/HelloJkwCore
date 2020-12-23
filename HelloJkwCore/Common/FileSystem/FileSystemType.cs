using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.FileSystem
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FileSystemType
    {
        InMemory,
        Local,
        Dropbox,
        Azure,
    }
}
