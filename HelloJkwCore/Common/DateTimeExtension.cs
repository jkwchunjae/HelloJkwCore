using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum DateLanguage
    {
        KR, EN
    }

    public enum WeekdayFormat
    {
        /// <summary> 월, Mon </summary>
        D,
        /// <summary> 월요일, Monday </summary>
        DDD,
    }

    public static class DateTimeExtension
    {
        public static string GetWeekday(this DateTime date, DateLanguage lang, WeekdayFormat format)
        {
            switch (lang)
            {
                case DateLanguage.KR:
                    var dayKr = "일월화수목금토".Substring(date.DayOfWeek.ToString("d").ToInt(), 1);
                    switch (format)
                    {
                        case WeekdayFormat.D:
                            return dayKr;
                        case WeekdayFormat.DDD:
                            return dayKr + "요일";
                    }
                    break;
                case DateLanguage.EN:
                    switch (format)
                    {
                        case WeekdayFormat.D:
                            return date.DayOfWeek.ToString("f").Substring(0, 3);
                        case WeekdayFormat.DDD:
                            return date.DayOfWeek.ToString("f");
                    }
                    break;
            }
            return string.Empty;
        }
    }
}
