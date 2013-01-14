namespace Honeycomb.Azure
{
    using System;
    using System.IO;
    using EventBus;
    using Infrastructure;
    using Newtonsoft.Json;

    public static class JsonEvent
    {
        private static readonly JsonSerializer serialiser = JsonSerializer.Create(new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});
        private const string propEventType = "EventType";
        private const string propTransactionId = "TransactionId";

        public static RaisedEvent ConvertMessageToEvent(BrokeredMessage message)
        {
            var stream = message.GetBody();
            using (var reader = new StreamReader(stream))
            {
                return (RaisedEvent)serialiser.Deserialize(reader, typeof (RaisedEvent));
            }
        }

        public static BrokeredMessage ConvertEventToMessage(RaisedEvent @event)
        {
            var stream = new PreservedMemoryStream();
            using (var streamWriter = new StreamWriter(stream))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                serialiser.Serialize(writer, @event);
                writer.Flush();
                stream.Position = 0;
            }

            var msg = new BrokeredMessageWrapper(new Microsoft.ServiceBus.Messaging.BrokeredMessage(stream, true));
            msg.Properties[propEventType] = @event.EventType;
            msg.Properties[propTransactionId] = @event.TransactionId;
            return msg;
        }
    }
}