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
        Name = "umoya deploy",
        FullName = "UMOYA (CLI) action deploy",
        Description = Constants.DeployCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class DeployCommand
    {

        [Required]
        [Argument(0, "CompressRepoFilePath", Description = "Give file path (.zip) ")]
        public string CompressRepoFilePath { get; set; }

        [Required]
        [Argument(1, "FolderPathToDeploy", Description = "Give folder path to deploy repository.")]
        public string FolderPathToDeploy { get; set; }

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