using System;
using System.Collections.Generic;
using System.Linq;
using AsyncMethodLibrary;
using Kosdas.Models;

namespace Kosdas.Extensions
{
    public static partial class StockExtension
    {
        [ForAsync]
        public static IEnumerable<PriceBase> LoadPrice(this Models.StockBase stock, DateTime from, DateTime to) => PriceLoader.Yahoo.Load(stock.Code, @from, to);

        [ForAsync]
        public static PriceBase LoadPrice(this Models.StockBase stock, DateTime date) => PriceLoader.Yahoo.Load(stock.Code, date);

        [ForAsync]
        public static PriceBase LoadPrice(this Models.StockBase stock, int year, int month, int day) => PriceLoader.Yahoo.Load(stock.Code, year, month, day);

        [ForAsync]
        public static PriceBase LoadPrice(this Models.StockBase stock) => PriceLoader.Yahoo.Load(stock.Code);

        [ForAsync]
        public static IEnumerable<PriceBase> LoadPrice(this Models.StockBase stock, int days) => PriceLoader.Yahoo.Load(stock.Code, days);

        public static Dictionary<string, Models.StockBase> ToDictionary(this IEnumerable<Models.StockBase> source) => source.ToDictionary(x => x.Code, x => x);
    }
}