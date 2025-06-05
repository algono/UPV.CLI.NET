using System.Text;

namespace UPV.CLI.Connectors.Helpers
{
    public class PowerShellCommandBuilder
    {
        private readonly StringBuilder _commandBuilder;
        public PowerShellCommandBuilder()
        {
            _commandBuilder = new StringBuilder();
        }
        public void AddCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("Command cannot be null or empty.", nameof(command));
            }

            if (_commandBuilder.Length > 0)
            {
                throw new InvalidOperationException("A command has already been added. Use a new instance of PowerShellCommandBuilder for a new command.");
            }

            _commandBuilder.Append($"{command} ");
        }
        public void AddParameter(string key, string value, bool isLiteral = false)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Key and value cannot be null or empty.");
            }

            ValidateCommand();

            _commandBuilder.Append($" -{key} {PowerShellHelper.EscapeString(value, isLiteral)}");
        }

        public void AddXmlParameter(string key, string value) => AddParameter(key, value, isLiteral: true);

        public void AddParameter(string key, bool value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            ValidateCommand();

            if (value)
                _commandBuilder.Append($" -{key}");
        }

        private void ValidateCommand()
        {
            if (_commandBuilder.Length == 0)
            {
                throw new InvalidOperationException("No command has been added. Use AddCommand first.");
            }
        }

        public string Build() => _commandBuilder.ToString();
        public override string ToString() => Build();
    }
}