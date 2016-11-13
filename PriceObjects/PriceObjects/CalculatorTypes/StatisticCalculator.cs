using System.Linq;

namespace PriceObjects
{
    public class StatisticCalculator : IStatisticsCalculator
    {
        public StatisticCalculator(double period)
        {
            Period = period;
        }

        public double Period { get; }


        public DescriptiveStatisticsDataPoints Calculate(SecurityPrice price)
        {
            if (price != null)
            {
                if (price.Index> Period)
                {
                    var previousPeriods = price.GetPreviousPrices((int)Period);
                    if (previousPeriods != null)
                    {
                        var stats = new DescriptiveStatisticsDataPoints(previousPeriods.Select(p=>p.Price));
                        return stats;
                    }
                    else
                    {

                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {

                return null;
            }
        }
    }
}