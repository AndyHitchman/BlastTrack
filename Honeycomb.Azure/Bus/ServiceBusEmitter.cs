namespace Honeycomb.Azure.Bus
{
    using Honeycomb.Infrastructure;
    using Honeycomb.Plumbing;
    using Infrastructure;

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
