#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Kosdas
{
    public partial class PriceLoader
    {
        public Task<IEnumerable<Price>> LoadAsync(string stockCode, DateTime from, DateTime to) =>
            Task.Factory.StartNew(() => Load(stockCode, from, to));

        public Task<IEnumerable<Price>> LoadAsync(string stockCode, int days) =>
            Task.Factory.StartNew(() => Load(stockCode, days));

        public Task<Price> LoadAsync(string stockCode, DateTime date) =>
            Task.Factory.StartNew(() => Load(stockCode, date));

        public Task<Price> LoadAsync(string stockCode, int year, int month, int day) =>
            Task.Factory.StartNew(() => Load(stockCode, year, month, day));

        public Task<Price> LoadAsync(string stockCode) =>
            Task.Factory.StartNew(() => Load(stockCode));
    }
}