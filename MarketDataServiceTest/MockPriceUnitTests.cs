using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarketDataPriceServiceTest
{
    [TestClass]
    public class MockPriceUnitTests
    {
        [TestMethod]
        public void Subscribe_ValidSubscriptionTestCase()
        {
            MarketData.MarketDataService service = new MockPriceService.MockPriceService();

            //service.Subscribe()
        }
    }
}
