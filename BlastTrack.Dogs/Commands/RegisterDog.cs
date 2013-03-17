namespace BlastTrack.Dogs.Commands
{
    public class RegisterDog : DogCommand
    {
        public RegisterDog(string earbrand, string microchip, string vaccinationCertificateNumber) : base(earbrand)
        {
            Microchip = microchip;
            VaccinationCertificateNumber = vaccinationCertificateNumber;
        }

        public string Microchip { get; private set; }

        public string VaccinationCertificateNumber { get; private set; }
    }
}