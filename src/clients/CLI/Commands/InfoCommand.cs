using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using McMaster.Extensions.CommandLineUtils;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya info",
        FullName = "UMOYA (CLI) action info",
        Description = Constants.InfoCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class InfoCommand
    {

        [Option("-u|--repo-url", "Set Repo Server URL.", CommandOptionType.SingleValue)]
        public string RepoSourceURL { get; set; }

        [Option("-k|--access-key", "Set access key for Repo Server. If you don't have then login to ZMOD and get from Settings.", CommandOptionType.SingleValue)]
        public string Accesskey { get; set; }

        [Option("-o|--owner", "Set default Owner(s) of the resource (Optional).", CommandOptionType.SingleValue)]
        public string Owner { get; set; }

        [Option("-sp|--show-progress", "Show progress when resource is downloading. (Default : true)", CommandOptionType.SingleValue)]
        public string ShowProgress { get; set; }

        [Option("-d|--debug", "Set debugging flag. (Default : false)", CommandOptionType.SingleValue)]
        public string ISDebugging { get; set; }
        
        private async Task OnExecuteAsync()
        {
            Logger.Do("Command : Info is in process");
            try
            {
                if (Console.IsZMODConfigured())
                {
                    bool NeedToUpdateRepoSourceURL = !String.IsNullOrEmpty(RepoSourceURL);
                    bool NeedToUpdateAccesskey = !String.IsNullOrEmpty(Accesskey);
                    bool NeedToUpdateISDebugging = !String.IsNullOrEmpty(ISDebugging);
                    bool NeedToUpdateProgressStatus = !String.IsNullOrEmpty(ShowProgress);
                    bool NeedToUpdateOwner = !String.IsNullOrEmpty(Owner);
                    bool NeedToUpdateAnyField = NeedToUpdateRepoSourceURL || NeedToUpdateAccesskey || NeedToUpdateISDebugging || NeedToUpdateOwner || NeedToUpdateProgressStatus;
                    if (NeedToUpdateAnyField)
                    {
                        RepoSourceURL = NeedToUpdateRepoSourceURL ? RepoSourceURL : Info.Instance.Source.Url;
                        Accesskey = NeedToUpdateAccesskey ? Accesskey : Info.Instance.Source.Accesskey;
                        ISDebugging = NeedToUpdateISDebugging ? ISDebugging : Info.Instance.ISDebugging.ToString();
                        ShowProgress = NeedToUpdateProgressStatus ? ShowProgress : Info.Instance.ShowProgress.ToString();
                        Owner = NeedToUpdateOwner ? Owner : Info.Instance.Owner;
                        string UmoyaHome = Info.Instance.UmoyaHome;
                        string ZmodHome = Info.Instance.ZmodHome;
                        string nugetPath = Constants.ResourceDirecotryDefaultPath + Constants.PathSeperator + "nuget.config";
                        Logger.Do("NeedToUpdate ProgressStatus " + NeedToUpdateProgressStatus);
                        if (!UpdateNugetConfig(nugetPath)) throw new Exceptions.ConfigurationNotFoundException(Constants.InfoCommandName);
                        if (Console.SetInfoConfigurationValues(RepoSourceURL, Accesskey, Owner, Boolean.Parse(ISDebugging) , Boolean.Parse(ShowProgress)) > 0) Console.LogLine("Configurations are updated successfully.");
                        else throw new Exceptions.ActionNotSuccessfullyPerformException(Constants.InfoCommandName, "Error When updating configurations");
                        Info.Instance.ReLoad();
                    }
                    Console.PrintInfo();
                }
                else throw new Exceptions.ConfigurationNotFoundException(Constants.InfoCommandName);
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
        }
        public bool UpdateNugetConfig(string path)
        {
            bool status = true;
            if (!File.Exists(path))
            {
                status = false;
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                foreach (XmlElement element in xmlDoc.DocumentElement)
                {
                    if (element.Name.Equals("apikeys"))
                    {
                        element.FirstChild.Attributes[0].Value = RepoSourceURL;
                        element.FirstChild.Attributes[1].Value = Accesskey;
                    }
                    if(element.Name.Equals("packageSources"))
                    {
                        element.LastChild.Attributes[1].Value = RepoSourceURL;                       
                    }
                }
                xmlDoc.Save(path);
                status = true;
            }
            return status;
        }

        private static string GetVersion()
                    => typeof(InfoCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}