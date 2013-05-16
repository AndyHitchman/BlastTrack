namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ProjectorMap
    {
        private readonly Dictionary<Assembly, Type[]> projectorsForAssembly = new Dictionary<Assembly, Type[]>();

        private readonly Dictionary<Type, ProjectorInfo[]> projectorsForEvent =
            new Dictionary<Type, ProjectorInfo[]>();

        public ProjectorInfo[] this[Event @event]
        {
            get
            {
                var eventType = @event.GetType();
                
                if (!projectorsForEvent.ContainsKey(eventType))
                {
                    foreach (
                        var assembly in
                            AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !projectorsForAssembly.ContainsKey(assembly)))
                    {
                        projectorsForAssembly[assembly] =
                            assembly
                                .GetTypes()
                                .Where(type => type.IsClass)
                                .Where(possibleProjector => typeof (Project).IsAssignableFrom(possibleProjector))
                                .ToArray();
                    }

                    projectorsForEvent[eventType] =
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
                                                .Where(iface => typeof (Project).IsAssignableFrom(iface))
                                                .Where(projector => projector != typeof (Project))
                                                .Where(projector => projector.GetGenericArguments()[0].IsAssignableFrom(eventType))
                                    })
                            .Where(possibleProjectors => possibleProjectors.Projectors.Any())
                            .SelectMany(
                                possibleProjectors => possibleProjectors.Projectors,
                                (possibleProjectors, projector) =>
                                new ProjectorInfo(
                                    projector.GetGenericArguments()[0],
                                    (Project) Activator.CreateInstance(possibleProjectors.PossibleProjectorType)))
                            .ToArray();
                }

                return projectorsForEvent[eventType];
            }
        }

        public class ProjectorInfo
        {
            public ProjectorInfo(Type @eventType, Project projector)
            {
                EventType = eventType;
                Projector = projector;
            }

            public Type EventType { get; private set; }
            public Project Projector { get; private set; }
        }
    }
}