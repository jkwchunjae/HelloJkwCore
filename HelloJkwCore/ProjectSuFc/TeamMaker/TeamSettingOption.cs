namespace ProjectSuFc;

public class MergeSplitOption
{
    public List<MemberName> Names { get; set; } = new();
    public double Probability { get; set; }

    [JsonIgnore] public bool Filled => (Names?.Count ?? 0) >= 2;
}
public class TeamSettingOption
{
    public List<MergeSplitOption> SplitOptions { get; set; } = new();
    public List<MergeSplitOption> MergeOptions { get; set; } = new();
    public List<MergeSplitOption> ClassOptions { get; set; } = new();
}