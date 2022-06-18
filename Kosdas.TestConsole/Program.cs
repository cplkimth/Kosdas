using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using AsyncMethodLibrary;
using Kosdas.Models;

namespace Kosdas.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateAsyncWrapper();
            return;
            // Example();
            // return;

            // WriteStockCodes();
            // FindRecommended();

            var instance = StockLoader.Instance;
            var sj = instance[Stock.삼성전자];
            Console.WriteLine(sj);

            Console.WriteLine("press any key to exit.");
            Console.ReadKey();
        }

        private static void WriteStockCodes()
        {
            StockLoader.Instance.Load();

            ConcurrentDictionary<string, Stock> dictionary = new ConcurrentDictionary<string, Stock>();

            foreach (var stock in StockLoader.Instance)
                dictionary.TryAdd(stock.Code, stock);

            var s1 = dictionary[Stock.삼성전자];

            // var json = JsonSerializer.Serialize(dictionary);
            // File.WriteAllText(@"C:\Users\thkim\Desktop\stocks.json", json);
            
            var json = JsonSerializer.SerializeToUtf8Bytes(dictionary);
            File.WriteAllBytes(@"C:\Users\thkim\Desktop\stocks.json", json);

            var span = new ReadOnlySpan<byte>(json);
            var stocks = JsonSerializer.Deserialize<ConcurrentDictionary<string, Stock>>(json);
            var s = stocks[Stock.삼성전자];
            Console.WriteLine(s);
        }

        private static void FindRecommended()
        {
            var watch = Stopwatch.StartNew();
            watch.Start();
            StockLoader.Instance.Load();
            var stocks = StockLoader.Instance.ToList();
            stocks.AsParallel().ForAll(x => x.LoadConsensus());
            Console.WriteLine(watch.Elapsed.TotalMilliseconds.ToString("N0")); // 11,196

            List<string> purchasedStockCodes = LoadPurchasedStockCodes();

            RealTimePriceLoader.Instance.Load();
            foreach (var stock in StockLoader.Instance)
                stock.현재가 = RealTimePriceLoader.Instance[stock.Code];

            var query = from x in stocks
                where x.ConsensusCount > 10 && x.Consensus >= 4 && x.Code.EndsWith("0") && purchasedStockCodes.Contains(x.Code) == false
                // orderby x.TargetPerClose descending, x.ConsensusCount descending, x.Consensus descending
                orderby x.CloseOfTarget, x.ConsensusCount descending, x.Consensus descending
                select x;

            StringBuilder builder = new StringBuilder();
            foreach (var stock in query.Take(10))
            {
                var market = stock.Market == Market.KS ? "KRX" : "KOSDAQ";
                var date = DateTime.Today.AddDays(1).ToString("yyyy.MM.dd");
                var line = $"{date}\t{market}\t{stock.Code}\t{stock.Name}\t{stock.Consensus:N2}\t{stock.ConsensusCount}\t{stock.TargetPrice:N0}\t{stock.CloseOfTarget:P2}";
                builder.AppendLine(line);
                Console.WriteLine(line);
            }

            WriteToFile(builder);
        }

        private static void GenerateAsyncWrapper()
        {
            Generator.Generate(@"C:\git\Kosdas\Kosdas\generated", typeof(Extensions.StockExtension), typeof(PriceLoader));
        }

        private static void Example()
        {
            // 전 종목의 정보를 구한다.
            StockLoader.Instance.Load();
            var stocks = StockLoader.Instance.ToList();
            
            // 전 종목의 컨센서스를 구한다.
            stocks.AsParallel().ForAll(x => x.LoadConsensus());
            
            // 전 종목의 실시간 시세를 구한다.
            RealTimePriceLoader.Instance.Load();

            var query = from x in stocks
                // 추천 레포트가 10개 이상이고 투자의견이 4.0 이상인 종목을 필터링
                where x.ConsensusCount > 10 && x.Consensus >= 4.0M
                // (현재가/목표주가) 순으로 정렬
                orderby x.CloseOfTarget
                select x;

            // 목록 중 앞에서 10개를 반환
            var list = query.Take(10).ToList();

            foreach (var stock in list)
            {
                // 종목코드 / 종목명 / 투자의견 / 레포트수 / 목표주가 / (현재가/목표주가)
                Console.WriteLine($"{stock.Code}\t{stock.Name}\t{stock.Consensus:N2}\t{stock.ConsensusCount}\t{stock.TargetPrice:N0}\t{stock.CloseOfTarget:P2}");
            }
        }

        private static void WriteToFile(StringBuilder builder)
        {
            string directory = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                directory = "/home/thkim/git/NaverFinanceApi/NaverFinanceApi.TestConsole";
            else
                directory = @"C:\git\NaverFinanceApi\NaverFinanceApi.TestConsole";

            var path = Path.Combine(directory, DateTime.Today.ToString("yyyy-MM-dd") + ".tsv");
            File.WriteAllText(path, builder.ToString());
        }

        private static List<string> LoadPurchasedStockCodes()
        {
            List<string> codes = new();

            string directory = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                directory = "/home/thkim/git/NaverFinanceApi/NaverFinanceApi.TestConsole";
            else
                directory = @"C:\git\NaverFinanceApi\NaverFinanceApi.TestConsole";
            
            try
            {
                var lines = File.ReadAllLines(Path.Combine(directory, "purchased.tsv"));
                return lines.Select(x => x.Split('\t')[2]).ToList();
            }
            catch
            {
            }

            return codes;
        }

        private void Instance_OnRequestSending(object sender, StockLoader.RequestSendingEventArgs e)
        {
            Console.WriteLine($"{e.Market} / {e.Page} / {e.Url}");
        }

        private void Instance_OnProgressChanged(object sender, StockLoader.ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.Percent);
        }
    }
}