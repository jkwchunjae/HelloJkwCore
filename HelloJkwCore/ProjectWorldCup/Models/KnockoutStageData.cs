namespace ProjectWorldCup;

public class KnockoutStageData
{
    /// <summary> 결승 </summary>
    public KnMatch Final { get; set; } = new();
    /// <summary> 3,4위 전 </summary>
    public KnMatch ThirdPlacePlayOff { get; set; } = new();
    /// <summary> 4강 </summary>
    public List<KnMatch> SemiFinals { get; set; } = new();
    /// <summary> 8강 </summary>
    public List<KnMatch> QuarterFinals { get; set; } = new();
    /// <summary> 16강 </summary>
    public List<KnMatch> Round16 { get; set; } = new();
}