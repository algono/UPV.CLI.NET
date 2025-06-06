namespace UPV.CLI.Connectors.Helpers
{
    public class ProcessResult
    {
        public bool Succeeded { get; }
        public string Output { get; }
        public string Error { get; }

        public ProcessResult(bool succeeded, string output, string error)
        {
            Succeeded = succeeded;
            Output = output;
            Error = error;
        }

        public bool OutputOrErrorContains(string value) => Output.Contains(value) || Error.Contains(value);
    }
}
