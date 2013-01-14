namespace Test.Honeycomb.Azure
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using global::Honeycomb.Azure;
    using global::Honeycomb.Infrastructure;
    using Should;

    [TestFixture]
    public class TestJsonEvent
    {
        [Test]
        public void it_will_serialise_raised_events_to_a_brokered_message()
        {
            var expected = new RaisedEvent(
                new TestEvent {Prop = "value"},
                "txid",
                new DateTimeOffset(2013, 01, 14, 21, 46, 22, TimeZoneInfo.Utc.BaseUtcOffset));

            var actual = JsonEvent.ConvertEventToMessage(expected);

            using(var reader = new StreamReader(actual.GetBody()))
            {
                var json = reader.ReadToEnd();
                json.ShouldEqual("{\"TransactionId\":\"txid\",\"Event\":{\"$type\":\"Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure\",\"Prop\":\"value\"},\"EventType\":\"Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"RaisedTimestamp\":\"2013-01-14T21:46:22+00:00\"}");
            }
        }
    }
}