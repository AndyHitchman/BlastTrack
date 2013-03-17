namespace Honeycomb.Azure.Bus.Infrastructure
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class SubscriptionReceiver : IMessageReceiver
    {
        private readonly string connectionString;
        private readonly string topicPath;
        private readonly string subscriptionName;
        private SubscriptionClient subscriptionClient;
        private static readonly object createLock = new object();

        public SubscriptionReceiver(string connectionString, string topicPath, string subscriptionName, Filter filter)
        {
            this.connectionString = connectionString;
            this.topicPath = topicPath;
            this.subscriptionName = subscriptionName;
            ensureSubscriptionExists(filter);
            createClient();
        }

        private void createClient()
        {
            subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, topicPath, subscriptionName);
        }

        private void ensureSubscriptionExists(Filter filter)
        {
            lock (createLock)
            {
                var nsMgr = NamespaceManager.CreateFromConnectionString(connectionString);
                if (nsMgr.SubscriptionExists(topicPath, subscriptionName))
                    return;

                var definition = new SubscriptionDescription(topicPath, subscriptionName)
                                     {
                                         LockDuration = TimeSpan.FromMinutes(5),
                                         EnableDeadLetteringOnFilterEvaluationExceptions = true,
                                         EnableDeadLetteringOnMessageExpiration = true
                                     };

                nsMgr.CreateSubscription(definition, filter);
            }
        }

        public BrokeredMessage Receive()
        {
            var brokeredMessage = subscriptionClient.Receive();
            return brokeredMessage != null ? new BrokeredMessageWrapper(brokeredMessage) : null;
        }

        public BrokeredMessage Receive(TimeSpan serverWaitTime)
        {
            var brokeredMessage = subscriptionClient.Receive(serverWaitTime);
            return brokeredMessage != null ? new BrokeredMessageWrapper(brokeredMessage) : null;
        }

        public void Close()
        {
            subscriptionClient.Close();
        }

        public void Reopen()
        {
            if (!subscriptionClient.IsClosed)
                subscriptionClient.Close();

            createClient();
        }
    }
}