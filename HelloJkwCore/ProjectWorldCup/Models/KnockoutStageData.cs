using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup;

public class KnockoutStageData
{
    /// <summary> 결승 </summary>
    public Match Final { get; set; } = new();
    /// <summary> 3,4위 전 </summary>
    public Match ThirdPlacePlayOff { get; set; } = new();
    /// <summary> 4강 </summary>
    public List<Match> SemiFinals { get; set; } = new();
    /// <summary> 8강 </summary>
    public List<Match> QuarterFinals { get; set; } = new();
    /// <summary> 16강 </summary>
    public List<Match> Round16 { get; set; } = new();
}