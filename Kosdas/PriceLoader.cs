#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AsyncMethodLibrary;
#endregion

namespace Kosdas
{
    /// <summary>
    /// 가격정보 로더
    /// </summary>
    public partial class PriceLoader
    {
        static PriceLoader()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private const int 최장휴장일 = 10;

        #region singleton
        private static PriceLoader _instance;

        public static PriceLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PriceLoader();
                return _instance;
            }
        }

        private PriceLoader()
        {
        }
        #endregion

        /// <summary>
        /// 일정기간의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="from">시작일</param>
        /// <param name="to">종료일</param>
        /// <returns></returns>
        [ForAsync]
        public IEnumerable<Price> Load(string stockCode, DateTime from, DateTime to)
        {
            string url = $"https://fchart.stock.naver.com/siseJson.nhn?symbol={stockCode}&requestType=1&startTime={from:yyyyMMdd}&endTime={to:yyyyMMdd}&timeframe=day";

            WebClient web = new WebClient();
            web.Encoding = Encoding.UTF8;

            var text = web.DownloadString(url).Split('\n');
            var lines = text.Where(x => x.StartsWith("[\""));

            return lines.Select(x => ParsePrice(x));
        }

        /// <summary>
        /// 최근 N일간의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="days">최근 N일</param>
        /// <returns></returns>
        [ForAsync]
        public IEnumerable<Price> Load(string stockCode, int days) => LoadCore(stockCode, days);

        /// <summary>
        /// 특정일의 가격을 가져온다. 휴장일일 경우에는 마지막 거래일의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="date">특정일</param>
        /// <returns>조건에 맞는 가격정보가 없으면 null.</returns>
        [ForAsync]
        public Price Load(string stockCode, DateTime date)
        {
            return Load(stockCode, date.AddDays(최장휴장일 * -1), date).LastOrDefault();
        }

        /// <summary>
        /// 특정일의 가격을 가져온다. 휴장일일 경우에는 마지막 거래일의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        [ForAsync]
        public Price Load(string stockCode, int year, int month, int day) => Load(stockCode, new DateTime(year, month, day));

        /// <summary>
        /// 마지막 거래일의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <returns></returns>
        [ForAsync]
        public Price Load(string stockCode) => Load(stockCode, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        private IEnumerable<Price> LoadCore(string stockCode, int days)
        {
            DateTime from = DateTime.Today.AddDays(days * -1);

            return Load(stockCode, from, DateTime.Today);
        }

        private Price ParsePrice(string line)
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