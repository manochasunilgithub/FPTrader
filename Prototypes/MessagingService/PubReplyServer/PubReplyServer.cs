using System;
using System.Collections;
using System.Collections.Generic;
using MessagingContext;
using NetMq_MessagingContext;

namespace PubReplyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> topics = new List<string>();
            topics.Add("Topic_A");
            topics.Add("Topic_B");
            topics.Add("Topic_C");
            NetMqMessagingContext serverContext = new NetMqMessagingContext(ContextType.PUB_RESP, topics);
        }
    }
}