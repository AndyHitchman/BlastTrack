namespace BlastTrack.BoundedContext.MemberServices.Dog.Events
{
    public class DogRegistered : DogEvent
    {
        public DogRegistered(string earbrand, string vaccinationCertificateNumber) : base(earbrand)
        {
            VaccinationCertificateNumber = vaccinationCertificateNumber;
        }

        public string VaccinationCertificateNumber { get; private set; }
    }
}