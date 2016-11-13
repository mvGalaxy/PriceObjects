using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace PriceObjects
{
    class Class1
    {
    }

    public interface ITransactionPNL
    {
        double AverageCost { get; }
        double ExitPrice { get; }
        int TradeCount { get; }
        double ProfitLoss { get; }
    }

    public abstract class TransactionPNL<T>: ITransactionPNL where T: TradeIdentifierStrategy<T>
    {
        public double AverageCost { get; }
        public double ExitPrice { get; }
        public int TradeCount { get; }
        public double ProfitLoss { get; }

        public IEnumerable<ISecurityPrice> Trades { get; }

        public TransactionPNL(TradeGroup<T> tradesGroup )
        {
            
            if (tradesGroup != null)
            {
                var trades = tradesGroup.Trades.Select(i=>i.SecurityPriceData);
                if (trades.All(t => t != null))
                {
                    this.Trades = trades;
                    this.TradeCount = trades.Count();
                    this.AverageCost = trades.Average(t => t.Price);
                    this.ExitPrice = trades.FirstOrDefault(t => t.PriceDate == trades.Max(t2 => t2.PriceDate)).Price;
                    this.ProfitLoss = CalculatePNL();
                }
                else
                {
                    throw new ArgumentNullException("TransactionPNL cannot accept any null reference for any trade in trades collection");
                }
            }
            else
            {
                throw  new ArgumentNullException("TransactionPNL cannot accept a null reference for trades collection");
            }
        }

        protected virtual double CalculatePNL()
        {
           return  this.ExitPrice- this.AverageCost;
        }
    }


    public class TransactionPNLCalculator<T> :TransactionPNL<T> where T : TradeIdentifierStrategy<T>
    {
        public TransactionPNLCalculator(TradeGroup<T> tradeGroup):base(tradeGroup)
        {
                
        }
    }

    public class TradeGrouper<T>: IEnumerable<TradeGroup<T>> where T: TradeIdentifierStrategy<T> 
    {
        private IEnumerable<T> Trades { get; }
        private List<TradeGroup<T>> _groupedTrades;




        public TradeGrouper(IEnumerable<T> trades)
        {
            if (trades != null)
            {
                this.Trades = trades;

                var groupedTrades=this.Trades.GroupBy(t => t.SecurityPriceData.Id);

                this._groupedTrades = groupedTrades.Select(t => new TradeGroup<T>(t.Key, t.ToList().AsEnumerable())).ToList();

            }
            else
            {
                throw new ArgumentNullException("TradeGrouper cannot accept a null trades collection");
            }
        }


        public IEnumerator<TradeGroup<T>> GetEnumerator()
        {
            return this._groupedTrades.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._groupedTrades.GetEnumerator();
        }
    }




    public class TradeGroup<T> where T: TradeIdentifierStrategy<T>
    {
        public int TradeGroupID { get; }
        public IEnumerable<T> Trades { get; }

        public TradeGroup(int tradeGroupID,IEnumerable<T> groupedTrades)
        {
            if (groupedTrades != null)
            {
                this.TradeGroupID = tradeGroupID;
                this.Trades = groupedTrades;
            }
            else
            {
                throw  new ArgumentNullException("TradeGroup cannot accept a null trades collection");
            }
        }

    }

    
}
