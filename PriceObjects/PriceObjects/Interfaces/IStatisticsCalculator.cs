namespace PriceObjects
{
    public interface IStatisticsCalculator
    {
        DescriptiveStatisticsDataPoints Calculate(SecurityPrice price);
    }
}