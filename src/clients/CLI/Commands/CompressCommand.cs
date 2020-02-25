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
        Name = "umoya compress",
        FullName = "UMOYA (CLI) action compress",
        Description = Constants.CompressCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class CompressCommand
    {

        [Required]
        [Argument(0, "CompressRepoFilePath", Description = "Give Compress file path (.zip)")]
        public string CompressRepoFilePath { get; set; }

        private async Task OnExecuteAsync()
        {
            try
            {
                Console.PrintActionPerformSuccessfully(Constants.CompressCommandName); 
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exceptions.ResourceInfoInvalidFormatException etr) { Logger.Do(etr.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
        }
        private static string GetVersion()
                    => typeof(DeleteCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    }
}