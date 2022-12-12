#region
using System;
using Kosdas.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endregion

namespace Kosdas.UnitTest
{
    [TestClass()]
    public class StockLoaderTests
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            StockLoader.Instance.LoadAsync().Wait();
        }

        [TestMethod()]
        public void LoadKospi()
        {
            if (DateTime.Today != new DateTime(2021, 2, 16))
            {
                Assert.Inconclusive("2021-2-16 시점에서 테스트 통과함.");
                return;
            }

            var stock = StockLoader.Instance["000660"];

            Assert.AreEqual("000660", stock.Code);
            Assert.AreEqual("SK하이닉스", stock.Name);
            Assert.AreEqual(Market.KS, stock.Market);
            Assert.AreEqual(3201832, stock.거래량);
            Assert.AreEqual(132500, stock.시가);
            Assert.AreEqual(134000, stock.고가);
            Assert.AreEqual(130500, stock.저가);
            Assert.AreEqual(964603, stock.시가총액);
            Assert.AreEqual(269907, stock.매출액);
            Assert.AreEqual(647895, stock.자산총계);
            Assert.AreEqual(168463, stock.부채총계);
            Assert.AreEqual(27127, stock.영업이익);
            Assert.AreEqual(20164, stock.당기순이익);
            Assert.AreEqual(3942, stock.주당순이익);
            Assert.AreEqual(1000, stock.보통주배당금);
            Assert.AreEqual(-33.27, stock.매출액증가율);
            Assert.AreEqual(-86.99, stock.영업이익증가율);
            Assert.AreEqual(50.04, stock.외국인비율);
            Assert.AreEqual(33.61, stock.PER);
            Assert.AreEqual(4.25, stock.ROE);
            Assert.AreEqual(3.14, stock.ROA);
            Assert.AreEqual(1.80, stock.PBR);
            Assert.AreEqual(1287.0, stock.유보율);
        }

        [TestMethod()]
        public void StockExtension을_사용하여_특정일의_가격을_가져옮()
        {
            var price = StockLoader.Instance[Models.Stock.삼성전자].LoadPrice(2021, 2, 16);

            Assert.AreEqual(new DateTime(2021, 2, 16), price.At);
            Assert.AreEqual(84500, price.Open);
            Assert.AreEqual(86000, price.High);
            Assert.AreEqual(84200, price.Low);
            Assert.AreEqual(84900, price.Close);
            Assert.AreEqual(20483100, price.Volume);
        }

        [TestMethod()]
        public void LoadConsensus()
        {
            if (DateTime.Today != new DateTime(2021, 2, 16))
            {
                Assert.Inconclusive("2021-2-16 시점에서 테스트 통과함.");
                return;
            }

            var stock = StockLoader.Instance[Models.Stock.메리츠증권];
            stock.LoadConsensus();

            Assert.AreEqual(3.71M, stock.Consensus);
            Assert.AreEqual(4443M, stock.TargetPrice);
            Assert.AreEqual(7M, stock.ConsensusCount);
        }

        [TestMethod()]
        public void 유효하지_않은_종목코드이면_null을_반환()
        {
            var stock = StockLoader.Instance["999999"];

            Assert.IsNull(stock);
        }
    }
}