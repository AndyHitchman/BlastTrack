namespace Honeycomb.Azure.EventBus
{
    using Infrastructure;
    using Plumbing;

    public class ServiceBusEmitter : EventEmitter
    {
        private readonly MessageSender messageSender;

        public ServiceBusEmitter(MessageSender messageSender)
        {
            this.messageSender = messageSender;
        }

        public void Emit(RaisedEvent @event)
        {
            var msg = JsonEvent.ConvertEventToMessage(@event);
            messageSender.Send(msg);
        }
    }
}
