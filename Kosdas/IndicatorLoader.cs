#region
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Kosdas.Models;
using Kosdas.Properties;
#endregion

namespace Kosdas;

public class IndicatorLoader
{
    static IndicatorLoader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    #region singleton
    private static readonly Lazy<IndicatorLoader> _instance = new(() => new IndicatorLoader());

    public static IndicatorLoader Instance => _instance.Value;

    private IndicatorLoader()
    {
        var json = Encoding.UTF8.GetString(Resources.indicators);
        List = JsonSerializer.Deserialize<List<Indicator>>(json)!;
        RemoveDuplicates(List);
    }
    #endregion

    public List<Indicator> List { get; init; }

    private static readonly HttpClient _http = new();

    public Indicator LoadLatest(string stockId)
    {
        try
        {
            return Load(stockId)[^2];
        }
        catch
        {
            return null;
        }
    }

    public List<Indicator> Load(string stockId)
    {
        var series = LoadCore(stockId, out var years).DistinctBy(x => x.Name).ToDictionary(x => x.Name, x => x.Data);

        List<Indicator> list = new List<Indicator>();
        for (int i = 0; i < years.Count; i++)
        {
            Indicator indicator = new Indicator(
                years[i],
                series["보통주.수정주가(기말)"][i],
                series["PER"][i],
                series["PBR(좌)"][i],
                series["PSR(좌)"][i],
                series["PCR(좌)"][i],
                series["EPS"][i],
                series["BPS"][i],
                series["SPS"][i],
                series["CPS"][i],
                series["ROE"][i],
                series["ROA"][i],
                series["매출액(좌)"][i],
                series["영업이익률"][i],
                series["순이익률"][i],
                series["P/C(Adj.,High)(좌)"][i],
                series["P/C(Adj.,Low)(좌)"][i],
                series["당기순이익(좌)"][i],
                series["ROIC"][i],
                series["매출액증가율"][i],
                series["영업이익증가율"][i],
                series["순이익증가율"][i],
                series["총자산증가율"][i],
                series["유동자산증가율"][i],
                series["유형자산증가율"][i],
                series["자기자본증가율"][i],
                series["부채비율"][i],
                series["유동부채비율"][i],
                series["비유동부채비율"][i],
                series["이자발생부채"][i],
                series["순부채"][i],
                series["이자보상배율(좌)"][i],
                series["총자산회전율"][i],
                series["매출채권회전율"][i],
                series["재고자산회전율"][i],
                series["매입채무회전율"][i],
                series["매출채권회전일수"][i],
                series["재고자산회전일수"][i],
                series["매입채무회전일수"][i],
                series["Cash Cycle"][i]);
            list.Add(indicator);
        }

        RemoveDuplicates(list);

        return list;
    }

    private List<Series> LoadCore(string stockId, out List<int> years)
    {
        years = null;

        List<Series> list = new List<Series>();

        List<string> urls = Enumerable.Range(1, 6).Select(x => $"https://navercomp.wisereport.co.kr/company/chart/c1030001.aspx?cmp_cd={stockId}&frq=Y&rpt={x}&finGubun=MAIN&chartType=svg").ToList();
        urls.Add($"https://navercomp.wisereport.co.kr/company/chart/c1030001.aspx?cmp_cd{stockId}=&frq=Y&rpt=CFM&finGubun=MAIN&chartType=svg");

        foreach (var url in urls)
        {
            var json = _http.GetStringAsync(url).Result;
            Root root = JsonSerializer.Deserialize<Root>(json);
            list.AddRange(root.ChartData1.Series);
            list.AddRange(root.ChartData2.Series);

            if (years == null)
                years = root.ChartData1.Categories.Select(x => ParseYear(x)).ToList();
        }

        return list;
    }

    private int ParseYear(string text)
    {
        try
        {
// 2017/12
            var match = Regex.Match(text, "\\d{4}").Value;

            return int.Parse(match);
        }
        catch
        {
            return 0;
        }
    }

    private static void RemoveDuplicates(List<Indicator> list)
    {
        var toRemove = new List<Indicator>();

        var groups = list.GroupBy(x => new {x.StockCode, x.Year});
        foreach (var group in groups)
        {
            if (group.Count() == 1)
                continue;

            toRemove.AddRange(group.Skip(1));
        }

        list.RemoveAll(x => toRemove.Contains(x));
    }

    #region records
    public record ChartData1(
        [property: JsonPropertyName("series")]
        IReadOnlyList<Series> Series,
        [property: JsonPropertyName("categories")]
        IReadOnlyList<string> Categories,
        [property: JsonPropertyName("yAxis_title")]
        IReadOnlyList<string> YAxisTitle,
        [property: JsonPropertyName("title")]
        string Title
    );

    public record ChartData2(
        [property: JsonPropertyName("series")]
        IReadOnlyList<Series> Series,
        [property: JsonPropertyName("categories")]
        IReadOnlyList<string> Categories,
        [property: JsonPropertyName("yAxis_title")]
        IReadOnlyList<string> YAxisTitle,
        [property: JsonPropertyName("title")]
        string Title
    );

    public record Root(
        [property: JsonPropertyName("chartData1")]
        ChartData1 ChartData1,
        [property: JsonPropertyName("chartData2")]
        ChartData2 ChartData2
    );

    public record Series(
        [property: JsonPropertyName("unit")]
        string Unit,
        [property: JsonPropertyName("name")]
        string Name,
        [property: JsonPropertyName("data")]
        IReadOnlyList<double?> Data,
        [property: JsonPropertyName("yAxis")]
        int YAxis,
        [property: JsonPropertyName("type")]
        string Type
    );
    #endregion
}