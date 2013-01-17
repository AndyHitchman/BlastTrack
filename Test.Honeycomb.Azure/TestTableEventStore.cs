namespace Test.Honeycomb.Azure
{
    using System;
    using System.Diagnostics;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;
    using NUnit.Framework;
    using global::Honeycomb;
    using global::Honeycomb.Azure;
    using global::Honeycomb.Azure.Store;
    using global::Honeycomb.Infrastructure;
    using Should;

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

            await subject.InsertEvent(expected);

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

            var expectedConsumptionLog1 = new ConsumptionLog(
                expectedEvent,
                expectedEvent.RaisedTimestamp.AddSeconds(1),
                new AggregateInfo(typeof(DummyAggregate), 123));

            var expectedConsumptionLog2 = new ConsumptionLog(
                expectedEvent,
                expectedEvent.RaisedTimestamp.AddSeconds(2),
                new AggregateInfo(typeof(DummyAggregate), 456));

            await subject.InsertConsumptionLogs(expectedEvent, new[] {expectedConsumptionLog1, expectedConsumptionLog2});

            var actual1 = storedConsumptionLogEntity(expectedConsumptionLog1);
            var actual2 = storedConsumptionLogEntity(expectedConsumptionLog2);

            actual1.ShouldNotBeNull();
            actual1.ConsumedTimestamp.ShouldEqual(expectedConsumptionLog1.ConsumedTimestamp);
            actual1.ConsumptionException.ShouldEqual(expectedConsumptionLog1.ConsumptionException);
            actual1.Event.ShouldEqual(JsonEvent.ConvertToJson(expectedEvent));
            actual1.EventThumbprint.ShouldEqual(expectedConsumptionLog1.EventThumbprint);

            actual2.ShouldNotBeNull();
            actual2.ConsumedTimestamp.ShouldEqual(expectedConsumptionLog2.ConsumedTimestamp);
            actual2.ConsumptionException.ShouldEqual(expectedConsumptionLog2.ConsumptionException);
            actual2.Event.ShouldEqual(JsonEvent.ConvertToJson(expectedEvent));
            actual2.EventThumbprint.ShouldEqual(expectedConsumptionLog2.EventThumbprint);

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