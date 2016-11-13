using System;

namespace PriceObjects
{
    public interface ISecurityPrice
    {
        int Id { get; set; }
        int Index { get; }
        bool IsIdentified { get; set; }
        double Price { get; }
        DateTime PriceDate { get; }
    }
}