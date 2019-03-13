using System;
using MessagingContext;

namespace ZeroMq_MessagingContext
{
    public class ZeroMqMessagingContext : IMessagingContext
    {
        public Func<byte[], string> OnRequest { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Func<string, byte[]> IMessagingContext.OnRequest { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Status Publish(string path, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public Status Reply(ulong requestId, byte[] data, bool isFinal)
        {
            throw new NotImplementedException();
        }

        public Status Request(string path, byte[] bytes, Action<byte[]> callback, Action<ulong> onTimeout, TimeSpan timeout, out ulong rid, object tag = null)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public Status Subscribe(string path, IObserver<byte[]> observer)
        {
            throw new NotImplementedException();
        }

        public bool Unsubscribe(string path, IObserver<byte[]> observer)
        {
            throw new NotImplementedException();
        }
    }
}
