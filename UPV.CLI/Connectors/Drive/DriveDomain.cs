namespace UPV.CLI.Connectors.Drive
{
    public enum DomainStyle { BackSlashStyle, AtSignStyle }
    public readonly struct DriveDomain
    {
        public string Name { get; }
        public string? Folder { get; }
        public DomainStyle PreferredStyle { get; }

        public DriveDomain(string name, DomainStyle style = DomainStyle.BackSlashStyle, string? folder = null)
        {
            Name = name;
            PreferredStyle = style;
            Folder = folder;
        }
        public string GetFullUsername(string userName) => GetFullUserName(userName, PreferredStyle);
        public string GetFullUserName(string userName, DomainStyle style)
        {
            return style switch
            {
                DomainStyle.AtSignStyle => $"{userName}@{Name}",
                DomainStyle.BackSlashStyle => $@"{Name}\{userName}",
                _ => throw new ArgumentOutOfRangeException(nameof(style)),
            };
        }
    }
}
