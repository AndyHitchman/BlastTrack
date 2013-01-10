namespace BlastTrack.Dogs
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