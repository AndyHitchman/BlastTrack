namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using Plumbing;

    public class AggregateResourceManager : ISinglePhaseNotification
    {
        private readonly EventEmitter eventEmitter;
        private readonly string transactionId;
        private readonly List<RaisedEvent> changes;
 
        public AggregateResourceManager(EventEmitter eventEmitter, Transaction transaction)
        {
            this.eventEmitter = eventEmitter;
            transactionId = transaction.TransactionInformation.LocalIdentifier;
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

        public void RecordEvent(Event @event)
        {
            var ue = new RaisedEvent(@event, transactionId, DateTimeOffset.UtcNow);
            changes.Add(ue);
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