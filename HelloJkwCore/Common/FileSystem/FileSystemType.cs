using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common;

[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
public enum FileSystemType
{
    InMemory,
    Local,
    Dropbox,
    Azure,
}