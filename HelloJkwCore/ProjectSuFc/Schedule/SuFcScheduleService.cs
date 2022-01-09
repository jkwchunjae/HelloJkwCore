using Common;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public partial class SuFcService : ISuFcService
    {
        private List<ScheduleData> _scheduleList = null;
        public async Task<List<ScheduleData>> GetAllSchedule()
        {
            var cache = _scheduleList;
            if (cache != null)
                return cache;

            var files = await _fs.GetFilesAsync(path => path[SuFcPathType.SuFcSchedulesPath]);

            var list = await files.Select(async file =>
                {
                    return await _fs.ReadJsonAsync<ScheduleData>(path => path[SuFcPathType.SuFcSchedulesPath] + "/" + file);
                })
                .WhenAll();

            cache = list.OrderByDescending(x => x.Date).ToList();
            _scheduleList = cache;

            return cache;
        }

        public async Task<bool> SaveSchedule(ScheduleData schedule)
        {
            if (schedule.Id == 0)
            {
                var list = await GetAllSchedule();
                schedule.Id = list.MaxOrNull(x => x.Id) + 1 ?? 1;
            }

            var fileName = $"sufc-schedule-{schedule.Id}.json";

            if (fileName.HasInvalidFileNameChar())
            {
                return false;
            }

            await _fs.WriteJsonAsync(path => path[SuFcPathType.SuFcSchedulesPath] + "/" + fileName, schedule);

            _scheduleList = null;

            return true;
        }

        public async Task<ScheduleData> GetSchedule(int scheduleId)
        {
            var found = _scheduleList?.Find(x => x.Id == scheduleId);
            if (found != null)
                return found;

            var fileName = $"sufc-schedule-{scheduleId}.json";

            if (fileName.HasInvalidFileNameChar())
            {
                return null;
            }

            var schedule = await _fs.ReadJsonAsync<ScheduleData>(path => path[SuFcPathType.SuFcSchedulesPath] + "/" + fileName);

            return schedule;
        }

        public async Task<(bool Success, ScheduleData Result)> Vote(ScheduleData prevSchedule, MemberName memberName, ScheduleMemberStatus memberStatus)
        {
            var schedule = await GetSchedule(prevSchedule.Id);
            if (schedule == null)
                return (false, null);

            var memberData = schedule.Members.FirstOrDefault(x => x.Name == memberName);
            if (memberData == null)
                return (false, null);

            memberData.Status = memberStatus;

            var result = await SaveSchedule(schedule);
            return (result, result ? schedule : null);
        }
    }
}
