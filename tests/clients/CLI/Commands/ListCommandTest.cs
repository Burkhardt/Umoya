using System;
using Xunit;
using System.IO;
using System.Diagnostics;
using Repo.Clients.CLI;
using System.Collections.Generic;

namespace Repo.Clients.CLI.Commands.Tests
{
    public class ListCommandTest
    {
        public static string TestName = "ListCommand";
        public ListCommandTest()
        {
            //Check repo is running
            //If not then need to start repo.
            //Need to setup baseline in repo publish folder.
            // Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            #region Check and start the repo server, if not already started.
            if (!TestAPIs.IsRepoRunning())
            {//UnZip publish folder in umoya repo bin folder
                Assert.True(TestAPIs.UnZipPublishFolder(), "Failed to exract publish folder to get binary file.");
                Assert.True(TestAPIs.StartRepo(), "Repo server could not be started.");
            }
            #endregion

            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
        }

        #region Umoya list local resources with already added resources (baseline)
        //Baseline    
        //1. Resource HelloWorld.pmml@1.0.0 , HelloWorldData.csv@1.0.0, HelloWorldCode.ipynb@1.0.0 is present in Repo
        //2. User has initialized ZMOD.    Folders are created in ZMODHome/Models , Code  Data 
        //3. Captured expected output.      
        #endregion
        [Fact]
        public void LocalListWithBaselineResourcesTest()
        {
            string TestScenariosName = "LocalListWithBaselineResourcesTest";
            //Check the ResourceType  Code/Data/Models() 

            #region Setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region to add a repo resource to local and capture output to file..
            string ResourceToAdd = "HelloWorldCode.ipynb";
            string ResourceVersion = "1.0.0";
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");

            ResourceToAdd = "HelloWorld.pmml";
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");

            ResourceToAdd = "HelloWorldData.csv";
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");

            #endregion

            #region verify the resources available on local ans store in expected output file
            string ExpectedOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "expected-output" +
             Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            string ExpectedContents = File.ReadAllText(ExpectedOutputFilePath);
            #endregion

            #region Writing actual content to output file
            string ActualContentFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" +
            Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            TestAPIs.CaptureConsoleOutPut("list", string.Empty, ZMODPath, ActualContentFilePath);
            string ActualContents = File.ReadAllText(ActualContentFilePath);
            #endregion

            #region Check if there is any difference between expected output file and actual output file
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff), "Action output is not matched with expected one. Diff : " + OutputDiff);
            #endregion

            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
            #region Stop Server
            //TestAPIs.StopRepo();
            #endregion stop server

        }

        #region Umoya list repo resources with baseline resources
        //Baseline    
        //1. Repo has HelloWorld.pmml@1.0.0, HelloWorldCode.ipynb@1.0.0 and HelloWorldData.csv@1.0.0

       // [Fact]
        public void RepoListWithBaselineResourcesTest()
        {
            #region Setup
            string TestScenariosName = "RepoListWithBaselineResourcesTest";
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region verify the resources available on repo. Capture in expected output file.
            string ExpectedOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "expected-output" +
            Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            string ExpectedContents = File.ReadAllText(ExpectedOutputFilePath);
            #endregion

            #region Capture in actual output file.
            string list = "list" + " " + "-f" + " " + "repo";
            string ActualContentFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" +
            Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            TestAPIs.CaptureConsoleOutPut(list, string.Empty, ZMODPath, ActualContentFilePath);
            string ActualContents = File.ReadAllText(ActualContentFilePath);
            #endregion

            #region Check difference between expected and actual output file
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff), "Action output is not matched with expected one. Diff : " + OutputDiff);
            #endregion

            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
            #region Stop Server
            //TestAPIs.StopRepo();
            #endregion stop server
        }
        #endregion
    }
}