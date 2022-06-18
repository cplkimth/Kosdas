#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Kosdas
{
    public  partial class PriceLoader
    {
        public Task<System.Collections.Generic.IEnumerable<Kosdas.Models.Price>> LoadAsync( System.String stockCode, System.DateTime from, System.DateTime to) => 
            Task.Factory.StartNew(() => Load(stockCode, from, to));

public Task<System.Collections.Generic.IReadOnlyList<Kosdas.Models.Price>> LoadAsListAsync( System.String stockCode, System.DateTime from, System.DateTime to) => 
            Task.Factory.StartNew(() => LoadAsList(stockCode, from, to));

public Task<System.Collections.Generic.IEnumerable<Kosdas.Models.Price>> LoadAsync( System.String stockCode, System.Int32 days) => 
            Task.Factory.StartNew(() => Load(stockCode, days));

public Task<System.Collections.Generic.IReadOnlyList<Kosdas.Models.Price>> LoadAsListAsync( System.String stockCode, System.Int32 days) => 
            Task.Factory.StartNew(() => LoadAsList(stockCode, days));

public Task<Kosdas.Models.Price> LoadAsync( System.String stockCode, System.DateTime date) => 
            Task.Factory.StartNew(() => Load(stockCode, date));

public Task<Kosdas.Models.Price> LoadAsync( System.String stockCode, System.Int32 year, System.Int32 month, System.Int32 day) => 
            Task.Factory.StartNew(() => Load(stockCode, year, month, day));

public Task<Kosdas.Models.Price> LoadAsync( System.String stockCode) => 
            Task.Factory.StartNew(() => Load(stockCode));


    }
}