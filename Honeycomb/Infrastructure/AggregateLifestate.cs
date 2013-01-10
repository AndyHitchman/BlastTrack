namespace Honeycomb.Infrastructure
{
    public enum AggregateLifestate
    {
        Restoring = 0,
        Live,
        Lapsed
    }
}