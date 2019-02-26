using System;

namespace MarketDataService
{
    public enum SymbolType
    {
        Ric,
        Bloomberg,
        ExchangeSymbol
    }

    public enum SubscriptionType
    {
        TopLevel,
        Depth,
        TopLevelAndDepth
    }

    public interface MarketDataService
    {
        //This is to subscribe
        bool Subscribe(string symbol, SymbolType symbolType, SubscriptionType subscriptionType);

        bool UnSubscribe(string symbol, SymbolType symbolType);




    }
}
