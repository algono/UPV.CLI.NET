using UPV.CLI.Connectors.Helpers;
using UPV.CLI.Connectors.VPN;
using static UPV.CLI.Connectors.Drive.DriveExceptions;

namespace UPV.CLI.Connectors.Drive
{
    public static class DriveFactory
    {
        #region W Drive
        public static readonly IDictionary<UPVDomain, DriveDomain> UPVDomains = new Dictionary<UPVDomain, DriveDomain>()
        {
            { UPVDomain.ALUMNO, new DriveDomain("ALUMNO", DomainStyle.BackSlashStyle, "alumnos") },
            { UPVDomain.UPVNET, new DriveDomain("UPVNET", DomainStyle.BackSlashStyle, "discos") }
        };

        public static NetworkDrive GetDriveW(string user, UPVDomain domain, char drive = default)
        {
            bool connectedViaEthernet = VPNHelper.GetValidInterfaces().FirstOrDefault()?.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet;

            NetworkDrive driveW = new(GetAddressW, user, UPVDomains[domain], drive)
            {
                Name = "W Drive",
                DefaultLetter = 'W',
                NeedsUsername = true,
                ExplicitUserArgument = connectedViaEthernet,
                NeedsPassword = connectedViaEthernet
            };

            return driveW;
        }

        public static void CheckWDriveConnectionProcess(ProcessResult e)
        {
            if (!e.Succeeded && e.OutputOrErrorContains("1223"))
            {
                throw new CredentialsBugException();
            }
        }

        public const string WAddress = "nasupv.upv.es";

        private static string GetAddressW(string username, DriveDomain domain) => $@"\\{WAddress}\{domain.Folder}\{username[0]}\{username}";
        #endregion
    }
}
