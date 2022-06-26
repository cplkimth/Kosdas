using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Kosdas;

public class ValueLoader
{
    #region singleton
    private static readonly Lazy<ValueLoader> _instance = new(() => new ValueLoader());

    public static ValueLoader Instance => _instance.Value;

    private ValueLoader()
    {
    }

    private readonly HttpClient _http = new();

    public List<Value> Load(string stockId, Type type = Type.Annual)
    {
        string key = ExtractKey(stockId);

        string url = $"https://navercomp.wisereport.co.kr/v2/company/cF4002.aspx?cmp_cd={stockId}&frq=0&rpt=5&finGubun=IFRSS&frqTyp={(int)type}&cn=&encparam={key}";

        var json = _http.GetStringAsync(url).Result;
        Root root = JsonSerializer.Deserialize<Root>(json);
        // Root root = _http.GetFromJsonAsync<Root>(url).Result;

        int[] years = root.YYMM.Select(x => ParseYear(x)).Where(x => x.HasValue).Select(x => x.Value).Distinct().ToArray();

        var eps = root.DATA.First(x => x.ACCNM == "EPS").ToArray();
        var bps = root.DATA.First(x => x.ACCNM == "BPS").ToArray();
        var cps = root.DATA.First(x => x.ACCNM == "CPS").ToArray();
        var sps = root.DATA.First(x => x.ACCNM == "SPS").ToArray();
        var per = root.DATA.First(x => x.ACCNM == "PER").ToArray();
        var pbr = root.DATA.First(x => x.ACCNM == "PBR").ToArray();
        var pcr = root.DATA.First(x => x.ACCNM == "PCR").ToArray();
        var psr = root.DATA.First(x => x.ACCNM == "PSR").ToArray();
        var ebitda = root.DATA.First(x => x.ACCNM == "EV/EBITDA").ToArray();
        var dps = root.DATA.First(x => x.ACCNM == "DPS").ToArray();
        var 현금배당수익률 = root.DATA.First(x => x.ACCNM == "현금배당수익률").ToArray();
        var 현금배당성향 = root.DATA.First(x => x.ACCNM == "현금배당성향(%)").ToArray();

        return Enumerable.Range(0, years.Length).Select(x => new Value(
                                                     years[x],
                                                     eps[x],
                                                     bps[x],
                                                     cps[x],
                                                     sps[x],
                                                     per[x],
                                                     pbr[x],
                                                     pcr[x],
                                                     psr[x],
                                                     ebitda[x],
                                                     dps[x],
                                                     현금배당수익률[x],
                                                     현금배당성향[x]
                                                 )).ToList();
    }

    private int? ParseYear(string text)
    {
        // 2017/12<br />(IFRS별도)
        var match = Regex.Match(text, "\\d{4}").Value;

        var parsed = int.TryParse(match, out int year);
        return parsed ? year : null;
    }

    private string ExtractKey(string stockId)
    {
        string outerUrl = $"https://navercomp.wisereport.co.kr/v2/company/c1040001.aspx?cmp_cd={stockId}&cn=";
        string html = _http.GetStringAsync(outerUrl).Result;

        return Regex.Match(html, "encparam: '(\\w{32})'").Groups[1].Value;
    }

    #endregion

    public enum Type
    {
        Annual = 0,
        Quarter = 1
    }

    #region records
    private record DATum(
        [property: JsonPropertyName("ACKIND")]
        string ACKIND,
        [property: JsonPropertyName("ACCODE")]
        string ACCODE,
        [property: JsonPropertyName("ACC_NM")]
        string ACCNM,
        [property: JsonPropertyName("LVL")]
        int LVL,
        [property: JsonPropertyName("GRP_TYP")]
        int GRPTYP,
        [property: JsonPropertyName("UNT_TYP")]
        int UNTTYP,
        [property: JsonPropertyName("P_ACCODE")]
        string PACCODE,
        [property: JsonPropertyName("DATA1")]
        double? DATA1,
        [property: JsonPropertyName("DATA2")]
        double? DATA2,
        [property: JsonPropertyName("DATA3")]
        double? DATA3,
        [property: JsonPropertyName("DATA4")]
        double? DATA4,
        [property: JsonPropertyName("DATA5")]
        double? DATA5,
        [property: JsonPropertyName("DATA6")]
        double? DATA6,
        [property: JsonPropertyName("YYOY")]
        double? YYOY,
        [property: JsonPropertyName("YEYOY")]
        double? YEYOY,
        [property: JsonPropertyName("DATAQ1")]
        double? DATAQ1,
        [property: JsonPropertyName("DATAQ4")]
        double? DATAQ4,
        [property: JsonPropertyName("DATAQ5")]
        double? DATAQ5,
        [property: JsonPropertyName("DATAQ2")]
        double? DATAQ2,
        [property: JsonPropertyName("DATAQ6")]
        double? DATAQ6,
        [property: JsonPropertyName("QOQ")]
        double? QOQ,
        [property: JsonPropertyName("YOY")]
        double? YOY,
        [property: JsonPropertyName("QOQ_COMMENT")]
        string QOQCOMMENT,
        [property: JsonPropertyName("YOY_COMMENT")]
        string YOYCOMMENT,
        [property: JsonPropertyName("QOQ_E")]
        double? QOQE,
        [property: JsonPropertyName("YOY_E")]
        double? YOYE,
        [property: JsonPropertyName("QOQ_E_COMMENT")]
        string QOQECOMMENT,
        [property: JsonPropertyName("YOY_E_COMMENT")]
        string YOYECOMMENT,
        [property: JsonPropertyName("POINT_CNT")]
        int POINTCNT 
    )
    {
        public double?[] ToArray() => new[] {DATA1, DATA2, DATA3, DATA4, DATA5, DATA6};
    }

    private record Root(
        [property: JsonPropertyName("YYMM")] IReadOnlyList<string> YYMM,
        [property: JsonPropertyName("DATA")] IReadOnlyList<DATum> DATA,
        [property: JsonPropertyName("FIN")] string FIN,
        [property: JsonPropertyName("FRQ")] string FRQ
    );
    #endregion
}

public record Value(int Year, double? Eps, double? Bps, double? Cps, double? Sps, double? Per, double? Pbr, double? Pcr, double? Psr, double? EvEbitda, double? Dps, double? 현금배당수익률, double? 현금배당성향);
