namespace Honeycomb
{
    public interface Consumes<TEvent> where TEvent : Event
    {
        void Receive(TEvent @event);
    }
}