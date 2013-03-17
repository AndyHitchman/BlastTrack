namespace Honeycomb.Plumbing
{
    using Infrastructure;

    public interface EventEmitter
    {
        void Emit(RaisedEvent @event);
    }
}