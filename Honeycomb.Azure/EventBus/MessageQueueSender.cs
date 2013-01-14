namespace Honeycomb.Azure.EventBus
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class MessageQueueSender : IMessageSender
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
            lock(createLock)
            {
                var nsMgr = NamespaceManager.CreateFromConnectionString(connectionString);
                if(nsMgr.QueueExists(serviceBusQueue)) 
                    return;

                var definition = new QueueDescription(serviceBusQueue)
                    {
                        MaxSizeInMegabytes = 1024,
                        DefaultMessageTimeToLive = TimeSpan.FromDays(365)
                    };

                nsMgr.CreateQueue(definition);
            }
        }

        public void Send(Microsoft.ServiceBus.Messaging.BrokeredMessage message)
        {
            queueClient.Send(message);
        }
    }
}