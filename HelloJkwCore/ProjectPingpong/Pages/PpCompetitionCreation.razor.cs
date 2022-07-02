using ProjectPingpong.Utils;

namespace ProjectPingpong.Pages;

public partial class PpCompetitionCreation : JkwPageBase
{
    [Inject] IPpService? Service { get; set; }

    string CompetitionNameText = string.Empty;
    private async Task CreateCompetition(string competitionNameText)
    {
        var competitionData = await Service!.CreateCompetitionAsync(new CompetitionName(competitionNameText));

        if (competitionData != null)
        {
            await Service!.UpdateCompetitionAsync(competitionData.Name, competition =>
            {
                competition.Owner = User.Id;
                return competition;
            });
            Navi.GotoCompetitionPage(competitionData.Name);
        }

    }
}
