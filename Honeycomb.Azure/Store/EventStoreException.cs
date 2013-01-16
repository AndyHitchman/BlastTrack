namespace Honeycomb.Azure.Store
{
    using System;
    using System.Runtime.Serialization;

    public class EventStoreException : ApplicationException
    {
        public EventStoreException(string message) : base(message)
        {
        }

        public EventStoreException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EventStoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}