namespace BlastTrack.BoundedContext.MemberServices.Dog.Projections
{
    using Honeycomb;

    public class UpdateIndex : Projector<Dog, Event>
    {
        public void Project(Dog aggregate, Event @event)
        {
            //Update Lucene
        }
    }
}