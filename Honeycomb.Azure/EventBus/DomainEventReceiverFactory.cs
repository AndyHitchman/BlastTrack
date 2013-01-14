namespace Honeycomb.Azure.EventBus
{
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// We receive events by subscribing at different priorities to the domain event topic
    /// </summary>
    /// <returns></returns>
    public delegate IMessageReceiver DomainEventReceiverFactory(SqlFilter sqlFilter);
}