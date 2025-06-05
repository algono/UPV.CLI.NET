using System.Text.RegularExpressions;
using static UPV.CLI.Connectors.Drive.DriveExceptions;

namespace UPV.CLI.Connectors.Drive
{
    public static partial class DriveLetterTools
    {
        internal const string InvalidLetterMessage = "The letter has to be an alphabetical letter (A-Z).";
        internal const string InvalidDriveLetterMessage = "The drive letter has an invalid format. It should be a letter followed by a colon.";

        /// <exception cref="ArgumentOutOfRangeException">If the letter is not valid.</exception>
        public static bool IsAvailable(char letter)
        {
            if (IsValid(letter))
            {
                return !Directory.Exists(ToDriveLetter(letter));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(letter), letter, InvalidLetterMessage);
            }
        }

        public static bool IsValid(char letter) => char.IsLetter(letter);

        public const string DriveLetterPattern = @"^[A-Za-z]:[\\/]?$";
        public static bool IsValid(string driveLetter)
            => GetDriveLetterRegex().IsMatch(driveLetter);

        /// <exception cref="ArgumentOutOfRangeException">If any of the prioritized letters is not valid.</exception>
        public static char GetFirstAvailable(params char[] prioritize)
        {
            char letter = prioritize.FirstOrDefault(IsAvailable);
            
            return letter == default ? GetFirstAvailable() : letter;
        }

        public static char GetFirstAvailable()
        {
            char letter = GetDriveLetters(onlyIfAvailable: true).FirstOrDefault();
            
            if (letter == default) throw new NotAvailableDriveException();
            
            return letter;
        }

        public static IEnumerable<char> GetDriveLetters(bool onlyIfAvailable = false)
        {
            for (char letter = 'Z'; letter >= 'A'; letter--)
            {
                if (!onlyIfAvailable || IsAvailable(letter))
                {
                    yield return letter;
                }
            }
        }

        public static string ToDriveLetter(char letter)
        {
            if (IsValid(letter))
            {
                return char.ToUpper(letter) + ":";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(letter), letter, InvalidLetterMessage);
            }

        }

        public static char FromDriveLetter(string driveLetter)
        {
            if (IsValid(driveLetter))
            {
                return driveLetter[0];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(driveLetter), driveLetter, InvalidDriveLetterMessage);
            }
        }

        [GeneratedRegex(DriveLetterPattern)]
        internal static partial Regex GetDriveLetterRegex();
    }
}
