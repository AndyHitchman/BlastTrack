namespace BlastTrack.Dogs
{
    public class DogRegistered : DogEvent
    {
        public DogRegistered(string earbrand) : base(earbrand)
        {
        }
    }
}