namespace BlastTrack.Dogs
{
    using Honeycomb;

    public class Dog : Aggregate, 
        Consumes<DogRegistered>,
        Consumes<DogNamed>
    {
        private string earbrand;
        private string name;

        
        public Dog(RegisterDog cmd)
        {
            //Check no other dog exists with this earbrand

            this.Raise(new DogRegistered(cmd.Earbrand, cmd.VaccinationCertificateNumber));
        }

        public Dog(DogRegistered @event)
        {
            earbrand = @event.Earbrand;
        }

        public void Accept(NameDog cmd)
        {
            if (name != null) throw new RefusedCommandException("The dog is already named");

            this.Raise(new DogNamed(earbrand, cmd.GivenName));
        }

        public void Receive(DogRegistered @event)
        {
            earbrand = @event.Earbrand;
            if(@event.VaccinationCertificateNumber == null)
                this.Raise(new DogRequiresVaccinationWithin12Weeks(earbrand));
        }

        public void Receive(DogNamed @event)
        {
            name = @event.GivenName;
        }
    }
}