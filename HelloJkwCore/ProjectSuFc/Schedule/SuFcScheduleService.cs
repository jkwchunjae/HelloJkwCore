﻿using Common;
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
        public async Task<List<ScheduleData>> GetAllSchedule()
        {
            var files = await _fs.GetFilesAsync(path => path.GetPath(PathType.SuFcSchedulesPath));

            var list = await files.Select(async file =>
                {
                    return await _fs.ReadJsonAsync<ScheduleData>(path => path.GetPath(PathType.SuFcSchedulesPath) + "/" + file);
                })
                .WhenAll();

            return list.OrderByDescending(x => x.Date).ToList();
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

            await _fs.WriteJsonAsync(path => path.GetPath(PathType.SuFcSchedulesPath) + "/" + fileName, schedule);

            return true;
        }

        public async Task<ScheduleData> GetSchedule(int scheduleId)
        {
            var fileName = $"sufc-schedule-{scheduleId}.json";

            if (fileName.HasInvalidFileNameChar())
            {
                return null;
            }

            var schedule = await _fs.ReadJsonAsync<ScheduleData>(path => path.GetPath(PathType.SuFcSchedulesPath) + "/" + fileName);

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
