#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kosdas.Extensions;
using Kosdas.Models;
#endregion

namespace Kosdas
{
    public class YahooPriceLoader : PriceLoader
    {
        internal YahooPriceLoader()
        {
        }

        public override IEnumerable<PriceRecord> Load(string stockCode, DateTime @from, DateTime to)
        {
            try
            {
                if (stockCode.Contains('.') == false)
                    stockCode = StockLoader.Instance[stockCode].Ticker;

                string url = $"https://query1.finance.yahoo.com/v7/finance/download/{stockCode}?period1={from.ToInt()}&period2={to.ToInt()}&interval=1d&events=history&includeAdjustedClose=true";

                WebClient web = new WebClient();
                var lines = web.DownloadString(url).Split('\n').Skip(1);

                return lines
                    .Select(x => ParsePrice(x))
                    .Where(x => x != null);
            }
            catch
            {
                return null;
            }
        }

        protected override PriceRecord ParsePrice(string line)
        {
            var tokens = line.Split(',');

            if (tokens.Length != 7)
                return null;
            if (tokens[1].Trim() == "null")
                return null;

            return new PriceRecord(
                DateTime.Parse(tokens[0]),
                Convert.ToDecimal(tokens[1]),
                Convert.ToDecimal(tokens[2]),
                Convert.ToDecimal(tokens[3]),
                Convert.ToDecimal(tokens[4]),
                Convert.ToDecimal(tokens[6])
            );
        }
    }
}