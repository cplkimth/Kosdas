#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Kosdas
{
    public  partial class PriceLoader
    {
        public Task<System.Collections.Generic.IEnumerable<Kosdas.Models.PriceBase>> LoadAsync( System.String stockCode, System.DateTime from, System.DateTime to) => 
            Task.Run(() => Load(stockCode, from, to));

public Task<System.Collections.Generic.IReadOnlyList<Kosdas.Models.PriceBase>> LoadAsListAsync( System.String stockCode, System.DateTime from, System.DateTime to) => 
            Task.Run(() => LoadAsList(stockCode, from, to));

public Task<System.Collections.Generic.IEnumerable<Kosdas.Models.PriceBase>> LoadAsync( System.String stockCode, System.Int32 days) => 
            Task.Run(() => Load(stockCode, days));

public Task<System.Collections.Generic.IReadOnlyList<Kosdas.Models.PriceBase>> LoadAsListAsync( System.String stockCode, System.Int32 days) => 
            Task.Run(() => LoadAsList(stockCode, days));

public Task<Kosdas.Models.PriceBase> LoadAsync( System.String stockCode, System.DateTime date) => 
            Task.Run(() => Load(stockCode, date));

public Task<Kosdas.Models.PriceBase> LoadAsync( System.String stockCode, System.Int32 year, System.Int32 month, System.Int32 day) => 
            Task.Run(() => Load(stockCode, year, month, day));

public Task<Kosdas.Models.PriceBase> LoadAsync( System.String stockCode) => 
            Task.Run(() => Load(stockCode));


    }
}