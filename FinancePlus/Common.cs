using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancePlus.Storage;

namespace FinancePlus
{
    static class Common
    {
        public static bool Equals(double a, double b)
        {
            return Math.Abs(a - b) < Database.EPSILON;
        }

        public static string toShortMonthYearString(DateTime date)
        {
            string res = "";

            if (date.Month < 10)
                res += "0";
            res += date.Month + "/" + date.Year;

            return res;
        }

    }
}
