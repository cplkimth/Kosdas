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

        public override IEnumerable<Price> Load(string stockCode, DateTime @from, DateTime to)
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

        protected override Price ParsePrice(string line)
        {
            var tokens = line.Split(',');

            if (tokens.Length != 7)
                return null;
            if (tokens[1].Trim() == "null")
                return null;

            return new Price(
                DateTime.Parse(tokens[0]),
                Convert.ToDouble(tokens[1]),
                Convert.ToDouble(tokens[2]),
                Convert.ToDouble(tokens[3]),
                Convert.ToDouble(tokens[4]),
                Convert.ToDouble(tokens[6])
            );
        }
    }
}