namespace Honeycomb
{
    public interface Projector {}

    public interface Projector<in TAggregate, in TEvent> : Projector where TAggregate : Aggregate where TEvent : Event
    {
        /// <summary>
        /// Project from an aggregate that has consumed an event.
        /// </summary>
        /// <param name="aggregate"></param>
        /// <param name="event"></param>
        void Project(TAggregate aggregate, TEvent @event);
    }
}