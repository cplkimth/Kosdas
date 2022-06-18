#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Kosdas.Extensions
{
    public static partial class StockExtension
    {
        public static Task<System.Collections.Generic.IEnumerable<Kosdas.Models.Price>> LoadPriceAsync(this  Kosdas.Models.Stock stock, System.DateTime from, System.DateTime to) => 
            Task.Factory.StartNew(() => LoadPrice(stock, from, to));

public static Task<Kosdas.Models.Price> LoadPriceAsync(this  Kosdas.Models.Stock stock, System.DateTime date) => 
            Task.Factory.StartNew(() => LoadPrice(stock, date));

public static Task<Kosdas.Models.Price> LoadPriceAsync(this  Kosdas.Models.Stock stock, System.Int32 year, System.Int32 month, System.Int32 day) => 
            Task.Factory.StartNew(() => LoadPrice(stock, year, month, day));

public static Task<Kosdas.Models.Price> LoadPriceAsync(this  Kosdas.Models.Stock stock) => 
            Task.Factory.StartNew(() => LoadPrice(stock));

public static Task<System.Collections.Generic.IEnumerable<Kosdas.Models.Price>> LoadPriceAsync(this  Kosdas.Models.Stock stock, System.Int32 days) => 
            Task.Factory.StartNew(() => LoadPrice(stock, days));


    }
}