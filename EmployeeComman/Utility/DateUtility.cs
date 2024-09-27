using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.Utility
{
    public static class DateUtility
    {
        public static List<DateTime> GetAllDatesInMonth(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            var datesInMonth = new List<DateTime>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                datesInMonth.Add(new DateTime(year, month, day));
            }

            return datesInMonth;
        }
    }
}
