namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class DogSelector :
        SelectKeyForAggregate<Dog, DogCommand>,
        SelectKeyForAggregate<Dog, DogEvent>
    {
        public object SelectKey(DogCommand message)
        {
            return message.Earbrand;
        }


        public object SelectKey(DogEvent message)
        {
            return message.Earbrand;
        }
    }
}