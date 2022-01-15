﻿namespace ProjectWorldCup.Pages;

public partial class TeamScore : JkwPageBase
{
    [Parameter]
    public Team Team { get; set; }
    [Parameter]
    public int? Score { get; set; }
    [Parameter]
    public string Class { get; set; } = string.Empty;
    [Parameter]
    public string Style { get; set; } = string.Empty;
}