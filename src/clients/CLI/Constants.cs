using System;

namespace Repo.Clients.CLI
{
    public static class Constants
    {
        public const string UmoyaHomeName = ".umoya";
        public static readonly char PathSeperator = System.IO.Path.DirectorySeparatorChar;
        public static readonly string OwnerAsCurrentUser = Environment.UserName;
        public static readonly string UmoyaDefaultHome = Environment.CurrentDirectory + Constants.PathSeperator + Constants.UmoyaHomeName;
        public static readonly string ZmodDefaultHome = Environment.CurrentDirectory;
        public const string InitCommandName = "init";
        public const string InitCommandDescription = "Initialize ZMOD local directory with UMOYA (CLI) configurations.";
        public const string AddCommandName = "add";
        public const string AddCommandDescription = "Add resource from Repo to ZMOD local directory";
        public const string DeleteCommandName = "delete";
        public const string DeleteCommandDescription = "Remove resource from ZMOD local directory or from Repo";
        public const string InfoCommandName = "info";
        public const string InfoCommandDescription = "Show or Setup configurations for Umoya / Repo server";
        public const string ListCommandName = "list";
        public const string ListCommandDescription = "List resources from ZMOD local directory or Repo";
        public const string PublishCommandName = "publish";
        public const string PublishCommandDescription = "Publish resource like Model, Data and Code on Repo.";
        public const string RemoteCommandName = "remote";
        public const string RemoteCommandDescription = "Setup configuration for Repo and access-key for this directory.";
        public const string UpgradeCommandName = "upgrade";
        public const string UpgradeCommandDescription = "Upgrade ZMOD local resources' with latest version from Repo (coming soon)";
        public static readonly string ResourceDirecotryDefaultPath = UmoyaHomeName + PathSeperator + "resources";

        public static readonly string ResourceProjectDefaultPath = UmoyaHomeName + PathSeperator + "resources" + PathSeperator + "resources.csproj";

        public static readonly string ResourcePackDirecotryDefaultPath = UmoyaHomeName + PathSeperator + "resource-pack";

        public static readonly string PackOutputDirecotryDefaultPath = UmoyaHomeName + PathSeperator + "publish";

        public static string TempDirecotryDefaultPath = UmoyaHomeName + PathSeperator + "temp";

        public const string AuthorDefault = "Rainer";

        public static readonly string CodeDirName = "Code";
        public static readonly string DataDirName = "Data";
        public static readonly string ModelDirName = "Models";

        public static readonly string UmoyaDirName = ".umoya";

        public static readonly string ConfigFileName = UmoyaHomeName + Constants.PathSeperator + "info.json";

        public static readonly string DotNetCommand = "dotnet";

        public static readonly string DotNetAddNugetCommand = "add";

        public static readonly string ZmodResourceConfiguration = ZmodDefaultHome + Constants.PathSeperator + "zmod.json";

        public static readonly string DefaultSourceURL = "http://localhost:8007/v3/index.json";

        public static readonly string DefaultSourceAccessKey = "NoAPIKey";

        public static readonly string DefaultTestDataDir = Environment.CurrentDirectory + Constants.PathSeperator + "umoya-testdata";

        public static readonly string DefaultNoVersionValue = "NoVersion";

        public static readonly string DefaultResourceCacheDirectory = ResourceDirecotryDefaultPath + Constants.PathSeperator + "cache";

        public static readonly string DefaultResourceCacheCleanupToDoFile = Constants.DefaultResourceCacheDirectory + Constants.PathSeperator + "cleanup.txt";

        public const bool DefaultCacheStrategy = false;

        public const string BackupCommandName = "backup";
        public const string BackupCommandDescription = "Take backup of resource(s) into .zip file.";
        public const string CompressCommandName = "compress";
        public const string CompressCommandDescription = "Compress resource(s) and its depenent resource(s) into .zip file.";

        public const string DeployCommandName = "deploy";
        public const string DeployCommandDescription = "Deploy resource(s) and their dependent resource(s) of repository to given folder.";
    }
}
