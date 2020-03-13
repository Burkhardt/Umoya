using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
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

        private static List<string> ListOfFoldersToIgnoreForBackup = new List<string>() { ".umoya"+ Constants.PathSeperator +"publish", ".umoya" + Constants.PathSeperator + ".git" + Constants.PathSeperator, ".umoya" + Constants.PathSeperator + "resources" + Constants.PathSeperator + "obj", ".umoya" + Constants.PathSeperator + "resources" + Constants.PathSeperator + "cache" + Constants.PathSeperator , ".umoya" + Constants.PathSeperator + "temp" };
        [Required]
        [Argument(0, "FilePath", Description = "Give file path where you want to backup")]
        public string BackupFilePath { get; set; }
[Option("-j|--json", "To output in json file i.e. --json myresources.json", CommandOptionType.SingleValue)]
        public string OutputJSONFile { get; set; }
        private async Task OnExecuteAsync()
        {
            try
            {                   
                Resources.DoCompress(Constants.ZmodDefaultHome, BackupFilePath, ListOfFoldersToIgnoreForBackup);                
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