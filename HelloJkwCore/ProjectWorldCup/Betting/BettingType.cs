namespace ProjectWorldCup;

[TextJsonConverter(typeof(StringEnumConverter))]
public enum BettingType
{
    GroupStage,
    Round16,
    Final,
}
