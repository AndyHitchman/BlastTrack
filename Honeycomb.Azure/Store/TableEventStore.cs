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

    public class TableEventStore : EventStore
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly string domainPrefix;
        private readonly CloudTableClient cloudTableClient;
        private readonly CloudTable eventSteamTable;
        private readonly CloudTable aggregateEventTable;

        public TableEventStore(CloudStorageAccount cloudStorageAccount, string domainPrefix)
        {
            this.cloudStorageAccount = cloudStorageAccount;
            this.domainPrefix = domainPrefix;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTableClient.RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(100), 20);
            eventSteamTable = cloudTableClient.GetTableReference(domainPrefix + "_eventstore");
            eventSteamTable.CreateIfNotExists();
            aggregateEventTable = cloudTableClient.GetTableReference(domainPrefix + "_aggregateevents");
            aggregateEventTable.CreateIfNotExists();
        }

        public Event[] EventsForAggregate(Type aggregateType, object key)
        {
            throw new NotImplementedException();
        }

        public async void RecordEvent(RaisedEvent raisedEvent, IEnumerable<ConsumptionLog> consumptionLogs)
        {           
            var eventEntity = new EventEntity(raisedEvent);
            var insertEvent = TableOperation.Insert(eventEntity);
            var insertedEvent = Task.Factory.FromAsync<TableOperation, TableResult>(eventSteamTable.BeginExecute, eventSteamTable.EndExecute, insertEvent, null);

            var insertedAggregates = new List<Task<TableResult>>();

            foreach (var consumptionLog in consumptionLogs)
            {
                var aggregateEventEntity = new AggregateEventEntity(consumptionLog, raisedEvent);
                var insertAggregateEvent = TableOperation.Insert(aggregateEventEntity);
                insertedAggregates.Add(
                    Task.Factory.FromAsync<TableOperation, TableResult>(aggregateEventTable.BeginExecute, aggregateEventTable.EndExecute, insertAggregateEvent, null));
                }


            var result = await insertedEvent;
        }

        public void UpdateConsumptionOutcome(ConsumptionLog consumptionLog)
        {
            throw new NotImplementedException();
        }
    }
}