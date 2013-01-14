namespace Honeycomb.Plumbing
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;

    public class InProcessEventBus : EventEmitter, EventCollector
    {
        private readonly ConcurrentQueue<RaisedEvent> buffer;
        private readonly AutoResetEvent nudge;
        private readonly ManualResetEventSlim stoppedSignal;
        private readonly CancellationTokenSource cancellationTokenSource;

        public InProcessEventBus()
        {
            buffer = new ConcurrentQueue<RaisedEvent>();
            nudge = new AutoResetEvent(false);
            stoppedSignal = new ManualResetEventSlim(false);
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void Emit(RaisedEvent @event)
        {
            buffer.Enqueue(@event);
            nudge.Set();
        }

        public void StartCollectingEvents(Domain propogationDomain)
        {
            Task.Factory.StartNew(
                () =>
                    {
                        while (true)
                        {
                            RaisedEvent @event;
                            if (buffer.TryDequeue(out @event))
                            {
                                propogationDomain.Consume(@event);
                            }
                            else
                            {
                                //We empty the queue before stopping as this queue is in-memory only.
                                if (cancellationTokenSource.Token.IsCancellationRequested)
                                    return;

                                nudge.WaitOne();
                            }
                        }
                    })
                .ContinueWith(task => stoppedSignal.Set());
        }

        public void StopCollecting(Action stopped)
        {
            cancellationTokenSource.Cancel();
            stoppedSignal.Wait();
            stopped();
        }
    }
}