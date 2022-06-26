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
    public partial class Stock
    {
        /// <summary>
        ///     종목 정보
        /// </summary>
        public Stock(string Code, string Name, Market Market, double? 현재가, double? 등락률, double? 거래량, double? 시가, double? 고가, double? 저가, double? 시가총액, double? 매출액, double? 자산총계, double? 부채총계, double? 영업이익,
            double? 당기순이익, double? 주당순이익, double? 보통주배당금, double? 매출액증가율, double? 영업이익증가율, double? 외국인비율, double? PER, double? ROE, double? ROA, double? PBR, double? 유보율)
        {
            this.Code = Code;
            this.Name = Name;
            this.Market = Market;
            this.등락률 = 등락률;
            this.거래량 = 거래량;
            this.시가 = 시가;
            this.고가 = 고가;
            this.저가 = 저가;
            this.시가총액 = 시가총액;
            this.매출액 = 매출액;
            this.자산총계 = 자산총계;
            this.부채총계 = 부채총계;
            this.영업이익 = 영업이익;
            this.당기순이익 = 당기순이익;
            this.주당순이익 = 주당순이익;
            this.보통주배당금 = 보통주배당금;
            this.매출액증가율 = 매출액증가율;
            this.영업이익증가율 = 영업이익증가율;
            this.외국인비율 = 외국인비율;
            this.PER = PER;
            this.ROE = ROE;
            this.ROA = ROA;
            this.PBR = PBR;
            this.유보율 = 유보율;
            this.현재가 = 현재가;
        }

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
        public double? Consensus { get; private set; }

        /// <summary>
        ///     목표주가
        /// </summary>
        [JsonInclude]
        public double? TargetPrice { get; private set; }

        /// <summary>
        ///     레포트 수
        /// </summary>
        [JsonInclude]
        public double? ConsensusCount { get; private set; }

        /// <summary>
        ///     현재가 / 목표주가
        /// </summary>
        public double? CloseOfTarget
        {
            get
            {
                if (현재가 == null || TargetPrice == null)
                    return null;

                return 현재가 / TargetPrice;
            }
        }

        public double? 현재가 { get; set; }
        public string Code { get; init; }
        public string Name { get; init; }
        public Market Market { get; init; }
        public double? 등락률 { get; init; }
        public double? 거래량 { get; init; }
        public double? 시가 { get; init; }
        public double? 고가 { get; init; }
        public double? 저가 { get; init; }
        public double? 시가총액 { get; init; }
        public double? 매출액 { get; init; }
        public double? 자산총계 { get; init; }
        public double? 부채총계 { get; init; }
        public double? 영업이익 { get; init; }
        public double? 당기순이익 { get; init; }
        public double? 주당순이익 { get; init; }
        public double? 보통주배당금 { get; init; }
        public double? 매출액증가율 { get; init; }
        public double? 영업이익증가율 { get; init; }
        public double? 외국인비율 { get; init; }
        public double? PER { get; init; }
        public double? ROE { get; init; }
        public double? ROA { get; init; }
        public double? PBR { get; init; }
        public double? 유보율 { get; init; }

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