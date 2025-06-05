using Cocona;
using UPV.CLI.Connectors;
using UPV.CLI.Connectors.Drive;
using static UPV.CLI.Connectors.Drive.DriveExceptions;

namespace UPV.CLI
{
    // Drive Commands
    public class DriveCommands
    {
        [Command("connect")]
        public void Connect([Argument] string user, [Argument] UPVDomain domain)
        {
            Console.WriteLine($"Connecting to W drive with username {user} and domain {domain}");

            try
            {
                var drive = DriveFactory.GetDriveW(user: user, domain: domain);
                var process = drive.Connect();
                var result = CmdHelper.WaitAndCheck(process);
                drive.OnProcessConnected(result);

                Console.WriteLine($"Successfully connected to {drive.Name} at {drive.ConnectedDriveLetter}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while connecting to the drive:\n{ex.Message}");
            }
        }

        [Command("disconnect")]
        public void Disconnect([Argument] string driveLetter = "W:", [Option] bool force = false)
        {
            Console.WriteLine($"Disconnecting drive at: {driveLetter}");

            try
            {
                var process = DriveConnectionHelper.Disconnect(driveLetter, force);
                var result = CmdHelper.WaitAndCheck(process);
                DriveConnectionHelper.OnProcessDisconnected(driveLetter, result);
            }
            catch (OpenedFilesException)
            {
                Console.Error.WriteLine($"Cannot disconnect drive {driveLetter} because there are opened files. Run this again with the --force option to disconnect the drive anyways, accepting that information could be lost.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while disconnecting from the drive:\n{ex.Message}");
            }
        }
    }
}