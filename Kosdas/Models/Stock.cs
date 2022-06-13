using System.Net;
using System.Text.Json.Serialization;
using HtmlAgilityPack;

namespace Kosdas.Models
{
    /// <summary>
    /// 종목 정보
    /// </summary>
    public partial class Stock
    {
        /// <summary>
        /// 종목코드
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// 종목명
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 시장구분
        /// </summary>
        public Market Market { get; set; }
        
        /// <summary>
        /// 야후 파이낸스 티커
        /// </summary>
        public string Ticker => $"{Code}.{Market}";

        public override string ToString()
        {
            return $"{Ticker} {Name}";
        }

        public decimal? 현재가 { get; set; }
        
        public decimal? 등락률 { get; set; }
        
        public decimal? 거래량 { get; set; }
        
        public decimal? 시가 { get; set; }
        
        public decimal? 고가 { get; set; }
        
        public decimal? 저가 { get; set; }
        
        public decimal? 시가총액 { get; set; }
        
        public decimal? 매출액 { get; set; }
        
        public decimal? 자산총계 { get; set; }
        
        public decimal? 부채총계 { get; set; }
        
        public decimal? 영업이익 { get; set; }
        
        public decimal? 당기순이익 { get; set; }
        
        public decimal? 주당순이익 { get; set; }
        
        public decimal? 보통주배당금 { get; set; }
        
        public decimal? 매출액증가율 { get; set; }
        
        public decimal? 영업이익증가율 { get; set; }
        
        public decimal? 외국인비율 { get; set; }
        
        public decimal? PER{ get; set; }
        
        public decimal? ROE{ get; set; }
        
        public decimal? ROA{ get; set; }
        
        public decimal? PBR{ get; set; }
        
        public decimal? 유보율 { get; set; }
        
        /// <summary>
        /// 투자의견
        /// </summary>
        public decimal? Consensus { get; set; }
        
        /// <summary>
        /// 목표주가
        /// </summary>
        public decimal? TargetPrice { get; set; }
        
        /// <summary>
        /// 레포트 수
        /// </summary>
        public decimal? ConsensusCount { get; set; }

        /// <summary>
        /// 현재가 / 목표주가
        /// </summary>
        [JsonIgnore]
        public decimal? CloseOfTarget
        {
            get
            {
                if (현재가 == null || TargetPrice == null)
                    return null;

                return 현재가 / TargetPrice;
            }
        }

        /// <summary>
        /// 컨센서스를 로드한다.
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