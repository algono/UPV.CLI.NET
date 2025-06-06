namespace UPV.CLI.Connectors.Drive.Errors
{
    [Serializable]
    public class NotAvailableDriveError : DriveError
    {
        public const string ErrorCode = "85";
        public override string Code => ErrorCode;

        public char Letter { get; }

        public const string DefaultMessageFormat = "The drive defined for the disk ({2}:) is already associated with another disk (error code: {1}).\n"
            + "Disconnect the associated disk or change the drive used for the disk, and try again.";

        public NotAvailableDriveError(char letter, string errorMessage) : base(errorMessage, DefaultMessageFormat)
        {
            Letter = letter;
        }

        protected override string GetFormattedMessage(bool showFullError) => string.Format(GetMessageFormat(showFullError), Message, Code, Letter);
    }
}
