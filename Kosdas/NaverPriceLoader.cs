#region
using System.Net;
using System.Text;
using Kosdas.Models;
#endregion

namespace Kosdas;

public class NaverPriceLoader : PriceLoader
{
    internal NaverPriceLoader()
    {
    }

    protected virtual PriceType PriceType => PriceType.Day;

    public override IEnumerable<Price> Load(string stockCode, DateTime from, DateTime to)
    {
        string url = $"https://fchart.stock.naver.com/siseJson.nhn?symbol={stockCode}&requestType=1&startTime={from:yyyyMMdd}&endTime={to:yyyyMMdd}&timeframe={PriceType.ToString().ToLower()}";

        WebClient web = new WebClient();
        web.Encoding = Encoding.UTF8;

        var text = web.DownloadString(url).Split('\n');
        var lines = text.Where(x => x.StartsWith("[\""));

        if (PriceType == PriceType.Minute)
            lines = lines.Reverse();

        return lines.Select(ParsePrice);
    }

    protected override Price ParsePrice(string line)
    {
        line = line.Trim().Substring(2, line.Length - 4);
        line = line.Replace("\"", string.Empty);

        var tokens = line.Split(',');

        return ParsePriceCore(tokens);
    }

    protected virtual Price ParsePriceCore(string[] tokens)
    {
        return new Price(
            PriceType, 
            DateTime.ParseExact(tokens[0], "yyyyMMdd", null),
            Convert.ToDouble(tokens[4]),
            Convert.ToDouble(tokens[5]),
            Convert.ToDouble(tokens[1]),
            Convert.ToDouble(tokens[2]),
            Convert.ToDouble(tokens[3])
        );
    }
}