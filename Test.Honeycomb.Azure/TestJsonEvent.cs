﻿namespace Test.Honeycomb.Azure
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using global::Honeycomb.Azure;
    using global::Honeycomb.Azure.EventBus;
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

            var actual = expected.ConvertEventToMessage();

            using(var reader = new StreamReader(actual.GetBody()))
            {
                var json = reader.ReadToEnd();
                json.ShouldEqual("{\"TransactionId\":\"txid\",\"Event\":{\"$type\":\"Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure\",\"Prop\":\"value\"},\"EventType\":\"Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"RaisedTimestamp\":\"2013-01-14T21:46:22+00:00\"}");
            }
        }

        [Test]
        public void it_will_deserialise_a_brokered_msg_to_a_raised_event()
        {
            var body = "{\"TransactionId\":\"txid\",\"Event\":{\"$type\":\"Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure\",\"Prop\":\"value\"},\"EventType\":\"Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"RaisedTimestamp\":\"2013-01-14T21:46:22+00:00\"}";
            var stream = new PreservedMemoryStream();
            using(var sw = new StreamWriter(stream))
                sw.Write(body);
            
            var expected = new BrokeredMessageWrapper(new Microsoft.ServiceBus.Messaging.BrokeredMessage(stream, true));
            
            var actual = expected.ConvertMessageToEvent();

            actual.ShouldBeType<RaisedEvent>();
            actual.EventType.ShouldEqual(typeof(TestEvent));
            actual.Event.ShouldBeType<TestEvent>();
            ((TestEvent)actual.Event).Prop.ShouldEqual("value");
            actual.RaisedTimestamp.ShouldEqual(new DateTimeOffset(2013, 01, 14, 21, 46, 22, TimeZoneInfo.Utc.BaseUtcOffset));
            actual.TransactionId.ShouldEqual("txid");
        }
    }
}