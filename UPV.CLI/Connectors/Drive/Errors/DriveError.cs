namespace UPV.CLI.Connectors.Drive.Errors
{
    public abstract class DriveError(string message, string? messageFormat)
    {
        public abstract string? Code { get; }

        public string Message { get; } = message;
        public string? MessageFormat { get; set; } = messageFormat;

        public const string FullErrorMessageFormat = "Full error:\n{0}";
        public string FullMessageFormat => $"{MessageFormat}\n\n{FullErrorMessageFormat}";

        protected string GetMessageFormat(bool showFullError)
        {
            if (MessageFormat is not null)
            {
                return showFullError ? FullMessageFormat : MessageFormat;
            }
            return FullErrorMessageFormat;
        }

        protected virtual string GetFormattedMessage(bool showFullError)
        {
            return string.Format(GetMessageFormat(showFullError), Message, Code);
        }

        public string GetErrorMessage(bool showFullError)
        {
            if (MessageFormat is not null)
            {
                return GetFormattedMessage(showFullError);
            }

            if (Code is not null)
            {
                return $"{GetType().Name} (Error Code: {Code})\n\nFull error:\n{Message}";
            }

            return $"{GetType().Name}\n\nFull error:\n{Message}";
        }

        public string GetFullErrorMessageOnly()
        {
            return string.Format(FullErrorMessageFormat, Message);
        }

        public override string ToString() => GetErrorMessage(showFullError: true);
    }
}
