namespace ProjectWorldCup;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BettingType
{
    GroupStage,
    Round16,
    Final,
}
