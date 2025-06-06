namespace UPV.CLI.Connectors.Drive.Errors
{
    [Serializable]
    public class OpenedFilesError : DriveError
    {
        /// <summary>
        /// This always returns null because this error does not have a specific error code.
        /// </summary>
        public override string? Code => null; // No specific error code for this error

        public string? DriveLetter { get; }

        //public const string WarningTitle = "Archivos abiertos";

        //public const string WarningMessage =
        //    "Existen archivos abiertos y/o búsquedas incompletas de directorios pendientes en el disco. Si no los cierra antes de desconectarse, podría perder datos.\n\n"
        //    + "¿Desea continuar la desconexión y forzar el cierre?";

        public const string DefaultMessageFormat = "There are open files and/or pending directory searches on the drive ({1}). If you disconnect without closing them, you may lose data.\nRun this again with the --force option to disconnect the drive anyways, accepting that information could be lost.\n\nFull error:\n{0}";

        public OpenedFilesError(string? driveLetter, string errorMessage) : base(errorMessage, DefaultMessageFormat)
        {
            DriveLetter = driveLetter;
        }

        protected override string GetFormattedMessage() => string.Format(MessageFormat!, Message, DriveLetter ?? "Unknown");
    }
}
