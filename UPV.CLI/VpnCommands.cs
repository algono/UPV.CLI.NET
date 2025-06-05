using Cocona;
using UPV.CLI.Connectors;
using UPV.CLI.Connectors.VPN;

// VPN Commands
public class VpnCommands
{
    [Command("create")]
    public void Create([Argument] string name)
    {
        Console.WriteLine($"Adding VPN connection: {name}");
        VPNHelper.Create(name);
    }

    [Command("connect")]
    public void Connect([Argument] string name)
    {
        Console.WriteLine($"Connecting to VPN: {name}");

        var process = VPNHelper.Connect(name);

        if (process != null)
        {
            var result = CmdHelper.WaitAndCheck(process);
            if (result.Succeeded)
            {
                Console.WriteLine($"Successfully connected to VPN: {name}");
            }
            else
            {
                Console.WriteLine($"Failed to connect to VPN: {name}\nError: {result.Error}");
            }
        }
        else
        {
            Console.WriteLine($"Failed to start connection process for VPN: {name}");
        }
    }

    [Command("disconnect")]
    public void Disconnect([Argument] string name)
    {
        Console.WriteLine($"Disconnecting from VPN: {name}");

        var process = VPNHelper.Disconnect(name);

        if (process != null)
        {
            var result = CmdHelper.WaitAndCheck(process);
            if (result.Succeeded)
            {
                Console.WriteLine($"Successfully disconnected from VPN: {name}");
            }
            else
            {
                Console.WriteLine($"Failed to disconnect from VPN: {name}\nError: {result.Error}");
            }
        }
        else
        {
            Console.WriteLine($"Failed to start disconnection process for VPN: {name}");
        }
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
