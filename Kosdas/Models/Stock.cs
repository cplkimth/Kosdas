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
        public Stock(string Code, string Name, Market Market, decimal? 현재가, decimal? 등락률, decimal? 거래량, decimal? 시가, decimal? 고가, decimal? 저가, decimal? 시가총액, decimal? 매출액, decimal? 자산총계, decimal? 부채총계, decimal? 영업이익,
            decimal? 당기순이익, decimal? 주당순이익, decimal? 보통주배당금, decimal? 매출액증가율, decimal? 영업이익증가율, decimal? 외국인비율, decimal? PER, decimal? ROE, decimal? ROA, decimal? PBR, decimal? 유보율)
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

        public decimal? 현재가 { get; set; }
        public string Code { get; init; }
        public string Name { get; init; }
        public Market Market { get; init; }
        public decimal? 등락률 { get; init; }
        public decimal? 거래량 { get; init; }
        public decimal? 시가 { get; init; }
        public decimal? 고가 { get; init; }
        public decimal? 저가 { get; init; }
        public decimal? 시가총액 { get; init; }
        public decimal? 매출액 { get; init; }
        public decimal? 자산총계 { get; init; }
        public decimal? 부채총계 { get; init; }
        public decimal? 영업이익 { get; init; }
        public decimal? 당기순이익 { get; init; }
        public decimal? 주당순이익 { get; init; }
        public decimal? 보통주배당금 { get; init; }
        public decimal? 매출액증가율 { get; init; }
        public decimal? 영업이익증가율 { get; init; }
        public decimal? 외국인비율 { get; init; }
        public decimal? PER { get; init; }
        public decimal? ROE { get; init; }
        public decimal? ROA { get; init; }
        public decimal? PBR { get; init; }
        public decimal? 유보율 { get; init; }

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