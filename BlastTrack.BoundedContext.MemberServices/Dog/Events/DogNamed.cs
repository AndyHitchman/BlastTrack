namespace BlastTrack.BoundedContext.MemberServices.Dog.Events
{
    using System;

    public class DogNamed : DogEvent
    {
        public DogNamed(string earbrand, string givenName, DateTime assignedDate) : base(earbrand)
        {
            GivenName = givenName;
            AssignedDate = assignedDate;
        }

        public string GivenName { get; private set; }
        public DateTime AssignedDate { get; private set; }
    }
}