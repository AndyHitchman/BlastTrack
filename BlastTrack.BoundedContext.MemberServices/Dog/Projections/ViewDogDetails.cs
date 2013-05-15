namespace BlastTrack.BoundedContext.MemberServices.Dog.Projections
{
    using Honeycomb;

    public class ViewDogDetails : Projector<Dog, Event>
    {
        public void Project(Dog aggregate, Event @event)
        {
            //Update view for this dog
        }
    }
}