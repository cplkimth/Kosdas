#region
using System.Net;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
#endregion

namespace Kosdas.Models
{
    /// <summary>
    ///     종목 정보
    /// </summary>
    public partial record StockRecord(string Code, string Name, Market Market, decimal? 현재가, decimal? 등락률, decimal? 거래량, decimal? 시가, decimal? 고가, decimal? 저가, decimal? 시가총액, decimal? 매출액, decimal? 자산총계, decimal? 부채총계, decimal? 영업이익,
        decimal? 당기순이익, decimal? 주당순이익, decimal? 보통주배당금, decimal? 매출액증가율, decimal? 영업이익증가율, decimal? 외국인비율, decimal? PER, decimal? ROE, decimal? ROA, decimal? PBR, decimal? 유보율)
    {
        /// <summary>
        ///     야후 파이낸스 티커
        /// </summary>
        public string Ticker => $"{Code}.{Market}";

        public override string ToString()
        {
            return $"{Ticker} {Name}";
        }

        /// <summary>
        ///     투자의견
        /// </summary>
        [JsonInclude]
        public decimal? Consensus { get; private set; }

        /// <summary>
        ///     목표주가
        /// </summary>
        [JsonInclude]
        public decimal? TargetPrice { get; private set; }

        /// <summary>
        ///     레포트 수
        /// </summary>
        [JsonInclude]
        public decimal? ConsensusCount { get; private set; }

        /// <summary>
        ///     현재가 / 목표주가
        /// </summary>
        public decimal? CloseOfTarget
        {
            get
            {
                if (현재가 == null || TargetPrice == null)
                    return null;

                return 현재가 / TargetPrice;
            }
        }

        public decimal? 현재가 { get; set; } = 현재가;
        
        /// <summary>
        ///     컨센서스를 로드한다.
        /// </summary>
        public void LoadConsensus()
        {
            string url = $"https://navercomp.wisereport.co.kr/v2/company/c1010001.aspx?cmp_cd={Code}";
            WebClient web = new WebClient();
            var html = web.DownloadString(url);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            var tds = document.DocumentNode.SelectNodes("//*[@id=\"cTB15\"]/tr[2]/td");

            if (tds == null || tds.Count != 5)
                return;

            Consensus = tds[0].ParseCell();
            TargetPrice = tds[1].ParseCell();
            ConsensusCount = tds[4].ParseCell();
        }
    }
}