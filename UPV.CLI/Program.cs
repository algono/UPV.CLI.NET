using Cocona;

// Main program
var app = CoconaLiteApp.Create();

app.AddSubCommand("vpn", x => x.AddCommands<VpnCommands>())
    .WithDescription("Manage VPN connections");

app.AddSubCommand("drive", x => x.AddCommands<DriveCommands>())
    .WithDescription("Manage network drives");

app.Run();