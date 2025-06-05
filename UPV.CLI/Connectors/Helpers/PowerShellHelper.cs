using System.Diagnostics;

namespace UPV.CLI.Connectors.Helpers
{
    public static class PowerShellHelper
    {
        /// <summary>
        /// Executes a PowerShell script asynchronously and returns the process (or <see langword="null"/> if no PowerShell executable was found).
        /// </summary>
        public static async Task<Process?> StartPowerShellProcessAsync(string script)
        {
            // Try PowerShell 7+ first, fall back to Windows PowerShell
            string[] powerShellPaths = ["pwsh.exe", "powershell.exe"];

            foreach (string psPath in powerShellPaths)
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = psPath,
                            Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command -",
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();

                    // Write the script to the PowerShell process
                    await process.StandardInput.WriteAsync(script);

                    // Ensure the script ends the last line and then adds a second newline (like pressing Enter twice)
                    // to execute it properly (this is REQUIRED, without this it doesn't work)
                    await process.StandardInput.WriteLineAsync();
                    await process.StandardInput.WriteLineAsync();

                    await process.StandardInput.FlushAsync();
                    process.StandardInput.Close(); // Close the input stream to signal end of script

                    return process;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // PowerShell executable not found, try next one
                    continue;
                }
            }

            Console.Error.WriteLine("Error: No PowerShell executable found. Please ensure PowerShell is installed on your system.");
            return null; // If no PowerShell executable was found
        }

        public static string EscapeString(string input, bool isLiteral = false)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (isLiteral)
            {
                return $"@'{Environment.NewLine}{input}{Environment.NewLine}'@"; // Here-string syntax (literal string)
            }
            else
            {
                return $"'{input.Replace("'", "''")}'"; // Escape single quotes for PowerShell
            }
        }
    }
}
