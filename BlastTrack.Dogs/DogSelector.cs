namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class DogSelector :
        SelectKeyForAggregate<Dog, DogCommand>,
        SelectKeyForAggregate<Dog, DogEvent>
    {
        #region SelectKeyForAggregate<Dog,DogCommand> Members

        public object SelectKey(DogCommand message)
        {
            return message.Earbrand;
        }

        #endregion

        #region SelectKeyForAggregate<Dog,DogEvent> Members

        public object SelectKey(DogEvent message)
        {
            return message.Earbrand;
        }

        #endregion
    }
}