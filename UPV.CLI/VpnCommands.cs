using Cocona;
using UPV.CLI.Connectors.Helpers;
using UPV.CLI.Connectors.VPN;

namespace UPV.CLI
{
    // VPN Commands
    public class VpnCommands
    {
        [Command("create")]
        public void Create([Argument] string name, [Option(Description = "Automatically try to connect to the VPN after creating it")] bool connect = false)
        {
            Console.WriteLine($"Creating VPN connection: {name}");
            var process = VPNHelper.Create(name);

            var success = CmdHelper.WaitCheckAndOutput(process,
                $"Successfully created VPN connection: {name}",
                $"Failed to add VPN connection: {name}\nError: {{0}}",
                $"Failed to start creation process for VPN connection: {name}");

            if (success && connect)
            {
                Connect(name); // Automatically connect after creation (if the connect option is specified)
            }
        }

        [Command("delete")]
        public void Delete([Argument] string name, [Option(Description = "Delete without asking for confirmation")] bool force = false)
        {
            if (!(force || CommandsHelper.GetYesNoConfirmation($"Are you sure you want to delete the VPN connection '{name}'? This action cannot be undone.")))
            {
                Console.WriteLine("Deletion cancelled.");
                return;
            }

            Console.WriteLine($"Deleting VPN connection: {name}");
            var process = VPNHelper.Delete(name);
            
            CmdHelper.WaitCheckAndOutput(process,
                $"Successfully deleted VPN connection: {name}",
                $"Failed to delete VPN connection: {name}\nError: {{0}}",
                $"Failed to start deletion process for VPN connection: {name}");
        }

        [Command("connect")]
        public void Connect([Argument] string name)
        {
            Console.WriteLine($"Connecting to VPN: {name}");

            var process = VPNHelper.Connect(name);

            CmdHelper.WaitCheckAndOutput(process,
                $"Successfully opened connection dialog for VPN: {name}.\nCheck your VPN settings to see if the connection was successful or not.\nAlternatively, you can use the \"{typeof(VpnCommands).Assembly.GetName().Name} vpn check\" command to see if your current IP is in the UPV network.",
                $"Failed to connect to VPN: {name}\nError: {{0}}",
                $"Failed to start connection process for VPN: {name}");
        }

        [Command("disconnect")]
        public void Disconnect([Argument] string name)
        {
            Console.WriteLine($"Disconnecting from VPN: {name}");

            var process = VPNHelper.Disconnect(name);

            CmdHelper.WaitCheckAndOutput(process,
                $"Successfully disconnected from VPN: {name}",
                $"Failed to disconnect from VPN: {name}\nError: {{0}}",
                $"Failed to start disconnection process for VPN: {name}");
        }

        [Command("check")]
        public void Check()
        {
            //Console.WriteLine($"Checking if your IP is in the UPV network...");
            var isConnected = VPNHelper.IsConnected();
            string isInUPVNetwork = isConnected ? "Yes" : "No";
            Console.WriteLine($"In UPV Network: {isInUPVNetwork}");
        }

        [Command("list")]
        public void List()
        {
            Console.WriteLine("Available VPN connections:");
            foreach (var vpnName in VPNHelper.List())
            {
                Console.WriteLine($"- {vpnName}");
            }
        }
    }
}