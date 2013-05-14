namespace Honeycomb.Infrastructure
{
    using System.Runtime.CompilerServices;
    using System.Transactions;
    using Plumbing;

    public class TransactionTracker
    {
        private readonly EventEmitter emitter;

        /// <summary>
        /// A dictionary with a weakly referenced key.
        /// </summary>
        /// <remarks>
        /// See http://stackoverflow.com/questions/10225727/should-conditionalweaktabletkey-tvalue-be-used-for-non-compiler-purposes
        /// </remarks>
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