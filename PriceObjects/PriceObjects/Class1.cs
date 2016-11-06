using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PriceObjects
{
    public class SecurityPrice : IComparable<SecurityPrice>
    {

        public DateTime PriceDate { get; }
        public double Price { get; }
        public int Index { get; }

        public bool IsIdentified { get; set; }
        public double Id { get; set; }
        
        public List<ITradeIdentifier> TradeIdentifiers { get; set; }

        SecurityPrices Prices { get; }

        public SecurityPrice(DateTime priceDate, double price, SecurityPrices prices)
        {
            this.PriceDate = priceDate;
            this.Price = price;
            this.Prices = prices;
            this.Index = prices.Prices.Count;
        }

        public SecurityPrice GetPreviousPrice()
        {
            if (this.Index > 0)
            {
                return this.Prices[this.Index - 1];
            }
            else
            {
                return null;
            }
        }

        public SecurityPrices GetPreviousPrices(int periodsBack)
        {
            var security_prices = new SecurityPrices(this.Prices.SecurityId);//  new List<SecurityPrice>(periodsBack - 1);
            var prices = new List<SecurityPrice>(periodsBack);
            if (periodsBack < this.Index)
            {
                for (int i = 0; i <= periodsBack; i++)
                {
                    if (i == 0)
                    {
                        prices.Add(this);
                    }
                    else
                    {
                        var price = prices[i - 1].GetPreviousPrice();
                        prices.Add(price);
                    }
                }
                prices.Reverse();
                foreach (var sprice in prices)
                {
                    security_prices.Add(sprice);
                }
                return security_prices;
            }
            else
            {
                return null;
            }
        }

        public void RunTradeIdentifier()
        {
            var identified =   TradeIdentifiers.All(p => p.Identify(this));
            this.IsIdentified = identified;
            double id = 0;
            var previous = GetPreviousPrice();
            if (this.IsIdentified && previous.Id == 0)
            {
                this.Id = this.Index;
            }
            else if (this.IsIdentified && previous.Id > 0)
            {
                this.Id = previous.Id;
            }
            else if (previous!=null && previous.IsIdentified && this.IsIdentified == false)
            {
                this.Id = previous.Id;
            }


        }

        public bool Equals(SecurityPrice price)
        {
            return price != null ?
                  this.Price == price.Price && this.PriceDate.Month == price.PriceDate.Month && this.PriceDate.Day == price.PriceDate.Day && this.PriceDate.Year == price.PriceDate.Year ? true
                  : false
                  : false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PriceDate.Month.GetHashCode()+ PriceDate.Day.GetHashCode()+ PriceDate.Year.GetHashCode() * 13) + (Price.GetHashCode() * 7) + Index.GetHashCode();
            }

        }

        public override bool Equals(object obj)
        {
            return obj is SecurityPrice ? this.Equals(obj as SecurityPrice) : false;
        }

        public int CompareTo(SecurityPrice other)
        {
            const int precedes = -1;
            const int same = 0;
            const int follows = 1;

            if (this.PriceDate < other.PriceDate)
            {
                return precedes;
            }

            if (this.PriceDate == other.PriceDate)
            {
                return same;
            }


            return follows;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Prices.SecurityId).Append(",").Append(this.PriceDate).Append(",").Append(this.Price).Append("\n");
            return sb.ToString();
        }
    }

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



        public void Add(DateTime priceDate, double price, IEnumerable<ITradeIdentifier> tradeIdentifier=null)
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
                     secPrice = new SecurityPrice(priceDate, price, this) { TradeIdentifiers=tradeIdentifier.ToList()};

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

    public class YearMonthDateComparer : EqualityComparer<DateTime>
    {
        public override bool Equals(DateTime x, DateTime y)
        {
            return x.Year == y.Year && x.Month == y.Month && x.Day == y.Day;
        }

        public override int GetHashCode(DateTime obj)
        {
            unchecked
            {
              return  (obj.Month.GetHashCode() + obj.Day.GetHashCode() + obj.Year.GetHashCode() * 13);
            }
        }
    }

    public interface ITradeIdentifier
    {
        bool Identify(SecurityPrice price);
    }


    public class PriceIsBelow15DayAverage : ITradeIdentifier
    {
        public bool Identify(SecurityPrice price)
        {
            if (price != null && price.Index > 15)
            {
                var avg = price.GetPreviousPrices(15).Select(p => p.Price).Average();
                var px = price.Price;

                return px < avg;
            }
            else
            {
                return false;
            }
        }
    }

    
}
