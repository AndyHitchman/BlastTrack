namespace Honeycomb.Plumbing
{
    using System;
    using Infrastructure;

    public interface EventEmitter
    {
        void Emit(Type eventType, Event untypedEvent);
    }
}