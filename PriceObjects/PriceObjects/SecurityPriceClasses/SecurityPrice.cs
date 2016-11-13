using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PriceObjects
{
    public class SecurityPrice : IComparable<SecurityPrice>, ISecurityPrice
    {

        public DateTime PriceDate { get; }
        public double Price { get; }
        public int Index { get; }

        public bool IsIdentified { get; set; }
        public int Id { get; set; }
        
        public ITradeIdentifier TradeIdentifier { get; set; }
       // public TradeIdentifier  TradeStrategyIdentifier {get;set;}

        private SecurityPrices Prices { get; }

        public SecurityPrice(DateTime priceDate, double price, SecurityPrices prices)
        {
            PriceDate = priceDate;
            Price = price;
            Prices = prices;
            Index = prices.Prices.Count;
        }

        public SecurityPrice GetPreviousPrice()
        {
            if (Index > 0)
                return Prices[Index - 1];
            else
                return null;
        }

        public SecurityPrices GetPreviousPrices(int periodsBack)
        {
            var security_prices = new SecurityPrices(Prices.SecurityId);
            var prices = new List<SecurityPrice>(periodsBack);
            if (periodsBack < Index)
            {
                for (var i = 0; i <= periodsBack; i++)
                    if (i == 0)
                    {
                        prices.Add(this);
                    }
                    else
                    {
                        var price = prices[i - 1].GetPreviousPrice();
                        prices.Add(price);
                    }
                var c = prices.Count;
                if (c > 1)
                { prices.RemoveAt(c - 1); }
                prices.Reverse();
                
                //remove the last price so that periods actually equal count of items. we are including current item in collection so we remove the last.
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
            var identified = TradeIdentifier.Identify(this);
            IsIdentified = identified;
            double id = 0;
            var previous = GetPreviousPrice();
            if (IsIdentified && (previous.Id == 0) || (previous!=null &&previous.IsIdentified==false && (previous.Id > 0)))
                Id = Index;
            else if (IsIdentified && (previous.Id > 0))
                Id = previous.Id;
            else if ((previous != null) && previous.IsIdentified && (IsIdentified == false))
                Id = previous.Id;
        }

        public bool Equals(SecurityPrice price)
        {
            return price != null ?
                (Price == price.Price) && (PriceDate.Month == price.PriceDate.Month) && (PriceDate.Day == price.PriceDate.Day) && (PriceDate.Year == price.PriceDate.Year) ? true
                    : false
                : false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return PriceDate.Month.GetHashCode()+ PriceDate.Day.GetHashCode()+ PriceDate.Year.GetHashCode() * 13 + Price.GetHashCode() * 7 + Index.GetHashCode();
            }

        }

        public override bool Equals(object obj)
        {
            return obj is SecurityPrice ? Equals(obj as SecurityPrice) : false;
        }

        public int CompareTo(SecurityPrice other)
        {
            const int precedes = -1;
            const int same = 0;
            const int follows = 1;

            if (PriceDate < other.PriceDate)
                return precedes;

            if (PriceDate == other.PriceDate)
                return same;


            return follows;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Prices.SecurityId).Append(",").Append(PriceDate).Append(",").Append(Price).Append("\n");
            return sb.ToString();
        }
    }
}