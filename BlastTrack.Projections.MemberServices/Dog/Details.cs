namespace BlastTrack.Projections.MemberServices.Dog
{
    using BoundedContext.MemberServices.Dog.Events;
    using Honeycomb;
    using Newtonsoft.Json.Linq;

    public class Details : Project<DogRegistered>
    {
        public void Project(DogRegistered @event)
        {
            //There is not resource before this event. Create

            var dogDetails = new
                {
                    @event.Earbrand,
                    @event.VaccinationCertificateNumber,
                    IsNamed = false
                };

            JObject.FromObject(dogDetails);

            //TODO
        }
    }
}