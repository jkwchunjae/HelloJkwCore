namespace ProjectPingpong;

[JsonConverter(typeof(StringEnumConverter))]
public enum KnockoutDepth
{
    None,
    Final,
    SemiFinal,
    QuarterFinal,
    Round16,
    Round32,
    Round64,
    Round128,
    Round256,
    Round512,
    Round1024,
    Round2048,
}


