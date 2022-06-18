#region
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AsyncMethodLibrary;
using Kosdas.Models;
#endregion

namespace Kosdas
{
    /// <summary>
    ///     가격정보 로더
    /// </summary>
    public abstract partial class PriceLoader
    {
        static PriceLoader()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private const int 최장휴장일 = 10;

        private protected PriceLoader()
        {
        }

        #region Yahoo
        private static PriceLoader _yahoo;

        /// <summary>
        /// 야후 가격정보 로더
        /// </summary>
        public static PriceLoader Yahoo
        {
            get
            {
                if (_yahoo == null)
                    _yahoo = new YahooPriceLoader();
                return _yahoo;
            }
        }
        #endregion

        #region Naver
        private static PriceLoader _naver;

        /// <summary>
        /// 네이버 가격정보 로더
        /// </summary>
        public static PriceLoader Naver
        {
            get
            {
                if (_naver == null)
                    _naver = new NaverPriceLoader();
                return _naver;
            }
        }
        #endregion

        protected abstract Price ParsePrice(string line);

        /// <summary>
        /// 기본 가격정보 로더 (Yahoo와 동일)
        /// </summary>
        public static PriceLoader Instance => Yahoo;

        /// <summary>
        ///     일정기간의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="from">시작일</param>
        /// <param name="to">종료일</param>
        /// <returns></returns>
        [ForAsync]
        public abstract IEnumerable<Price> Load(string stockCode, DateTime from, DateTime to);

        [ForAsync]
        public IReadOnlyList<Price> LoadAsList(string stockCode, DateTime from, DateTime to) => Load(stockCode, from, to).ToImmutableList();

        /// <summary>
        ///     최근 N일간의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="days">최근 N일</param>
        /// <returns></returns>
        [ForAsync]
        public IEnumerable<Price> Load(string stockCode, int days)
        {
            DateTime from = DateTime.Today.AddDays(days * -1);

            return Load(stockCode, from, DateTime.Today);
        }

        [ForAsync]
        public IReadOnlyList<Price> LoadAsList(string stockCode, int days) => Load(stockCode, days).ToImmutableList();

        /// <summary>
        ///     특정일의 가격을 가져온다. 휴장일일 경우에는 마지막 거래일의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="date">특정일</param>
        /// <returns>조건에 맞는 가격정보가 없으면 null.</returns>
        [ForAsync]
        public Price Load(string stockCode, DateTime date)
        {
            return Load(stockCode, date.AddDays(최장휴장일 * -1), date)?.LastOrDefault();
        }

        /// <summary>
        ///     특정일의 가격을 가져온다. 휴장일일 경우에는 마지막 거래일의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        [ForAsync]
        public Price Load(string stockCode, int year, int month, int day) => Load(stockCode, new DateTime(year, month, day));

        /// <summary>
        ///     마지막 거래일의 가격을 가져온다.
        /// </summary>
        /// <param name="stockCode">종목코드</param>
        /// <returns></returns>
        [ForAsync]
        public Price Load(string stockCode) => Load(stockCode, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
    }
}