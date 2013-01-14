namespace Honeycomb.Azure
{
    using System;
    using System.IO;
    using EventBus;
    using Infrastructure;
    using Newtonsoft.Json;

    public static class JsonEvent
    {
        public static RaisedEvent ConvertMessageToEvent(BrokeredMessage message)
        {
            var stream = message.GetBody();
            using (var reader = new StreamReader(stream))
            {
                var deserialiser = JsonSerializer.Create(new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});
                return (RaisedEvent)deserialiser.Deserialize(reader, typeof (RaisedEvent));
            }
        }

        public static BrokeredMessage ConvertEventToMessage(RaisedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}