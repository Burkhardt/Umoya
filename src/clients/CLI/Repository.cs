using System.IO;
using System.IO.Compression;

namespace Repo.Clients.CLI.Commands
{
    public static class Repository
    {  public static bool Setup(string UmoyaHome)
        {
            string ZipFileName = "umoya-resources.zip";
            string ZipFilePath = UmoyaHome + Constants.PathSeperator + ZipFileName;
            EmbeddedResources.Extract(UmoyaHome, ZipFileName);
            ZipFile.ExtractToDirectory(ZipFilePath, UmoyaHome);
            File.Delete(ZipFilePath);
            return true;
        }
    }
}