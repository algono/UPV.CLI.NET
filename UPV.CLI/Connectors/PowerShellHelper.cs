using System.Diagnostics;
using System.Text;

namespace UPV.CLI.Connectors
{
    public static class PowerShellHelper
    {
        public static async Task<bool> ExecutePowerShellScript(string script)
        {
            var (Success, _, _) = await RunPowerShellProcess(script);
            return Success;
        }

        public static async Task<string> ExecutePowerShellScriptWithOutput(string script)
        {
            var (Success, Output, _) = await RunPowerShellProcess(script);
            return Success ? Output : string.Empty;
        }

        public static async Task<(bool Success, string Output, string Error)> RunPowerShellProcess(string script)
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

                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    return (process.ExitCode == 0, output.Trim(), error.Trim());
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // PowerShell executable not found, try next one
                    continue;
                }
            }

            return (false, "", "No PowerShell installation found");
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
