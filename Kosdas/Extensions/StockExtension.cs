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
        public static IEnumerable<Price> LoadPrice(this Models.Stock stock, DateTime from, DateTime to) => PriceLoader.Yahoo.Load(stock.Code, @from, to);

        [ForAsync]
        public static Price LoadPrice(this Models.Stock stock, DateTime date) => PriceLoader.Yahoo.Load(stock.Code, date);

        [ForAsync]
        public static Price LoadPrice(this Models.Stock stock, int year, int month, int day) => PriceLoader.Yahoo.Load(stock.Code, year, month, day);

        [ForAsync]
        public static Price LoadPrice(this Models.Stock stock) => PriceLoader.Yahoo.Load(stock.Code);

        [ForAsync]
        public static IEnumerable<Price> LoadPrice(this Models.Stock stock, int days) => PriceLoader.Yahoo.Load(stock.Code, days);

        public static Dictionary<string, Models.Stock> ToDictionary(this IEnumerable<Models.Stock> source) => source.ToDictionary(x => x.Code, x => x);
    }
}