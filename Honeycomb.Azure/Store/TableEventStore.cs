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

            var insertedAggregates = new Dictionary<Task<TableResult>, AggregateEventEntity>();

            foreach (var consumptionLog in consumptionLogs)
            {
                var aggregateEventEntity = new AggregateEventEntity(consumptionLog, raisedEvent);
                var insertAggregateEvent = TableOperation.Insert(aggregateEventEntity);
                insertedAggregates.Add(
                    Task.Factory.FromAsync<TableOperation, TableResult>(aggregateEventTable.BeginExecute, aggregateEventTable.EndExecute, insertAggregateEvent, null),
                    aggregateEventEntity);
            }

            var storeResult = await insertedEvent;
            if (storeResult.HttpStatusCode != 201)
            {
                throw new EventStoreException(string.Format("Could not insert event {0}/{1} into Azure table store",
                                                            eventEntity.PartitionKey, eventEntity.RowKey));
            }

            var aggregateResults = await Task.WhenAll(insertedAggregates.Keys);
            var aggregateResult = aggregateResults.FirstOrDefault(_ => _.HttpStatusCode != 201);
            if (aggregateResult != null)
            {
                var failedEntity = insertedAggregates.Single(_ => _.Key.Result == aggregateResult).Value;
                throw new EventStoreException(string.Format("Could not insert aggregate consumption log {0}/{1} into Azure table store",
                                                            failedEntity.PartitionKey, failedEntity.RowKey));
            }
        }

        public void UpdateConsumptionOutcome(ConsumptionLog consumptionLog)
        {
            throw new NotImplementedException();
        }
    }
}