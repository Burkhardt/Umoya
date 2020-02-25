using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using McMaster.Extensions.CommandLineUtils;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya backup",
        FullName = "UMOYA (CLI) action backup",
        Description = Constants.BackupCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class BackupCommand
    {

        [Required]
        [Argument(0, "BackupFilePath", Description = "Give Backup file path (.zip)")]
        public string BackupFilePath { get; set; }

        private async Task OnExecuteAsync()
        {
            try
            {
                Console.PrintActionPerformSuccessfully(Constants.BackupCommandName); 
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exceptions.ResourceInfoInvalidFormatException etr) { Logger.Do(etr.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
        }
        private static string GetVersion()
                    => typeof(DeleteCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    }
}