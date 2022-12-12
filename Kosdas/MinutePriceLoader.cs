using Kosdas.Models;
using System.Net;
using System.Text;

namespace Kosdas;

public class MinutePriceLoader
{
    #region singleton
    private static readonly Lazy<MinutePriceLoader> _instance = new(() => new MinutePriceLoader());

    public static MinutePriceLoader Instance => _instance.Value;

    private MinutePriceLoader()
    {
    }
    #endregion

    public IEnumerable<MinutePrice> Load(string stockCode, DateTime date) => Load(stockCode, date, date);

    public IEnumerable<MinutePrice> Load(string stockCode, DateTime from, DateTime to)
    {
        string url = $"https://fchart.stock.naver.com/siseJson.nhn?symbol={stockCode}&requestType=1&startTime={from:yyyyMMdd}&endTime={to:yyyyMMdd}&timeframe=minute";

        WebClient web = new WebClient();
        web.Encoding = Encoding.UTF8;

        var text = web.DownloadString(url).Split('\n');
        var lines = text.Where(x => x.StartsWith("[\""));

        return lines.Select(ParsePrice).Reverse();
    }

    protected MinutePrice ParsePrice(string line)
    {
        line = line.Trim().Substring(2, line.Length - 4);
        line = line.Replace("\"", string.Empty);

        var tokens = line.Split(',');

        return new MinutePrice(
            DateTime.ParseExact(tokens[0], "yyyyMMddHHmm", null),
            Convert.ToDouble(tokens[4]),
            Convert.ToDouble(tokens[5])
        );
    }
}