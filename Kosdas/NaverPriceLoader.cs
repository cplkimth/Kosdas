using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using AsyncMethodLibrary;
using Kosdas.Models;

namespace Kosdas
{
    public class NaverPriceLoader : PriceLoader
    {
        internal NaverPriceLoader()
        {
        }

        public override IEnumerable<Price> Load(string stockCode, DateTime @from, DateTime to)
        {
            string url = $"https://fchart.stock.naver.com/siseJson.nhn?symbol={stockCode}&requestType=1&startTime={from:yyyyMMdd}&endTime={to:yyyyMMdd}&timeframe=day";

            WebClient web = new WebClient();
            web.Encoding = Encoding.UTF8;

            var text = web.DownloadString(url).Split('\n');
            var lines = text.Where(x => x.StartsWith("[\""));

            return lines.Select(x => ParsePrice(x));
        }

        protected override Price ParsePrice(string line)
        {
            line = line.Trim().Substring(2, line.Length - 4);
            line = line.Replace("\"", string.Empty);

            var tokens = line.Split(',');

            return new Price(
                DateTime.ParseExact(tokens[0], "yyyyMMdd", null),
                Convert.ToDecimal(tokens[1]),
                Convert.ToDecimal(tokens[2]),
                Convert.ToDecimal(tokens[3]),
                Convert.ToDecimal(tokens[4]),
                Convert.ToDecimal(tokens[5])
            );
        }
    }
}