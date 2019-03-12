using System;

namespace MessagingContext
{
    public enum ContextType
    {
        SUB = 1,// Subscription mode
        REQ = 2,// Request mode
        REP = 3,// Reply mode
        SUB_REQ = 4,// Subscription-Request Mode
        PUB_RESP = 5 // Publish & Reply Mode
    }


    public interface IMessagingContext
    {

        /// <summary>
        /// Subscribe an observer to the given subject string that cannot be wildcard
        /// Here path is a topic
        /// </summary>
        Status Subscribe(string path, IObserver<byte[]> observer);

        /// <summary>
        /// Remove the subscription for the given observer on the given subject
        /// </summary>
        bool Unsubscribe(string path, IObserver<byte[]> observer);

        /// <summary>
        /// Send a message via the context on the given subject
        /// </summary>
        Status Publish(string path, byte[] bytes);

        /// <summary>
        /// Send a request on the given subject with the ability to timeout
        /// </summary>
        Status Request(string path, byte[] bytes, Action<byte[]> callback, Action<ulong> onTimeout,
            TimeSpan timeout, out ulong rid, object tag = null);

        /// <summary>
        /// Send a reply to a request for the given request id
        /// </summary>
        Status Reply(ulong requestId, byte[] data, bool isFinal);
    }
}
