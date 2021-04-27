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
        public async Task<List<ScheduleData>> GetAllSchedule()
        {
            var files = await _fs.GetFilesAsync(path => path.GetPath(PathType.SuFcSchedulesPath));

            var list = await files.Select(async file =>
                {
                    return await _fs.ReadJsonAsync<ScheduleData>(path => path.GetPath(PathType.SuFcSchedulesPath) + "/" + file);
                })
                .WhenAll();

            return list.ToList();
        }

        public async Task<bool> SaveSchedule(ScheduleData schedule)
        {
            var fileName = $"{schedule.Date:yyyyMMdd}-{schedule.Title}.json";

            if (fileName.HasInvalidFileNameChar())
            {
                return false;
            }

            await _fs.WriteJsonAsync(path => path.GetPath(PathType.SuFcSchedulesPath) + "/" + fileName, schedule);

            return true;
        }
    }
}
