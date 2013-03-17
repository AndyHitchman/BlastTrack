namespace Honeycomb.Azure.Store
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;
    using Plumbing;
    using System.Linq;

    public class TableEventStore : EventStore
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly string domainPrefix;
        private readonly CloudTableClient cloudTableClient;
        public readonly CloudTable EventStoreTable;
        private readonly CloudTable logTable;

        public TableEventStore(CloudStorageAccount cloudStorageAccount, string domainPrefix)
        {
            this.cloudStorageAccount = cloudStorageAccount;
            this.domainPrefix = domainPrefix;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTableClient.RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(100), 20);
            EventStoreTable = cloudTableClient.GetTableReference(domainPrefix + "EventStore");
            EventStoreTable.CreateIfNotExists();
            logTable = cloudTableClient.GetTableReference(domainPrefix + "ConsumptionLog");
            logTable.CreateIfNotExists();
        }

        public Event[] EventsForAggregate(Type aggregateType, object key)
        {
            throw new NotImplementedException();
        }

        public async void RecordEvent(RaisedEvent raisedEvent)
        {
            var eventEntity = new EventEntity(raisedEvent);
            var insert = TableOperation.Insert(eventEntity);
            var inserted = Task.Factory.FromAsync<TableOperation, TableResult>(EventStoreTable.BeginExecute, EventStoreTable.EndExecute, insert, null);
            await inserted;
        }

        public async void LogConsumption(RaisedEvent raisedEvent, ConsumptionLog consumptionLog)
        {
            var logEntity = new ConsumptionLogEntity(consumptionLog, raisedEvent);
            var insert = TableOperation.Insert(logEntity);
            var inserted = Task.Factory.FromAsync<TableOperation, TableResult>(logTable.BeginExecute, logTable.EndExecute, insert, null);
            await inserted;
        }
    }
}