namespace UPV.CLI.Connectors.Drive.Errors
{
    /// <summary>
    /// 1326 - Error del sistema "El usuario o la contraseña son incorrectos"
    /// </summary>
    public class IncorrectUserNameOrPasswordError : DriveError
    {
        public const string ErrorCode = "1326";
        public override string Code => ErrorCode;
        
        public const string DefaultMessageFormat = "The username or password is incorrect (error code: {1}).";
        
        public IncorrectUserNameOrPasswordError(string errorMessage) : base(errorMessage, DefaultMessageFormat)
        {
        }
    }
}
