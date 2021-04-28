using JkwExtensions;
using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcTeamMaker : JkwPageBase
    {
        [Inject]
        private ISuFcService SuFcService { get; set; }

        private readonly List<(TeamMakerStrategy Strategy, string Name)> TeamMakerStrategies = new List<(TeamMakerStrategy, string)>
        {
            (TeamMakerStrategy.FullRandom, "완전 랜덤"),
            (TeamMakerStrategy.Manual, "직접 입력"),
        };

        TeamResult TeamResult = null;
        int TeamCount = 3;
        string Title = string.Empty;
        TeamMakerStrategy TeamMakerStrategy;

        private List<Member> Members;
        private List<MemberName> LeftMembers = new();
        private TeamName SelectedTeamName;
        private MemberName SelectedMemberName;

        private Dictionary<MemberName, bool> MemberDeleteButton = new();

        private List<TeamResult> TeamResultList = new();

        protected override async Task OnPageInitializedAsync()
        {
            Members = await SuFcService.GetAllMember();
            LeftMembers = Members.Select(x => x.Name).OrderBy(x => x).ToList();
            await LoadTeamListAsync();
        }

        async Task LoadTeamListAsync()
        {
            TeamResultList = await SuFcService.GetAllTeamResult();
        }

        async Task MakeTeam()
        {
            var players = await SuFcService.GetAllMember();
            var names = players.Select(x => x.Name).ToList();
            TeamResult = await SuFcService.MakeTeam(names, TeamCount, TeamMakerStrategy);
            LeftMembers = Members.Select(x => x.Name)
                .OrderBy(x => x)
                .Where(x => TeamResult.Players.Empty(e => e.MemberName == x))
                .ToList();
        }

        async Task SaveFile()
        {
            if (Title.HasInvalidFileNameChar() || Title.Empty())
            {
                return;
            }
            TeamResult.Title = Title;

            await SuFcService.SaveTeamResult(TeamResult);

            await LoadTeamListAsync();
            StateHasChanged();
        }

        void StrategyChanged()
        {
            if (TeamResult != null)
            {
                LeftMembers = Members.Select(x => x.Name)
                    .OrderBy(x => x)
                    .Where(x => TeamResult.Players.Empty(e => e.MemberName == x))
                    .ToList();
                StateHasChanged();
            }
        }

        void AddToTeam(TeamName team, MemberName member)
        {
            TeamResult.Players.Add((member, team));
            LeftMembers.Remove(member);
            MemberDeleteButton[member] = false;
            SelectedMemberName = null;
            StateHasChanged();
        }

        void DeleteFromResult(MemberName name)
        {
            var index = TeamResult.Players.FindIndex(x => x.MemberName == name);
            if (index != -1)
            {
                TeamResult.Players.RemoveAt(index);
                LeftMembers.Add(name);
                StateHasChanged();
            }
        }

        void ChangeDeleteButton(MemberName name, bool enable)
        {
            MemberDeleteButton[name] = enable;
            StateHasChanged();
        }
    }
}
