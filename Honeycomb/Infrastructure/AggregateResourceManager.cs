namespace Honeycomb.Infrastructure
{
    using System.Collections.Generic;
    using System.Transactions;

    public class AggregateResourceManager : ISinglePhaseNotification
    {
        public AggregateResourceManager(Transaction transaction)
        {
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
            changes = new List<Event>();
        }

        private readonly List<Event> changes; 

        public void RecordChange(Event @event)
        {
            changes.Add(@event);
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

        private void emitAllRecordedChanges()
        {
            
        }
    }
}