using System;
using System.Text;

namespace PriceObjects
{
    public enum PriceLocationOnChart
    {
        PriceIsBelowLowerBand,
        PriceIsAboveUpperBand,
        PriceIsBetweenLowerAndUpperBands
    }

    public class MeanReversionStrategy : TradeIdentifierStrategy<MeanReversionStrategy>
    {

        public double MeanValue { get; }
        public double UpperStandardUnitLimit { get; }
        public double LowerStandardUnitLimit { get; }
        public PriceLocationOnChart PriceLocation { get; }
        public double CurrentLowerBand { get; private set; }
        public double CurrentUpperBand { get; private set; }
        public double CurrentSU { get; private set; }
        public double CurrentMean { get; private set; }
        public double CurrentStDev { get; private set; }

        public MeanReversionStrategy(double meanValue,double upperBandUnitLimit,  double lowerBandUnitLimit, PriceLocationOnChart priceLocation)
        {
            MeanValue = meanValue;
            this.UpperStandardUnitLimit = Math.Abs(upperBandUnitLimit);
            this.LowerStandardUnitLimit = Math.Abs(lowerBandUnitLimit);
            
            PriceLocation = priceLocation;

        }

        public override MeanReversionStrategy CreateNewInstance()
        {
            return new MeanReversionStrategy(this.MeanValue,this.UpperStandardUnitLimit,this.LowerStandardUnitLimit, this.PriceLocation);
        }

        public override bool Identify(SecurityPrice price)
        {
            if (price.Index > MeanValue == false) return false;

            IStatisticsCalculator calc = new StatisticCalculator(MeanValue);
            var calculation = calc.Calculate(price);

            var currentSU = calculation.SU;

            var currentStdev = calculation.StandardDeviation;
            var currentMean = calculation.Mean;
            var currentPrice = price.Price;
            CurrentMean = currentMean;
            CurrentStDev = currentStdev;
            CurrentSU = currentSU;

            double bottomBand = 0;
            double upperBand = 0;

            if (PriceLocation == PriceLocationOnChart.PriceIsBelowLowerBand)
            {
                if (currentSU >= (LowerStandardUnitLimit) * -1)
                {
                    return false;
                }
            }

            if (PriceLocation == PriceLocationOnChart.PriceIsAboveUpperBand)
            {
                if (currentSU <= UpperStandardUnitLimit)
                {
                    return false;
                }
            }




            if (currentSU < 0)
            {
                bottomBand = currentMean - Math.Abs(currentStdev);
                CurrentLowerBand = bottomBand;
            }
            else if (currentSU > 0)
            {
                upperBand = currentMean + Math.Abs(currentStdev);
                CurrentUpperBand = upperBand;
            }

            if (PriceLocation == PriceLocationOnChart.PriceIsBelowLowerBand)
            {
                if (currentPrice < bottomBand)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (PriceLocation == PriceLocationOnChart.PriceIsAboveUpperBand)
            {
                if (currentPrice > upperBand)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (PriceLocation == PriceLocationOnChart.PriceIsBetweenLowerAndUpperBands)
            {
                if (currentPrice < upperBand && currentPrice > bottomBand)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;

        }


        public override string ToString()
        {
            var sb = new StringBuilder(10);


            sb.Append(nameof(MeanValue))
                .Append(",")
                .Append(nameof(UpperStandardUnitLimit))
                .Append(",")
                .Append(nameof(LowerStandardUnitLimit))
                .Append(",")
                .Append(nameof(PriceLocation))
                .Append(",")
                .Append(nameof(CurrentLowerBand))
                .Append(",")
                .Append(nameof(CurrentUpperBand))
                .Append(",")
                
                .Append(nameof(CurrentMean))
                .Append(",")

                .Append(nameof(CurrentStDev))
                .Append(",")

                .Append(nameof(CurrentSU))
                .Append(",")

                .Append("Id")
                .Append(",")
                .Append("Index")
                .Append(",")
                .Append("IsIdentified")
                .Append(",")
                .Append("Price")
                .Append(",")
                .Append("PriceDate")
                .AppendLine()
                .Append(MeanValue)
                .Append(",")
                .Append(UpperStandardUnitLimit)
                .Append(",")
                .Append(LowerStandardUnitLimit)
                .Append(",")
                .Append(PriceLocation.ToString("g"))
                .Append(",")
                .Append(CurrentLowerBand)
                .Append(",")
                .Append(CurrentUpperBand)
                .Append(",")


                .Append(CurrentMean)
                .Append(",")

                .Append(CurrentStDev)
                .Append(",")

                .Append(CurrentSU)
                .Append(",")



                .Append(SecurityPriceData?.Id)
                .Append(",")
                .Append(SecurityPriceData?.Index)
                .Append(",")
                .Append(SecurityPriceData?.IsIdentified)
                .Append(",")
                .Append(SecurityPriceData?.Price)
                .Append(",")
                .Append(SecurityPriceData?.PriceDate);
            return sb.ToString();
        }
    }

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