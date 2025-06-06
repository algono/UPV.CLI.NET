using System.Diagnostics;
using UPV.CLI.Connectors.Drive.Errors;
using UPV.CLI.Connectors.Helpers;
using static UPV.CLI.Connectors.Helpers.CmdHelper;

namespace UPV.CLI.Connectors.Drive
{
    public class NetworkDrive
    {
        private readonly Func<string, DriveDomain, string>? _getAddress;
        private readonly string? _address;
        public string Address => _getAddress?.Invoke(Username, Domain) ?? _address ?? throw new ArgumentNullException(nameof(Address));
        public DriveDomain Domain { get; set; }

        public string? ConnectedDriveLetter
            => ConnectedLetter == default ? default : DriveLetterTools.ToDriveLetter(ConnectedLetter);
        public char ConnectedLetter { get; private set; }

        public string? DriveLetter
            => Letter == default ? default : DriveLetterTools.ToDriveLetter(Letter);

        private char letter;
        private bool letterWasAutoAssigned;
        private bool needsUsername, needsPassword;

        public char Letter
        {
            get => letter;
            set
            {
                if (value == default || DriveLetterTools.IsValid(value))
                {
                    letter = value;
                    letterWasAutoAssigned = false;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(Letter), value, DriveLetterTools.InvalidLetterMessage);
                }
            }
        }

        public string Username { get; set; }
        public string? Password { get; set; }
        public System.Security.SecureString? SecurePassword { get; set; }
        public bool ExplicitUserArgument { get; set; }
        public bool AreCredentialsStored { get; set; }
        public bool NeedsUsername { get => !AreCredentialsStored && (needsUsername || ExplicitUserArgument); set => needsUsername = value; }
        public bool NeedsPassword { get => !AreCredentialsStored && needsPassword; set => needsPassword = value; }
        public bool YesToAll { get; set; }

        public string FullUsername => Domain.GetFullUsername(Username) ?? Username;

        public bool IsConnected
        {
            get => ConnectedLetter != default;
            protected set => ConnectedLetter = value ? Letter : default;
        }

        public string? Name { get; set; }

        public char DefaultLetter { get; set; }

        public NetworkDrive(Func<string, DriveDomain, string> getAddress, string user, DriveDomain domain, char letter = default, string? password = null) : this(address: null!, user, domain, password, letter)
        {
            _getAddress = getAddress;
        }

        public NetworkDrive(string address, string username, DriveDomain domain, string? password = null, char letter = default)
        {
            _address = address;
            Domain = domain;

            if (letter != default)
            {
                Letter = letter;
            }

            Username = username;
            Password = password;
            NeedsUsername = username != null;
            ExplicitUserArgument = false;
            NeedsPassword = password != null;
        }

        public override string? ToString() => Name ?? Address;

        public void Open()
        {
            if (IsConnected) Process.Start("explorer.exe", ConnectedDriveLetter!);
            else throw new InvalidOperationException("El disco debe estar conectado para poder abrirlo");
        }


        #region Connection Process
        protected void CheckArguments()
        {
            if (NeedsUsername && string.IsNullOrEmpty(Username)) throw new ArgumentNullException(nameof(Username));
            if (NeedsPassword && string.IsNullOrEmpty(Password)) throw new ArgumentNullException(nameof(Password));

            if (!DriveLetterTools.IsValid(Letter))
            {
                letter = DefaultLetter == default
                    ? DriveLetterTools.GetFirstAvailable() ?? default
                    : DriveLetterTools.GetFirstAvailable(prioritize: DefaultLetter) ?? default;

                letterWasAutoAssigned = true;
            }
        }

        private void ApplyArguments(ProcessStartInfo netInfo)
        {
            netInfo.Arguments = $"use {DriveLetter} {Address}";

            // The net use command doesn't allow to specify an user without a password
            if (NeedsPassword)
            {
                netInfo.Arguments += $" \"{Password}\"";

                if (ExplicitUserArgument)
                {
                    netInfo.Arguments += $" /USER:{FullUsername}";
                }
            }

            if (YesToAll) netInfo.Arguments += " /y";
        }

        public DriveProcess Connect()
        {
            CheckArguments();

            var netInfo = DriveConnectionHelper.CreateNetProcessInfo();
            ApplyArguments(netInfo);
            var process = StartProcess(netInfo) ?? throw new InvalidOperationException("Failed to start the connection process.");

            return new DriveProcess(process, Address, Letter);
        }

        public DriveError? OnProcessConnected(DriveProcess process, ProcessResult result)
        {
            if (result.Succeeded)
            {
                IsConnected = true;
            }

            if (letterWasAutoAssigned)
            {
                letter = default;
                letterWasAutoAssigned = false;
            }

            return DriveConnectionHelper.CheckDriveConnectionErrors(process, result);
        }

        public Process Disconnect() => DriveConnectionHelper.Disconnect(ConnectedDriveLetter!, YesToAll);

        public void OnProcessDisconnected(ProcessResult result)
        {
            if (result.Succeeded)
            {
                IsConnected = false;
            }
            else
            {
                DriveConnectionHelper.CheckDriveDisconnectionErrors(ConnectedDriveLetter, result);
            }
        }

        public void ForceDisconnect()
        {
            bool oldYesToAll = YesToAll;
            YesToAll = true;
            Disconnect();
            YesToAll = oldYesToAll;
        }
        #endregion
    }
}