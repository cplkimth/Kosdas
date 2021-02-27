#region
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AsyncMethodLibrary;
using HtmlAgilityPack;
#endregion

namespace Kosdas
{
    /// <summary>
    /// 종목 정보 로더. 열거 가능.
    /// </summary>
    public class StockLoader : IEnumerable<Stock>
    {
        static StockLoader()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #region singleton
        private static StockLoader _instance;

        public static StockLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StockLoader();
                return _instance;
            }
        }

        private StockLoader()
        {
        }
        #endregion

        private readonly ConcurrentDictionary<string, Stock> _dictionary = new(Environment.ProcessorCount * 4, DictionaryCapacity);

        private const int ItemsPerPage = 50;

        /// <summary>
        ///     https://finance.naver.com/sise/sise_market_sum.nhn?sosok=0&page=32 의 마지막 페이지
        /// </summary>
        private const int MaxPageForKospi = 32;

        /// <summary>
        ///     https://finance.naver.com/sise/sise_market_sum.nhn?sosok=1&page=30 의 마지막 페이지
        /// </summary>
        private const int MaxPageForKosdaq = 30;

        private static int DictionaryCapacity => (MaxPageForKospi + MaxPageForKosdaq) * ItemsPerPage;

        private bool _loaded;

        /// <summary>
        /// 종목코드에 해당하는 종목정보를 반환한다. 사용 전 Load 혹은 LoadAsync 메서드를 먼저 호출하는 것을 권장함.
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns>종목코드가 유효하지 않으면 null.</returns>
        public Stock this[string stockCode]
        {
            get
            {
                if (_loaded == false)
                    Load();

                if (_dictionary.ContainsKey(stockCode))
                    return _dictionary[stockCode];

                return null;
            }
        }

        #region IEnumerable<Stock>
        public IEnumerator<Stock> GetEnumerator()
        {
            foreach (var stock in _dictionary)
            {
                yield return stock.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region generate
        /// <summary>
        ///     종목명 상수 클래스를 생성한다.
        /// </summary>
        /// <param name="targetPath">생성할 클래스의 경로. ex)C:\git\KrxHelper\KrxHelper\Stock.constant.cs</param>
        /// <param name="namespace">생성할 클래스의 네임스페이스</param>
        /// <param name="class">생성할 클래스의 이름</param>
        public void Generate(string targetPath, string @namespace, string @class)
        {
            var noises = new HashSet<char>("()& -.%/+".ToCharArray());

            StringBuilder builder = new();

            foreach (var stock in Instance)
            {
                string stockName = RemoveNoise(stock.Name, noises);
                builder.AppendLine($"public const string {stockName} = \"{stock.Code}\";");
            }

            string text = @$"namespace {@namespace}
{{
    public partial class {@class}
    {{
        {builder}
    }}
}}";

            File.WriteAllText(targetPath, text);
        }

        private string RemoveNoise(string stockName, HashSet<char> noises)
        {
            foreach (var noise in noises)
            {
                if (stockName.Contains(noise))
                    stockName = stockName.Replace(noise, '_');
            }

            if (char.IsNumber(stockName[0]))
                stockName = "_" + stockName;

            return stockName;
        }
        #endregion

        public Task LoadAsync(int maxPageForKospi = MaxPageForKospi, int maxPageForKosdaq = MaxPageForKosdaq)
        {
            return Task.Factory.StartNew(() => Load(maxPageForKospi, maxPageForKosdaq));
        }

        /// <summary>
        /// 전 종목정보를 구한다.
        /// </summary>
        /// <param name="maxPageForKospi">https://finance.naver.com/sise/sise_market_sum.nhn?sosok=0&page=32 의 마지막 페이지. 기본값 사용 권장.</param>
        /// <param name="maxPageForKosdaq">https://finance.naver.com/sise/sise_market_sum.nhn?sosok=1&page=30 의 마지막 페이지. 기본값 사용 권장.</param>
        public void Load(int maxPageForKospi = MaxPageForKospi, int maxPageForKosdaq = MaxPageForKosdaq)
        {
            var totalPages = new Dictionary<Market, int>
                             {
                                 {Market.KS, maxPageForKospi},
                                 {Market.KQ, maxPageForKosdaq},
                             };

            var urls = new List<(string, Market, int)>();
            foreach (var item in totalPages)
            {
                for (int i = 1; i <= item.Value; i++)
                {
                    string url = $"https://finance.naver.com/sise/sise_market_sum.nhn?sosok={(int) item.Key}&page={i}";
                    urls.Add((url, item.Key, i));
                }
            }

#if DEBUG
            urls.ForEach(x => LoadCore(x));
#else
            urls.AsParallel().ForAll(x => LoadCore(x));
#endif
            _loaded = true;
        }

        private void LoadCore((string url, Market market, int page) tuple)
        {
            var args = OnRequestSendingWithReturn(tuple.url, tuple.market, tuple.page, false);

            if (args.Cancel)
                return;

            WebClient web = new WebClient();
            web.Encoding = Encoding.GetEncoding("EUC-KR");
            web.Headers.Add(HttpRequestHeader.Cookie, "field_list=12|07E0FFFF;");
            var html = web.DownloadString(tuple.url);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var tbody = document.DocumentNode.SelectSingleNode("//*[@id=\"contentarea\"]/div[3]/table[1]/tbody");
            var trs = tbody.ChildNodes.Where(x => x.Name == "tr");

            foreach (var tr in trs)
            {
                var tds = tr.ChildNodes.Where(x => x.Name == "td").ToArray();

                if (tds.Length != 29)
                    continue;

                string stockCode = ReadStockCode(tds[1]);
                Stock stock = new Stock(stockCode, tds[1].InnerText, tuple.market);
                stock.현재가 = tds[2].ParseCell();
                stock.등락률 = tds[4].ParseCell();
                stock.거래량 = tds[6].ParseCell();
                stock.시가 = tds[9].ParseCell();
                stock.고가 = tds[10].ParseCell();
                stock.저가 = tds[11].ParseCell();
                stock.시가총액 = tds[12].ParseCell();
                stock.매출액 = tds[13].ParseCell();
                stock.자산총계 = tds[14].ParseCell();
                stock.부채총계 = tds[15].ParseCell();
                stock.영업이익 = tds[16].ParseCell();
                stock.당기순이익 = tds[17].ParseCell();
                stock.주당순이익 = tds[18].ParseCell();
                stock.보통주배당금 = tds[19].ParseCell();
                stock.매출액증가율 = tds[20].ParseCell();
                stock.영업이익증가율 = tds[21].ParseCell();
                stock.외국인비율 = tds[22].ParseCell();
                stock.PER = tds[23].ParseCell();
                stock.ROE = tds[24].ParseCell();
                stock.ROA = tds[25].ParseCell();
                stock.PBR = tds[26].ParseCell();
                stock.유보율 = tds[27].ParseCell();

                _dictionary.TryAdd(stock.Code, stock);
            }

            double percent = _dictionary.Count * 100.0 / DictionaryCapacity;
            OnProgressChanged(percent);
        }

        private string ReadStockCode(HtmlNode td)
        {
            var href = td.FirstChild.GetAttributeValue("href", default(string));
            return href.Substring(href.Length - 6);
        }

        #region ProgressChanged event things for C# 3.0
        /// <summary>
        ///     진행상황이 변경되었음
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

        private ProgressChangedEventArgs OnProgressChanged(double percent)
        {
            ProgressChangedEventArgs args = new ProgressChangedEventArgs(percent);
            OnProgressChanged(args);

            return args;
        }

        private ProgressChangedEventArgs OnProgressChangedForOut()
        {
            ProgressChangedEventArgs args = new ProgressChangedEventArgs();
            OnProgressChanged(args);

            return args;
        }

        public class ProgressChangedEventArgs : EventArgs
        {
            public double Percent { get; set; }

            public ProgressChangedEventArgs()
            {
            }

            public ProgressChangedEventArgs(double percent)
            {
                Percent = percent;
            }
        }
        #endregion

        /// <summary>
        ///     페이지에 요청을 보낼려고 함. 취소가능.
        /// </summary>

        #region RequestSending event things for C# 3.0
        public event EventHandler<RequestSendingEventArgs> RequestSending;

        protected virtual void OnRequestSending(RequestSendingEventArgs e)
        {
            if (RequestSending != null)
                RequestSending(this, e);
        }

        protected virtual void OnRequestSending(string url, Market market, int page, bool cancel)
        {
            OnRequestSending(new RequestSendingEventArgs(url, market, page, cancel));
        }

        protected virtual RequestSendingEventArgs OnRequestSendingWithReturn(string url, Market market, int page, bool cancel)
        {
            RequestSendingEventArgs args = new RequestSendingEventArgs(url, market, page, cancel);
            OnRequestSending(args);

            return args;
        }

        public class RequestSendingEventArgs : EventArgs
        {
            public string Url { get; set; }
            public Market Market { get; set; }
            public int Page { get; set; }
            public bool Cancel { get; set; }

            public RequestSendingEventArgs()
            {
            }

            public RequestSendingEventArgs(string url, Market market, int page, bool cancel)
            {
                Url = url;
                Market = market;
                Page = page;
                Cancel = cancel;
            }
        }
        #endregion
    }
}