using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Repo.Clients.CLI
{
    public class SourceInfo
    {
        public string Url { get; set; }
        public string Accesskey { get; set; }
        public SourceInfo(string SourceURL, string SourceAccessKey)
        {
            this.Url = SourceURL;
            this.Accesskey = SourceAccessKey;
        }

        public SourceInfo()
        {
            this.Url = Constants.DefaultSourceURL;
            this.Accesskey = Constants.DefaultSourceAccessKey;
        }
    }
    public class Info
    {
        private static Info CurrentInstance;
        public static Info Instance
        {
            get
            {
                if (CurrentInstance == null) CurrentInstance = new Info();
                return CurrentInstance;
            }
        }
        public string UmoyaHome { get; set; }
        public string ZmodHome { get; set; }
        public string Owner { get; set; }
        public string Version { get; set; }
        public SourceInfo Source { get; set; }
        public bool ISDebugging = false;

        public Info(string ZMODPath)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(File.ReadAllText(ZMODPath));
            UmoyaHome = jsonObject.UmoyaHome;
            ZmodHome = jsonObject.ZmodHome;
            Owner = jsonObject.Owner;
            Version = jsonObject.Version;
            SourceInfo src = new SourceInfo();
            if (jsonObject.Source.Url != null) src.Url = jsonObject.Source.Url;
            if (jsonObject.Source.Accesskey != null) src.Accesskey = jsonObject.Source.Accesskey;
            this.Source = src;
            this.ISDebugging = jsonObject.ISDebugging;
        }

        public Info()
        {
            Load();
        }
        public Info(string umoya, string zmode, string owner, string accesskey, string url)
        {
            if (!String.IsNullOrEmpty(umoya))
                UmoyaHome = umoya;
            if (!String.IsNullOrEmpty(zmode))
                ZmodHome = zmode;
            if (!String.IsNullOrEmpty(owner))
                Owner = owner;
            Version = GetVersion();
            SourceInfo src = new SourceInfo();
            if (!String.IsNullOrEmpty(accesskey))
                src.Accesskey = accesskey;
            if (!String.IsNullOrEmpty(url))
                src.Url = url;
            this.Source = src;
        }
        private static string GetVersion()
                => typeof(Info).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

      
        public static bool CreateFile(string umoyaHome, string zmodHome, string owner, string sourceUrl, string accessKey)
        {
            bool status = false;
            Info info = new Info(umoyaHome, zmodHome, owner, null, null);
            string JSONresult = SerializeFile(info);
            string path = Constants.ConfigFileName;
            if (!File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.Close();
                    status = true;
                }
            }
            return status;
        }
        public static bool UpdateFileForInit(string umoyaHome, string zmodHome, string owner)
        {
            bool status = false;
            try
            {
                Info info = new Info(umoyaHome, zmodHome, owner, null, null);
                string JSONresult = JsonConvert.SerializeObject(info);
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(JSONresult);
                jsonObject.UmoyaHome = umoyaHome;
                jsonObject.Owner = owner;
                var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                // Logger.Do(modifiedJsonString);
                File.WriteAllText(Constants.ConfigFileName, modifiedJsonString);
                status = true;
            }
            catch
            {
            }
            return status;
        }
        private static string SerializeFile(Info info)
        {
            //Logger.Do(info.UmoyaHome);
            string JSONresult = JsonConvert.SerializeObject(info);
            return JSONresult;
        }

        private void Load()
        {
            try
            {
                if (Console.IsZMODConfigured())
                {
                    string path = Constants.ConfigFileName;
                    dynamic jsonObject = JsonConvert.DeserializeObject(File.ReadAllText(Constants.ConfigFileName));
                    UmoyaHome = jsonObject.UmoyaHome;
                    ZmodHome = jsonObject.ZmodHome;
                    Owner = jsonObject.Owner;
                    Version = jsonObject.Version;
                    SourceInfo src = new SourceInfo();
                    if (jsonObject.Source.Url != null) src.Url = jsonObject.Source.Url;
                    if (jsonObject.Source.Accesskey != null) src.Accesskey = jsonObject.Source.Accesskey;
                    this.Source = src;
                    this.ISDebugging = jsonObject.ISDebugging;
                }
                else
                {
                    UmoyaHome = Constants.UmoyaDefaultHome;
                    ZmodHome = Constants.ZmodDefaultHome;
                    Owner = Constants.AuthorDefault;
                    Version = Console.GetVersion();
                    this.Source = new SourceInfo();
                }
            }
            catch (Exception ex)
            {
                Logger.Do(ex.Message);
                Logger.Do("Switch to default settings.");
            }
        }

        public void ReLoad()
        {
            Load();
        }
       
    }
}