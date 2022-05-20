namespace ProjectSuFc;

public class MergeSplitOption
{
    public List<MemberName> Names { get; set; } = new();
    public double Probability { get; set; }
}
public class TeamSettingOption
{
    public List<MergeSplitOption> SplitOptions { get; set; } = new();
    public List<MergeSplitOption> MergeOptions { get; set; } = new();
}