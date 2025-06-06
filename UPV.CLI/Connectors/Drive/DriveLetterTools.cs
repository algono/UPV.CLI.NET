using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UPV.CLI.Connectors.Drive
{
    public static partial class DriveLetterTools
    {
        public const string InvalidLetterMessage = "The letter has to be an alphabetical letter (A-Z).";
        public const string InvalidDriveLetterMessage = "The drive letter has an invalid format. It should be a letter followed by a colon.";

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
        public static char? GetFirstAvailable(params char[] prioritize)
        {
            char? prioritizedLetter = prioritize.FirstOrDefault(IsAvailable);
            return prioritizedLetter ?? GetFirstAvailable();
        }

        public static char? GetFirstAvailable() => GetDriveLetters(onlyIfAvailable: true).FirstOrDefault();

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

        public static bool TryGetLetter(string driveLetter, [MaybeNullWhen(false)] out char? letter)
        {
            if (driveLetter.Length == 0)
            {
                letter = null;
                return false;
            }

            var letterChar = driveLetter[0];
            if (IsValid(letterChar))
            {
                letter = char.ToUpper(letterChar);
                return true;
            }

            letter = null;
            return false;
        }

        public static bool TryNormalizeDriveLetter(string driveLetter, [MaybeNullWhen(false)] out string normalizedDriveLetter)
        {
            if (driveLetter.Length == 1 && char.IsLetter(driveLetter[0]))
            {
                normalizedDriveLetter = ToDriveLetter(driveLetter[0]);
                return true;
            }
            else if (IsValid(driveLetter))
            {
                normalizedDriveLetter = driveLetter;
                return true;
            }
            else
            {
                normalizedDriveLetter = null;
                return false;
            }
        }

        [GeneratedRegex(DriveLetterPattern)]
        internal static partial Regex GetDriveLetterRegex();
    }
}
