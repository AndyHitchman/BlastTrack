namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using ReflectionMagic;

    public static class AggregateFactory
    {
        public static void Buildup(AggregateInfo aggregateInfo, ICollection<Event> replayEvents)
        {
            var creationEvent = replayEvents.First();
            var changeEvents = replayEvents.Skip(1);

            var aggregate = blank(aggregateInfo.Type);

            aggregateInfo.Instance = aggregate;
            aggregateInfo.Lifestate = AggregateLifestate.Building;

            construct(aggregateInfo, creationEvent);
            replay(changeEvents, aggregate);

            aggregateInfo.Lifestate = AggregateLifestate.Live;
        }

        public static void Create(AggregateInfo aggregateInfo, Event @event)
        {
            var aggregate = blank(aggregateInfo.Type);

            aggregateInfo.Instance = aggregate;
            aggregateInfo.Lifestate = AggregateLifestate.Live;

            construct(aggregateInfo, @event);
        }

        public static void Create(AggregateInfo aggregateInfo, Command command)
        {
            var aggregate = blank(aggregateInfo.Type);

            aggregateInfo.Instance = aggregate;

            construct(aggregateInfo, command);
        }

        private static Aggregate blank(Type aggregateType)
        {
            return (Aggregate) FormatterServices.GetUninitializedObject(aggregateType);
        }

        private static void construct(AggregateInfo aggregateInfo, Message creationMessage)
        {
            var creationConstructor = aggregateInfo.Type.GetConstructor(new[] {creationMessage.GetType()});
            if (creationConstructor == null)
                throw new MissingMethodException(aggregateInfo.Type.FullName, "constructor");

            creationConstructor.Invoke(aggregateInfo.Instance, new object[] {creationMessage});
        }

        private static void replay(IEnumerable<Event> changeEvents, Aggregate aggregate)
        {
            foreach (var @event in changeEvents)
                try
                {
                    aggregate.AsDynamic().Receive(@event);
                }
                catch (ApplicationException e)
                {
                    if (e.Source == "ReflectionMagic")
                        throw new MissingMethodException(aggregate.GetType().FullName,
                                                         "Receive(" + @event.GetType() + ")");

                    throw;
                }
        }
    }
}