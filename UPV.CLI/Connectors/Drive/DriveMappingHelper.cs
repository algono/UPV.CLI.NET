using System.Diagnostics;

namespace UPV.CLI.Connectors.Drive
{
    /// <summary>
    /// Mapped Drives Static Methods
    /// </summary>
    public static class DriveMappingHelper
    {
        public static IEnumerable<char> SelectAvailable(IEnumerable<char> drives)
        {
            HashSet<char> mappedDrives = GetMappedDrives().ToHashSet();
            return drives.Where((drive) => IsAvailable(drive, mappedDrives));
        }

        public static bool IsAvailable(char drive) => IsAvailable(drive, GetMappedDrives());

        private static bool IsAvailable(char drive, IEnumerable<char> mappedDrives)
            => !mappedDrives.Contains(drive)
               && DriveLetterTools.IsAvailable(drive);

        public static IEnumerable<char> GetMappedDrives()
        {
            ProcessStartInfo info = CmdHelper.CreateProcessInfo("net.exe");
            info.Arguments = "use";
            Process process = Process.Start(info) ?? throw new InvalidOperationException("Error trying to call net.exe");
            string output = process.StandardOutput.ReadToEnd();
            string[] splits = output.Split(':');
            for (int i = 0; i < splits.Length - 1; i++)
            {
                string split = splits[i];
                yield return split[^1];
            }
        }
    }
}
