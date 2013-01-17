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
        private readonly CloudTable eventStoreTable;
        private readonly CloudTable logTable;

        public TableEventStore(CloudStorageAccount cloudStorageAccount, string domainPrefix)
        {
            this.cloudStorageAccount = cloudStorageAccount;
            this.domainPrefix = domainPrefix;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTableClient.RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(100), 20);
            eventStoreTable = cloudTableClient.GetTableReference(domainPrefix + "EventStore");
            eventStoreTable.CreateIfNotExists();
            logTable = cloudTableClient.GetTableReference(domainPrefix + "ConsumptionLog");
            logTable.CreateIfNotExists();
        }

        public Event[] EventsForAggregate(Type aggregateType, object key)
        {
            throw new NotImplementedException();
        }

        public async void RecordEvent(RaisedEvent raisedEvent, IEnumerable<ConsumptionLog> consumptionLogs)
        {
            await Task.WhenAll(
                InsertEvent(raisedEvent),
                InsertConsumptionLogs(raisedEvent, consumptionLogs));
        }

        public async Task InsertEvent(RaisedEvent raisedEvent)
        {
            var eventEntity = new EventEntity(raisedEvent);
            var insert = TableOperation.Insert(eventEntity);
            var inserted = Task.Factory.FromAsync<TableOperation, TableResult>(eventStoreTable.BeginExecute, eventStoreTable.EndExecute, insert, null);
            await inserted;
        }

        public async Task InsertConsumptionLogs(RaisedEvent raisedEvent, IEnumerable<ConsumptionLog> consumptionLogs)
        {
            var inserted =
                consumptionLogs.Select(log => new ConsumptionLogEntity(log, raisedEvent))
                               .Select(logEntity => TableOperation.Insert(logEntity))
                               .Select(insert => Task.Factory.FromAsync<TableOperation, TableResult>(logTable.BeginExecute, logTable.EndExecute, insert, null));

            await Task.WhenAll(inserted);
        }

        public void UpdateConsumptionOutcome(ConsumptionLog consumptionLog)
        {
            throw new NotImplementedException();
        }
    }
}