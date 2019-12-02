using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Xml;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Threading.Tasks;

namespace Repo.Clients.CLI
{
    public static class Resources
    {

        public enum ResourceType { Model, Code, Data, Other, Any }
        public static readonly string[] CodeFileExtensions = { "py", "ipynb", "r", "jar", "mon" };
        public static readonly string[] ModelFileExtensions = { "pmml", "h5", "pb", "pbtxt", "onnx" };
        public static readonly string[] DataFileExtensions = { "json", "csv", "png", "jpg", "jpeg", "zip", "txt", "md", "webp" };

        private static readonly string UmoyaResourcesHome = Constants.ResourceDirecotryDefaultPath + Constants.PathSeperator + "resources";
        private static string GetLowerCaseFileExtension(string fileName)
        {
            string extension = string.Empty;
            try
            {
                if(fileName.Contains('.'))
                {
                    string[] splitByDot = fileName.Split('.');
                    extension = splitByDot[splitByDot.Length - 1];
                    if (extension.Trim().Length == 0)
                    {
                        throw new Exceptions.ResourceTypeException();
                    }
                    extension =  extension.Trim().ToLower();
                }
            }
            catch(Exception x)
            {
                Logger.Do("GetLowerCaseFileExtension Error " + x.StackTrace);
            }
            return extension;
        }

        public static bool IsCodeFileName(string extension)
        {
            switch (extension)
            {
                case "py":
                case "ipynb":
                case "r":
                case "jar":
                case "mon":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsModelFileName(string extension)
        {
            switch (extension)
            {
                case "pmml":
                case "h5":
                case "pb":
                case "pbtxt":
                case "onnx":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDataFileName(string extension)
        {
            switch (extension)
            {
                case "json":
                case "csv":
                case "png":
                case "jpg":
                case "jpeg":
                case "zip":
                case "txt":
                case "md":
                case "webp":
                    return true;
                default:
                    return false;
            }
        }

        public static ResourceType GetResourceType(string ResourceFileName)
        {
            Logger.Do("GetResourceType start with " + ResourceFileName);
            ResourceType OutType = ResourceType.Other;
            try
            {
                string Extension = GetLowerCaseFileExtension(ResourceFileName);
                if (IsModelFileName(Extension)) OutType = ResourceType.Model;
                else if (IsCodeFileName(Extension)) OutType = ResourceType.Code;
                else if (IsDataFileName(Extension)) OutType = ResourceType.Data;
            }
            catch(Exception e)
            {
                Logger.Do("Error GetResourceType " + e.StackTrace);
            }
            Logger.Do("GetResourceType end with " + OutType);
            return OutType;
        }

        public static bool IsResourceInfoFormatValidWithVersion(string ResourceInfo)
        {
            //Need to check version number x.y.z format as well with regex
            bool Status = false;
            if (ResourceInfo.Contains('@')) Status = true;
            return Status;
        }

        public static bool DoesResourceFileExist(string ResourceFile, out string ResourceName)
        {
            bool Status = false;
            ResourceName = ResourceFile;
            if (File.Exists(ResourceFile))
            {
                FileInfo FInfo = new FileInfo(ResourceFile);
                ResourceName = FInfo.Name;
                Status = true;
            }
            return Status;
        }

        public static string PrepareResourceDependenciesForSpec(ResourceIdentifier ResourceDependencyInfo)
        {
            string DependenciesString = string.Empty;
            string SpecResourceStrig = "<dependency id=\"${DependentResource}\" version=\"${DependentResourceVersion}\" />";
            return SpecResourceStrig.Replace("${DependentResource}", ResourceDependencyInfo.ResourceName).Replace("${DependentResourceVersion}", ResourceDependencyInfo.Version);
        }

        public static string GetResourceVersionFromProject(string ResourceName)
        {
            string VersionOut = Constants.DefaultNoVersionValue;
            XmlDocument doc = new XmlDocument();
            doc.Load(Constants.ResourceProjectDefaultPath);
            XmlNodeList list = doc.DocumentElement.GetElementsByTagName("PackageReference");
            foreach (XmlNode xnode in list)
            {
                string TempResourceName = xnode.Attributes["Include"].Value.ToString();
                string ResourceVersion = xnode.Attributes["Version"].Value.ToString();
                if (TempResourceName.Equals(ResourceName))
                {
                    Logger.Do("Resolving Resource Version " + ResourceVersion);
                    VersionOut = ResourceVersion;
                    break;
                }
            }
            return VersionOut;
        }

        public static async Task<Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>>> GetListOfResourcesAdded(string ResourceProjectPath)
        {
            bool FromRepo=false;
            Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>> ListOfResourceLocallyAdded = new Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>>();
            Dictionary<string, ResourceIdentifier> TempListOfResourceLocallyAdded = new Dictionary<string, ResourceIdentifier>();
            List<ResourceIdentifier> TempListOfDependentResourcesAdded = new List<ResourceIdentifier>();
            ListOfResourceLocallyAdded.Add(ResourceType.Model, new Dictionary<string, ResourceIdentifier>());
            ListOfResourceLocallyAdded.Add(ResourceType.Code, new Dictionary<string, ResourceIdentifier>());
            ListOfResourceLocallyAdded.Add(ResourceType.Data, new Dictionary<string, ResourceIdentifier>());
            ListOfResourceLocallyAdded.Add(ResourceType.Other, new Dictionary<string, ResourceIdentifier>());
            Logger.Do("GetListOfResourcesAdded " + ResourceProjectPath);
            XmlDocument doc = new XmlDocument();
            doc.Load(ResourceProjectPath);
            XmlNodeList list = doc.DocumentElement.GetElementsByTagName("PackageReference");
            foreach (XmlNode xnode in list)
            {
                ResourceIdentifier RId = ResourceIdentifier.Empty;
                string ResourceName = xnode.Attributes["Include"].Value.ToString();
                string ResourceVersion = xnode.Attributes["Version"].Value.ToString();
                ResourceType RType = GetResourceType(ResourceName);
                RId.ResourceName = ResourceName;
                RId.Version = ResourceVersion;
                RId.TypeOfResource = RType;
                XmlDocument ResourceSpecDoc = await GetResourceSpec(RId, FromRepo);
                string ResourceDescription = ResourceSpecDoc.DocumentElement.GetElementsByTagName("description")[0].InnerText;
                string ResourceAuthors = ResourceSpecDoc.DocumentElement.GetElementsByTagName("authors")[0].InnerText;
                RId.Description = ResourceDescription;
                RId.Authors = ResourceAuthors;
                if(!FromRepo) RId.Size = Resources.GetResourceSize(RId.TypeOfResource, RId.ResourceName);
                if (!ListOfResourceLocallyAdded[RType].ContainsKey(ResourceName)) ListOfResourceLocallyAdded[RType][ResourceName] = RId;
                TempListOfDependentResourcesAdded = await GetDependentResources(ResourceName, ResourceVersion, RType, ResourceSpecDoc);
                UpdateDependentResourcesAdded(ref ListOfResourceLocallyAdded, TempListOfDependentResourcesAdded);
            }
            return ListOfResourceLocallyAdded;
        }

        private static void UpdateDependentResourcesAdded(ref Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>> ListOfResources, List<ResourceIdentifier> InterestedResources)
        {
            foreach (ResourceIdentifier RId in InterestedResources)
            {
                if (!ListOfResources[RId.TypeOfResource].ContainsKey(RId.ResourceName)) ListOfResources[RId.TypeOfResource][RId.ResourceName] = RId;
            }
        }
        public static string GetZMODResourceType(ResourceType UmoyaResourceType)
        {
            if (UmoyaResourceType.Equals(ResourceType.Model)) return "Models";
            else if (UmoyaResourceType.Equals(ResourceType.Code)) return "Code";
            else if (UmoyaResourceType.Equals(ResourceType.Data)) return "Data";
            else return "Other";
        }

        public static async Task<List<ResourceIdentifier>> GetDependentResources(ResourceIdentifier ResourceId, bool FromRepo=false)
        {
            return await GetDependentResources(ResourceId.ResourceName, ResourceId.Version, ResourceId.TypeOfResource, await GetResourceSpec(ResourceId, FromRepo), FromRepo);
        }

        public static async Task<List<ResourceIdentifier>> GetDependentResources(string ResourceName, string ResourceVersion, Resources.ResourceType ResourceType, XmlDocument ResourceSpecDoc, bool FromRepo=false)
        {
            Logger.Do("GetDependentResources Started for resource name "  + ResourceName  +  " version " + ResourceVersion  + " Type " + ResourceType + " FromRepo " + FromRepo);
            List<ResourceIdentifier> Out = new List<ResourceIdentifier>();
            try
            {
                Dictionary<string, string> ListOfDependencies = GetListOfDependenciesFromSpec(ResourceSpecDoc);
                if (ListOfDependencies.Count > 0)
                {
                    foreach (string DependentResourceName in ListOfDependencies.Keys)
                    {
                        Logger.Do("DependentResource " + DependentResourceName);
                        XmlDocument DependentResourceSpecDoc;
                        ResourceIdentifier DependentResourceIdentifier = ResourceIdentifier.Empty;  
                        DependentResourceIdentifier.ResourceName = DependentResourceName;
                        DependentResourceIdentifier.Version = ListOfDependencies[DependentResourceName].ToString();                                         
                        DependentResourceIdentifier.TypeOfResource = GetResourceType(DependentResourceName);
                        DependentResourceSpecDoc = await GetResourceSpec(DependentResourceIdentifier, FromRepo);                        
                        DependentResourceIdentifier.Description = DependentResourceSpecDoc.DocumentElement.GetElementsByTagName("description")[0].InnerText;
                        DependentResourceIdentifier.Authors = DependentResourceSpecDoc.DocumentElement.GetElementsByTagName("authors")[0].InnerText;
                        if(!FromRepo) DependentResourceIdentifier.Size = GetResourceSize(DependentResourceIdentifier.TypeOfResource, DependentResourceIdentifier.ResourceName);
                        Out.Add(DependentResourceIdentifier);
                        Out.AddRange(await GetDependentResources(DependentResourceIdentifier, FromRepo));
                    }
                }
                Logger.Do("GetDependentResources End");
            }
            catch (Exception ex)
            {
                Logger.Do(ex.StackTrace);
            }            
            return Out;
        }


        public static async Task<XmlDocument> GetResourceSpec(ResourceIdentifier ResourceId, bool FromRepo=false)
        {
            Logger.Do("GetResourceSpec " + ResourceId.ResourceName + " FromRepo " + FromRepo.ToString());
            XmlDocument ResourceSpec = new XmlDocument();
            if(FromRepo)
            {
                string ResourceSpecEndPoint = GetResourceSpecURLByNameAndVersionFromRepo(ResourceId.ResourceName, ResourceId.Version);
                ResourceSpec.Load(ResourceSpecEndPoint);
            }
            else
            {
                string ResourceSpecFile = Constants.ResourceDirecotryDefaultPath + Constants.PathSeperator + "resources" + Constants.PathSeperator + ResourceId.ResourceName.ToLower() + Constants.PathSeperator + ResourceId.Version + Constants.PathSeperator + "resource-spec.nuspec";
                if (!File.Exists(ResourceSpecFile))
                {
                    ResourceSpecFile = Constants.ResourceDirecotryDefaultPath + Constants.PathSeperator + "resources" + Constants.PathSeperator + ResourceId.ResourceName.ToLower() + Constants.PathSeperator + ResourceId.Version + Constants.PathSeperator + ResourceId.ResourceName.ToLower() + ".nuspec";
                    if (!File.Exists(ResourceSpecFile)) throw new Exception("Resource Spec " + ResourceSpecFile + " is not found");
                }                
                ResourceSpec.Load(ResourceSpecFile);
            }            
            return ResourceSpec;
        }

        public static Dictionary<string, string> GetListOfDependenciesFromSpec(XmlDocument ResourceSpecDoc)
        {
            Logger.Do("GetListOfDependenciesFromSpec Started");
            Dictionary<string, string> ListOfDependencies = new Dictionary<string, string>();
            try
            {
                XmlNodeList XNodeList = ResourceSpecDoc.DocumentElement.GetElementsByTagName("dependency");
                if (XNodeList.Count > 0)
                {
                    for (int i = 0; i < XNodeList.Count; i++)
                    {
                        XmlNode XNode = XNodeList[i];
                        string ResourceName = XNode.Attributes["id"].Value.ToString();
                        string ResourceVersion = XNode.Attributes["version"].Value.ToString();
                        ListOfDependencies.Add(ResourceName, ResourceVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Do(ex.Message);
            }
            Logger.Do("GetListOfDependenciesFromSpec Started");
            return ListOfDependencies;
        }

        public static async Task<bool> IsResourcePresentInRepoAsync(ResourceIdentifier ResourceId)
        {
            string RequestedUrl = string.Empty;
            if (ResourceId.HasVersion()) RequestedUrl = GetEndPointForResourceVersionIndex(ResourceId.ResourceName, ResourceId.Version);
            else RequestedUrl = GetEndPointForResourceIndex(ResourceId);
            Logger.Do("IsResourcePresentInRepoAsync with URL " + RequestedUrl);
            return await RestOps.IsEndPointPresentAsync(RequestedUrl);
        }

        public static string GetEndPointForResourceIndex(ResourceIdentifier ResourceId)
        {
            return GetRepoURL() + "/v3/registration/" + ResourceId.ResourceName + "/index.json";
        }

        public static string GetEndPointForResourceVersionIndex(string ResourceName, string ResourceVersion)
        {
            return GetRepoURL() + "/v3/registration/" + ResourceName + "/" + ResourceVersion + ".json";
        }

        public static IEnumerable<string> GetResourceNames(ResourceType ResouceFileType)
        {
            try
            {
                IEnumerable<string> ListOut;
                if (ResouceFileType.Equals(ResourceType.Code)) ListOut = new DirectoryInfo(ResourceDirPath(ResouceFileType)).EnumerateDirectories().Select(dir => dir.Name);
                else ListOut = new DirectoryInfo(ResourceDirPath(ResouceFileType)).EnumerateFiles().Select(file => file.Name);
                return ListOut;
            }
            catch (System.Exception)
            {
                throw new Exceptions.ResourceTypeInvalidException();
            }
        }

        public static string ResourceDirPath(ResourceType resourceType)
        {
            if (resourceType == ResourceType.Code) return Constants.CodeDirName;
            else if (resourceType == ResourceType.Data) return Constants.DataDirName;
            else if (resourceType == ResourceType.Model) return Constants.ModelDirName;
            else throw new Exceptions.ResourceTypeInvalidException();
        }

        public static async Task<Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>>> GetLocalResourceList(ResourceType TypeOfResource)
        {
            Dictionary<Resources.ResourceType, Dictionary<string, ResourceIdentifier>> ListOfUmoyaResourcesWithMetaData = new Dictionary<Resources.ResourceType, Dictionary<string, ResourceIdentifier>>();
            Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>> ListOfLocalResources = new Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>>();
            List<ResourceType> ListOfResourceTypes = TypeOfResource.Equals(ResourceType.Any) ? new List<ResourceType> { ResourceType.Model, ResourceType.Data, ResourceType.Code } : new List<ResourceType> { TypeOfResource };
            foreach (ResourceType resourceType in ListOfResourceTypes)
            {
                Dictionary<string, ResourceIdentifier> ListOfResourceForSpecificType = new Dictionary<string, ResourceIdentifier>();
                foreach (string resourceName in GetResourceNames(resourceType))
                {
                    string version = Constants.DefaultNoVersionValue;
                    string size = GetResourceSize(resourceType, resourceName);
                    ResourceIdentifier RId = new ResourceIdentifier(resourceName, version, resourceType, size);
                    ListOfResourceForSpecificType[resourceName] = RId;
                    Logger.Do(">> " + RId.ResourceName + " " + RId.Version);
                }
                ListOfLocalResources[resourceType] = ListOfResourceForSpecificType;
            }
            ListOfUmoyaResourcesWithMetaData = await Resources.GetListOfResourcesAdded(Constants.ResourceProjectDefaultPath);
            Resources.UpdateZMODResourceWithMetaData(ListOfUmoyaResourcesWithMetaData, ref ListOfLocalResources);
            return ListOfLocalResources;
        }

        public static void UpdateZMODResourceWithMetaData(Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>> UmoyaResources, ref Dictionary<ResourceType, Dictionary<string, ResourceIdentifier>> ZMODResources)
        {
            Dictionary<string, ResourceIdentifier> Temp;
            ResourceIdentifier TempResourceIdentifier = ResourceIdentifier.Empty;
            foreach (ResourceType TypeOfResource in UmoyaResources.Keys)
            {
                Temp = UmoyaResources[TypeOfResource];
                foreach (string ResourceName in Temp.Keys)
                {
                    TempResourceIdentifier = Temp[ResourceName];
                    if(TempResourceIdentifier.ExistsLocally) ZMODResources[TypeOfResource][ResourceName] = TempResourceIdentifier;
                }
            }
        }

        public static string GetRepoURL()
        {
            //get schema , host and port new UriBuilder(Info.Instance.Source.Url)
            //build repo url that http://localhost:8007
            UriBuilder uri = new UriBuilder(Info.Instance.Source.Url);
            string RequestedUrl = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
            if (uri.Port != 80) RequestedUrl = RequestedUrl + ":" + uri.Port;
            return RequestedUrl;
        }

        public static string GetRepoSearchURL()
        {
            string SearchURL = GetRepoURL() + "/v3/search?semVerLevel=2.0.&prerelease=true";
            return SearchURL;
        }

        public static string GetRepoSearchURLByQuery(string QueryString)
        {
            return RestOps.AppendQueryInEndPoint(GetRepoSearchURL(), "q" , QueryString);
        }

        public static string GetResourceSize(ResourceType TypeOfResource, string ResourceName)
        {
            try
            {
                string ResourcePath = Resources.ResourceDirPath(TypeOfResource) + Constants.PathSeperator + ResourceName;
                if (TypeOfResource.Equals(ResourceType.Code)) ResourcePath = Resources.ResourceDirPath(TypeOfResource) + Constants.PathSeperator + ResourceName + Constants.PathSeperator + ResourceName;
                if (File.Exists(ResourcePath))
                {
                    FileInfo ResourceInfo = new FileInfo(ResourcePath);
                    return FSOps.BytesToString(ResourceInfo.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Do(ex.StackTrace);
            }
            return string.Empty;
        }

        public static bool IsResourcePresentLocally(ResourceType TypeOfResource, string ResourceName)
        {
            bool Status = true;
            try
            {
                string ResourcePath = Resources.ResourceDirPath(TypeOfResource) + Constants.PathSeperator + ResourceName;
                if (TypeOfResource.Equals(ResourceType.Code)) ResourcePath = Resources.ResourceDirPath(TypeOfResource) + Constants.PathSeperator + ResourceName + Constants.PathSeperator + ResourceName;
                Status = File.Exists(ResourcePath);
            }
            catch (Exception ex)
            {
                Logger.Do(ex.StackTrace);
            }
            return Status;
        }
        public static bool GenerateOutputJSONFile(List<Package> RepoItems, List<ResourceIdentifier> LocalItems, string From, string FileName)
        {
            try
            {
                bool Status = true;
                //  System.Console.WriteLine(Path.GetDirectoryName(Path.GetFullPath(FileName)));
               // currentPath = Directory.GetParent(currentPath).FullName;
                //System.Console.WriteLine(Path.GetDirectoryName(currentPath));
                if (Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(FileName))))
                {
                    if (File.Exists(FileName))
                        File.Delete(FileName);
                    using (var tw = new StreamWriter(FileName, true))
                    {
                        tw.Close();
                    }
                    if (From.ToLower() == "repo")
                    {
                        var json = JsonConvert.SerializeObject(RepoItems);
                        File.WriteAllText(FileName, json);
                    }
                    else
                    {
                        var json = JsonConvert.SerializeObject(LocalItems);
                        File.WriteAllText(FileName, json);
                    }
                }
                else
                {
                    Status = false;
                }
                return Status;
            }
            catch (Exception) { return false; }
        }

        public static string GetResourceSpecURLByNameAndVersionFromRepo(string ResourceName, string ResourceVersion)
        {
            //https://hub.zmod.org/v3/package/helloworld.pmml/5.0.0/helloworld.pmml.5.0.0.nuspec
            string Out = GetRepoURL() + "/v3/package/" + ResourceName.ToLower() + "/" + ResourceVersion + "/" + ResourceName.ToLower() + "." + ResourceVersion + ".nuspec";
            Logger.Do("GetResourceSpecURLByNameAndVersionFromRepo " + Out);
            return Out;
        }

        public static string GetResourcePackageURLByNameAndVersionFromRepo(string ResourceName, string ResourceVersion)
        {
            //https://hub.zmod.org/v3/package/helloworld.pmml/5.0.0/helloworld.pmml.5.0.0.nupkg
            string Out = GetRepoURL() + "/v3/package/" + ResourceName.ToLower() + "/" + ResourceVersion + "/" + ResourceName.ToLower() + "." + ResourceVersion + ".nupkg";
            Logger.Do("GetResourceSpecURLByNameAndVersionFromRepo " + Out);
            return Out;
        }

        public static async Task<ResourceIdentifier> GetResourceInfoByNameAndVersionFromRepo(string ResourceName,string ResourceVersion)
        {
            Logger.Do("GetResourceInfoByNameAndVersionFromRepo started , Resource Name " + ResourceName + " Version " + ResourceVersion);
            ResourceIdentifier ResourceId = ResourceIdentifier.Empty; 
            ResourceId.ResourceName = ResourceName;
            ResourceId.Version = ResourceVersion;
            ResourceId.TypeOfResource = GetResourceType(ResourceName);
            XmlDocument ResourceSpecDoc = await GetResourceSpec(ResourceId, true);
            ResourceId.Description = ResourceSpecDoc.DocumentElement.GetElementsByTagName("description")[0].InnerText;
            ResourceId.Authors = ResourceSpecDoc.DocumentElement.GetElementsByTagName("authors")[0].InnerText;
            List<ResourceIdentifier> ListOfDependencies = new List<ResourceIdentifier>();
            ListOfDependencies = await GetDependentResources(ResourceId, true);
            foreach(ResourceIdentifier Temp in ListOfDependencies)
            {
                ResourceId.Dependencies[Temp.ResourceName] = Temp;
            }
            Logger.Do("GetResourceInfoByNameAndVersionFromRepo end " + ResourceId.ToString());
            return ResourceId;
        }

        public static async Task<ResourceIdentifier> GetLatestResourceInfoByNameFromRepo(string ResourceName)
        {
            string EndPointURL = GetRepoSearchURLByQuery(ResourceName);
            ResourceIdentifier ResourceId = ResourceIdentifier.Empty;
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(EndPointURL);
            if (ResponseFromRepo.IsSuccessStatusCode)
            {
                var SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<ListResponse>();
                List<Package> ListOfResources;                    
                ListOfResources = SearchResultSetInListOfResources.Data.ToList();
                Package FirstResouce = ListOfResources.FirstOrDefault();
                ResourceId.ResourceName = FirstResouce.Id;
                ResourceId.Authors = string.Join(",", FirstResouce.Authors);
                ResourceId.Description = FirstResouce.Description;
                ResourceId.TypeOfResource = GetResourceType(ResourceId.ResourceName);
                ResourceId.Version = FirstResouce.Version;
                List<ResourceIdentifier> ListOfDependencies = new List<ResourceIdentifier>();
                ListOfDependencies = await GetDependentResources(ResourceId, true);
                foreach(ResourceIdentifier Temp in ListOfDependencies) ResourceId.Dependencies[Temp.ResourceName] = Temp;
            }
            Logger.Do("GetLatestResourceInfoByNameFromRepo out " + ResourceId.ToString());
            return ResourceId;
        }

        public static bool IsResourceTypeValid(string ResourceTypeString)
        {
            return Enum.IsDefined(typeof(ResourceType), ToTitleCase(ResourceTypeString));
        }
        public static string ToTitleCase(this string s) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
        public static void DownloadResourcesInCache(ResourceIdentifier ResourceId, string DownloadFolderPath)
        {
            string ResourceResourcePath = GetResourcePackageURLByNameAndVersionFromRepo(ResourceId.ResourceName, ResourceId.Version);
            string ResourceResourceFileName = GetResourceCompressFileName(ResourceId);
            if(!Directory.Exists(DownloadFolderPath)) Directory.CreateDirectory(DownloadFolderPath);
            string TempDestinationFile = DownloadFolderPath + Constants.PathSeperator + ResourceResourceFileName;
            Downloader ResourceDownloaderInstance = null;
            if(!File.Exists(TempDestinationFile)) 
            {
                ResourceDownloaderInstance = new Downloader(ResourceResourcePath, TempDestinationFile, true);
                WaitForDownloadCompletion(ref ResourceDownloaderInstance);
            }
            foreach(KeyValuePair<string, ResourceIdentifier> KeyPair in ResourceId.Dependencies)
            {
                ResourceResourcePath = GetResourcePackageURLByNameAndVersionFromRepo(KeyPair.Value.ResourceName, KeyPair.Value.Version);
                ResourceResourceFileName = GetResourceCompressFileName(KeyPair.Value);
                TempDestinationFile = DownloadFolderPath + Constants.PathSeperator + ResourceResourceFileName;
                if(!File.Exists(TempDestinationFile))
                {
                    ResourceDownloaderInstance = new Downloader(ResourceResourcePath, TempDestinationFile);
                    WaitForDownloadCompletion(ref ResourceDownloaderInstance);
                }
            }
        }

        public static void WaitForDownloadCompletion(ref Downloader DownloaderInstance)
        {
            for (; ; )
            {
                System.Threading.Thread.Sleep(100);
                DownloaderInstance.PrintProgressInfo();
                if (DownloaderInstance.Status)
                {
                    DownloaderInstance.CleanProgressInfo();
                    break;
                }
            }
        }

        public static string GetResourceCompressFileName(ResourceIdentifier ResourceId)
        {
            return ResourceId.ResourceName.ToLower() + "." + ResourceId.Version + ".nupkg";
        }
    }
}
