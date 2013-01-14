namespace Honeycomb.Azure.EventBus
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class QueueReceiver : IMessageReceiver
    {
        private readonly string connectionString;
        private readonly string queueName;
        private QueueClient queueClient;
        private static readonly object createLock = new object();

        public QueueReceiver(string connectionString, string queueName)
        {
            this.connectionString = connectionString;
            this.queueName = queueName;
            ensureQueueExists();
            createClient();
        }

        private void createClient()
        {
            queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName);
        }


        private  void ensureQueueExists()
        {
            lock (createLock)
            {
                var nsMgr = NamespaceManager.CreateFromConnectionString(connectionString);
                if (nsMgr.QueueExists(queueName))
                    return;

                var definition = new QueueDescription(queueName)
                {
                    MaxSizeInMegabytes = 1024,
                    DefaultMessageTimeToLive = TimeSpan.FromDays(365)
                };

                nsMgr.CreateQueue(definition);
            }
        }


        public BrokeredMessage Receive()
        {
            var brokeredMessage = queueClient.Receive();
            return brokeredMessage != null ? new BrokeredMessageWrapper(brokeredMessage) : null;
        }

        public BrokeredMessage Receive(TimeSpan serverWaitTime)
        {
            var brokeredMessage = queueClient.Receive(serverWaitTime);
            return brokeredMessage != null ? new BrokeredMessageWrapper(brokeredMessage) : null;
        }

        public void Close()
        {
            queueClient.Close();
        }

        public void Reopen()
        {
            if (!queueClient.IsClosed)
                queueClient.Close();

            createClient();
        }
    }
}