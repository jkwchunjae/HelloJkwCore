namespace ProjectWorldCup;

[JsonConverter(typeof(StringEnumConverter))]
public enum BettingType
{
    GroupStage,
    Round16,
    Final,
}
