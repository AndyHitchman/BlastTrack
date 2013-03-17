namespace Honeycomb.Azure.Bus
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
            var msg = @event.ConvertToMessage();
            messageSender.Send(msg);
        }
    }
}