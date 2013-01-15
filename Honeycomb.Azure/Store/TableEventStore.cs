namespace Honeycomb.Azure.Store
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;
    using Plumbing;

    public class TableEventStore : EventStore
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly string domainPrefix;
        private readonly CloudTableClient cloudTableClient;

        public TableEventStore(CloudStorageAccount cloudStorageAccount, string domainPrefix)
        {
            this.cloudStorageAccount = cloudStorageAccount;
            this.domainPrefix = domainPrefix;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTableClient.RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(100), 20);
            cloudTableClient.GetTableReference(domainPrefix + "_eventstore");
        }

        public Event[] EventsForAggregate(Type aggregateType, object key)
        {
            throw new NotImplementedException();
        }

        public void RecordEvent(RaisedEvent raisedEvent, IEnumerable<ConsumptionLog> consumptionLogs)
        {
            throw new NotImplementedException();
        }

        public void UpdateConsumptionOutcome(ConsumptionLog consumptionLog)
        {
            throw new NotImplementedException();
        }
    }
}