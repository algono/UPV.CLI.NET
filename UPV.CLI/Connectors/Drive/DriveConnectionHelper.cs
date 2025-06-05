using System.Diagnostics;
using static UPV.CLI.Connectors.CmdHelper;
using static UPV.CLI.Connectors.Drive.DriveExceptions;

namespace UPV.CLI.Connectors.Drive
{
    public static class DriveConnectionHelper
    {
        public static ProcessStartInfo CreateNetProcessInfo() => CreateProcessInfo("net.exe");

        public static Process Disconnect(string driveLetter, bool force)
        {
            var netInfo = CreateNetProcessInfo();
            netInfo.Arguments = $"use {driveLetter} /delete";
            if (force) netInfo.Arguments += " /y";
            return StartProcess(netInfo);
        }

        public static void OnProcessDisconnected(string driveLetter, ProcessEventArgs e)
        {
            if (!e.Succeeded)
                //Esa secuencia es parte de "(S/N)", con lo que deducimos que nos pide confirmación (porque tenemos archivos abiertos)
                if (e.OutputOrErrorContains("/N)"))
                    throw new OpenedFilesException(() => Disconnect(driveLetter, force: true));
        }
    }
}
