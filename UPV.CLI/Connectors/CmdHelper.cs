using System.Diagnostics;

namespace UPV.CLI.Connectors
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

        public static ProcessEventArgs WaitAndCheck(Process process)
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

            return new ProcessEventArgs(succeeded, output, error);
        }

        public class ProcessEventArgs : EventArgs
        {
            public bool Succeeded { get; }
            public string Output { get; }
            public string Error { get; }

            public ProcessEventArgs(bool succeeded, string output, string error)
            {
                Succeeded = succeeded;
                Output = output;
                Error = error;
            }

            public bool OutputOrErrorContains(string value) => Output.Contains(value) || Error.Contains(value);
        }
    }
}
