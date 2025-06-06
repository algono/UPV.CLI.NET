using System.Diagnostics;

namespace UPV.CLI.Connectors.Helpers
{
    public static class CmdHelper
    {
        public static ProcessStartInfo CreateProcessInfo(string fileName, string arguments = "")
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true, // In case you need to pass/input something to the process
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        public static Process StartProcess(ProcessStartInfo info)
        {
            Process process = Process.Start(info) ?? throw new InvalidOperationException($"Error trying to call {info.FileName}");
            process.StandardInput.Close(); //Para que falle si pide input al usuario
            return process;
        }

        public static ProcessResult WaitAndCheck(Process process)
        {
            string output = "", error = "";
            if (!process.StartInfo.UseShellExecute)
            {
                if (process.StartInfo.RedirectStandardOutput) output = process.StandardOutput.ReadToEnd();
                if (process.StartInfo.RedirectStandardError) error = process.StandardError.ReadToEnd();
            }

            process.WaitForExit();

            bool succeeded = process.ExitCode == 0;

            process.Close();

            return new ProcessResult(succeeded, output, error);
        }

        public static bool WaitCheckAndOutput(Process? process, string successFormat, string errorFormat, string nullMessage)
        {
            if (process == null)
            {
                Console.WriteLine(nullMessage);
                return false;
            }

            var result = WaitAndCheck(process);
            if (result.Succeeded)
            {
                Console.WriteLine(string.Format(successFormat, result.Output));
                return true;
            }
            else
            {
                Console.Error.WriteLine(string.Format(errorFormat, result.Error));
                return false;
            }
        }
    }
}
