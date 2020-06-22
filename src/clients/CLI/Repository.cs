using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Repo.Clients.CLI.Commands
{
    public static class Repository
    {
        public static bool Setup(string UmoyaHome)
        {
            bool Status = false;
            string ZipFileName = "CLI.EmbeddedResources.umoya-resources.zip";
            string ZipFilePath =  Constants.ZmodDefaultHome + Constants.PathSeperator + ZipFileName;
            if (!File.Exists(ZipFilePath))
            {
                Console.LogLine("Repository template not found here : " + ZipFilePath);
                WriteResourceToFile();
            }
            if (File.Exists(ZipFilePath))
            {
                EmbeddedResources.Extract(UmoyaHome, ZipFileName);
                ZipFile.ExtractToDirectory(ZipFilePath, UmoyaHome);
                File.Delete(ZipFilePath);
                Status = true;
            }
            Console.LogLine("Repository.Setup " + Status);
            return Status;
        }

        private static void WriteResourceToFile()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                foreach (string filename in names)
                {
                    Logger.Do("Embeded resource " + filename);
                    var stream = assembly.GetManifestResourceStream(filename);
                    var rawFile = new byte[stream.Length];
                    stream.Read(rawFile, 0, (int)stream.Length);
                    using (var fs = new FileStream(filename, FileMode.Create))
                    {
                        fs.Write(rawFile, 0, (int)stream.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.StackTrace);
            }
        }
    
    }
}
