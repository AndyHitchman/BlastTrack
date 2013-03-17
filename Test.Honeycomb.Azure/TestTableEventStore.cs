namespace Test.Honeycomb.Azure
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using NUnit.Framework;
    using Should;
    using global::Honeycomb.Azure;
    using global::Honeycomb.Azure.Store;
    using global::Honeycomb.Infrastructure;

    [TestFixture]
    public class TestTableEventStore
    {
        [TestFixtureSetUp]
        public void once_up()
        {
            AzureEmulator.StartStorage();
        }

        [Test]
        public async void it_should_insert_the_event_into_the_store()
        {
            deleteEventStoreTable();

            var subject = new TableEventStore(CloudStorageAccount.DevelopmentStorageAccount, "test");

            var expected = new RaisedEvent(
                Guid.NewGuid(),
                new DummyEvent {Prop = "Property"},
                new DateTimeOffset(2013, 01, 17, 12, 06, 39, 967, TimeZoneInfo.Utc.BaseUtcOffset),
                "txid");

            var eventEntity = new EventEntity(expected);
            var insert = TableOperation.Insert(eventEntity);
            var inserted = Task.Factory.FromAsync<TableOperation, TableResult>(subject.EventStoreTable.BeginExecute,
                                                                               subject.EventStoreTable.EndExecute, insert, null);
            await inserted;

            var actual = storedEventEntity(expected);

            actual.ShouldNotBeNull();
            actual.Event.ShouldEqual(JsonEvent.ConvertToJson(expected));
            actual.RaisedTimestamp.ShouldEqual(expected.RaisedTimestamp);
            actual.TransactionId.ShouldEqual(expected.TransactionId);
        }

        [Test]
        public async void it_should_insert_the_consumption_logs_into_the_store()
        {
            deleteConsumptionLogTable();

            var subject = new TableEventStore(CloudStorageAccount.DevelopmentStorageAccount, "test");

            var expectedEvent = new RaisedEvent(
                Guid.NewGuid(),
                new DummyEvent {Prop = "Property"},
                new DateTimeOffset(2013, 01, 17, 12, 06, 39, 967, TimeZoneInfo.Utc.BaseUtcOffset),
                "txid");

            var expectedConsumptionLog = new ConsumptionLog(
                expectedEvent,
                expectedEvent.RaisedTimestamp.AddSeconds(1),
                new AggregateInfo(typeof (DummyAggregate), 123));

            subject.LogConsumption(expectedEvent, expectedConsumptionLog);

            var actual = storedConsumptionLogEntity(expectedConsumptionLog);

            actual.ShouldNotBeNull();
            actual.ConsumedTimestamp.ShouldEqual(expectedConsumptionLog.ConsumedTimestamp);
            actual.ConsumptionException.ShouldEqual(expectedConsumptionLog.ConsumptionException);
            actual.Event.ShouldEqual(JsonEvent.ConvertToJson(expectedEvent));
            actual.EventThumbprint.ShouldEqual(expectedConsumptionLog.EventThumbprint);
        }

        private void deleteEventStoreTable()
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = cloudTableClient.GetTableReference("testEventStore");
            table.DeleteIfExists();
        }

        private void deleteConsumptionLogTable()
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = cloudTableClient.GetTableReference("testConsumptionLog");
            table.DeleteIfExists();
        }

        private ConsumptionLogEntity storedConsumptionLogEntity(ConsumptionLog consumptionLog)
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = cloudTableClient.GetTableReference("testConsumptionLog");
            var retrieve = TableOperation.Retrieve<ConsumptionLogEntity>(
                consumptionLog.AffectedAggregate.Type.FullName + "@" + consumptionLog.AffectedAggregate.Key,
                consumptionLog.ConsumedTimestamp.Ticks.ToString("0000000000000000000"));
            var result = table.Execute(retrieve);
            return result.Result as ConsumptionLogEntity;
        }

        private EventEntity storedEventEntity(RaisedEvent raisedEvent)
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = cloudTableClient.GetTableReference("testEventStore");
            var retrieve = TableOperation.Retrieve<EventEntity>(raisedEvent.Thumbprint.ToString(), raisedEvent.Event.GetType().FullName);
            var result = table.Execute(retrieve);
            return result.Result as EventEntity;
        }
    }
}