﻿namespace Honeycomb.Azure
{
    using System.IO;
    using Bus.Infrastructure;
    using Infrastructure;
    using Newtonsoft.Json;

    public static class JsonEvent
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings =
            new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                };

        private static readonly JsonSerializer serialiser = JsonSerializer.Create(jsonSerializerSettings);

        private const string propEventType = "EventType";
        private const string propTransactionId = "TransactionId";

        public static RaisedEvent ConvertToEvent(string json)
        {
            return JsonConvert.DeserializeObject<RaisedEvent>(json, jsonSerializerSettings);
        }

        public static string ConvertToJson(RaisedEvent @event)
        {
            return JsonConvert.SerializeObject(@event, jsonSerializerSettings);
        }

        public static RaisedEvent ConvertToEvent(this BrokeredMessage message)
        {
            var stream = message.GetBody();
            using (var reader = new StreamReader(stream))
            {
                return (RaisedEvent) serialiser.Deserialize(reader, typeof (SerialisableRaisedEvent));
            }
        }

        public static BrokeredMessage ConvertToMessage(this RaisedEvent @event)
        {
            var stream = new PreservedMemoryStream();
            using (var streamWriter = new StreamWriter(stream))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                serialiser.Serialize(writer, @event);
                writer.Flush();
            }

            //We hand ownership of the stream to the brokered message. It is responsible for cleanly disposing of the resource.
            var msg = new BrokeredMessageWrapper(new Microsoft.ServiceBus.Messaging.BrokeredMessage(stream, true));
            msg.Properties[propEventType] = @event.Event.GetType();
            msg.Properties[propTransactionId] = @event.TransactionId;
            return msg;
        }
    }
}