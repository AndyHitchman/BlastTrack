namespace BlastTrack.BoundedContext.MemberServices.Dog.Events
{
    using Honeycomb;

    public class DogRequiresVaccinationWithin12Weeks : Event
    {
        public DogRequiresVaccinationWithin12Weeks(string earbrand)
        {
            Earbrand = earbrand;
        }

        public string Earbrand { get; private set; }
    }
}