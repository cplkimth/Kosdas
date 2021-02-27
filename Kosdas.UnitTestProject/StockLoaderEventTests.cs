#region
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endregion

namespace Kosdas.UnitTest
{
    [TestClass()]
    public class StockLoaderEventTests
    {
        private static void StockLoader_RequestSending(object sender, StockLoader.RequestSendingEventArgs e)
        {
            if (e.Market == Market.KQ)
                e.Cancel = true;
        }

        [TestMethod()]
        public void 마켓이_코스닥이면_요청을_취소하여야_함()
        {
            StockLoader.Instance.RequestSending += StockLoader_RequestSending;

            StockLoader.Instance.LoadAsync().Wait();

            int count = StockLoader.Instance.Count(x => x.Market == Market.KQ);

            Assert.AreEqual(0, count);

            StockLoader.Instance.RequestSending -= StockLoader_RequestSending;
        }
    }
}