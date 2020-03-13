using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya publish",
        FullName = "UMOYA (CLI) action publish",
        Description = Constants.PublishCommandDescription
    )]

    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class PublishCommand
    {
        private string ZMODHome = Constants.ZmodDefaultHome;
        private string UmoyaHome = Constants.UmoyaDefaultHome;

        [Required]
        [Argument(0, "ResourceInfo", Description = "Resource name@version to publish i.e. HelloWorld.pmml@1.0.0")]
        public string ResourceInfo { get; set; }
        public string ResourceType { get; set; }
        public string ResourceFile { get; set; }
        public string ResourceName { get; set; }
        public string ResourceVersion { get; set; }

        [Required]
        [Option("-d|--description", "Set description for your resource (Required)", CommandOptionType.SingleValue)]
        public string Descritpion { get; set; }

        [Option("-o|--owners", "Set owners for your resource with comma seperated i.e. --owners Rainer,Swapnil", CommandOptionType.SingleValue)]
        public string Owners { get; set; } = Constants.OwnerAsCurrentUser;

        [Option("-a|--authors", "Set authors for your resource with comma seperated i.e. --authers Vinay,Anil", CommandOptionType.SingleValue)]
        public string Authors { get; set; } = Constants.AuthorDefault;

        [Option("-t|--tags", "Tag your resource, This can help to co-relate and search group of resources i.e. --tags DurrModel", CommandOptionType.SingleValue)]
        public string Tags { get; set; } = string.Empty;

        [Option("-u|--using", "Set dependent resource(s) as resource-name@version-number i.e. --using XCode.py@1.2.0", CommandOptionType.SingleValue)]
        public string ListOfDependentResourceAndVersionStringWithCSV { get; set; } = string.Empty;
        public string DependentResourceVersion { get; set; }

        public string DependentResource { get; set; }

[Option("-j|--json", "To output in json file i.e. --json myresources.json", CommandOptionType.SingleValue)]
        public string OutputJSONFile { get; set; }
        private async Task OnExecuteAsync()
        {
            try
            {

                if (!Console.IsZMODConfigured()) throw new Exceptions.ActionNotSuccessfullyPerformException(Constants.PublishCommandName, "ZMOD is not configured. Please, Use init and remote to configure first.");
                Logger.Do("Command : Publish is in process.");
                Logger.Do("ResourceInfo " + ResourceInfo);
                if (!Resources.IsResourceInfoFormatValidWithVersion(ResourceInfo)) throw new Exceptions.ResourceInfoInvalidFormatException(Constants.PublishCommandName);
                ResourceIdentifier ResIdentifier = new ResourceIdentifier(ResourceInfo);
                ResourceFile = ResIdentifier.ResourceName;
                string TempResourceName = string.Empty;
                string TempResourceDependenciesInSpec = string.Empty;
                if (Resources.DoesResourceFileExist(ResourceFile, out TempResourceName)) ResourceName = TempResourceName;
                else throw new Exceptions.ResourceNotFoundException(Constants.PublishCommandName);
                ResourceType = ResIdentifier.TypeOfResource.ToString();
                ResourceVersion = ResIdentifier.Version;
                Logger.Do("Resource File " + ResourceFile);
                Logger.Do("Resource Name " + ResourceName);
                Logger.Do("ResourceType " + ResourceType);
                Logger.Do("Resource Version " + ResourceVersion);
                Logger.Do("Descrition " + Descritpion);
                Logger.Do("Owners " + Owners);
                Logger.Do("Authors " + Authors);
                if (Tags.Equals(string.Empty)) Tags = ResourceType;
                else Tags = Tags + "," + ResourceType;
                Logger.Do("Tags " + Tags);
                Logger.Do("List of Dependent Resource and Version " + ListOfDependentResourceAndVersionStringWithCSV);
                //Single or Multiple dependencies  
                bool HasMultipleDependencies = ListOfDependentResourceAndVersionStringWithCSV.Contains(',');
                if (HasMultipleDependencies)
                {
                    string[] ListOfDependentResourceInfo = ListOfDependentResourceAndVersionStringWithCSV.Split(',');
                    for (int i = 0; i < ListOfDependentResourceInfo.Length; i++)
                    {
                        if (Resources.IsResourceInfoFormatValidWithVersion(ListOfDependentResourceInfo[i]))
                        {
                            //Need to check resource is present on repo
                            if (!await Resources.IsResourcePresentInRepoAsync(new ResourceIdentifier(Path.GetFileName(ListOfDependentResourceInfo[i]), true)))
                                throw new Exceptions.ResourceNotFoundInRepoException(Constants.PublishCommandName);
                            TempResourceDependenciesInSpec = TempResourceDependenciesInSpec + Resources.PrepareResourceDependenciesForSpec(new ResourceIdentifier(ListOfDependentResourceInfo[i]));
                        }
                        else throw new Exceptions.ResourceInfoInvalidFormatException(Constants.PublishCommandName);
                    }
                }
                else
                {
                    //${Dependencies}
                    if (!ListOfDependentResourceAndVersionStringWithCSV.Equals(string.Empty))
                    {
                        if (Resources.IsResourceInfoFormatValidWithVersion(ListOfDependentResourceAndVersionStringWithCSV))
                        {
                            //Need to check resource is present on repo
                            if (!await Resources.IsResourcePresentInRepoAsync(new ResourceIdentifier(Path.GetFileName(ListOfDependentResourceAndVersionStringWithCSV),true)))
                                throw new Exceptions.ResourceNotFoundInRepoException(Constants.PublishCommandName);
                            TempResourceDependenciesInSpec = Resources.PrepareResourceDependenciesForSpec(new ResourceIdentifier(Path.GetFileName(ListOfDependentResourceAndVersionStringWithCSV)));
                        }
                        else throw new Exceptions.ResourceInfoInvalidFormatException(Constants.PublishCommandName);
                    }
                }
                Logger.Do("Dependencies " + TempResourceDependenciesInSpec);
                //Copy resource-spec to new temp folder
                //Update spec
                //Create Pack to publish folder
                //Publish pack                            
                #region Copy Resource Template to Temp folder
                string TempDirPath = UmoyaHome + Constants.PathSeperator + "temp";
                string PublishDirPath = UmoyaHome + Constants.PathSeperator + "publish";
                string TempResourcePackDirPath = TempDirPath + Constants.PathSeperator + "resource-pack";
                string TempResourcesDirPath = TempDirPath + Constants.PathSeperator + "resources";
                string TempResourcePackNugetSpecFilePath = TempResourcePackDirPath + Constants.PathSeperator + "resource-spec.nuspec";
                string TempResourcePackProjectPath = TempResourcePackDirPath + Constants.PathSeperator + "resource-spec-template.csproj";
                if (Directory.Exists(TempDirPath)) Directory.Delete(TempDirPath, true);
                if (Directory.Exists(PublishDirPath)) Directory.Delete(PublishDirPath, true);
                Directory.CreateDirectory(TempDirPath);
                Directory.CreateDirectory(TempResourcePackDirPath);
                Directory.CreateDirectory(TempResourcesDirPath);


                string SourcePath = Constants.ResourcePackDirecotryDefaultPath;
                string DestinationPath = TempResourcePackDirPath;
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);


                SourcePath = Constants.ResourceDirecotryDefaultPath;
                DestinationPath = TempResourcesDirPath;
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);


                //Copy resource to content folder according to resource file
                Logger.Do("Copying Resource " + Environment.CurrentDirectory + Constants.PathSeperator + ResourceFile);
                File.Copy(ResourceFile, TempResourcePackDirPath + Constants.PathSeperator + "contentFiles" + Constants.PathSeperator + ResourceType + Constants.PathSeperator + ResourceName, true);
                Console.LogLine("> Resource is prepared from template.");
                #endregion

                #region Update resource-spec nuget file with interested resource detail in temp folder/resource-spec
                //Read Spec and update
                string TempSpecContent = File.ReadAllText(TempResourcePackNugetSpecFilePath);
                TempSpecContent = TempSpecContent.Replace("${ResourceName}", ResourceName);
                TempSpecContent = TempSpecContent.Replace("${ResourceVersion}", ResourceVersion);
                TempSpecContent = TempSpecContent.Replace("${Authors}", Authors);
                TempSpecContent = TempSpecContent.Replace("${Owners}", Owners);
                TempSpecContent = TempSpecContent.Replace("${Description}", Descritpion);
                TempSpecContent = TempSpecContent.Replace("${Tags}", Tags);
                TempSpecContent = TempSpecContent.Replace("${ResourceType}", ResourceType);
                TempSpecContent = TempSpecContent.Replace("${Dependencies}", TempResourceDependenciesInSpec);
                Logger.Do("New Spec " + TempSpecContent);
                File.WriteAllText(TempResourcePackNugetSpecFilePath, TempSpecContent);
                Console.LogLine("> Resource spec is built.");
                #endregion

                #region Do pack and publish to folder
                //dotnet pack C:\Users\nva\Documents\GitHub\ZMOD\Umoya\temp\resource-pack\resource-spec-template.csproj -o C:\Users\nva\Documents\GitHub\ZMOD\Umoya\publish
                string PackCommandString = "pack " + TempResourcePackProjectPath + " -o " + PublishDirPath;
                List<string> ErrorInfo = new List<string>();
                List<string> OutputInfo = new List<string>();
                bool HasConflictResourceOnRepo = false;
                Logger.Do("Packing " + PackCommandString);
                ErrorInfo = PSOps.StartAndWaitForFinish(Constants.DotNetCommand, PackCommandString, out OutputInfo);
                if (ErrorInfo.Count > 0) throw new Exceptions.ActionNotSuccessfullyPerformException(Constants.PublishCommandName, string.Join('\n', ErrorInfo));
                else Console.LogLine("> Resource is packed.");
                #endregion

                #region Do push nuget publish/package with source configuration  
                string PushCommandString = "nuget push " + PublishDirPath + Constants.PathSeperator + ResourceName
                + "." + ResourceVersion + ".nupkg --source " + Info.Instance.Source.Url + " -k " + Info.Instance.Source.Accesskey;
                Logger.Do("Pushing " + PushCommandString);
                ErrorInfo = PSOps.StartAndWaitForFinish(Constants.DotNetCommand, PushCommandString, out OutputInfo);
                for (int i = 0; i < OutputInfo.Count; i++)
                {
                    if (OutputInfo[i].ToLower().Contains("conflict")) HasConflictResourceOnRepo = true;
                }
                if (HasConflictResourceOnRepo) throw new Exceptions.ActionNotSuccessfullyPerformException(Constants.PublishCommandName, "ResourceInfo (" + ResourceName + "@" + ResourceVersion + ") is already present on Repo, Try with different name and or version).");
                else if (ErrorInfo.Count > 0) throw new Exceptions.ActionNotSuccessfullyPerformException(Constants.PublishCommandName, string.Join('\n', ErrorInfo));
                else Console.LogLine("> Resource pushed to Repo (" + Info.Instance.Source.Url + ")");
                #endregion
                Console.PrintActionPerformSuccessfully(Constants.PublishCommandName);
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exceptions.ResourceNotFoundException e) { Logger.Do(e.Message); }
            catch (Exceptions.ResourceInfoInvalidFormatException etr) { Logger.Do(etr.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
        }
        private static string GetVersion() => Console.GetVersion();
    }
}