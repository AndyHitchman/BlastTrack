#region License
// Copyright (c) 2011 Greyhound Racing Victoria. All rights reserved.
#endregion
namespace Honeycomb.Azure.EventBus
{
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Expose core message client functions to enable abstraction.
    /// </summary>
    public interface IMessageSender
    {
        void Send(Microsoft.ServiceBus.Messaging.BrokeredMessage message);

//        void SendBatch(IEnumerable<BrokeredMessage> message);
//
//        IAsyncResult BeginSend(BrokeredMessage message, AsyncCallback callback, object state);
//
//        void EndSend(IAsyncResult result);
//
//        IAsyncResult BeginSendBatch(IEnumerable<BrokeredMessage> message, AsyncCallback callback, object state);
//
//        void EndSendBatch(IAsyncResult result);
    }
}