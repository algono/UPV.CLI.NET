using System.Diagnostics;
using UPV.CLI.Connectors.Drive.Errors;
using UPV.CLI.Connectors.Helpers;
using static UPV.CLI.Connectors.Helpers.CmdHelper;

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

        public static DriveError? CheckDriveConnectionErrors(DriveProcess process, ProcessResult result)
        {
            if (result.Succeeded)
            {
                return null;
            }

            // 55 - Error del sistema "El recurso no se encuentra disponible" (es decir, la dirección no es valida).
            if (result.OutputOrErrorContains(InvalidAddressError.ErrorCode))
            {
                return new InvalidAddressError(process.Address, result.Error);
            }

            /*
            * 86 - Error del sistema "La contraseña de red es incorrecta"
            * 1326 - Error del sistema "El usuario o la contraseña son incorrectos"
            * Cuando las credenciales son erróneas, da uno de estos dos errores de forma arbitraria.
            * 
            * Suponemos que en el primer caso el error fue de la contraseña y en el segundo del usuario,
            * pero no lo sabemos con seguridad.
            */

            if (result.OutputOrErrorContains(IncorrectPasswordError.ErrorCode))
            {
                return new IncorrectPasswordError(result.Error);
            }

            if (result.OutputOrErrorContains(IncorrectUserNameOrPasswordError.ErrorCode))
            {
                return new IncorrectUserNameOrPasswordError(result.Error);
            }

            if (result.OutputOrErrorContains(NotAvailableDriveError.ErrorCode))
            {
                return new NotAvailableDriveError(process.Letter, result.Error);
            }

            // Only found so far trying to connect to the W Drive.
            // It requires the user to re-establish their connection to UPV (either VPN or Wi-Fi).
            if (result.OutputOrErrorContains(CredentialsBugError.ErrorCode))
            {
                return new CredentialsBugError(result.Error);
            }

            // Unknown error, we throw a generic error.
            return new UnknownDriveError(result.Error);
        }

        public static DriveError? CheckDriveDisconnectionErrors(string? driveLetter, ProcessResult result)
        {
            if (result.Succeeded)
            {
                return null;
            }

            //Esa secuencia es parte de "(S/N)", con lo que deducimos que nos pide confirmación (porque tenemos archivos abiertos)
            if (result.OutputOrErrorContains("/N)"))
            {
                return new OpenedFilesError(driveLetter, result.Error);
            }

            // Unknown error, we throw a generic error.
            return new UnknownDriveError(result.Error);
        }
    }
}
