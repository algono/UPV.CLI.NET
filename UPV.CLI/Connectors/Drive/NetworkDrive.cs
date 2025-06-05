using System.Diagnostics;
using static UPV.CLI.Connectors.CmdHelper;
using static UPV.CLI.Connectors.Drive.DriveExceptions;

namespace UPV.CLI.Connectors.Drive
{
    public class NetworkDrive
    {
        private readonly Func<string, DriveDomain, string>? _getAddress;
        private readonly string? _address;
        public string? Address => _getAddress?.Invoke(Username, Domain) ?? _address;
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
            if (IsConnected) Process.Start(ConnectedDriveLetter!);
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
                    ? DriveLetterTools.GetFirstAvailable()
                    : DriveLetterTools.GetFirstAvailable(prioritize: DefaultLetter);

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

        public Process Connect()
        {
            CheckArguments();

            var netInfo = DriveConnectionHelper.CreateNetProcessInfo();
            ApplyArguments(netInfo);
            return StartProcess(netInfo);
        }

        public void OnProcessConnected(ProcessEventArgs e)
        {
            if (e.Succeeded)
            {
                IsConnected = true;
            }

            if (letterWasAutoAssigned)
            {
                letter = default;
                letterWasAutoAssigned = false;
            }

            if (!e.Succeeded)
            {
                // 55 - Error del sistema "El recurso no se encuentra disponible" (es decir, la dirección no es valida).
                if (e.OutputOrErrorContains("55"))
                    throw new ArgumentOutOfRangeException(nameof(Address));

                /**
                * 86 - Error del sistema "La contraseña de red es incorrecta"
                * 1326 - Error del sistema "El usuario o la contraseña son incorrectos"
                * Cuando las credenciales son erróneas, da uno de estos dos errores de forma arbitraria.
                * 
                * Suponemos que en el primer caso el error fue de la contraseña y en el segundo del usuario,
                * pero no lo sabemos con seguridad.
                */

                if (e.OutputOrErrorContains("86"))
                    throw new ArgumentException(e.Error, nameof(Password));
                if (e.OutputOrErrorContains("1326"))
                    throw new ArgumentException(e.Error, nameof(Username));


                if (e.OutputOrErrorContains("85"))
                    throw new NotAvailableDriveException(Letter);
            }
        }

        public Process Disconnect() => DriveConnectionHelper.Disconnect(ConnectedDriveLetter!, YesToAll);

        public void OnProcessDisconnected(ProcessEventArgs e)
        {
            if (e.Succeeded)
            {
                IsConnected = false;
            }

            if (!e.Succeeded)
                //Esa secuencia es parte de "(S/N)", con lo que deducimos que nos pide confirmación (porque tenemos archivos abiertos)
                if (e.OutputOrErrorContains("/N)"))
                    throw new OpenedFilesException(ForceDisconnect);
        }

        private void ForceDisconnect()
        {
            bool oldYesToAll = YesToAll;
            YesToAll = true;
            Disconnect();
            YesToAll = oldYesToAll;
        }
        #endregion
    }
}