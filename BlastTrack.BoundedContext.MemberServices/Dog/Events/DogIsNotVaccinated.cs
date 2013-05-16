namespace BlastTrack.BoundedContext.MemberServices.Dog.Events
{
    using System;
    using Honeycomb;

    public class DogIsNotVaccinated : DogEvent
    {
        public DogIsNotVaccinated(string earbrand, DateTime requiredVaccinationDate) : base(earbrand)
        {
            RequiredVaccinationDate = requiredVaccinationDate;
        }

        public DateTime RequiredVaccinationDate { get; private set; }
    }
}