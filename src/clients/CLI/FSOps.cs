using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using static Repo.Clients.CLI.Resources;
using System;
using System.IO.Compression;

namespace Repo.Clients.CLI
{
    public static class FSOps
    {

        private static readonly string UmoyaResourcesHome = Constants.ResourceDirecotryDefaultPath + Constants.PathSeperator + "resources";

        public class FSOpsException : System.Exception
        {
            public FSOpsException(string message)
            : base(message)
            {
            }
        }
        private static readonly string[] DirNames = new string[] { Constants.CodeDirName, Constants.DataDirName, Constants.ModelDirName, Constants.UmoyaDirName };

        private static string GetResourceDirPath(ResourceType resourceType)
        {
            if (resourceType == ResourceType.Code) return Constants.CodeDirName;
            if (resourceType == ResourceType.Data) return Constants.DataDirName;
            if (resourceType == ResourceType.Model) return Constants.ModelDirName;
            throw new FSOpsException($"No matching resource directory for resource type {resourceType}");
        }

        public static bool ConfigFileExists()
        {
            return File.Exists(Constants.ConfigFileName);
        }
        public static bool HasNecessaryDirs()
        {
            try
            {
                foreach (string dirName in DirNames)
                {
                    if (!Directory.Exists(dirName))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (System.Exception)
            {
                throw new FSOpsException("Failed to check for necessary directories and files");
            }
        }
        public static bool InitializeDirs(
            bool logExisting = false,
            bool logCreated = false)
        {
            try
            {
                bool successful = true;

                foreach (string dirName in DirNames)
                {
                    TryCreateDirIfNonExistent(
                        dirName,
                        logExisting,
                        logCreated
                    );
                }

                return successful;
            }
            catch (FSOpsException ex)
            {
                throw ex;
            }
        }

        private static bool TryCreateDirIfNonExistent(
            string dirName,
            bool logExisting,
            bool logCreated)
        {
            try
            {
                if (Directory.Exists(dirName))
                {
                    if (logExisting)
                    {
                        Console.LogLine($"Directory \"{dirName}\" already exists");
                    }
                    return true;
                }
                else
                {
                    Directory.CreateDirectory(dirName);

                    if (logCreated)
                    {
                        Console.LogLine($"Directory \"{dirName}\" created");
                    }
                    return true;
                }
            }
            catch (System.Exception)
            {
                throw new FSOpsException($"Failed to create directory \"{dirName}\"");
            }
        }

        public static bool ResourceFileExists(ResourceType resourceType, string resourceName)
        {
            try
            {
                return File.Exists(Path.Join(GetResourceDirPath(resourceType), resourceName));
            }
            catch (System.Exception)
            {
                throw new FSOpsException($"Failed to check file system for whether {resourceType.ToString().ToLower()} resource {resourceName} exists");
            }
        }

        public static IEnumerable<string> ResourceNames(ResourceType resourceType)
        {
            try
            {
                return new DirectoryInfo(GetResourceDirPath(resourceType)).EnumerateFiles().Select(file => file.Name);
            }
            catch (System.Exception)
            {
                throw new FSOpsException($"Failed to get list of {resourceType.ToString()} resources from file system");
            }
        }
        //Initialize config.json file
        public static FileStream CreateResourceFile(ResourceType resourceType, string resourceName)
        {
            try
            {
                return File.Create(Path.Join(GetResourceDirPath(resourceType), resourceName));
            }
            catch (System.Exception)
            {
                throw new FSOpsException($"Failed to create file for {resourceType.ToString().ToLower()} resource {resourceName}");
            }
        }
        public static long RetResourceSize(ResourceType resourceType, string resourceName)
        {
            try
            {
                return new FileInfo(Path.Join(GetResourceDirPath(resourceType), resourceName)).Length;
            }
            catch (System.Exception)
            {
                throw new FSOpsException($"Failed to determine file size of file for {resourceType.ToString().ToLower()} resource {resourceName}");
            }
        }
        public static FileStream readResourceFile(ResourceType resourceType, string fileName)
        {
            try
            {
                string filePath = Path.Join(GetResourceDirPath(resourceType), fileName);
                return File.OpenRead(filePath);
            }
            catch (System.Exception)
            {
                throw new FSOpsException($"Failed to open {resourceType.ToString().ToLower()} resource {fileName}. Does this file exist?");
            }
        }

        public static bool UpdateInfo(string RepoSourceURL, string AccessKey)
        {
            try
            {
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(Constants.ConfigFileName));
                jsonObject.Source.Url = RepoSourceURL;
                jsonObject.Source.Accesskey = AccessKey;
                var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                File.WriteAllText(Constants.ConfigFileName, modifiedJsonString);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Do(ex.Message);
                return false;
            }
        }

      public static bool UpdateInfoValue(string RepoSourceURL, string Accesskey, string Owner, bool IsDebugging , bool ShowProgess)
        {
            try
            {                
                Info info = new Info(Info.Instance.UmoyaHome, Info.Instance.ZmodHome, Owner, RepoSourceURL , Accesskey, IsDebugging, ShowProgess);
                string JSONresult = JsonConvert.SerializeObject(info);
                File.WriteAllText(Constants.ConfigFileName, JSONresult);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Do(ex.Message);
                return false;
            }
        }

        public static string BytesToString(long bytes)
        {
            const long KSize = 1024;
            const long MSize = 1048576;
            const long GSize = 1073741824;
            const long TSize = 1099511627776;

            long unit;
            string suffix;
            if (bytes < KSize)
            {
                unit = 1;
                suffix = "B";
            }
            else if (bytes < MSize)
            {
                unit = KSize;
                suffix = "KB";
            }
            else if (bytes < GSize)
            {
                unit = MSize;
                suffix = "MB";
            }
            else if (bytes < TSize)
            {
                unit = GSize;
                suffix = "GB";
            }
            else
            {
                unit = TSize;
                suffix = "TB";
            }

            float dividedByUnits = bytes / ((float)unit);

            // represent either as integer or to two decimal places
            string numToString = dividedByUnits % 1 == 0 ? dividedByUnits.ToString() : string.Format("{0:0.00}", dividedByUnits);

            return $"{numToString} {suffix}";
        }

        public static void DeleteDirectory(string DirPath)
        {
            try
            {
                Logger.Do("Deleting " + DirPath);
                File.SetAttributes(DirPath, FileAttributes.Normal);
                Directory.Delete(DirPath, true);
            }
            catch (Exception ex)
            {
                Logger.Do(ex.Message);
            }
        }

        public static void CleanUpCacheByResource(string ResourceName, string ResourceVersion)
        {
            try
            {     
                /*Debugging*/     
                //resources\resources\helloworld.pmml\1.0.0
                string ResourceCacheDir = Constants.ResourceDirecotryDefaultPath + Constants.PathSeperator + "resources" + Constants.PathSeperator + ResourceName.ToLower() + Constants.PathSeperator + ResourceVersion;
                string ResourceCacheContentFileDir = ResourceCacheDir + Constants.PathSeperator + "contentFiles";
                DeleteDirectory(ResourceCacheContentFileDir);
                string ResourceCacheNugetFile = ResourceCacheDir + Constants.PathSeperator + ".nupkg.metadata";
                Logger.Do("Deleting " + ResourceCacheNugetFile);
                File.Delete(ResourceCacheNugetFile);
                string ResourceCachePackageFile = ResourceCacheDir + Constants.PathSeperator + ResourceName.ToLower() + "." + ResourceVersion + ".nupkg";
                Logger.Do("Deleting " + ResourceCachePackageFile);
                File.Delete(ResourceCachePackageFile);
            }
            catch(Exception ex)
            {
                Logger.Do(ex.Message);
            }
        }

        public static void Delete(ResourceIdentifier ResourceId)
        {
            try
            {
                string ResourcePath = string.Empty;
                if (ResourceId.IsZipResource)
                {                    
                    ResourcePath = Resources.ResourceDirPath(ResourceId.TypeOfResource) + Constants.PathSeperator + ResourceId.ZipResourceName;
                    Logger.Do("Deleting Resource " + ResourcePath);
                    System.IO.DirectoryInfo di = new DirectoryInfo(ResourcePath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    Directory.Delete(ResourcePath);
                }
                else
                {
                    ResourcePath = Resources.ResourceDirPath(ResourceId.TypeOfResource) + Constants.PathSeperator + ResourceId.ResourceName;
                    Logger.Do("Deleting Resource " + ResourcePath);
                    if (ResourceId.TypeOfResource.Equals(ResourceType.Code)) Directory.Delete(ResourcePath, true);
                    else File.Delete(ResourcePath);
                }
            }
            catch(Exception e)
            {
                Logger.Do(e.StackTrace);
            }
        }

        public static void AddFromCacheToZMOD(ResourceIdentifier ResourceId)
        {
            try
            {
                Logger.Do(">> Resource : " + ResourceId.TypeOfResource.ToString() + " " + ResourceId.ResourceName + " " + ResourceId.Version);
                string SourcePath = UmoyaResourcesHome + Constants.PathSeperator + ResourceId.ResourceName.ToLower() + Constants.PathSeperator + ResourceId.Version + Constants.PathSeperator + "contentFiles" + Constants.PathSeperator + ResourceId.TypeOfResource;
                string DestinationPath = Constants.ZmodDefaultHome + Constants.PathSeperator + Resources.GetZMODResourceType(ResourceId.TypeOfResource);

                if (ResourceId.TypeOfResource.Equals(Resources.ResourceType.Code))
                {
                    DestinationPath = DestinationPath + Constants.PathSeperator + ResourceId.ResourceName;
                    Directory.CreateDirectory(DestinationPath);
                }

                Logger.Do("Source " + SourcePath + " Destination " + DestinationPath);
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);

                //if data and .zip file then extract                
                if (ResourceId.IsZipResource)
                {                    
                    if (ResourceId.ResourceName.IndexOf(".zip") == ResourceId.ResourceName.Length - 4)
                    {
                        Console.LogWriteLine("> Extracting Data (.zip) Resource");
                        Logger.Do(">> Resource : " + ResourceId.TypeOfResource.ToString() + " " + ResourceId.ResourceName + " " + ResourceId.Version);
                        string ResourceFilePath = DestinationPath + Constants.PathSeperator + ResourceId.ResourceName;
                        ZipFile.ExtractToDirectory(ResourceFilePath, DestinationPath, true);
                        if (!Directory.Exists(DestinationPath + Constants.PathSeperator + ResourceId.ZipResourceName))
                        {
                            throw new Exception("Not able to extract .zip file successfully");
                        }
                        File.Delete(ResourceFilePath);
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Do(e.Message);
            }
        }
    }
}
