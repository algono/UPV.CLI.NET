namespace UPV.CLI.Connectors.Drive.Errors
{
    /// <summary>
    /// 86 - Error del sistema "La contraseña de red es incorrecta"
    /// </summary>
    public class IncorrectPasswordError : DriveError
    {
        public const string ErrorCode = "86";
        public override string Code => ErrorCode;
        
        public const string DefaultMessageFormat = "The network password is incorrect (error code: {1}).\n\nFull error:\n{0}";
        
        public IncorrectPasswordError(string errorMessage) : base(errorMessage, DefaultMessageFormat)
        {
        }
    }
}
