namespace Honeycomb.Azure.EventBus
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Infrastructure;
    using Microsoft.ServiceBus.Messaging;
    using Plumbing;

    public class ServiceBusEmitter : EventEmitter
    {
        private readonly IMessageSender messageSender;

        public ServiceBusEmitter(IMessageSender messageSender)
        {
            this.messageSender = messageSender;
        }

        public void Emit(RaisedEvent @event)
        {
            var msg = JsonEvent.ConvertEventToMessage(@event);

            Recover.RetryTransaction(
                e =>
                    e is CommunicationObjectFaultedException |
                    e is CommunicationObjectAbortedException |
                    e is MessagingCommunicationException |
                    e is TimeoutException |
                    e is ServerBusyException,
                () => messageSender.Send(capturedMessage));
        }
    }
}
