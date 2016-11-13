using System;
using System.Collections.Generic;

namespace PriceObjects
{
    public class YearMonthDateComparer : EqualityComparer<DateTime>
    {
        public override bool Equals(DateTime x, DateTime y)
        {
            return x.Year == y.Year && x.Month == y.Month && x.Day == y.Day;
        }

        public override int GetHashCode(DateTime obj)
        {
            unchecked
            {
                return  (obj.Month.GetHashCode() + obj.Day.GetHashCode() + obj.Year.GetHashCode() * 13);
            }
        }
    }
}