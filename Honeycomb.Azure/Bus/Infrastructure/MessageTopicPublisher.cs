namespace Honeycomb.Azure.Bus.Infrastructure
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    ///   Domain events are published to a topic.
    /// </summary>
    /// <returns> </returns>
    public class MessageTopicPublisher : MessageSender
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
            lock (createLock)
            {
                var nsMgr = NamespaceManager.CreateFromConnectionString(connectionString);
                if (nsMgr.TopicExists(serviceBusTopic))
                    return;

                var definition = new TopicDescription(serviceBusTopic)
                                     {
                                         MaxSizeInMegabytes = 1024,
                                         DefaultMessageTimeToLive = TimeSpan.FromDays(365)
                                     };

                nsMgr.CreateTopic(definition);
            }
        }

        public void Send(BrokeredMessage message)
        {
            Retry.Work(
                () => topicClient.Send(((InternalBrokeredMessage) message).Real),
                e => e is CommunicationObjectFaultedException |
                     e is CommunicationObjectAbortedException |
                     e is MessagingCommunicationException |
                     e is MessagingException |
                     e is TimeoutException |
                     e is ServerBusyException);
        }
    }
}