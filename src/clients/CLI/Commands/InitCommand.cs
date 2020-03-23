using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya init",
        FullName = "UMOYA (CLI) action init",
        Description = Constants.InitCommandDescription
    )]

    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class InitCommand
    {

        public string Owner { get; set; } = Constants.OwnerAsCurrentUser;
        public string ZmodHome { get; set; } = Constants.ZmodDefaultHome;
        public string UmoyaHome { get; set; } = Constants.UmoyaDefaultHome;

        [Option("-r|--reset-forcefully", "This will forcefully reset existing configurations if already present.", CommandOptionType.SingleValue)]
        public bool ForceToOverWrite { get; set; } = false;
        [Option("-j|--json", "To output in json file i.e. --json myresources.json", CommandOptionType.SingleValue)]
        public string OutputJSONFile { get; set; }
        private async Task OnExecuteAsync()
        {
            try
            {
                Logger.Do("Command : Init is in process");
                Logger.Do("Umoya Home " + UmoyaHome);
                Logger.Do("ZMOD Home " + ZmodHome);
                Logger.Do("Checking if resources and configuration exists locally or not");
                string GitHubResourceRepoURL = ConfigurationManager.AppSettings["GithubRepository"];
                if (!Console.IsZMODConfigured())
                {
                    Console.LogWarning("ZMOD is initializing..");
                    FSOps.InitializeDirs();
                    Logger.Do("Getting latest Umoya Resources from github");
                    //Ask: If repository is already checked out, what needs to be done? Pull or escape?
                    if (Directory.Exists(Constants.ResourceDirecotryDefaultPath) && Directory.Exists(Constants.ResourcePackDirecotryDefaultPath))
                    {
                        Console.LogError("Action <init> is not performed successfully.");
                        Console.LogWarning("It seems older UMOYA (CLI) configuration found.");
                        bool UserResponse = Console.AskYesOrNo("Do you want to overwrite this configuration forcefully ? If \"Yes\" then It will remove already added resources and configurations");
                        if (UserResponse)
                        {
                            Directory.Delete(UmoyaHome, true);
                            Directory.CreateDirectory(UmoyaHome);
                            Repository.Setup(UmoyaHome);
                        }
                    }
                    else
                    {
                        Repository.Setup(UmoyaHome);
                    }
                    if (!Directory.Exists(Constants.ResourcePackDirecotryDefaultPath + Constants.PathSeperator + "contentFiles"))
                    {
                        Directory.CreateDirectory(Constants.ResourcePackDirecotryDefaultPath + Constants.PathSeperator + "contentFiles");
                        Directory.CreateDirectory(Constants.ResourcePackDirecotryDefaultPath + Constants.PathSeperator + "contentFiles" + Constants.PathSeperator + "Data");
                        Directory.CreateDirectory(Constants.ResourcePackDirecotryDefaultPath + Constants.PathSeperator + "contentFiles" + Constants.PathSeperator + "Code");
                        Directory.CreateDirectory(Constants.ResourcePackDirecotryDefaultPath + Constants.PathSeperator + "contentFiles" + Constants.PathSeperator + "Model");
                    }
                    //ToDo Surbhi Create config file from FSOps
                    if (Info.CreateFile(UmoyaHome, ZmodHome, Owner, null, null))
                    {
                        if (Info.UpdateFileForInit(UmoyaHome, ZmodHome, Owner)) Console.LogWarning("Configurations are created.");
                        else Console.LogError("Error while updating configurations");
                    }
                }
                else
                {
                    if (ForceToOverWrite)
                    {
                        if (Info.UpdateFileForInit(UmoyaHome, ZmodHome, Owner)) Console.LogWarning("Configurations are updated.");
                        else Console.LogError("Error while updating configurations");
                    }
                }

                Console.LogLine("ZMOD initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.LogError("Action <init> is not performed successfully.");
                Console.LogError(ex.Message);
            }
        }

        private static string GetVersion()
            => typeof(InitCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}