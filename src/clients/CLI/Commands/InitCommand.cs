using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LibGit2Sharp;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

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

        //[Option("-o|--owner", "Set default Owner(s) of the resource (Optional).", CommandOptionType.SingleValue)]
        public string Owner { get; set; } = Constants.OwnerAsCurrentUser;

        //[Option("-z|--zmod-home-dir", "Set ZMOD home path. If not set then It sets current directory (Optional).", CommandOptionType.SingleValue)]
        
        public string ZmodHome { get; set; }= Constants.ZmodDefaultHome;
        
        public string UmoyaHome { get; set; }= Constants.UmoyaDefaultHome;
               
        [Option("-r|--reset-forcefully", "This will forcefully reset existing configurations if already present.", CommandOptionType.SingleValue)]
        public bool ForceToOverWrite { get; set; }= false;

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
                    if(Directory.Exists(Constants.ResourceDirecotryDefaultPath) && Directory.Exists(Constants.ResourcePackDirecotryDefaultPath)) 
                    {
                        Console.LogError("Action <init> is not performed successfully.");
                        Console.LogWarning("It seems older UMOYA (CLI) configuration found.");
                        bool UserResponse = Console.AskYesOrNo("Do you want to overwrite this configuration forcefully ? If \"Yes\" then It will remove already added resources and configurations");
                        if(UserResponse) 
                        {
                            Directory.Delete(UmoyaHome, true);
                            Directory.CreateDirectory(UmoyaHome);
                            Repository.Clone(GitHubResourceRepoURL, UmoyaHome);
                        }
                    }
                    else Repository.Clone(GitHubResourceRepoURL, UmoyaHome);
                    if(!Directory.Exists(Constants.ResourcePackDirecotryDefaultPath + Constants.PathSeperator + "contentFiles")) 
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
                        using (var repo = new Repository(UmoyaHome))
                        {
                            LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                            options.FetchOptions = new FetchOptions();
                            var signature = new LibGit2Sharp.Signature(new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);
                            Logger.Do("Updating umoya configurations/template from github " + GitHubResourceRepoURL);
                            LibGit2Sharp.Commands.Pull(repo, signature, options);
                        }
                        if (Info.UpdateFileForInit(UmoyaHome, ZmodHome, Owner)) Console.LogWarning("Configurations are updated.");
                        else Console.LogError("Error while updating configurations");
                    }                    
                }
                
                Console.LogLine("ZMOD initialized successfully.");
            }
            catch(Exception ex)
            {
                Console.LogError("Action <init> is not performed successfully.");
                Console.LogError(ex.Message);
            }            
        }

        private static string GetVersion()
            => typeof(InitCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}