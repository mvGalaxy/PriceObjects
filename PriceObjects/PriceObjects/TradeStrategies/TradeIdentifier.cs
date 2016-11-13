namespace PriceObjects
{
    public abstract class TradeIdentifier : ITradeIdentifier
    {
        public abstract bool Identify(SecurityPrice price);
    }
}