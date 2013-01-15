namespace Honeycomb.Infrastructure
{
    using System.Runtime.CompilerServices;
    using System.Transactions;
    using Plumbing;

    public class TransactionTracker
    {
        private readonly EventEmitter emitter;

        private readonly ConditionalWeakTable<Transaction, EventResourceManager> resourceManagersForTransactions =
            new ConditionalWeakTable<Transaction, EventResourceManager>();

        public TransactionTracker(EventEmitter emitter)
        {
            this.emitter = emitter;
        }

        public EventResourceManager this[Transaction transaction]
        {
            get { return resourceManagersForTransactions.GetValue(transaction, key => new EventResourceManager(emitter, key)); }
        }
    }
}