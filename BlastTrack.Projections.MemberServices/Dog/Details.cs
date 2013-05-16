namespace BlastTrack.Projections.MemberServices.Dog
{
    using BoundedContext.MemberServices.Dog.Events;
    using Honeycomb;
    using Honeycomb.Azure.Projection;

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
            //There is no resource before this event. Create.
            dynamic dogDetails = new
                {
                    @event.Earbrand,
                    @event.VaccinationCertificateNumber,
                    IsNamed = false
                };

            view(@event).UpdateContent(dogDetails);
        }

        public void Project(DogIsNotVaccinated @event)
        {
            var view = this.view(@event);
            var content = view.GetContent();

            content.IsVaccinated = false;
            content.VaccinatedRequiredByDate = @event.RequiredVaccinationDate;
        }

        public void Project(DogNamed @event)
        {
            var view = this.view(@event);
            var content = view.GetContent();


        }

        private BlobView view(DogEvent @event)
        {
            return new BlobView(blobViewContainer, resourceTemplate.WithParams(@event.Earbrand));
        }
    }
}