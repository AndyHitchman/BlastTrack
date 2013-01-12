namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class DogEvent : Event
    {
        public DogEvent(string earbrand)
        {
            Earbrand = earbrand;
        }

        public string Earbrand { get; private set; }
    }
}