namespace BlastTrack.Projections.MemberServices.Dog
{
    using BoundedContext.MemberServices.Dog.Events;
    using Honeycomb;
    using Honeycomb.Azure.Projection;
    using Newtonsoft.Json.Linq;

    public class Details : Project<DogRegistered>
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
    }
}