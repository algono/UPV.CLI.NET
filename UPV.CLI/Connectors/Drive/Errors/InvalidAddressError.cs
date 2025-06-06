namespace UPV.CLI.Connectors.Drive.Errors
{
    /// <summary>
    /// 55 - Error del sistema "El recurso no se encuentra disponible" (es decir, la dirección no es valida).
    /// </summary>
    public class InvalidAddressError : DriveError
    {
        public const string ErrorCode = "55";
        public override string Code => ErrorCode;

        public string Address { get; }

        public const string DefaultMessageFormat = "Invalid address. Make sure your username and domain are correct (error code: {1}).\nAddress: {2}";

        public InvalidAddressError(string address, string errorMessage) : base(errorMessage, DefaultMessageFormat)
        {
            Address = address;
        }

        protected override string GetFormattedMessage(bool showFullError) => string.Format(GetMessageFormat(showFullError), Message, Code, Address);
    }
}
