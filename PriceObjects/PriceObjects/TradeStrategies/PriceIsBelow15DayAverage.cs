using System;

namespace PriceObjects
{
    public class PriceIsBelow15DayAverage : TradeIdentifierStrategy<PriceIsBelow15DayAverage>
    { 
        private double _suThreshold;


        public PriceIsBelow15DayAverage()
        {
                
        }

        public PriceIsBelow15DayAverage(double suThreshold)
        {
            this._suThreshold = suThreshold;
        }


        public override bool Identify(SecurityPrice price)
        {
            if (price != null && price.Index > 15)
            {
                IStatisticsCalculator calc = new StatisticCalculator(15);
                
                var dataPoints = calc.Calculate(price);
             //   Console.WriteLine(dataPoints.SU);
                if (price.Price < dataPoints.Mean - Math.Abs(dataPoints.StandardDeviation) && dataPoints.SU < _suThreshold )
                {

                    return true;
                }
                else
                {

                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public override PriceIsBelow15DayAverage CreateNewInstance()
        {
            return  new PriceIsBelow15DayAverage(this._suThreshold);
        }
    }

 
}