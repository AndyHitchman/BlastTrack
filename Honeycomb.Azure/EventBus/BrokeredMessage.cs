#region License
// Copyright (c) 2011 Greyhound Racing Victoria. All rights reserved.
#endregion
namespace Honeycomb.Azure.EventBus
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// To enable unit testing of brokered messages
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
}