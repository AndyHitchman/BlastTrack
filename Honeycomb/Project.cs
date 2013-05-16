namespace Honeycomb
{
    public interface Project {}

    public interface Project<in TEvent> : Project where TEvent : Event
    {
        /// <summary>
        /// Project from an event.
        /// </summary>
        /// <param name="event"></param>
        void Project(TEvent @event);
    }
}