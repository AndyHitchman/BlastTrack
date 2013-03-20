namespace BlastTrack.BoundedContext.MemberServices.Dog.Commands
{
    using Honeycomb;

    public abstract class DogCommand : Command
    {
        protected DogCommand(string earbrand)
        {
            Earbrand = earbrand;
        }

        public string Earbrand { get; private set; }
    }
}