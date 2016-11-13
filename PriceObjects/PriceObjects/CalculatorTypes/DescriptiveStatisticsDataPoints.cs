using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace PriceObjects
{
    public class DescriptiveStatisticsDataPoints : DescriptiveStatistics
    {

        public double SU { get; }


        public DescriptiveStatisticsDataPoints(IEnumerable<double> data, bool increasedAccuracy = false) : base(data, increasedAccuracy)
        {
            var x = data.Last();
            var m = base.Mean;
            var s = base.StandardDeviation;

            if (s != 0)
            {
                SU = (x - m)/s;
            }
            else
            {

                SU = 0;
            }
        }

        public DescriptiveStatisticsDataPoints(IEnumerable<double?> data, bool increasedAccuracy = false) : base(data, increasedAccuracy)
        {
            throw new NotImplementedException();
            ;
        }
    }
}