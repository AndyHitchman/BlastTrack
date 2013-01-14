namespace Honeycomb.Azure.EventBus
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Domain events are published to a topic.
    /// </summary>
    /// <returns></returns>
    public class MessageTopicPublisher : IMessageSender
    {
        private readonly TopicClient topicClient;
        private static readonly object createLock = new object();

        public MessageTopicPublisher(string connectionString, string serviceBusTopic)
        {
            ensureTopicExists(connectionString, serviceBusTopic);

            topicClient = TopicClient.CreateFromConnectionString(connectionString, serviceBusTopic);
        }

        private static void ensureTopicExists(string connectionString, string serviceBusTopic)
        {
            lock(createLock)
            {
                var nsMgr = NamespaceManager.CreateFromConnectionString(connectionString);
                if(nsMgr.TopicExists(serviceBusTopic)) 
                    return;

                var definition = new TopicDescription(serviceBusTopic)
                    {
                        MaxSizeInMegabytes = 1024,
                        DefaultMessageTimeToLive = TimeSpan.FromDays(365)
                    };

                nsMgr.CreateTopic(definition);
            }
        }

        public void Send(Microsoft.ServiceBus.Messaging.BrokeredMessage message)
        {
            topicClient.Send(message);
        }

//        public void SendBatch(IEnumerable<BrokeredMessage> message)
//        {
//            topicClient.SendBatch(message);
//        }
//
//        public IAsyncResult BeginSend(BrokeredMessage message, AsyncCallback callback, object state)
//        {
//            return topicClient.BeginSend(message, callback, state);
//        }
//
//        public void EndSend(IAsyncResult result)
//        {
//            topicClient.EndSend(result);
//        }
//
//        public IAsyncResult BeginSendBatch(IEnumerable<BrokeredMessage> message, AsyncCallback callback, object state)
//        {
//            return topicClient.BeginSendBatch(message, callback, state);
//        }
//
//        public void EndSendBatch(IAsyncResult result)
//        {
//            topicClient.EndSendBatch(result);
//        }
    }
}