namespace ProjectWorldCup;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BettingType
{
    GroupStage,
    Round32,
    Round16,
    Final,
}
