using System;
using System.Collections.Generic;
using System.Linq;
using Kosdas.Extensions;

namespace Kosdas.TestConsole;

public class Trade
    {
        public Trade()
        {
        }

        public Trade(string stockId, DateTime buyAt, double buyValue, int quantity)
        {
            StockId = stockId;
            BuyValue = buyValue;
            BuyAt = buyAt;
            Quantity = quantity;
        }

        private double? _rate;

        public string StockId { get; internal set; }
        public double BuyValue { get; internal set; }
        public double SellValue { get; set; }
        public DateTime BuyAt { get; internal set; }
        public DateTime SellAt { get; set; }
        public int Quantity { get; internal set; }

        public double Rate
        {
            get
            {
                if (_rate.HasValue == false)
                    _rate = SellValue.RateOf(BuyValue);

                return _rate.Value;
            }
        }
        
        public double BuyTotal => BuyValue * Quantity;

        public double SellTotal => SellValue * Quantity;

        public string StockName => StockLoader.Instance[StockId].Name;

        public bool Selled => SellAt != DateTime.MinValue;
        
        public bool Unsold => Selled is false;

        public override string ToString()
        {
            // return $"\t{BuyAt:d}\t{BuyValue:N2}\t{Quantity:N0}\t{BuyTotal:N2}\t{SellAt:d}\t{SellValue:N2}\t{Quantity:N0}\t{SellTotal:N2}\t{Rate:P2}";
            return $"\t{BuyAt:t}\t{BuyValue:N0}\t{SellAt:t}\t{SellValue:N0}\t{Rate:P2}";
        }
    }

    public enum TradeType
    {
        매수, 매도
    }

    public static class TradeListExtension
    {
        public static double GetBoughtAverage(this List<Trade> trades) => GetBoughtAverage(trades, _ => true);

        public static double GetBoughtAverage(this List<Trade> trades, Func<Trade, bool> predicate)
        {
            var query = trades.Where(predicate);
            if (query.Any())
                return query.Average(x => x.BuyValue);
            else
                return 0;
        }

        public static double GetTotalRate(this List<Trade> trades)
        {
            if (trades.Count == 0)
                return 0;

            return trades.Sum(x => x.SellTotal).RateOf(trades.Sum(x => x.BuyTotal));
        }
    }