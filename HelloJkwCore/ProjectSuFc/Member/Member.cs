namespace ProjectSuFc;

[JsonNetConverter(typeof(StringIdJsonNetConverter<MemberName>))]
[TextJsonConverter(typeof(StringIdTextJsonConverter<MemberName>))]
public class MemberName : StringId
{
    [JsonNetIgnore]
    [TextJsonIgnore]
    public string Name => Id;

    public MemberName() { }
    public MemberName(string name)
        : base(name)
    {
    }
}

public class Member
{
    public int No { get; set; }
    public MemberName Name { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime Birthday { get; set; }
    public List<string> ConnectIdList { get; set; } = new();
    public List<SeasonId> JoinedSeason { get; set; } = new();
    public Dictionary<MemberSpecType, double> Spec { get; set; } = new();

    private static Dictionary<MemberSpecType, SpecConfigAttribute> SpecConfig = typeof(MemberSpecType).GetValues<MemberSpecType>()
        .Select(x => new { SpecType = x, SpecConfig = typeof(MemberSpecType).GetMember(x.ToString()).First().GetAttribute<SpecConfigAttribute>() })
        .ToDictionary(x => x.SpecType, x => x.SpecConfig);

    public double GetSpecValue(MemberSpecType specType)
    {
        if (Spec.TryGetValue(specType, out var value))
        {
            return value;
        }
        else if (SpecConfig.TryGetValue(specType, out var config))
        {
            return config.Default;
        }
        else
        {
            return 0;
        }
    }
}