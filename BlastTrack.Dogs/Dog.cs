namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class Dog : Aggregate, 
        Consumes<DogRegistered>,
        Consumes<DogNamed>
    {
        public class Selector :
            SelectAggregate<Dog, RegisterDog>,
            SelectAggregate<Dog, DogRegistered>,
            SelectAggregate<Dog, DogCommand>,
            SelectAggregate<Dog, DogEvent>
        {
            private readonly AggregateFactory<Dog, string> aggregateFactory;

            public Selector(AggregateFactory<Dog, string> aggregateFactory)
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
                return aggregateFactory.Buildup(message.Earbrand);
            }

            public Dog Select(DogEvent message)
            {
                return aggregateFactory.Buildup(message.Earbrand);
            }
        }

        private string earbrand;
        private string name;

        
        public Dog(RegisterDog cmd)
        {
            //Check no other dog exists with this earbrand

            this.Raise(new DogRegistered(cmd.Earbrand));
        }

        public Dog(DogRegistered @event)
        {
            earbrand = @event.Earbrand;
        }

        public void Accept(NameDog cmd)
        {
            if (name != null) throw new RefusedCommandException("The dog is already named");

            this.Raise(new DogNamed(earbrand, cmd.GivenName));
        }

        public void Receive(DogRegistered @event)
        {
            earbrand = @event.Earbrand;
        }

        public void Receive(DogNamed @event)
        {
            name = @event.GivenName;
        }
    }
}