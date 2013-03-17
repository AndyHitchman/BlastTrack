namespace Honeycomb.Azure.Bus
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.ServiceBus.Messaging;
    using Plumbing;
    using BrokeredMessage = Infrastructure.BrokeredMessage;

    public class ServiceBusCollector : EventCollector
    {
        private readonly TimeSpan receiveWaitTime;
        private readonly CancellationTokenSource receiverCancellationTokenSource;
        private readonly IMessageReceiver receiver;
        private readonly ManualResetEventSlim stopped;

        public ServiceBusCollector(IMessageReceiver messageReceiver)
        {
            receiverCancellationTokenSource = new CancellationTokenSource();
            receiveWaitTime = TimeSpan.FromMinutes(1);
            receiver = messageReceiver;
            stopped = new ManualResetEventSlim(false);
        }

        public static EventReceiverFactory DefaultReceiverFactory(string sbConnectionString, string topicPath, string subscriberName)
        {
            return filter => new SubscriptionReceiver(sbConnectionString, topicPath, subscriberName, filter);
        }

        private static void reopenReceiver(IMessageReceiver receiver, Exception exception, CancellationToken cancellationToken)
        {
            Trace.TraceWarning("Restarting event receiver which died with exception:\n{0}", exception);

            while (true)
            {
                try
                {
                    //Allow escape on stop.
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    receiver.Reopen();
                    break; // Reopen loop.
                }
                catch (Exception reopenEx)
                {
                    Trace.TraceWarning("Attempt to restarting event receiver failed with exception:\n{0}", reopenEx);
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
            }
        }

        public void StartCollectingEvents(Domain propogationDomain)
        {
            Task.Factory.StartNew(
                () =>
                    {
                        while (true)
                        {
                            BrokeredMessage message;

                            try
                            {
                                message = receiver.Receive(receiveWaitTime);
                                if (message == null)
                                {
                                    Trace.TraceInformation("No message waiting");
                                    continue;
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                Trace.TraceInformation("Stopping receiver");
                                stopped.Set();
                                return;
                            }
                            catch (Exception exception)
                            {
                                if (exception is UnauthorizedAccessException |
                                    exception is CommunicationException |
                                    exception is MessagingException |
                                    exception is TimeoutException |
                                    exception is MessagingCommunicationException |
                                    exception is ServerBusyException)
                                {
                                    reopenReceiver(receiver, exception, receiverCancellationTokenSource.Token);
                                    continue; // Receiver loop.
                                }

                                Trace.TraceError("Stopping event receiver which died with exception:\n{0}", exception);
                                stopped.Set();
                                throw;
                            }

                            var raisedEvent = message.ConvertToEvent();
                            propogationDomain.Consume(raisedEvent);
                            message.Complete();
                        }
                    },
                TaskCreationOptions.LongRunning);
        }

        public void StopCollecting(Action onStopped)
        {
            receiverCancellationTokenSource.Cancel();
            receiver.Close();
            stopped.Wait();
            onStopped();
        }
    }
}