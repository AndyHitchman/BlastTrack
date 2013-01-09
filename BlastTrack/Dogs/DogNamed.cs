namespace BlastTrack.Dogs
{
    public class DogNamed : DogEvent
    {
        public DogNamed(string earbrand, string givenName) : base(earbrand)
        {
            GivenName = givenName;
        }

        public string GivenName { get; private set; }
    }
}