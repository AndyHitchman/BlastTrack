namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class SelectorMap
    {
        private readonly Dictionary<Assembly, Type[]> selectorsForAssembly = new Dictionary<Assembly, Type[]>();

        private readonly Dictionary<Type, SelectorInfo[]> selectorsForMessage =
            new Dictionary<Type, SelectorInfo[]>();

        public SelectorInfo[] this[Message message]
        {
            get
            {
                var messageType = message.GetType();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !selectorsForAssembly.ContainsKey(assembly)))
                {
                    selectorsForAssembly[assembly] =
                        assembly
                            .GetTypes()
                            .Where(type => type.IsClass)
                            .Where(possibleSelector => typeof (SelectKeyForAggregate).IsAssignableFrom(possibleSelector))
                            .ToArray();
                }

                if (!selectorsForMessage.ContainsKey(messageType))
                {
                    selectorsForMessage[messageType] =
                        selectorsForAssembly
                            .SelectMany(_ => _.Value)
                            .Select(
                                possibleSelectorType =>
                                new
                                    {
                                        PossibleSelectorType = possibleSelectorType,
                                        Selectors =
                                            possibleSelectorType
                                                .GetInterfaces()
                                                .Where(iface => typeof (SelectKeyForAggregate).IsAssignableFrom(iface))
                                                .Where(selector => selector != typeof (SelectKeyForAggregate))
                                                .Where(selector => selector.GetGenericArguments()[1].IsAssignableFrom(messageType))
                                    })
                            .Where(possibleSelectors => possibleSelectors.Selectors.Any())
                            .SelectMany(
                                possibleSelectors => possibleSelectors.Selectors,
                                (possibleSelectors, selector) =>
                                new SelectorInfo(
                                    selector.GetGenericArguments()[0],
                                    (SelectKeyForAggregate)
                                    Activator.CreateInstance(possibleSelectors.PossibleSelectorType)))
                            .ToArray();
                }

                return selectorsForMessage[messageType];
            }
        }

        public class SelectorInfo
        {
            public SelectorInfo(Type aggregateType, SelectKeyForAggregate selector)
            {
                AggregateType = aggregateType;
                Selector = selector;
            }

            public Type AggregateType { get; private set; }
            public SelectKeyForAggregate Selector { get; private set; }
        }
    }
}