using System;
using System.Collections.Generic;
using System.Linq;
using AsyncMethodLibrary;

namespace Kosdas
{
    public static partial class StockExtension
    {
        [ForAsync]
        public static IEnumerable<Price> LoadPrice(this Stock stock, DateTime from, DateTime to) => PriceLoader.Instance.Load(stock.Code, @from, to);

        [ForAsync]
        public static Price LoadPrice(this Stock stock, DateTime date) => PriceLoader.Instance.Load(stock.Code, date);

        [ForAsync]
        public static Price LoadPrice(this Stock stock, int year, int month, int day) => PriceLoader.Instance.Load(stock.Code, year, month, day);

        [ForAsync]
        public static Price LoadPrice(this Stock stock) => PriceLoader.Instance.Load(stock.Code);

        [ForAsync]
        public static IEnumerable<Price> LoadPrice(this Stock stock, int days) => PriceLoader.Instance.Load(stock.Code, days);

        public static Dictionary<string, Stock> ToDictionary(this IEnumerable<Stock> source) => source.ToDictionary(x => x.Code, x => x);
    }
}