using Cocona;

namespace UPV.CLI
{
    // Drive Commands
    public class DriveCommands
    {
        [Command("connect")]
        public void Connect([Argument] string path, [Option] string? username = null)
        {
            Console.WriteLine($"Connecting to drive: {path}");
            if (username != null)
                Console.WriteLine($"Username: {username}");
        }

        [Command("disconnect")]
        public void Disconnect([Argument] string path)
        {
            Console.WriteLine($"Disconnecting drive: {path}");
        }
    }
}