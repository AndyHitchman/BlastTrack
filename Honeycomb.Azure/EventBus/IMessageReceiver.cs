namespace Honeycomb.Azure.EventBus
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