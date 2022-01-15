using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common;

[JsonConverter(typeof(StringEnumConverter))]
public enum ThemeType
{
    Default,
    Dark,
}