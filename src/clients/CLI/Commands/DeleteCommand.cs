using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya delete",
        FullName = "UMOYA (CLI) action delete",
        Description = Constants.DeleteCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class DeleteCommand
    {
        private static string FromOptionValue = "local";

        [Required]
        [Argument(0, "ResourceInfo", Description = "Give ResourceInfo to be deleted i.e. HelloWorld.pmml@1.0.0. For repo, version is mandatory.")]
        public string ResourceInfo { get; set; }
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }
        public string ResourceVersion { get; set; }
    
        [Option("-f|--from", "Set (local or repo), where you want to remove resource. Default is local.", CommandOptionType.SingleValue)]
        public string From { get; set; } = FromOptionValue;
 [Option("-j|--json", "To output in json file i.e. --json myresources.json", CommandOptionType.SingleValue)]
        public string OutputJSONFile { get; set; }
        private async Task OnExecuteAsync()
        {
            try
            {
                ResourceIdentifier ResourceId;
                bool FromLocalOption = FromOptionValue.ToLower().Equals("local");
                if (FromLocalOption && !Resources.IsResourceInfoFormatValidWithVersion(ResourceInfo)) ResourceId = new ResourceIdentifier(ResourceInfo, false);
                ResourceId = new ResourceIdentifier(ResourceInfo);
                ResourceName = ResourceId.ResourceName;               
                ResourceType = ResourceId.TypeOfResource.ToString();
                ResourceVersion = ResourceId.Version;
                Logger.Do("Resource Name " + ResourceName);
                Logger.Do("Resource Version " + ResourceVersion);
                List<ResourceIdentifier> ListOfInterestedResourcesToProcess = new List<ResourceIdentifier>();            
                if (!Console.IsZMODConfigured()) throw new Exceptions.ConfigurationNotFoundException(Constants.DeleteCommandName);
                if(FromLocalOption)
                {
                    Console.LogLine("> Checking resource in ZMOD (local).");
                    Dictionary<Resources.ResourceType, Dictionary<string, ResourceIdentifier>> ListOfLocalResources = new Dictionary<Resources.ResourceType, Dictionary<string, ResourceIdentifier>>();
                    ListOfLocalResources = await Resources.GetLocalResourceList(Resources.ResourceType.Any);
                    foreach(string ResourceName in ListOfLocalResources[Resources.ResourceType.Model].Keys)
                    {
                        Logger.Do("Found Resources " + ResourceName);
                    }
                    if(!ListOfLocalResources[ResourceId.TypeOfResource].ContainsKey(ResourceId.ResourceName)) throw new Exceptions.ResourceNotFoundException(Constants.DeleteCommandName);
                    else 
                    {
                        ResourceId.Version = ListOfLocalResources[ResourceId.TypeOfResource][ResourceId.ResourceName].Version;
                        ListOfInterestedResourcesToProcess.Add(ResourceId);
                        if(ResourceId.HasVersion())
                        {
                            Console.LogLine("> Getting resource (and its dependencies) in ZMOD.");
                            ListOfInterestedResourcesToProcess.AddRange(await Resources.GetDependentResources(ResourceId));
                            List<string> ErrorInfo = new List<string>();
                            List<string> OutputInfo = new List<string>();
                            Console.LogLine("> Deleting resource.");
                            string DeleteResourceCommandString = "remove package " + ResourceId.ResourceName;
                            Logger.Do("Nuget delete package command " + DeleteResourceCommandString);
                            ErrorInfo = PSOps.StartAndWaitForFinish(Constants.DotNetCommand, DeleteResourceCommandString, out OutputInfo, Constants.ResourceDirecotryDefaultPath);
                        }
                        foreach(ResourceIdentifier RId in ListOfInterestedResourcesToProcess)
                        {
                            FSOps.Delete(RId);
                        }
                        Console.PrintActionPerformSuccessfully(Constants.DeleteCommandName);                        
                    }                    
                }
                else 
                {
                    throw new Exceptions.ActionNotSuccessfullyPerformException(Constants.DeleteCommandName);
                }
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exceptions.ResourceInfoInvalidFormatException etr) { Logger.Do(etr.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
        }
        private static string GetVersion()
                    => typeof(DeleteCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    }
}