using System;

namespace PriceObjects
{
    public class SecurityPriceData : ISecurityPrice
    {
        public SecurityPriceData()
        {
                
        }

        public int Id { get; set; }
        public int Index { get; }
        public bool IsIdentified { get; set; }
        public double Price { get; }
        public DateTime PriceDate { get; }

        public SecurityPriceData(ISecurityPrice priceData)
        {
            if (priceData != null)
            {
                this.Id = priceData.Id;
                this.Index = priceData.Index;
                this.Price = priceData.Price;
                this.PriceDate = priceData.PriceDate;
                this.IsIdentified = priceData.IsIdentified;
            }
        }


        public SecurityPriceData(int id, int index, double price, DateTime priceDate, bool isIdentified)
        {
            this.Id = id;
            this.Index = index;
            this.Price = price;
            this.PriceDate = priceDate;
            this.IsIdentified = isIdentified;
        }
    }
}