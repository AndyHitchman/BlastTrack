namespace Honeycomb.Infrastructure
{
    using System.Runtime.CompilerServices;
    using System.Transactions;
    using Plumbing;

    public class TransactionTracker
    {
        private readonly EventEmitter emitter;

        private readonly ConditionalWeakTable<Transaction, AggregateResourceManager> resourceManagersForTransactions =
            new ConditionalWeakTable<Transaction, AggregateResourceManager>();

        public TransactionTracker(EventEmitter emitter)
        {
            this.emitter = emitter;
        }

        public AggregateResourceManager this[Transaction transaction]
        {
            get { return resourceManagersForTransactions.GetValue(transaction, key => new AggregateResourceManager(emitter, key)); }
        }
    }
}