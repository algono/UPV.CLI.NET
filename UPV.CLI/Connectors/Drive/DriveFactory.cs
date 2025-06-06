using UPV.CLI.Connectors.VPN;

namespace UPV.CLI.Connectors.Drive
{
    public static class DriveFactory
    {
        #region W Drive
        public const char WDriveDefaultLetter = 'W';

        public static readonly IDictionary<UPVDomain, DriveDomain> UPVDomains = new Dictionary<UPVDomain, DriveDomain>()
        {
            { UPVDomain.ALUMNO, new DriveDomain("ALUMNO", DomainStyle.BackSlashStyle, "alumnos") },
            { UPVDomain.UPVNET, new DriveDomain("UPVNET", DomainStyle.BackSlashStyle, "discos") }
        };

        public static NetworkDrive GetDriveW(string user, UPVDomain domain, char letter = default)
        {
            bool connectedViaEthernet = VPNHelper.GetValidInterfaces().FirstOrDefault()?.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet;

            NetworkDrive driveW = new(GetAddressW, user, UPVDomains[domain], letter)
            {
                Name = "UPV Drive", // Disco W
                DefaultLetter = WDriveDefaultLetter,
                NeedsUsername = true,
                ExplicitUserArgument = connectedViaEthernet,
                NeedsPassword = connectedViaEthernet
            };

            return driveW;
        }

        public const string WAddress = "nasupv.upv.es";

        private static string GetAddressW(string username, DriveDomain domain) => $@"\\{WAddress}\{domain.Folder}\{username[0]}\{username}";
        #endregion
    }
}
