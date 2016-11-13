using System.Collections;
using System.Collections.Generic;

namespace PriceObjects
{
    public class MarketPrices: IEnumerable<MarketPrice>
    {
        private List<MarketPrice> _marketPrice= new List<MarketPrice>();


        public void AddMarketPrice(MarketPrice marketPrice)
        {
            this._marketPrice.Add(marketPrice);
        }

        public IEnumerator<MarketPrice> GetEnumerator()
        {
            return this._marketPrice.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._marketPrice.GetEnumerator();
        }
    }
}