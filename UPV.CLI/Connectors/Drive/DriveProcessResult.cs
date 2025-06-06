using System.Diagnostics;

namespace UPV.CLI.Connectors.Drive
{
    public class DriveProcess
    {
        public Process Process { get; }
        public string Address { get; }
        public char Letter { get; }

        public DriveProcess(Process process, string address, char letter)
        {
            Process = process;
            Address = address;
            Letter = letter;
        }
    }
}
