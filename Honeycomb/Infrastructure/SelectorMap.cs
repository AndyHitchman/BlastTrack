namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class SelectorMap
    {
        private readonly Dictionary<Type, SelectAggregate[]> selectorsForMessage = new Dictionary<Type, SelectAggregate[]>();
        private readonly Dictionary<Assembly, Type[]> selectorsForAssembly = new Dictionary<Assembly, Type[]>(); 

        public SelectAggregate[] this[Message message]
        {
            get
            {
                var messageType = message.GetType();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !selectorsForAssembly.ContainsKey(assembly)))
                    selectorsForAssembly[assembly] =
                        assembly
                            .GetTypes()
                            .Where(type => type.IsClass)
                            .Where(possibleSelector => typeof (SelectAggregate).IsAssignableFrom(possibleSelector))
                            .ToArray();

                if (!selectorsForMessage.ContainsKey(messageType))
                {
                    selectorsForMessage[messageType] =
                        selectorsForAssembly
                            .SelectMany(_ => _.Value)
                            .Where(
                                possibleSelectorForMessage =>
                                possibleSelectorForMessage
                                    .GetInterfaces()
                                    .Where(iface => typeof (SelectAggregate).IsAssignableFrom(iface))
                                    .Where(selector => selector != typeof(SelectAggregate))
                                    .Any(selector => messageType.IsAssignableFrom(selector.GetGenericArguments()[1])))
                            .Select(selectorType => (SelectAggregate)Activator.CreateInstance(selectorType))
                            .ToArray();

                }

                return selectorsForMessage[messageType];
            }
        }
    }
}