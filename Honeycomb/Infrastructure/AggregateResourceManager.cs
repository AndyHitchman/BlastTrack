namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using Plumbing;

    public class AggregateResourceManager : ISinglePhaseNotification
    {
        private readonly EventEmitter eventEmitter;
        private readonly List<UniqueEvent> changes;
        private readonly Dictionary<Guid, UniqueEvent> changesByReceipt;
 
        public AggregateResourceManager(EventEmitter eventEmitter, Transaction transaction)
        {
            this.eventEmitter = eventEmitter;
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
            changes = new List<UniqueEvent>();
            changesByReceipt = new Dictionary<Guid, UniqueEvent>();
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

        public Guid RecordEvent(Event @event, IEnumerable<AggregateInfo> applyToAggregates)
        {
            var ue = new UniqueEvent(@event, DateTimeOffset.UtcNow, applyToAggregates);
            changes.Add(ue);
            changesByReceipt.Add(ue.Receipt, ue);
            return ue.Receipt;
        }

        public void RecordConsumptionFailure(Guid receipt, AggregateInfo aggregateInfo, Exception exception)
        {
            changesByReceipt[receipt].RecordExceptionForConsumer(aggregateInfo, exception);
        }

        public void RecordConsumptionComplete(Guid receipt, AggregateInfo aggregateInfo)
        {
            changesByReceipt[receipt].RecordConsumptionComplete(aggregateInfo);
        }

        public IEnumerable<UniqueEvent> RecordedEvents
        {
            get { return changes; }
        }

        private void emitAllRecordedChanges()
        {
            foreach (var @event in changes)
            {
                eventEmitter.Emit(@event.EventType, @event.UntypedEvent);
            }
        }
    }
}