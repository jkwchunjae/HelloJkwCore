using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloJkwService.Diary
{
    public static class DiaryUtilExtension
    {
        public static IEnumerable<DiaryData> WhereAndDecrypt(this IEnumerable<DiaryData> list, DateTime date, string password = null)
        {
            var result = list.Where(x => x.Date == date);

            foreach (var diary in result)
            {
                if (diary.IsSecure && !string.IsNullOrEmpty(password))
                {
                    try
                    {
                        diary.Text = diary.Text.Decrypt(password);
                        diary.IsSecure = false;
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }
    }
}
