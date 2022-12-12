#region
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
#endregion

namespace Kosdas;

/// <summary>
///     실시간 가격 정보 로더. 열거 가능.
/// </summary>
public class RealTimePriceLoader : IEnumerable<KeyValuePair<string, double>>
{
    #region singleton
    private RealTimePriceLoader()
    {
    }

    private static RealTimePriceLoader _instance;

    public static RealTimePriceLoader Instance
    {
        get
        {
            if (_instance == null)
                _instance = new RealTimePriceLoader();

            return _instance;
        }
    }
    #endregion

    private ConcurrentDictionary<string, double> _dictionary { get; } = new();

    /// <summary>
    ///     종목의 현재 가격을 반환한다.
    /// </summary>
    /// <param name="stockCode"></param>
    /// <returns>현재 가격. 종목코드가 유효하지 않으면 null.</returns>
    public double? this[string stockCode] => _dictionary.ContainsKey(stockCode) ? _dictionary[stockCode] : null;

    public int Count => _dictionary.Count;

    /// <summary>
    ///     전 종목의 실시간 가격 정보를 로드한다.
    /// </summary>
    public void Load()
    {
        string[] markets = {"KOSPI", "KOSDAQ"};

        markets.AsParallel().ForAll(x => LoadCore(x));
    }

    private void LoadCore(string market)
    {
        string url = $"https://finance.daum.net/api/quotes/sectors?fieldName=&order=&perPage=&market={market}&page=&changes=UPPER_LIMIT%2CRISE%2CEVEN%2CFALL%2CLOWER_LIMIT";

        WebClient web = new();
        web.Headers[HttpRequestHeader.Referer] = $"https://finance.daum.net/domestic/all_stocks?market={market}";
        var json = web.DownloadString(url);
        var data = JsonSerializer.Deserialize<dynamic>(json);
        // JsonConvert.DeserializeObject<dynamic>(json).data;

        for (int i = 0; i < data.Count; i++)
        {
            var stocks = data[i].includedStocks;

            for (int j = 0; j < stocks.Count; j++)
            {
                var stockCode = ((string) stocks[j].symbolCode).Trim().Substring(1);
                var price = Convert.ToDecimal(stocks[j].tradePrice);

                _dictionary[stockCode] = price;
            }
        }
    }

    #region IEnumerable
    public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
    {
        foreach (var item in _dictionary)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}