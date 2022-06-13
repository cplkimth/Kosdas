#region
using System;
using System.Linq;
using Kosdas.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endregion

namespace Kosdas.UnitTest
{
    [TestClass()]
    public class PriceLoaderTests
    {
        [TestMethod()]
        public void 시작일과_종료일을_지정()
        {
            var prices = PriceLoader.Yahoo.Load("005930", new DateTime(2021, 2, 5), new DateTime(2021, 2, 13)).ToList();

            Assert.AreEqual(4, prices.Count);
            Assert.AreEqual(new DateTime(2021, 2, 5), prices[0].Date);
            Assert.AreEqual(new DateTime(2021, 2, 10), prices[^1].Date);
            Assert.AreEqual(82500, prices[0].Low);
            Assert.AreEqual(84200, prices[1].High);
            Assert.AreEqual(84000, prices[2].Open);
            Assert.AreEqual(81600, prices[3].Close);
            Assert.AreEqual(23025766, prices[3].Volume);
        }

        [TestMethod()]
        public void 최근_2일간의_가격()
        {
            if (DateTime.Today != new DateTime(2021, 2, 16))
            {
                Assert.Inconclusive("2021-2-16 시점에서 테스트 통과함.");
                return;
            }

            var prices = PriceLoader.Yahoo.Load("005930", 2).ToList();

            Assert.AreEqual(2, prices.Count);
            Assert.AreEqual(new DateTime(2021, 2, 15), prices[0].Date);
            Assert.AreEqual(new DateTime(2021, 2, 16), prices[1].Date);
            Assert.AreEqual(83800, prices[0].Open);
            Assert.AreEqual(84500, prices[0].High);
            Assert.AreEqual(83300, prices[0].Low);
            Assert.AreEqual(84200, prices[0].Close);
            Assert.AreEqual(23529706, prices[0].Volume);
            Assert.AreEqual(84500, prices[1].Open);
            Assert.AreEqual(86000, prices[1].High);
            Assert.AreEqual(84200, prices[1].Low);
            Assert.AreEqual(84900, prices[1].Close);
            Assert.AreEqual(20373346, prices[1].Volume);
        }

        [TestMethod()]
        public void 특정일()
        {
            var price = PriceLoader.Yahoo.Load("005930", 2021, 2, 16);

            Assert.AreEqual(new DateTime(2021, 2, 16), price.Date);
            Assert.AreEqual(84500, price.Open);
            Assert.AreEqual(86000, price.High);
            Assert.AreEqual(84200, price.Low);
            Assert.AreEqual(84900, price.Close);
            Assert.AreEqual(20483100, price.Volume);
        }

        [TestMethod()]
        public void 존재하지_않는_날짜()
        {
            var price = PriceLoader.Yahoo.Load("005930", 2099, 1, 1);

            Assert.IsNull(price);
        }

        [TestMethod()]
        public void 야후금융에서_코스닥_가격정보_가져오기()
        {
            var price = PriceLoader.Naver.Load(Stock.아이퀘스트, 2021, 2, 26);

            Assert.AreEqual(new DateTime(2021, 2, 26), price.Date);
            Assert.AreEqual(17500, price.Open);
            Assert.AreEqual(17750, price.High);
            Assert.AreEqual(16350, price.Low);
            Assert.AreEqual(16500, price.Close);
            Assert.AreEqual(226443, price.Volume);
        }

        [TestMethod()]
        public void 수정주가를_반영하여야_함()
        {
            var prices = PriceLoader.Yahoo.Load(Stock.삼성전자, new DateTime(2018,5,3), new DateTime(2018,5,4)).ToList();

            Assert.AreEqual(53000, prices[0].Open);
            Assert.AreEqual(53000, prices[0].Close);
            Assert.AreEqual(53000, prices[1].Open);
            Assert.AreEqual(51900, prices[1].Close);
        }
    }
}