namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class DogSelector :
        SelectAggregate<Dog, RegisterDog>,
        SelectAggregate<Dog, DogRegistered>,
        SelectAggregate<Dog, DogCommand>,
        SelectAggregate<Dog, DogEvent>
    {
        public DogSelector()
        {
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
            return Repository.Select<Dog>(message.Earbrand);
        }

        public Dog Select(DogEvent message)
        {
            return Repository.Select<Dog>(message.Earbrand);
        }
    }
}