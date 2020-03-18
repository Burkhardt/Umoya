using System.IO;
using System;
using System.IO.Compression;

namespace Repo.Clients.CLI.Commands
{
    public static class Repository
    {
        public static readonly DirectoryInfo CurrentDir = new DirectoryInfo(Environment.CurrentDirectory);
        public static readonly DirectoryInfo SrcDir = CurrentDir.Parent.Parent.Parent.Parent;
        public static bool Setup(string UmoyaHome)
        {
            string ResourcePackDirectoryDefaultPath = SrcDir.FullName + Constants.PathSeperator + "EmbeddedResources";
            string[] entries = Directory.GetFileSystemEntries(ResourcePackDirectoryDefaultPath, "*", SearchOption.AllDirectories);

            string ZipFilePath = string.Empty;
            string ExtractedPath = UmoyaHome + Constants.PathSeperator + ".umoya";
            foreach (var item in entries)
            {
                ZipFilePath = UmoyaHome + Constants.PathSeperator + Path.GetFileName(item).ToString();
                EmbeddedResources.Extract(UmoyaHome, Path.GetFileName(item).ToString());
                if (File.Exists(ExtractedPath))
                    File.Delete(ExtractedPath);
            }
            ZipFile.ExtractToDirectory(ZipFilePath, UmoyaHome);
                File.Delete(ZipFilePath);
            return true;
        }
    }
}