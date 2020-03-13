using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using LibGit2Sharp;
using McMaster.Extensions.CommandLineUtils;
using System.Configuration;
using System.Collections.Generic;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya add",
        FullName = "UMOYA (CLI) action add",
        Description = Constants.AddCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class AddCommand
    {

        private const string Name = "add";
        private string ZMODHome = Constants.ZmodDefaultHome;
        private string UmoyaHome = Constants.UmoyaDefaultHome;

        public string ResourceFile { get; set; }

        private string Owner = Environment.UserName;
        private string UmoyaResourcesHome = Constants.ResourceDirecotryDefaultPath;

        [Required]
        [Argument(0, "ResourceInfo", Description = "Give Resource name@version to add i.e. HelloWorld.pmml@1.0.0. Default version is latest, If not given.")]
        public string ResourceInfo { get; set; }

        [Option("-ol|--offline", "Set True If You want to add resource from cache only as off-line (default is false) --from-cache true", CommandOptionType.SingleValue)]
        public bool CacheStrategy { get; set; } = Constants.DefaultCacheStrategy;

        [Option("-j|--json", "To output in json file i.e. --json myresources.json", CommandOptionType.SingleValue)]
        public string OutputJSONFile { get; set; }
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }

        public string ResourceVersion { get; set; }
        private async Task OnExecuteAsync()
        {
            try
            {            
                string TempResourceName = string.Empty;
                string GitHubResourceRepoURL = ConfigurationManager.AppSettings["GithubRepository"];
                ResourceIdentifier ResourceId;
                if (!Resources.IsResourceInfoFormatValidWithVersion(ResourceInfo)) ResourceId = new ResourceIdentifier(ResourceInfo, false);
                ResourceId = new ResourceIdentifier(ResourceInfo);
                //System.Console.WriteLine(ResIdentifier.ToString());
                //ResourceFile = ResIdentifier.ResourceName;
                ResourceName = ResourceId.ResourceName;
                //Need to overwrite ?
                //if(Resources.DoesResourceFileExist(ResourceFile, out TempResourceName)) ResourceName = TempResourceName;       
                ResourceType = ResourceId.TypeOfResource.ToString();
                ResourceVersion = ResourceId.Version;
                Logger.Do("Resource Name " + ResourceName);
                Logger.Do("Resource Version " + ResourceVersion);
               // System.Console.WriteLine("Path "+Path.GetFileName(ResourceInfo));
                if (!Console.IsZMODConfigured())
                {
                    bool UserResponse = Console.AskYesOrNo("Resource directories are not present here. Create them now? ");
                    if (UserResponse)
                    {
                        Console.LogWarning("ZMOD is initializing..");
                        FSOps.InitializeDirs();
                        Logger.Do("Getting latest umoya Resources from github");
                        //Ask: If repository is already checked out, what needs to be done? Pull or escape?
                        if (Directory.Exists(Constants.ResourceDirecotryDefaultPath) && Directory.Exists(Constants.ResourcePackDirecotryDefaultPath))
                        {
                            Console.LogError("UMOYA action <init> is not performed successfully.");
                            Console.LogWarning("It seems older UMOYA (CLI) configurations are found.");
                            UserResponse = Console.AskYesOrNo("Do you want to overwrite this configuration forcefully ? If \"Yes\" then It will remove already added resources and configurations");
                            if (UserResponse)
                            {
                                Directory.Delete(UmoyaHome, true);
                                Directory.CreateDirectory(UmoyaHome);
                                Repository.Clone(GitHubResourceRepoURL, UmoyaHome);
                            }
                        }
                        else Repository.Clone(GitHubResourceRepoURL, UmoyaHome);
                        //ToDo Surbhi Create config file from FSOps
                        if (Info.CreateFile(UmoyaHome, ZMODHome, Owner, null, null))
                        {
                            if (Info.UpdateFileForInit(UmoyaHome, ZMODHome, Owner)) Console.LogWarning("Configurations are created.");
                            else Console.LogError("Error while updating configurations.");
                        }
                    }
                    else
                    {
                        Console.LogError("UMOYA action <add> is aborted.");
                        return;
                    }
                }
                if (Console.IsZMODConfigured())
                {
                    List<ResourceIdentifier> ListOfInterestedResourcesToProcess = new List<ResourceIdentifier>();
                    // Need to resolve when version is not passed
                    #region to check given resource and version is present.
                    Console.LogLine("> Checking resource in Repo.");
                    if (!await Resources.IsResourcePresentInRepoAsync(ResourceId, CacheStrategy)) throw new Exceptions.ResourceNotFoundInRepoException(Constants.AddCommandName);
                    #endregion
                    #region Clear Temp Cache
                    List<string> ErrorInfo = new List<string>();
                    List<string> OutputInfo = new List<string>();
                    string ClearCacheCommandString = "nuget locals global-packages -c";
                    Logger.Do("Clearning Temp Cache for Nuget " + ClearCacheCommandString);
                    ErrorInfo = PSOps.StartAndWaitForFinish(Constants.DotNetCommand, ClearCacheCommandString, out OutputInfo, Constants.ResourceDirecotryDefaultPath);
                    ClearCacheCommandString = "nuget locals http-cache -c";
                    Resources.DoCacheCleanUpIfNeeded();
                    Logger.Do("Clearning Temp Cache for Nuget " + ClearCacheCommandString);
                    ErrorInfo = PSOps.StartAndWaitForFinish(Constants.DotNetCommand, ClearCacheCommandString, out OutputInfo, Constants.ResourceDirecotryDefaultPath);
                    #endregion
                    #region Nuget add command
                    string AddCommandString = Constants.AddCommandName + " " + Constants.ResourceProjectDefaultPath + " package " + ResourceName;
                    bool HasResourceVersion = IsResourceVersionGiven(ResourceVersion);
                    if (HasResourceVersion) AddCommandString = AddCommandString + " --version " + ResourceVersion;
                    Logger.Do("Get resource info from repo");
                    if(CacheStrategy)
                    {
                        Console.LogLine("> Started using resource from cache.");
                    }
                    else
                    {
                        Console.LogLine("> Started downloading resource from Repo.");
                        if(!HasResourceVersion) 
                        {
                            Logger.Do("Got latest version " + ResourceVersion);
                            ResourceId = await Resources.GetLatestResourceInfoByNameFromRepo(ResourceName);
                        }
                        else ResourceId = await Resources.GetResourceInfoByNameAndVersionFromRepo(ResourceName, ResourceVersion);
                        Resources.DownloadResourcesInCache(ResourceId, Constants.DefaultResourceCacheDirectory);
                    }
                    Logger.Do("Add nuget command string " + AddCommandString);
                    ErrorInfo = PSOps.StartAndWaitForFinish(Constants.DotNetCommand, AddCommandString, out OutputInfo);
                    if (ErrorInfo.Count > 0) throw new Exceptions.ActionNotSuccessfullyPerformException(Name, string.Join('\n', ErrorInfo));
                    else Console.LogLine("> Resource is downloaded.");
                    #endregion
                    
                    #region Resolving ZMOD Dir
                    if(!ResourceId.HasVersion()) ResourceId.Version = Resources.GetResourceVersionFromProject(ResourceId.ResourceName);
                    if(ResourceId.HasVersion())
                    {
                        ListOfInterestedResourcesToProcess.Add(ResourceId);
                        ListOfInterestedResourcesToProcess.AddRange(await Resources.GetDependentResources(ResourceId));
                    }
                    foreach(ResourceIdentifier RId in ListOfInterestedResourcesToProcess)
                    {
                        Logger.Do("Interested Resource to process " + RId.ToString());
                        FSOps.AddFromCacheToZMOD(RId);
                    }                  
                    Console.LogLine("> Resource (and its dependencies) are resolved and added into ZMOD.");
                    #endregion
                    #region Clean up local resource cache  
                    foreach(ResourceIdentifier RId in ListOfInterestedResourcesToProcess)
                    {
                        Logger.Do("Clearning cache for resource " + RId.ToString());
                        FSOps.CleanUpCacheByResource(RId.ResourceName, RId.Version);
                    }                    
                    Console.LogLine("> Cleared resource cache.");
                    #endregion
                }
                else throw new Exceptions.ConfigurationNotSuccessfullyDoneException(Constants.AddCommandName);
                Console.PrintActionPerformSuccessfully(Constants.AddCommandName);
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exceptions.ResourceInfoInvalidFormatException etr) { Logger.Do(etr.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
           // Console.Close();
        }

        private bool IsResourceVersionGiven(string ResourceVersion)
        {
            bool Status = true;
            if (ResourceVersion.Equals("NoVersion")) Status = false;
            return Status;
        }
        private static string GetVersion()
                    => typeof(AddCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}