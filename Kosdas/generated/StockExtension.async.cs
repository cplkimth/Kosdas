#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kosdas.Models;
#endregion

namespace Kosdas
{
    public static partial class StockExtension
    {
        public static Task<IEnumerable<Price>> LoadPriceAsync(this Models.Stock stock, DateTime from, DateTime to) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, from, to));

        public static Task<Price> LoadPriceAsync(this Models.Stock stock, DateTime date) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, date));

        public static Task<Price> LoadPriceAsync(this Models.Stock stock, int year, int month, int day) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, year, month, day));

        public static Task<Price> LoadPriceAsync(this Models.Stock stock) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock));

        public static Task<IEnumerable<Price>> LoadPriceAsync(this Models.Stock stock, int days) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, days));
    }
}