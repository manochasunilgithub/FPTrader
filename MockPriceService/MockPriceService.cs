using System;
using MarketData;

namespace MockPriceService
{
    public class MockPriceService : MarketDataService
    {
        public bool Subscribe(string symbol, SymbolType symbolType, SubscriptionType subscriptionType)
        {
            throw new NotImplementedException();
        }

        public bool UnSubscribe(string symbol, SymbolType symbolType)
        {
            throw new NotImplementedException();
        }   
    }
}
