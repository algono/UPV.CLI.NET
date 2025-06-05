using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace UPV.CLI.Connectors.VPN
{
    public static class VPNHelper
    {
        public const string UPVVpnServer = "vpn.upv.es", UPVWebsite = "www.upv.es", UPVIPPrefix = "158.42.";

        private static void AddUPVCreationParameters(PowerShellCommandBuilder pb)
        {
            pb.AddParameter("AuthenticationMethod", "Eap");
            pb.AddParameter("EncryptionLevel", "Required");
            pb.AddParameter("TunnelType", "Sstp");

            // https://docs.microsoft.com/es-es/windows/client-management/mdm/eap-configuration
            pb.AddXmlParameter("EapConfigXmlStream", GetEapConfigXmlStreamFromEmbeddedResource());
        }

        private static string GetEapConfigXmlStreamFromEmbeddedResource()
        {
            using var stream = typeof(VPNHelper).Assembly.GetManifestResourceStream($"{nameof(UPV)}.{nameof(CLI)}.Resources.UPV_Config.xml")
                ?? throw new InvalidOperationException("Embedded resource UPV_Config.xml not found.");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static ProcessStartInfo GetConnectionInfo(string arguments = "") => CmdHelper.CreateProcessInfo("rasphone.exe", arguments);
        private static ProcessStartInfo GetDisconnectionInfo(string arguments = "") => CmdHelper.CreateProcessInfo("rasdial.exe", arguments);

        public static bool Create(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            return VPNPowerShell.Create(name, UPVVpnServer, AddUPVCreationParameters);
        }

        public static Process? Connect(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            var connectionInfo = GetConnectionInfo($"-d \"{name}\"");
            return Process.Start(connectionInfo);
        }

        public static Process? Disconnect(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            var disconnectionInfo = GetDisconnectionInfo($"\"{name}\" /DISCONNECT");
            return Process.Start(disconnectionInfo);
        }

        public static IEnumerable<string> List()
        {
            return VPNPowerShell.FindNames(UPVVpnServer);
        }

        // Check if the VPN is working by checking if the current IP address starts with UPV's prefix
        public static bool IsConnected() => GetConnectedIPAddresses().Any(ip => ip.StartsWith(UPVIPPrefix));

        private static IEnumerable<NetworkInterface> GetConnectedNetworkInterfaces() => NetworkInterface.GetAllNetworkInterfaces().Where(ni => ni.OperationalStatus == OperationalStatus.Up);

        private static IEnumerable<string> GetConnectedIPAddresses() => GetConnectedNetworkInterfaces()
            .Select(ni =>
                ni.GetIPProperties().UnicastAddresses
                    .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(ip => ip.Address.ToString()))
            .SelectMany(i => i); // Flatten
    }
}
