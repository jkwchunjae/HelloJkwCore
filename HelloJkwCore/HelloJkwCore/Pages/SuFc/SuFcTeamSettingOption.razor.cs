using HelloJkwCore.Shared;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcTeamSettingOption : JkwPageBase
    {
        [Inject]
        ISuFcService SuFcService { get; set; }

        TeamSettingOption Option { get; set; } = new();
        List<string> splitOptions = new();
        string SplitAddNames = string.Empty;

        private MemberName SelectedName;
        private List<Member> SelectableMembers = new();

        protected override async Task OnPageInitializedAsync()
        {
            SelectableMembers = await SuFcService.GetAllMember();

            await LoadTeamSettingOption();
        }

        async Task LoadTeamSettingOption()
        {
            Option = await SuFcService.GetTeamSettingOption();
            OnTeamSettingOptionUpdated();
        }

        async Task SaveSettingOption()
        {
            await SuFcService.SaveTeamSettingOption(Option);
        }

        void OnTeamSettingOptionUpdated()
        {
            splitOptions = Option.SplitOptions
                .Select(x => x.Names.StringJoin(", "))
                .ToList();
        }

        void DeleteSplitOption(int index)
        {
            Option.SplitOptions.RemoveAt(index);
            OnTeamSettingOptionUpdated();
        }

        void AddName(string name)
        {
            if (SplitAddNames == string.Empty)
            {
                SplitAddNames = name;
            }
            else
            {
                SplitAddNames += ", " + name;
            }
        }

        void AddSplitOption(string names)
        {
            var memberNames = names.Split(',')
                .Where(x => x.Trim() != string.Empty)
                .Select(x => new MemberName(x.Trim()))
                .ToList();

            SplitAddNames = string.Empty;

            Option.SplitOptions.Add(new()
            {
                Names = memberNames,
                Probability = 100,
            });
            OnTeamSettingOptionUpdated();
        }
    }
}
