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
        IDisposable Subscribe(string symbol, SymbolType symbolType, SubscriptionType subscriptionType);

        void UnSubscribe(string symbol, SymbolType symbolType);




    }
}
