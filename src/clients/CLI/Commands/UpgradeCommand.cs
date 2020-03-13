using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya upgrade",
        FullName = "UMOYA (CLI) action upgrade",
        Description = Constants.UpgradeCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class UpgradeCommand
    {
        private static string GetVersion()
                    => typeof(UpgradeCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}