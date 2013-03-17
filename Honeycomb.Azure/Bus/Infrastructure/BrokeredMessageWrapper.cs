namespace Honeycomb.Azure.Bus.Infrastructure
{
    using System.Collections.Generic;
    using System.IO;

    public class BrokeredMessageWrapper : InternalBrokeredMessage
    {
        private readonly Microsoft.ServiceBus.Messaging.BrokeredMessage _;

        public BrokeredMessageWrapper(Microsoft.ServiceBus.Messaging.BrokeredMessage brokeredMessage)
        {
            _ = brokeredMessage;
        }

        public void Complete()
        {
            _.Complete();
        }

        public void Abandon()
        {
            _.Abandon();
        }

        public string MessageId
        {
            get { return _.MessageId; }
        }

        public IDictionary<string, object> Properties
        {
            get { return _.Properties; }
        }

        public Stream GetBody()
        {
            return _.GetBody<Stream>();
        }

        public T GetBody<T>()
        {
            return _.GetBody<T>();
        }

        public Microsoft.ServiceBus.Messaging.BrokeredMessage Real
        {
            get { return _; }
        }
    }
}