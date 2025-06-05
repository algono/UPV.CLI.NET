namespace UPV.CLI.Connectors.Drive
{
    public static class DriveExceptions
    {
        public abstract class DriveException(string message) : Exception(message) { }

        [Serializable]
        public class NotAvailableDriveException : DriveException
        {
            public string DriveLetter => DriveLetterTools.ToDriveLetter(Letter);
            public char Letter { get; }

            public string DriveMessage => GetDriveMessage(DriveLetter);

            public const string NoDriveMessage = "No existe ninguna unidad disponible. Libere alguna unidad y vuelva a intentarlo.";

            public NotAvailableDriveException() : base(NoDriveMessage)
            {

            }

            public NotAvailableDriveException(char letter) : base(GetDriveMessage(DriveLetterTools.ToDriveLetter(letter)))
            {
                Letter = letter;
            }

            private static string GetDriveMessage(string letter)
                => $"La unidad definida para el disco ({letter}) ya contiene un disco asociado.\n\n"
                + "Antes de continuar, desconecte el disco asociado, o cambie la unidad utilizada para el disco.";
        }

        [Serializable]
        public class OpenedFilesException : DriveException
        {
            public const string WarningTitle = "Archivos abiertos";

            public const string WarningMessage =
                "Existen archivos abiertos y/o búsquedas incompletas de directorios pendientes en el disco. Si no los cierra antes de desconectarse, podría perder datos.\n\n"
                + "¿Desea continuar la desconexión y forzar el cierre?";

            public Action Continue { get; }

            public OpenedFilesException(Action continueMethod) : base(WarningMessage)
            {
                Continue = continueMethod;
            }
        }

        /**
        * <summary>
        * 1223 - Error del sistema "El usuario o la contraseña son incorrectos".
        * Esto es un bug porque debería obtener las credenciales de la conexión con la UPV
        * (ya sea VPN o WiFi).
        * Por ahora el bug sólo se ha visto desde red WiFi, y la solución es reconectarse.
        * </summary>
        */
        [Serializable]
        public class CredentialsBugException : DriveException
        {
            public const string ERROR_BUG = "There was a credentials error.\n\n"
                + "You must re-establish your connection to UPV manually (either VPN or Wi-Fi).";

            //public const string ERROR_BUG = "Ha habido un error de credenciales.\n\n"
            //    + "Debe reestablecer su conexión a la UPV manualmente (VPN o WiFi).";

            public CredentialsBugException() : base(ERROR_BUG) { }
        }
    }
}
