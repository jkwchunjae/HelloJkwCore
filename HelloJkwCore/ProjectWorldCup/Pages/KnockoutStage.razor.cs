using Common;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectWorldCup.Pages;

public class Round
{
    public string RoundName { get; set; }
    public List<Match> Matches { get; set; }
}
public partial class KnockoutStage : JkwPageBase
{
    [Inject]
    private IWorldCupService Service { get; set; }

    private KnockoutStageData Knockout { get; set; } = new();

    private Match Final => Knockout.Final;
    private Match ThirdPlacePlayOff => Knockout.ThirdPlacePlayOff;
    private List<Match> SemiFinals => Knockout.SemiFinals;
    private List<Match> QuarterFinals => Knockout.QuarterFinals;
    private List<Match> Round16 => Knockout.Round16;

    private List<Round> KnockoutRounds = new();

    protected override async Task OnPageInitializedAsync()
    {
        Knockout = await Service.GetKnockoutStageDataAsync();

        KnockoutRounds = new List<Round>
        {
            new Round
            {
                RoundName = "결승",
                Matches = new List<Match> { Knockout.Final },
            },
            new Round
            {
                RoundName = "3, 4위 전",
                Matches = new List<Match> { Knockout.ThirdPlacePlayOff },
            },
            new Round
            {
                RoundName = "4강",
                Matches = Knockout.SemiFinals,
            },
            new Round
            {
                RoundName = "8강",
                Matches = Knockout.QuarterFinals,
            },
            new Round
            {
                RoundName = "16강",
                Matches = Knockout.Round16,
            },
        };
    }
}