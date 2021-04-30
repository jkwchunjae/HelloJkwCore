using Common;
using HelloJkwCore.Shared;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace HelloJkwCore.Pages.SuFc
{
    public partial class SuFcMember : JkwPageBase
    {
        [Inject]
        private ISuFcService SuFcService { get; set; }
        [Inject]
        ILoggerFactory LoggerFactory { get; set; }
        private List<Member> Members { get; set; } = new();
        private bool IsUserSomeoneConnected = false;

        private List<(MemberSpecType SpecType, string SpecName, double Min, double Max, double Step)> SpecList = new();

        private Dictionary<(MemberName Name, MemberSpecType SpecType), Timer> _saveTimer = new();

        private ILogger<SuFcMember> _logger;

        private async Task LoadAsync()
        {
            Members = await SuFcService.GetAllMember();
            if (User != null)
            {
                IsUserSomeoneConnected = Members.Any(m => m.ConnectIdList.Contains(User.Id));
            }
        }

        protected override async Task OnPageInitializedAsync()
        {
            _logger = LoggerFactory.CreateLogger<SuFcMember>();

            SpecList = typeof(MemberSpecType).GetValues<MemberSpecType>()
                .Select(x => new { SpecType = x, SpecConfig = typeof(MemberSpecType).GetMember(x.ToString()).First().GetAttribute<SpecConfigAttribute>() })
                .Select(x => (x.SpecType, x.SpecConfig.Name, x.SpecConfig.Min, x.SpecConfig.Max, x.SpecConfig.Step))
                .ToList();

            await LoadAsync();
        }

        private async Task ConnectId(Member member, AppUser user)
        {
            member.ConnectIdList.Add(user.Id);
            var result = await SuFcService.SaveMember(member);
            if (result)
            {
                Navi.NavigateTo("/sufc");
            }
        }

        private async Task DisconnectId(Member member, AppUser user)
        {
            member.ConnectIdList.Remove(user.Id);
            var result = await SuFcService.SaveMember(member);
            if (result)
            {
                await LoadAsync();
            }
        }

        private bool NameAsc = true;
        private void SortName()
        {
            if (NameAsc)
            {
                Members = Members.OrderByDescending(x => x.Name).ToList();
            }
            else
            {
                Members = Members.OrderBy(x => x.Name).ToList();
            }
            NameAsc = !NameAsc;
        }

        private bool SpecAsc = true;
        private void SortSpec(MemberSpecType specType)
        {
            if (SpecAsc)
            {
                Members = Members.OrderByDescending(x => x.GetSpecValue(specType)).ToList();
            }
            else
            {
                Members = Members.OrderBy(x => x.GetSpecValue(specType)).ToList();
            }
            SpecAsc = !SpecAsc;
        }

        private Task ChangeSpecValue(Member member, MemberSpecType specType, ChangeEventArgs args)
        {
            var key = (member.Name, specType);
            if (_saveTimer.TryGetValue(key, out var prevTimer))
            {
                prevTimer.Stop();
                prevTimer.Close();
            }

            var isDouble = double.TryParse(args.Value?.ToString() ?? string.Empty, out var parsedValue);

            if (!isDouble)
                return Task.CompletedTask;

            if (member.GetSpecValue(specType) == parsedValue)
                return Task.CompletedTask; // 이미 같다!

            var timer = new Timer(1000);
            _saveTimer[key] = timer;
            timer.Elapsed += async (s, e) =>
            {
                timer.Stop();
                _saveTimer.Remove(key);

                // save;
                _logger.LogDebug("Save value {name} {type} {value}", member.Name, specType, parsedValue);
                var m = await SuFcService.FindMember(member.Name);
                m.Spec[specType] = parsedValue;
                await SuFcService.SaveMember(m);

                var index = Members.FindIndex(x => x.Name == member.Name);
                if (index != -1)
                    Members[index] = m;
            };
            timer.Start();

            return Task.CompletedTask;
        }
    }
}
