namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class DogRequiresVaccinationWithin12Weeks : Event
    {
        private readonly string earbrand;

        public DogRequiresVaccinationWithin12Weeks(string earbrand)
        {
            this.earbrand = earbrand;
        }
    }
}