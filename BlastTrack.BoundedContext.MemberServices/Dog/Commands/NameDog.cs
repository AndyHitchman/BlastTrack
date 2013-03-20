namespace BlastTrack.BoundedContext.MemberServices.Dog.Commands
{
    public class NameDog : DogCommand
    {
        public NameDog(string earbrand, string givenName) : base(earbrand)
        {
            GivenName = givenName;
        }

        public string GivenName { get; private set; }
    }
}