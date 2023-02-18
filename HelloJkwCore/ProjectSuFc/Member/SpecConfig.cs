namespace ProjectSuFc;

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum MemberSpecType
{
    [SpecConfig(specName: "출석 확률", defaultValue: 1, min: 0, max: 1, step: 0.1)]
    AttendProb,
}

public class SpecConfigAttribute : Attribute
{
    public string Name { get; }
    public double Default { get; }
    public double Min { get; }
    public double Max { get; }
    public double Step { get; }
    public SpecConfigAttribute(string specName, double defaultValue, double min, double max, double step)
    {
        Name = specName;
        Default = defaultValue;
        Min = min;
        Max = max;
        Step = step;
    }
}
