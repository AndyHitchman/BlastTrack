namespace Test.Honeycomb.Azure
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using NUnit.Framework;
    using global::Honeycomb.Azure;
    using global::Honeycomb.Azure.EventBus.Infrastructure;
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
                new DateTimeOffset(2013, 01, 14, 21, 46, 22, TimeZoneInfo.Utc.BaseUtcOffset));

            var actual = expected.ConvertEventToMessage();

            using(var reader = new StreamReader(actual.GetBody()))
            {
                var json = reader.ReadToEnd();

                var match =
                    new Regex(
                        @"\{""Thumbprint"":"".*?"",""Event"":\{""\$type"":""Test[.]Honeycomb[.]Azure[.]TestEvent,\sTest[.]Honeycomb[.]Azure"",""Prop"":""value""\},""EventType"":""Test[.]Honeycomb[.]Azure[.]TestEvent,\sTest[.]Honeycomb[.]Azure,\sVersion=1[.]0[.]0[.]0,\sCulture=neutral,\sPublicKeyToken=null"",""RaisedTimestamp"":""2013-01-14T21:46:22[+]00:00"",""TransactionId"":null\}");
                //Console.WriteLine(match);
                //Console.WriteLine(json);
                match.IsMatch(json).ShouldBeTrue();
            }
        }

        [Test]
        public void it_will_deserialise_a_brokered_msg_to_a_raised_event()
        {
            var body = @"{""Thumbprint"":""3BBADAF3-C41E-4697-9552-8E28439D2FDC"",""Event"":{""$type"":""Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure"",""Prop"":""value""},""EventType"":""Test.Honeycomb.Azure.TestEvent, Test.Honeycomb.Azure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",""RaisedTimestamp"":""2013-01-14T21:46:22+00:00"",""TransactionId"":""txid""}";
            var stream = new PreservedMemoryStream();
            using(var sw = new StreamWriter(stream))
                sw.Write(body);
            
            var expected = new BrokeredMessageWrapper(new Microsoft.ServiceBus.Messaging.BrokeredMessage(stream, true));
            
            var actual = expected.ConvertMessageToEvent();

            actual.EventType.ShouldEqual(typeof(TestEvent));
            actual.Event.ShouldBeType<TestEvent>();
            ((TestEvent)actual.Event).Prop.ShouldEqual("value");
            actual.RaisedTimestamp.ShouldEqual(new DateTimeOffset(2013, 01, 14, 21, 46, 22, TimeZoneInfo.Utc.BaseUtcOffset));
            actual.TransactionId.ShouldEqual("txid");
        }
    }
}