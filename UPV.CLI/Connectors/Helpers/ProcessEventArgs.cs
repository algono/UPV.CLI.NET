namespace UPV.CLI.Connectors
{
    public static partial class CmdHelper
    {
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
