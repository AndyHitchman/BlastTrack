namespace BlastTrack.Dogs
{
    using Honeycomb;

    public abstract class DogEvent : Event
    {
        protected DogEvent(string earbrand)
        {
            Earbrand = earbrand;
        }

        public string Earbrand { get; private set; }
    }
}