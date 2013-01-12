namespace Honeycomb.Infrastructure
{
    using System.Collections.Generic;
    using System.Transactions;

    public class AggregateResourceManager : ISinglePhaseNotification
    {
        private readonly List<Event> changes;

        public AggregateResourceManager(Transaction transaction)
        {
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
            changes = new List<Event>();
        }

        #region ISinglePhaseNotification Members

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
            //TODO Throw away

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

        #endregion

        public void RecordChange(Event @event)
        {
            changes.Add(@event);
        }

        private void emitAllRecordedChanges()
        {
        }
    }
}