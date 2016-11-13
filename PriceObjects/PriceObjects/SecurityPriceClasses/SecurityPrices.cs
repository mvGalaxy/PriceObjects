using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PriceObjects
{
    public class SecurityPrices : IEnumerable<SecurityPrice>
    {
        private List<SecurityPrice> _prices { get; }

        public ReadOnlyCollection<SecurityPrice> Prices
        {
            get
            {
                this._prices.Sort();
                return this._prices.AsReadOnly();
            }
        }

        private Dictionary<DateTime, int> _dateIndex = new Dictionary<DateTime, int>(new YearMonthDateComparer());

        public SecurityPrice this[int i]
        {
            get
            {
                return _prices[i];
            }
        }

        public SecurityPrice this[DateTime d]
        {
            get
            {
                return this._dateIndex.ContainsKey(d) ? this._prices[this._dateIndex[d]] : null;
            }
        }

        public string SecurityId { get; }

        public SecurityPrices(string securityId)
        {
            _prices = new List<SecurityPrice>(1000);
            this.SecurityId = securityId;
        }

        public SecurityPrices(string securityId, IEnumerable<SecurityPrice> prices)
            : this(securityId)
        {
            if (prices != null && prices.Count() > 0)
            {
                foreach (var price in prices)
                {
                    this.Add(price.PriceDate, price.Price);
                }
            }
        }

        public void Add(SecurityPrice price)
        {
            if (price == null)
            {
                var t = 5;
            }
            this.Add(price.PriceDate, price.Price);
        }



        public void Add(DateTime priceDate, double price, ITradeIdentifier tradeIdentifier=null)
        {
            if (this._dateIndex.ContainsKey(priceDate) == false)
            {
                SecurityPrice secPrice = null;

                if (tradeIdentifier == null)
                {
                    secPrice = new SecurityPrice(priceDate, price, this);
                }
                else
                {
                    secPrice = new SecurityPrice(priceDate, price, this) { TradeIdentifier=tradeIdentifier};

                }
                this._prices.Add(secPrice);
                this._dateIndex[priceDate] = secPrice.Index;
            }
        }

        public IEnumerator<SecurityPrice> GetEnumerator()
        {
            
            this._prices.Sort();
            return this._prices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this._prices.Sort();
            return this._prices.GetEnumerator();
        }
    }
}