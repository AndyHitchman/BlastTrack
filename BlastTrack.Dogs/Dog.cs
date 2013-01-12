namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class Dog : Aggregate
    {
        private readonly string earbrand;
        private string name;


        public Dog(RegisterDog cmd)
        {
            //TODO Check no other dog exists with this earbrand

            this.Raise(new DogRegistered(cmd.Earbrand, cmd.VaccinationCertificateNumber));
        }

        public Dog(DogRegistered @event)
        {
            earbrand = @event.Earbrand;
            if (@event.VaccinationCertificateNumber == null)
                this.Raise(new DogRequiresVaccinationWithin12Weeks(earbrand));
        }

        public void Accept(NameDog cmd)
        {
            if (name != null) throw new RefusedCommandException("The dog is already named");

            this.Raise(new DogNamed(earbrand, cmd.GivenName));
        }

        public void Receive(DogNamed @event)
        {
            name = @event.GivenName;
        }

        public void Receive(DogRequiresVaccinationWithin12Weeks @event)
        {
        }
    }
}