namespace ProjectWorldCup.Pages;

public class Round
{
    public string RoundName { get; set; }
    public List<KnMatch> Matches { get; set; }
}
public partial class KnockoutStage : JkwPageBase
{
    [Inject]
    private IWorldCupService Service { get; set; }

    private KnockoutStageData Knockout { get; set; } = new();

    private KnMatch Final => Knockout.Final;
    private KnMatch ThirdPlacePlayOff => Knockout.ThirdPlacePlayOff;
    private List<KnMatch> SemiFinals => Knockout.SemiFinals;
    private List<KnMatch> QuarterFinals => Knockout.QuarterFinals;
    private List<KnMatch> Round16 => Knockout.Round16;

    private List<Round> KnockoutRounds = new();

    protected override async Task OnPageInitializedAsync()
    {
        var knockoutMatches = await Service.GetKnockoutStageMatchesAsync();
        Knockout = new KnockoutStageData
        {
            Final = knockoutMatches.FirstOrDefault(m => m.StageId == Fifa.FinalStageId),
            ThirdPlacePlayOff = knockoutMatches.FirstOrDefault(m => m.StageId == Fifa.ThirdStageId),
            SemiFinals = knockoutMatches.Where(m => m.StageId == Fifa.Round4StageId).ToList(),
            QuarterFinals = knockoutMatches.Where(m => m.StageId == Fifa.Round8StageId).ToList(),
            Round16 = knockoutMatches.Where(m => m.StageId == Fifa.Round16StageId).ToList(),
        };

        KnockoutRounds = new List<Round>
        {
            new Round
            {
                RoundName = "결승",
                Matches = new List<KnMatch> { Knockout.Final },
            },
            new Round
            {
                RoundName = "3, 4위 전",
                Matches = new List<KnMatch> { Knockout.ThirdPlacePlayOff },
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