using System;
using System.Collections;
using System.Collections.Generic;
using MessagingContext;
using NetMq_MessagingContext;
using System.Threading;

namespace SubRequestClient
{
    class Program : IObserver<byte[]>
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            List<string> topics = new List<string>();
            topics.Add("Topic_A");

            // TODO: Use dependency injection to instantiate
            IMessagingContext clientContext = new NetMqMessagingContext(ContextType.SUB_REQ, topics);
            // TODO : cleaner way to know if Messaging service is ready, may be callback for readiness
            Thread.Sleep(5000);
            foreach (var topic in topics)
            {
                // we should always subscribe first and queue up the messages while waiting for snapshot
                clientContext.Subscribe(topic, p);
                clientContext.Request(topic, null, p.replyCallBack, null, new TimeSpan(), out var id, null);
            }
            Console.ReadKey();

        }

        public void replyCallBack(byte[] data)
        {
            Console.WriteLine($"Reply received with value of {BitConverter.ToInt32(data,0)}");
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(byte[] value)
        {
            Console.WriteLine($"Recieved notification with value of {BitConverter.ToInt32(value,0)}");
        }
    }
}