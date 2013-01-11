namespace Honeycomb.Infrastructure
{
    public enum AggregateLifestate
    {
        Untracked,
        Replaying,
        Live,
        Lapsed
    }
}