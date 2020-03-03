using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using McMaster.Extensions.CommandLineUtils;
using System.IO;

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
        [Argument(0, "FilePath", Description = "Give file path (.zip) of the compressed repository.")]
        public string CompressRepoFilePath { get; set; }

        [Required]
        [Argument(1, "FolderPath", Description = "Give folder path where you want to deploy repository.")]
        public string FolderPathToDeploy { get; set; }

        private async Task OnExecuteAsync()
        {
            try
            {
                //Extract folder
                if(!Directory.Exists(FolderPathToDeploy)) throw new Exception("Folder path is not present");
                if(!File.Exists(CompressRepoFilePath)) throw new Exception("Given compressed file is not present.");
                ZipFile.ExtractToDirectory(CompressRepoFilePath, FolderPathToDeploy);
                Console.PrintActionPerformSuccessfully(Constants.DeployCommandName); 
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exceptions.ResourceInfoInvalidFormatException etr) { Logger.Do(etr.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
        }
        private static string GetVersion()
                    => typeof(DeleteCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    }
}