namespace BlastTrack.Dogs
{
    using Honeycomb;
    using Honeycomb.Infrastructure;

    public class DogSelector :
        SelectAggregate<Dog, RegisterDog>,
        SelectAggregate<Dog, DogRegistered>,
        SelectAggregate<Dog, DogCommand>,
        SelectAggregate<Dog, DogEvent>
    {
        private readonly AggregateFactory<Dog, string> aggregateFactory;

        public DogSelector(AggregateFactory<Dog, string> aggregateFactory)
        {
            this.aggregateFactory = aggregateFactory;
        }

        public Dog Select(RegisterDog message)
        {
            return new Dog(message);
        }

        public Dog Select(DogRegistered message)
        {
            return new Dog(message);
        }

        public Dog Select(DogCommand message)
        {
            return aggregateFactory.Restore(message.Earbrand);
        }

        public Dog Select(DogEvent message)
        {
            return aggregateFactory.Restore(message.Earbrand);
        }
    }
}