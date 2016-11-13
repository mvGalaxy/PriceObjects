using System;

namespace PriceObjects
{
    public abstract class TradeIdentifierStrategy<T>: ITradeIdentifier, INewInstanceReplicator<T>
    {
    
        public ISecurityPrice SecurityPriceData { get; protected set; }

        public abstract T CreateNewInstance();
        public abstract bool Identify(SecurityPrice price);

        public void SetSecurityPriceData(ISecurityPrice price)
        {
            if (price != null)
            {

                this.SecurityPriceData = new SecurityPriceData(price);
            }
            else
            {
                
                throw new ArgumentNullException("null ISecurityPrice was passed to SetSecurityPriceData Method");
            }
        }
        
    }
}