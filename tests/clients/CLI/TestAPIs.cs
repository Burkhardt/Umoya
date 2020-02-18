using System;
using System.Diagnostics;
using System.IO;
using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

namespace Repo.Clients.CLI.Commands.Tests
{
    public class TestAPIs
    {
        public static readonly DirectoryInfo TestCurrentDir = new DirectoryInfo(Environment.CurrentDirectory);
        public static readonly DirectoryInfo SrcDir = TestCurrentDir.Parent.Parent.Parent.Parent.Parent.Parent;
        public static readonly string UmoyaPath = SrcDir.FullName + Constants.PathSeperator + "src" + Constants.PathSeperator + "clients" + Constants.PathSeperator + "CLI" + Constants.PathSeperator + "bin" + Constants.PathSeperator + "release" + Constants.PathSeperator + "netcoreapp2.2" + Constants.PathSeperator + "publish" + Constants.PathSeperator + "umoya.dll";

        public static readonly string RepoPath = SrcDir.FullName + Constants.PathSeperator + "src" + Constants.PathSeperator + "service-components" + Constants.PathSeperator + "Umoya" + Constants.PathSeperator + "bin" + Constants.PathSeperator + "Debug" + Constants.PathSeperator + "netcoreapp2.2" + Constants.PathSeperator + "publish";

        public static readonly string DotNetCommand = "dotnet";
        public static readonly string ArgumentName = "umoya.dll";
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
        }

        public static bool CompareActualAndExpectedOutput(string TestName, string TestScenariosName, out string DiffOutput)
        {
            bool Status = true;
            DiffOutput = "Diff";
            List<string> ExpectedContents = new List<string>().ToList();
            List<string> ActualContents = new List<string>();
            string ExpectedOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "expected-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            ExpectedContents = File.ReadAllLines(ExpectedOutputFilePath).ToList();
            string ActualOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            ActualContents = File.ReadAllLines(ActualOutputFilePath).ToList();
            var DifferenceBetweenExpectedAndActualOutput = ActualContents.Except(ExpectedContents).ToArray();
            if (DifferenceBetweenExpectedAndActualOutput.Length > 0)
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
            if (!IsInitialized)
            {
                if (Directory.Exists(Constants.DefaultTestDataDir))
                {
                    DirectoryInfo parentDirectoryInfo = new DirectoryInfo(Constants.DefaultTestDataDir);
                    ClearReadOnly(parentDirectoryInfo);
                    Directory.Delete(Constants.DefaultTestDataDir, true);
                }
                Directory.CreateDirectory(Constants.DefaultTestDataDir);
                Repository.Clone(TestDataGitHubRepoURL, Constants.DefaultTestDataDir);

                string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp";
                Directory.CreateDirectory(ZMODPath);
                IsInitialized = true;
            }
            return IsInitialized;
        }

        public static bool InitZMOD(string ZMODPath)
        {
            bool Status = true;
            CaptureConsoleOutPut("init", string.Empty, ZMODPath, string.Empty);
            return Status;
        }

        public static bool IsRepoRunning()
        {
            //ZMOD is configured under ZMODPath
            bool Status = false;            //
            if (RepoProcess != null)
            {
                if (!RepoProcess.HasExited) Status = true;
            }
            return Status;
        }

        public static bool StartRepo()
        {
            //Start Repo 
            try
            {
                System.Console.WriteLine("SurbhiRepoPath "+RepoPath);
                RepoProcess = new Process();
                RepoProcess.StartInfo.FileName = DotNetCommand;
                RepoProcess.StartInfo.Arguments = ArgumentName;
                RepoProcess.StartInfo.UseShellExecute = false;
                RepoProcess.StartInfo.CreateNoWindow = true;
                RepoProcess.StartInfo.WorkingDirectory = RepoPath;
                RepoProcess.Start();
                System.Console.WriteLine("Process started");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool StopRepo()
        {
            if (IsRepoRunning())
                RepoProcess.Kill();
            //Check Repo is running then do stop
            return true;
        }

        public static bool SetUpRepoServer()
        {
            bool Status = true;
            string SourcePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "repo-resources";
            string DestinationPath = RepoPath;
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);

            return Status;
        }
    }
}
