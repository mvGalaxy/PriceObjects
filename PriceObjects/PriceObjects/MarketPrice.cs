using System;

namespace PriceObjects
{
    public class MarketPrice:IDateValue
    {
        public DateTime MarketDate { get; set; }
        public double MarketDataPoint { get; set; }
    }
}