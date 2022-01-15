using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary;

public class DiarySearchData
{
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string DayOfWeek { get; set; }
    public string Keyword { get; set; }
}