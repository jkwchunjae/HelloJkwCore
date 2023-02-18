namespace ProjectSuFc;

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum ScheduleMemberStatus
{
    None,
    Yes,
    No,
    NotYet,
}

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum ScheduleStatus
{
    Feature,
    Active,
    Done,
}

public class ScheduleMember
{
    public MemberName Name { get; set; }
    public ScheduleMemberStatus Status { get; set; } = ScheduleMemberStatus.None;

    public ScheduleMember() { }

    public ScheduleMember(MemberName name)
    {
        Name = name;
    }
}

public class ScheduleData
{
    public int Id { get; set; }
    public string Title { get; set; } = "정기모임";
    public DateTime Date { get; set; }
    public string Location { get; set; } = "석수체육공원";
    public string Time { get; set; }
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Feature;
    public List<ScheduleMember> Members { get; set; } = new();
    public string TeamTitle { get; set; }
}