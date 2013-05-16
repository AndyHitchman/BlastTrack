namespace BlastTrack.Projections.MemberServices.Dog
{
    using BoundedContext.MemberServices.Dog.Events;
    using Honeycomb;
    using Honeycomb.Azure.Projection;
    using Newtonsoft.Json.Linq;

    public class Details : Project<DogRegistered>, Project<DogIsNotVaccinated>, Project<DogNamed>
    {
        private const string resourceTemplate = "dog/details/{0}";
        private readonly BlobViewContainer blobViewContainer;

        public Details(BlobViewContainer blobViewContainer)
        {
            this.blobViewContainer = blobViewContainer;
        }

        public void Project(DogRegistered @event)
        {
            var view = new BlobView(blobViewContainer, resourceTemplate.WithParams(@event.Earbrand));

            //There is no resource before this event. Create.
            var dogDetails = new
                {
                    @event.Earbrand,
                    @event.VaccinationCertificateNumber,
                    IsNamed = false
                };

            view.UpdateContent(JObject.FromObject(dogDetails));
        }

        public void Project(DogIsNotVaccinated @event)
        {
            throw new System.NotImplementedException();
        }

        public void Project(DogNamed @event)
        {
            throw new System.NotImplementedException();
        }
    }
}