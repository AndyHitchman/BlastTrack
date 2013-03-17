namespace Test.Honeycomb.Azure
{
    using System.Diagnostics;
    using System.IO;

    public class AzureEmulator
    {
        private const string AzureEmulatorPath = @"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator";

        public static void StartStorage()
        {
            var start = new ProcessStartInfo
                            {
                                Arguments = "/devstore:start",
                                FileName = Path.Combine(AzureEmulatorPath, "csrun.exe")
                            };

            var proc = new Process {StartInfo = start};
            proc.Start();
            proc.WaitForExit();
        }
    }
}