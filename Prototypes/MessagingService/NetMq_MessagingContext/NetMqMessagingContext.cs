using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using MessagingContext;
using NetMQ;
using NetMQ.Sockets;

namespace NetMq_MessagingContext
{
    // TODO : See if we need separate class for Server and Client Context to have clear roles and responsibilities
    // we can have scenraio where server will need both publisher and subscription role
    public class NetMqMessagingContext : IMessagingContext
    {
        Dictionary<ulong, Action<byte[]>> requestCallBacks = new Dictionary<ulong, Action<byte[]>>();

        private PublisherSocket publisherSocket; // Used by Server publisher
        private ResponseSocket responseSocket;// Used by Server Response socket

        private RequestSocket requestSocket;// Used by Client request
        private SubscriberSocket subcriberSocket;// Used by Client request

        private ContextType _contextType;


        public NetMqMessagingContext(ContextType contextType, List<string> topics)
        {
            _contextType = contextType;
            if (contextType == ContextType.PUB_RESP)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;

                InitializeServer();

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.White;
                InitializeClient(topics);

            }
        }

        private void InitializeClient(List<string> topics)
        {
            // Create and connect SUB socket
            subcriberSocket = new SubscriberSocket();
            subcriberSocket.Connect("tcp://127.0.0.1:5001");

            // Receive Sync messages, ideally we will queue these messages while taking the snapshot and use uniqiue way of identifying the message and throw away
            subcriberSocket.ReceiveReady += SubcriptionReceived;

            requestSocket = new RequestSocket();
            requestSocket.Connect("tcp://127.0.0.1:5002");
            requestSocket.ReceiveReady += ReplyReceived;

        }

        private void InitializeServer()
        {
            publisherSocket = new PublisherSocket();
            responseSocket = new ResponseSocket();
            publisherSocket.Bind("tcp://*:5001"); // binds the local ip address to port 5001

            responseSocket.Bind("tcp://*:5002");
            responseSocket.ReceiveReady += responseSocket_ReceiveReady;

        }

        private void ReplyReceived(object sender, NetMQSocketEventArgs e)
        {
            var replyMsg = e.Socket.ReceiveFrameString();
            var requestIdBytes = e.Socket.ReceiveFrameBytes();
            var bytes = e.Socket.ReceiveFrameBytes();
            var reqId = BitConverter.ToUInt64(requestIdBytes, 0);
            Console.WriteLine(
                $"Received reply on topic {replyMsg} with value of {BitConverter.ToInt32(bytes, 0)} and RequestId {BitConverter.ToUInt64(requestIdBytes, 0)} ");

            if (requestCallBacks.TryGetValue(reqId, out var callBack))
            {
                callBack(bytes);
                requestCallBacks.Remove(reqId);
            }
        }

        private void SubcriptionReceived(object sender, NetMQSocketEventArgs e)
        {
            var topicPath = e.Socket.ReceiveFrameString();
            var bytes = subcriberSocket.ReceiveFrameBytes();
            Console.WriteLine($"Received subscription message on topic {topicPath} with value of {BitConverter.ToInt32(bytes, 0)} ");
            if (observers.TryGetValue(topicPath, out var list))
            {
                foreach (var observer in list)
                {
                    observer.OnNext(bytes);
                }
            }
        }

        private readonly Dictionary<string, List<IObserver<byte[]>>> observers = new Dictionary<string, List<IObserver<byte[]>>>();

        private void responseSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            // we got a new snapshot request, let's send the response back
            var reqMsg = e.Socket.ReceiveFrameString();
            byte[] requestIdBytes = e.Socket.ReceiveFrameBytes();

            if (OnRequest != null)
            {
                byte[] replyBytes = OnRequest(reqMsg);
                // send the response back here.. if not found we should return error code. topic not found.. should be handled in the message
                if (replyBytes != null)
                {
                    e.Socket.SendFrame(reqMsg, true);
                    e.Socket.SendFrame(requestIdBytes, true);
                    e.Socket.SendFrame(replyBytes);
                }
                else // Error case if no topic found
                {
                    e.Socket.SendFrame(reqMsg, true);
                    e.Socket.SendFrame(requestIdBytes, true);
                    e.Socket.SendFrame(BitConverter.GetBytes(-1)); // we Should imrpove on this.
                }
            }

        }



        public Status Subscribe(string path, IObserver<byte[]> observer)
        {
            if (observers.TryGetValue(path, out var list))
            {
                list.Add(observer);
            }
            else
            {
                list = new List<IObserver<byte[]>> { observer };
                observers.Add(path, list);
                subcriberSocket.Subscribe(path);
            }
            return Status.OK;
        }

        public bool Unsubscribe(string path, IObserver<byte[]> observer)
        {
            if (observers.TryGetValue(path, out var list))
            {
                if (list.Contains(observer))
                {
                    list.Remove(observer);
                }
            }
            return false;
        }

        public Status Publish(string path, byte[] bytes)
        {
            try
            {
                publisherSocket?.SendFrame(path, true);
                publisherSocket?.SendFrame(bytes);
            }
            catch (Exception)
            {
                // TODO log it 
                return Status.Error;
            }
            return Status.OK;
        }

        private ulong requestId = 1;
        public Status Request(string path, byte[] bytes, Action<byte[]> callback, Action<ulong> onTimeout, TimeSpan timeout, out ulong rid,
            object tag = null)
        {
            requestId++;
            rid = requestId;
            requestCallBacks.Add(requestId, callback);
            requestSocket.SendFrame(path, true);
            requestSocket.SendFrame(BitConverter.GetBytes(rid));

            return Status.OK;
        }

        public Status Reply(ulong requestId, byte[] data, bool isFinal)
        {
            throw new NotImplementedException();
        }

        private bool started = false;
        public void Run()
        {
            if (!started)
            {
                new Thread(() =>
                {
                    if (_contextType == ContextType.PUB_RESP)
                    {
                        using (var poller = new NetMQPoller { publisherSocket, responseSocket })
                        {
                            poller.Run();
                        }
                    }
                    else if (_contextType == ContextType.SUB_REQ)
                    {
                        using (var poller = new NetMQPoller { subcriberSocket, requestSocket })
                        {
                            poller.Run();
                        }
                    }
                }).Start();
            }
        }

        public Func<string, byte[]> OnRequest { get; set; }
    }
}
