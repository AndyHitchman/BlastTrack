#region License

// Copyright (c) 2011 Greyhound Racing Victoria. All rights reserved.

#endregion

namespace Honeycomb.Azure.Bus.Infrastructure
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///   To enable unit testing of brokered messages
    /// </summary>
    public interface BrokeredMessage
    {
        string MessageId { get; }
        IDictionary<string, object> Properties { get; }
        void Complete();
        void Abandon();
        Stream GetBody();
        T GetBody<T>();
    }

    public interface InternalBrokeredMessage : BrokeredMessage
    {
        Microsoft.ServiceBus.Messaging.BrokeredMessage Real { get; }
    }
}