using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PriceObjects
{
    public class StrategyExecutive<T>: IEnumerable<T> where T: TradeIdentifierStrategy<T>
    {

        private readonly IEnumerable<IDateValue> _marketDataPoints;
        private readonly TradeIdentifierStrategy<T> _tradeIdentifyingStrategy;
        private readonly string _secId;
        private List<T> _tradeStrategies= new List<T>();
       

        public TradeIdentifierStrategy<T> TradeIdentifyingStrategy
        {
            get
            {
                return _tradeIdentifyingStrategy;
            }
        }


        public StrategyExecutive(string secId,IEnumerable<IDateValue> marketDataPoints , T tradeIdentifyingStrategy)
        {
            if (secId != null)
            {
                if (marketDataPoints != null)
                {
                    if (tradeIdentifyingStrategy != null)
                    {
                        this._marketDataPoints = marketDataPoints;

                        this._tradeIdentifyingStrategy = tradeIdentifyingStrategy;
                        ExecuteStrategy();
                    }
                    else
                    {

                        throw new ArgumentNullException(
                            "TradeIDentifierStrategy object in StrategyExecutive cannot be null");

                    }
                }
                else
                {

                    throw new ArgumentNullException("marketDatePoints collection in StrategyExecutive cannot be null");
                }
            }
            else
            {
                
                throw new ArgumentNullException("SecurityId cannot be null in StrategyExecutive");
            }
        }

        private void ExecuteStrategy()
        {
            var securityPrices = new SecurityPrices(_secId);
            var listOfDataPoints = _marketDataPoints.ToList();

            for (int i = 0; i < listOfDataPoints.Count; i++)
            {
                var strategy = TradeIdentifyingStrategy.CreateNewInstance();
                
                var dataPoint = listOfDataPoints[i];
                
                securityPrices.Add(dataPoint.MarketDate,dataPoint.MarketDataPoint,strategy);

                var currentSecurity = securityPrices[i];

                currentSecurity.RunTradeIdentifier();

                strategy.SetSecurityPriceData(currentSecurity);

                _tradeStrategies.Add(strategy);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._tradeStrategies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._tradeStrategies.GetEnumerator();
        }
    }
}