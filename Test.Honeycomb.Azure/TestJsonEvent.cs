namespace Test.Honeycomb.Azure
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using NUnit.Framework;
    using Should;
    using global::Honeycomb.Azure;
    using global::Honeycomb.Azure.Bus.Infrastructure;
    using global::Honeycomb.Infrastructure;
    using BrokeredMessage = Microsoft.ServiceBus.Messaging.BrokeredMessage;

    [TestFixture]
    public class TestJsonEvent
    {
        [Test]
        public void it_will_serialise_raised_events_to_a_brokered_message()
        {
            var expected = new RaisedEvent(
                new DummyEvent {Prop = "value"},
                new DateTimeOffset(2013, 01, 14, 21, 46, 22, TimeZoneInfo.Utc.BaseUtcOffset));

            var actual = expected.ConvertToMessage();

            using (var reader = new StreamReader(actual.GetBody()))
            {
                var json = reader.ReadToEnd();

                var match =
                    new Regex(
                        @"\{""Thumbprint"":"".*?"",""Event"":\{""\$type"":""Test[.]Honeycomb[.]Azure[.]DummyEvent,\sTest[.]Honeycomb[.]Azure"",""Prop"":""value""\},""RaisedTimestamp"":""2013-01-14T21:46:22[+]00:00"",""TransactionId"":null\}");
                //Console.WriteLine(match);
                //Console.WriteLine(json);
                match.IsMatch(json).ShouldBeTrue();
            }
        }

        [Test]
        public void it_will_deserialise_a_brokered_msg_to_a_raised_event()
        {
            var body =
                @"{""Thumbprint"":""3BBADAF3-C41E-4697-9552-8E28439D2FDC"",""Event"":{""$type"":""Test.Honeycomb.Azure.DummyEvent, Test.Honeycomb.Azure"",""Prop"":""value""},""RaisedTimestamp"":""2013-01-14T21:46:22+00:00"",""TransactionId"":""txid""}";
            var stream = new PreservedMemoryStream();
            using (var sw = new StreamWriter(stream))
                sw.Write(body);

            var expected = new BrokeredMessageWrapper(new BrokeredMessage(stream, true));

            var actual = expected.ConvertToEvent();

            actual.Event.ShouldBeType<DummyEvent>();
            ((DummyEvent) actual.Event).Prop.ShouldEqual("value");
            actual.RaisedTimestamp.ShouldEqual(new DateTimeOffset(2013, 01, 14, 21, 46, 22, TimeZoneInfo.Utc.BaseUtcOffset));
            actual.TransactionId.ShouldEqual("txid");
        }
    }
}