namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;

    public class AggregateResourceManager : ISinglePhaseNotification
    {
        private readonly List<UniqueEvent> changes;

        public AggregateResourceManager(Transaction transaction)
        {
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
            changes = new List<UniqueEvent>();
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

        public void RecordEvent(Event @event, List<AggregateInfo> applyToAggregates)
        {
            var ue = new UniqueEvent(Guid.NewGuid(), @event, DateTimeOffset.UtcNow, applyToAggregates);
            changes.Add(ue);
        }

        public IEnumerable<UniqueEvent> RecordedEvents
        {
            get { return changes; }
        }

        private void emitAllRecordedChanges()
        {
        }
    }
}