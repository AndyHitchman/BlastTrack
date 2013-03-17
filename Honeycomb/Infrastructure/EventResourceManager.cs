namespace Honeycomb.Infrastructure
{
    using System.Collections.Generic;
    using System.Transactions;
    using Plumbing;

    public class EventResourceManager : ISinglePhaseNotification
    {
        private readonly EventEmitter eventEmitter;
        private readonly List<RaisedEvent> changes;

        public EventResourceManager(EventEmitter eventEmitter, Transaction transaction)
        {
            this.eventEmitter = eventEmitter;
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
            changes = new List<RaisedEvent>();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Commit(Enlistment enlistment)
        {
            emitAllRecordedChanges();
            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            changes.Clear();
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            emitAllRecordedChanges();
            singlePhaseEnlistment.Committed();
        }

        public void RecordEvent(RaisedEvent @event)
        {
            changes.Add(@event);
        }

        public IEnumerable<RaisedEvent> RecordedEvents
        {
            get { return changes; }
        }

        private void emitAllRecordedChanges()
        {
            foreach (var @event in changes)
            {
                eventEmitter.Emit(@event);
            }
        }
    }
}