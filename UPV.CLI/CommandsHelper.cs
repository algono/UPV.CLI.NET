namespace UPV.CLI
{
    public static class CommandsHelper
    {
        public static bool GetYesNoConfirmation(string message)
        {
            Console.Write($"{message} (y/N): ");

            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            return input == "y" || input == "yes";
        }


        // Helper method for domain validation
        public static bool TryValidateAndParseEnum<TEnum>(string stringValue, string parameterName, out TEnum parsedValue) where TEnum : struct, Enum
        {
            if (Enum.TryParse<TEnum>(stringValue, true, out parsedValue))
            {
                return true;
            }

            var validValues = Enum.GetNames<TEnum>();
            var validValuesString = string.Join("', '", validValues);

            Console.Error.WriteLine($"Error: Argument '{parameterName}' can only be one of the following: '{validValuesString}'. '{stringValue}' is not a valid value.");
            return false;
        }
    }
}
