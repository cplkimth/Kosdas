#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Kosdas
{
    public static partial class StockExtension
    {
        public static Task<IEnumerable<Price>> LoadPriceAsync(this Stock stock, DateTime from, DateTime to) =>
            Task.Factory.StartNew(() => LoadPrice(stock, from, to));

        public static Task<Price> LoadPriceAsync(this Stock stock, DateTime date) =>
            Task.Factory.StartNew(() => LoadPrice(stock, date));

        public static Task<Price> LoadPriceAsync(this Stock stock, int year, int month, int day) =>
            Task.Factory.StartNew(() => LoadPrice(stock, year, month, day));

        public static Task<Price> LoadPriceAsync(this Stock stock) =>
            Task.Factory.StartNew(() => LoadPrice(stock));

        public static Task<IEnumerable<Price>> LoadPriceAsync(this Stock stock, int days) =>
            Task.Factory.StartNew(() => LoadPrice(stock, days));
    }
}