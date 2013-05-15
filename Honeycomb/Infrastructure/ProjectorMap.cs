namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ProjectorMap
    {
        private readonly Dictionary<Assembly, Type[]> projectorsForAssembly = new Dictionary<Assembly, Type[]>();

        private readonly Dictionary<Tuple<Type,Type>, ProjectorInfo[]> projectorsForEvent =
            new Dictionary<Tuple<Type,Type>, ProjectorInfo[]>();

        public ProjectorInfo[] this[Aggregate aggregate, Event @event]
        {
            get
            {
                var aggregateType = aggregate.GetType();
                var eventType = @event.GetType();
                var key = Tuple.Create(aggregateType, eventType);
                
                if (!projectorsForEvent.ContainsKey(key))
                {
                    foreach (
                        var assembly in
                            AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !projectorsForAssembly.ContainsKey(assembly)))
                    {
                        projectorsForAssembly[assembly] =
                            assembly
                                .GetTypes()
                                .Where(type => type.IsClass)
                                .Where(possibleProjector => typeof (Projector).IsAssignableFrom(possibleProjector))
                                .ToArray();
                    }

                    projectorsForEvent[key] =
                        projectorsForAssembly
                            .SelectMany(_ => _.Value)
                            .Select(
                                possibleProjectorType =>
                                new
                                    {
                                        PossibleProjectorType = possibleProjectorType,
                                        Projectors =
                                            possibleProjectorType
                                                .GetInterfaces()
                                                .Where(iface => typeof (Projector).IsAssignableFrom(iface))
                                                .Where(projector => projector != typeof (Projector))
                                                .Where(projector => projector.GetGenericArguments()[0].IsAssignableFrom(aggregateType))
                                                .Where(projector => projector.GetGenericArguments()[1].IsAssignableFrom(eventType))
                                    })
                            .Where(possibleProjectors => possibleProjectors.Projectors.Any())
                            .SelectMany(
                                possibleProjectors => possibleProjectors.Projectors,
                                (possibleProjectors, projector) =>
                                new ProjectorInfo(
                                    projector.GetGenericArguments()[0],
                                    projector.GetGenericArguments()[1],
                                    (Projector) Activator.CreateInstance(possibleProjectors.PossibleProjectorType)))
                            .ToArray();
                }

                return projectorsForEvent[key];
            }
        }

        public class ProjectorInfo
        {
            public ProjectorInfo(Type aggregateType, Type @eventType, Projector projector)
            {
                AggregateType = aggregateType;
                EventType = eventType;
                Projector = projector;
            }

            public Type AggregateType { get; private set; }
            public Type EventType { get; private set; }
            public Projector Projector { get; private set; }
        }
    }
}