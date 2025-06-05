using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
