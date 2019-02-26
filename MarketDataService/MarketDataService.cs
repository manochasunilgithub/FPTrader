using System;

namespace MarketData
{
    public enum SymbolType
    {
        Ric,
        Bloomberg,
        ExchangeSymbol
    }

    public enum SubscriptionType
    {
        Level1,
        Depth // Subscribe to market depth info
    }

    public interface MarketDataService
    {
        //This is to subscribe
        bool Subscribe(string symbol, SymbolType symbolType, SubscriptionType subscriptionType);

        bool UnSubscribe(string symbol, SymbolType symbolType);




    }
}
