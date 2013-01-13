namespace Honeycomb.Infrastructure
{
    using System.Runtime.CompilerServices;
    using System.Transactions;

    public class TransactionTracker
    {
        private readonly ConditionalWeakTable<Transaction, AggregateResourceManager> resourceManagersForTransactions =
            new ConditionalWeakTable<Transaction, AggregateResourceManager>();

        public AggregateResourceManager this[Transaction transaction]
        {
            get { return resourceManagersForTransactions.GetValue(transaction, key => new AggregateResourceManager(key)); }
        }
    }
}