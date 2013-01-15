namespace Honeycomb.Azure.EventBus
{
    using Honeycomb.Infrastructure;
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
            var msg = @event.ConvertEventToMessage();
            messageSender.Send(msg);
        }
    }
}
