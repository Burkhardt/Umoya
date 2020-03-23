using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO.Compression;
using System.Reflection;

namespace Repo.Clients.CLI.Commands.Tests
{
    public class TestAPIs
    {
        public static readonly DirectoryInfo TestCurrentDir = new DirectoryInfo(Environment.CurrentDirectory);
        public static readonly DirectoryInfo SrcDir = TestCurrentDir.Parent.Parent.Parent.Parent.Parent.Parent;
        public static readonly string UmoyaPath = SrcDir.FullName + Constants.PathSeperator + "src" + Constants.PathSeperator + "clients" + Constants.PathSeperator + "CLI" + Constants.PathSeperator + "bin" + Constants.PathSeperator + "release" + Constants.PathSeperator + "netcoreapp2.2" + Constants.PathSeperator + "publish" + Constants.PathSeperator + "umoya.dll";

        public static readonly string RepoPath = SrcDir.FullName + Constants.PathSeperator + "src" + Constants.PathSeperator + "service-components" + Constants.PathSeperator + "Umoya" + Constants.PathSeperator + "bin" + Constants.PathSeperator + "Debug" + Constants.PathSeperator + "netcoreapp2.2";

        public static readonly string DotNetCommand = "dotnet";
        public static readonly string UmoyaBinaryFileName = "umoya.dll";
        public static Process RepoProcess;

        public static readonly string TestDataGitHubRepoURL = "https://github.com/nimeshgit/umoya-testdata";

        public static bool IsInitialized = false;
        public static void CaptureConsoleOutPut(string ActionName, string ParamString, string WorkingDir, string ActualoutputFilePath)
        {
            var process = new Process();
            process.StartInfo.FileName = DotNetCommand;
            process.StartInfo.Arguments = UmoyaPath + " " + ActionName + " " + ParamString;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = WorkingDir;
            process.Start();
            if (!ActualoutputFilePath.Equals(string.Empty)) File.WriteAllText(ActualoutputFilePath, process.StandardOutput.ReadToEnd());


            process.WaitForExit();
            // string output = process.StandardOutput.ReadToEnd();
            //process.WaitForExit(10);

        }
        public static bool CompareActualAndExpectedOutput(string TestName, string TestScenariosName, out string DiffOutput)
        {
            bool Status = true;
            DiffOutput = "Diff";

            string ExpectedContents = string.Empty;
            string ActualContents = string.Empty;

            string ExpectedOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "expected-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            ExpectedContents = string.Join(",", File.ReadAllLines(ExpectedOutputFilePath).ToList())
            .Replace('\n', ' ').Replace('\t', ' ').Replace('\r', ' ').Replace("  ", string.Empty).Replace("   ", string.Empty)
            .Replace(" ", string.Empty);

            string ActualOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            ActualContents = string.Join(",", File.ReadAllLines(ActualOutputFilePath).ToList())
           .Replace('\n', ' ').Replace('\t', ' ').Replace('\r', ' ').Replace("  ", string.Empty).Replace("   ", string.Empty)
           .Replace(" ", string.Empty);

            var DifferenceBetweenExpectedAndActualOutput = string.Equals(ActualContents, ExpectedContents);
            System.Console.WriteLine("ActualContents " + ActualContents);
            System.Console.WriteLine("ExpectedContents " + ExpectedContents);
            if (!DifferenceBetweenExpectedAndActualOutput)
            {
                Status = false;
                DiffOutput = string.Join(',', DifferenceBetweenExpectedAndActualOutput);
            }
            return Status;
        }
        private static void ClearReadOnly(DirectoryInfo parentDirectory)
        {
            if (parentDirectory != null)
            {
                parentDirectory.Attributes = FileAttributes.Normal;
                foreach (FileInfo fi in parentDirectory.GetFiles())
                {
                    fi.Attributes = FileAttributes.Normal;
                }
                foreach (DirectoryInfo di in parentDirectory.GetDirectories())
                {
                    ClearReadOnly(di);
                }
            }
        }
        public static bool InitializeTestDataSetup()
        {
            if (IsInitialized) return true;
            try
            {
                DirectoryInfo parentDirectoryInfo = new DirectoryInfo(Constants.DefaultTestDataDir);
                if (Directory.Exists(Constants.DefaultTestDataDir))
                {
                    ClearReadOnly(parentDirectoryInfo);
                    Directory.Delete(Constants.DefaultTestDataDir, true);
                }
                Directory.CreateDirectory(Constants.DefaultTestDataDir);
                //CloneGitRepository(Environment.CurrentDirectory, TestDataGitHubRepoURL);
                CloneGitRepository(Environment.CurrentDirectory+Constants.PathSeperator+ "umoya-testdata");
                string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp";
                Directory.CreateDirectory(ZMODPath);
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                Logger.Do(ex.StackTrace);
                IsInitialized = false;
            }
            return IsInitialized;
        }

        public static bool InitZMOD(string ZMODPath)
        {
            bool Status = true;
            CaptureConsoleOutPut("init", string.Empty, ZMODPath, string.Empty);
            CaptureConsoleOutPut("info", "-sp False", ZMODPath, string.Empty);
            return Status;
        }

        public static bool IsRepoRunning()
        {
            bool Status = false;
            int StartingPort = 8007;
            try
            {
                IPEndPoint[] endPoints;
                List<int> PortArray = new List<int>();
                IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
                // getting active connections 
                TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
                PortArray.AddRange(from n in connections
                                   where n.LocalEndPoint.Port == StartingPort
                                   select n.LocalEndPoint.Port);

                // getting active tcp listeneres - wcf service Listening in tcp
                endPoints = properties.GetActiveTcpListeners();
                PortArray.AddRange(from n in endPoints
                                   where n.Port == StartingPort
                                   select n.Port);
                Status = (PortArray.Count > 0);
            }
            catch (SocketException ex)
            {
                System.Console.WriteLine(ex.StackTrace);
            }
            return Status;
        }


        public static bool CloneGitRepository(string DirectoryPathToClone)
        {
            try
            {
                string ZipFileName = "umoya-testdata.zip";
                string ZipFilePath = DirectoryPathToClone + Constants.PathSeperator + ZipFileName;
                Extract(DirectoryPathToClone, ZipFileName);
                ZipFile.ExtractToDirectory(ZipFilePath, DirectoryPathToClone);
                File.Delete(ZipFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Do("Error while starting repo: " + ex.StackTrace);
                return false;
            }
        }

 public static void Extract(string outDirectory,  string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var resNames =assembly.GetManifestResourceNames();
            using (Stream s = assembly.GetManifestResourceStream(resNames.ToList()[0]))
            {
                using (BinaryReader r = new BinaryReader(s))
                using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
                using (BinaryWriter w = new BinaryWriter(fs))
                    w.Write(r.ReadBytes((int)s.Length));
            }
        }
        public static bool StartRepo()
        {
            //Start Repo 
            bool Status = true;
            try
            {
                System.Console.WriteLine(RepoPath + Constants.PathSeperator + "publish" + Constants.PathSeperator + UmoyaBinaryFileName);
                if (File.Exists(RepoPath + Constants.PathSeperator + "publish" + Constants.PathSeperator + UmoyaBinaryFileName))
                {
                    System.Console.WriteLine("Starting Repo from " + RepoPath + Constants.PathSeperator + "publish" + Constants.PathSeperator + UmoyaBinaryFileName + "  is found.");
                    RepoProcess = new Process();
                    RepoProcess.StartInfo.FileName = DotNetCommand;
                    RepoProcess.StartInfo.Arguments = UmoyaBinaryFileName;
                    RepoProcess.StartInfo.UseShellExecute = false;
                    RepoProcess.StartInfo.CreateNoWindow = true;
                    RepoProcess.StartInfo.WorkingDirectory = RepoPath + Constants.PathSeperator + "publish";
                    RepoProcess.Start();
                }
                else
                {
                    System.Console.WriteLine("UmoyaBinaryFile " + RepoPath + Constants.PathSeperator + "publish" + Constants.PathSeperator + UmoyaBinaryFileName + "  not found.");
                    Status = false;
                }
            }
            catch (Exception ex)
            {
                RepoProcess.Kill();
                System.Console.Error.WriteLine("Not able to start repo " + ex.StackTrace);
                Status = false;
            }
            return Status;
        }
        public static bool StopRepo()
        {
            if (IsRepoRunning())
            {
                var prc = new ProcManager();
                prc.KillByPort(8007);
            }
            //RepoProcess.Kill();
            //Check Repo is running then do stop
            return true;
        }

        public static bool SetUpRepoServer()
        {
            bool Status = true;
            try
            {
                string SourcePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "repo-resources";
                string DestinationPath = RepoPath + Constants.PathSeperator + "publish";
                //Now Create all of the directories
                System.Console.WriteLine("SetUpRepoServer " + SourcePath + "  " + DestinationPath);
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                {
                    System.Console.WriteLine("dirPath: " + dirPath);
                    string TempDirectory = dirPath.Replace(SourcePath, DestinationPath);
                    System.Console.WriteLine("Creating Directory (TempDirectory)" + TempDirectory);
                    if (Directory.Exists(TempDirectory))
                        Directory.Delete(TempDirectory, true);
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
                    System.Console.WriteLine(dirPath.Replace(SourcePath, DestinationPath));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                {
                    System.Console.WriteLine("newPath: " + newPath);
                    System.Console.WriteLine("Replaced newPath: " + newPath.Replace(SourcePath, DestinationPath));
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
                }
            }
            catch (Exception ex)
            {
                Logger.Do(ex.StackTrace);
                System.Console.WriteLine("error " + ex.StackTrace);
            }
            return Status;
        }

        public static bool UnZipPublishFolder()
        {
            bool Status = true;
            string startPath = TestAPIs.RepoPath;
            string zipPath = "publish.zip";
            string extractPath = startPath + Constants.PathSeperator + "publish";
            var unzip = new Unzip(startPath + Constants.PathSeperator + zipPath);
            try
            {
                System.Console.WriteLine(" Zip folder path: " + startPath + Constants.PathSeperator + zipPath);
                if (File.Exists(startPath + Constants.PathSeperator + zipPath))
                {
                    // ZipFile.CreateFromDirectory(startPath, zipPath);
                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }

                    ZipFile.ExtractToDirectory(startPath + Constants.PathSeperator + zipPath, extractPath);
                    System.Console.WriteLine("Extracted folder to " + extractPath);
                }
            }
            catch (Exception ex)
            {
                unzip.Dispose();
                Logger.Do(ex.StackTrace);
            }

            return Status;

        }
        private static void ListFiles(Unzip unzip)
        {
            var tab = unzip.Entries.Any(e => e.IsDirectory) ? "\t" : string.Empty;

            foreach (var entry in unzip.Entries.OrderBy(e => e.Name))
            {
                if (entry.IsFile)
                {
                    //System.Console.WriteLine(tab + "{0}: {1} -> {2}", entry.Name, entry.CompressedSize, entry.OriginalSize);
                    continue;
                }

                //System.Console.WriteLine(entry.Name);
            }

            //System.Console.WriteLine();
        }

    }
}
