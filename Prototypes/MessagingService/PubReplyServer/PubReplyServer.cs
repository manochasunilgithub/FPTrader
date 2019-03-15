
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MessagingContext;
using NetMq_MessagingContext;

namespace PubReplyServer
{
    class Program
    {
        Dictionary<string, int> messageCache = new Dictionary<string, int>();
        private IMessagingContext serverContext;
        Random randomNumber = new Random(2);

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Program p = new Program();
            p.Initialize();
        }

        private void Initialize()
        {
            List<string> topics = new List<string>();
            topics.Add("Topic_A");
            topics.Add("Topic_B");
            topics.Add("Topic_C");
            // Initialize topics
            foreach (var topic in topics)
            {
                messageCache.Add(topic, 1);
            }

            
            serverContext = new NetMqMessagingContext(ContextType.PUB_RESP, topics);
            serverContext.OnRequest = ServerContext_OnRequest;
            var timer = new Timer(timerCallback, null, 1000, 1000);
            serverContext.Run();
        }

        private void timerCallback(object arg)
        {
            foreach (var key in messageCache.ToArray())
            {
                messageCache[key.Key] = randomNumber.Next(2, 1000);
                serverContext.Publish(key.Key, BitConverter.GetBytes(messageCache[key.Key]));
            }
        }




        private byte[] ServerContext_OnRequest(string arg)
        {
            if (messageCache.TryGetValue(arg, out var val))
            {
                return BitConverter.GetBytes(val);
            }
            return null;
        }
    }
}
