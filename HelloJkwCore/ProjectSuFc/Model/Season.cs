using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc;

[JsonConverter(typeof(StringIdJsonConverter<SeasonId>))]
public class SeasonId : StringId
{
}

public class Season
{
    public SeasonId Id { get; set; }
    public string Name { get; set; }
}