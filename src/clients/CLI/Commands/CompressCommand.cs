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

        private static List<string> ListOfFoldersToIgnoreForCompress = new List<string>() { ".umoya"+ Constants.PathSeperator +"publish", ".umoya" + Constants.PathSeperator + ".git" + Constants.PathSeperator, ".umoya" + Constants.PathSeperator + "resources" + Constants.PathSeperator + "obj", ".umoya" + Constants.PathSeperator + "resources" + Constants.PathSeperator + "cache" + Constants.PathSeperator , ".umoya" + Constants.PathSeperator + "temp" };
        [Required]
        [Argument(0, "FilePath", Description = "Give file path (.zip) of compressed repository.")]
        public string CompressRepoFilePath { get; set; }

        private async Task OnExecuteAsync()
        {
            try
            {
                Dictionary<Resources.ResourceType, Dictionary<string, ResourceIdentifier>> ListOfLocalResources =
                ListOfLocalResources = await Resources.GetLocalResourceList(Resources.ResourceType.Any);
                foreach(KeyValuePair<Resources.ResourceType, Dictionary<string, ResourceIdentifier>> Pair in ListOfLocalResources)
                {
                    Dictionary<string, ResourceIdentifier> TempListOfResourcesByTypes = Pair.Value;
                    foreach(KeyValuePair<string, ResourceIdentifier> ResourceIdAndValue in TempListOfResourcesByTypes)
                    {
                        if(ResourceIdAndValue.Value.Version.Equals("NoVersion"))
                        {
                            ListOfFoldersToIgnoreForCompress.Add(ResourceIdAndValue.Value.ResourceName);
                            Logger.Do("Ignoring Resource " + ResourceIdAndValue.Value.ResourceName + "   " + ResourceIdAndValue.Value.Version);
                        }                        
                    }
                }
                Resources.DoCompress(Constants.ZmodDefaultHome, CompressRepoFilePath, ListOfFoldersToIgnoreForCompress);                
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