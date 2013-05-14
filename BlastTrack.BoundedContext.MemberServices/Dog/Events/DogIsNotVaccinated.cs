namespace BlastTrack.BoundedContext.MemberServices.Dog.Events
{
    using Honeycomb;

    public class DogIsNotVaccinated : Event
    {
        public DogIsNotVaccinated(string earbrand)
        {
            Earbrand = earbrand;
        }

        public string Earbrand { get; private set; }
    }
}