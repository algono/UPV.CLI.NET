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
                            Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();

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

        public static string ParseAsBase64String(string input)
        {
            // Special handling for XML parameters - use base64 encoding
            var encodedInput = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
            return $"[System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{encodedInput}'))";
        }

        public static string EscapeString(string input, bool isLiteral = false)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (isLiteral)
            {
                return $"@'\n{input}\n'@"; // Here-string syntax (literal string)
            }
            else
            {
                return $"'{input.Replace("'", "''")}'"; // Escape single quotes for PowerShell
            }
        }
    }
}
