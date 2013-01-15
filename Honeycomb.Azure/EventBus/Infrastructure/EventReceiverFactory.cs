namespace Honeycomb.Azure.EventBus.Infrastructure
{
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// We receive events by subscribing at different priorities to the domain event topic
    /// </summary>
    /// <returns></returns>
    public delegate IMessageReceiver EventReceiverFactory(SqlFilter sqlFilter);
}