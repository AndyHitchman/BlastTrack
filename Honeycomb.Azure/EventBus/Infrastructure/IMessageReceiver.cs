namespace Honeycomb.Azure.EventBus.Infrastructure
{
    using System;

    public interface IMessageReceiver
    {
        BrokeredMessage Receive();

        BrokeredMessage Receive(TimeSpan serverWaitTime);

        void Close();

        void Reopen();
    }
}