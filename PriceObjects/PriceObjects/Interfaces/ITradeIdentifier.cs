namespace PriceObjects
{
    public interface ITradeIdentifier
    {
        bool Identify(SecurityPrice price);
    }
}