namespace Honeycomb.Azure.Bus.Infrastructure
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class MessageQueueSender : MessageSender
    {
        private readonly QueueClient queueClient;
        private static readonly object createLock = new object();

        public MessageQueueSender(string connectionString, string serviceBusQueue)
        {
            ensureQueueExists(connectionString, serviceBusQueue);

            queueClient = QueueClient.CreateFromConnectionString(connectionString, serviceBusQueue);
        }

        private static void ensureQueueExists(string connectionString, string serviceBusQueue)
        {
            lock (createLock)
            {
                var nsMgr = NamespaceManager.CreateFromConnectionString(connectionString);
                if (nsMgr.QueueExists(serviceBusQueue))
                    return;

                var definition = new QueueDescription(serviceBusQueue)
                                     {
                                         MaxSizeInMegabytes = 1024,
                                         DefaultMessageTimeToLive = TimeSpan.FromDays(365)
                                     };

                nsMgr.CreateQueue(definition);
            }
        }

        public void Send(BrokeredMessage message)
        {
            Retry.Work(
                () => queueClient.Send(((InternalBrokeredMessage) message).Real),
                e => e is CommunicationObjectFaultedException |
                     e is CommunicationObjectAbortedException |
                     e is MessagingCommunicationException |
                     e is TimeoutException |
                     e is ServerBusyException);
        }
    }
}