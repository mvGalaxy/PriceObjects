using System;

namespace PriceObjects
{
    public interface IDateValue
    {
        DateTime MarketDate { get; set; }
        double MarketDataPoint { get; set; }
    }
}