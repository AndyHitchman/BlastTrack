namespace BlastTrack.Dogs
{
    public class RegisterDog : DogCommand
    {
        public RegisterDog(string earbrand, string microchip) : base(earbrand)
        {
            Microchip = microchip;
        }

        public string Microchip { get; private set; }
    }
}