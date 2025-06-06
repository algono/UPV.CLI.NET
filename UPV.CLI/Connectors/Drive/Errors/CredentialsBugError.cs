namespace UPV.CLI.Connectors.Drive.Errors
{
    /**
    * <summary>
    * 1223 - Error del sistema "El usuario o la contraseña son incorrectos".
    * Esto es un bug porque debería obtener las credenciales de la conexión con la UPV
    * (ya sea VPN o WiFi).
    * Por ahora el bug sólo se ha visto desde red WiFi, y la solución es reconectarse.
    * </summary>
    */
    [Serializable]
    public class CredentialsBugError : DriveError
    {
        public const string ErrorCode = "1223";
        public override string Code => ErrorCode;

        public const string DefaultMessageFormat = "There was a credentials error.\n\n"
            + "You must re-establish your connection to UPV manually (either VPN or Wi-Fi).\n\n"
            + "Full error:\n{0}";

        //public const string ErrorMessageSpanish = "Ha habido un error de credenciales.\n\n"
        //    + "Debe reestablecer su conexión a la UPV manualmente (VPN o WiFi).";

        public CredentialsBugError(string errorMessage) : base(errorMessage, DefaultMessageFormat) { }
    }
}
