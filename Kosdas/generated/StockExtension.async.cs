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
        public static Task<IEnumerable<PriceRecord>> LoadPriceAsync(this Models.StockRecord stock, DateTime from, DateTime to) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, from, to));

        public static Task<PriceRecord> LoadPriceAsync(this Models.StockRecord stock, DateTime date) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, date));

        public static Task<PriceRecord> LoadPriceAsync(this Models.StockRecord stock, int year, int month, int day) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, year, month, day));

        public static Task<PriceRecord> LoadPriceAsync(this Models.StockRecord stock) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock));

        public static Task<IEnumerable<PriceRecord>> LoadPriceAsync(this Models.StockRecord stock, int days) =>
            Task.Factory.StartNew(() => Extensions.StockExtension.LoadPrice(stock, days));
    }
}