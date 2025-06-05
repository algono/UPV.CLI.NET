using Cocona;
using UPV.CLI.Connectors;
using UPV.CLI.Connectors.VPN;

namespace UPV.CLI
{
    // VPN Commands
    public class VpnCommands
    {
        [Command("create")]
        public void Create([Argument] string name)
        {
            Console.WriteLine($"Adding VPN connection: {name}");
            var process = VPNHelper.Create(name);

            CmdHelper.WaitCheckAndOutput(process,
                $"Successfully added VPN connection: {name}",
                $"Failed to add VPN connection: {name}\nError: {{0}}",
                $"Failed to start creation process for VPN connection: {name}");
        }

        [Command("delete")]
        public void Delete([Argument] string name)
        {
            if (!CommandsHelper.GetYesNoConfirmation($"Are you sure you want to delete the VPN connection '{name}'? This action cannot be undone."))
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
                $"Successfully connected to VPN: {name}",
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