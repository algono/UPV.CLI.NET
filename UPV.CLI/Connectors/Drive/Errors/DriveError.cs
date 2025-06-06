namespace UPV.CLI.Connectors.Drive.Errors
{
    public abstract class DriveError(string message, string? messageFormat)
    {
        public abstract string? Code { get; }

        public string Message { get; } = message;
        public string? MessageFormat { get; set; } = messageFormat;

        protected virtual string GetFormattedMessage() => string.Format(MessageFormat!, Message, Code);

        public string GetErrorMessage()
        {
            if (MessageFormat is not null)
            {
                return GetFormattedMessage();
            }

            if (Code is not null)
            {
                return $"{GetType().Name} (Error Code: {Code})\n\nFull error:\n{Message}";
            }

            return $"{GetType().Name}\n\nFull error:\n{Message}";
        }

        public override string ToString() => GetErrorMessage();
    }
}
