using System;
using Xunit;
using System.IO;

namespace Repo.Clients.CLI.Commands.Tests
{
    public class AddCommandTest
    {

        public static string TestName = "AddCommand";
        public AddCommandTest()
        {
            #region Check and start the repo server, if not already started.
            if (!TestAPIs.IsRepoRunning())
            { //UnZip publish folder in umoya repo bin folder
                Assert.True(TestAPIs.UnZipPublishFolder(), "Failed to exract publish folder to get binary file.");
                Assert.True(TestAPIs.StartRepo(), "Repo server could not be started.");
            }
            #endregion

            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
        }

        #region Umoya add model resource type which is present in repo
        //Baseline 
        //1. Resource HelloWorld.pmml@1.0.0 is present in Repo
        //2. User has initialized ZMOD.
        //3. Captured expected output.       
        [Fact]
        #endregion
        public void ResourceModelPresentInRepoTest()
        {
            #region Setup
            string TestScenariosName = "ResourceModelPresentInRepoTest";
            //Need to do Umoya Repo Baseline setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion
            #region Run command and capture output
            string ResourceToAdd = "HelloWorld.pmml";
            string ResourceVersion = "1.0.0";
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");
            #endregion
            #region Compare outpt with baseline output
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff), "Action output is not matched with expected one. Diff : " + OutputDiff);
            #endregion
            #region Verify resource is placed into local ZMOD directory
            string ResourceExpectedPath = ZMODPath + Constants.PathSeperator + Constants.ModelDirName + Constants.PathSeperator + ResourceToAdd;
            Assert.True(File.Exists(ResourceExpectedPath), "Model Resource is not found in ZMOD after the  ADD command is run.");
            #endregion
            TestAPIs.StopRepo();
            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }


        #region Umoya add code resource type which is present in repo
        //Baseline 
        //1. Resource HelloWorldCode.ipynb@1.0.0 is present in Repo
        //2. User has initialized ZMOD.
        //3. Captured expected output.       
       [Fact]
        #endregion
        public void ResourceCodePresentInRepoTest()
        {
            #region Setup
            string TestScenariosName = "ResourceCodePresentInRepoTest";
            //Need to do Umoya Repo Baseline setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion
            string ResourceToAdd = "HelloWorldCode.ipynb";
            string ResourceVersion = "1.0.0";
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff), "Action output is not matched with expected one. Diff : " + OutputDiff);
            string ResourceExpectedPath = ZMODPath + Constants.PathSeperator + Constants.CodeDirName + Constants.PathSeperator + ResourceToAdd + Constants.PathSeperator + ResourceToAdd;
            Assert.True(File.Exists(ResourceExpectedPath), "Code Resource is not found in ZMOD after the  ADD command is run.");
            TestAPIs.StopRepo();

            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }

        #region Umoya add data resource type which is present in repo
        //Baseline 
        //1. Resource HelloWorldData.csv@1.0.0 is present in Repo
        //2. User has initialized ZMOD.
        //3. Captured expected output.       
        [Fact]
        #endregion
        public void ResourceDataPresentInRepoTest()
        {
            #region Setup
            string TestScenariosName = "ResourceDataPresentInRepoTest";
            //Need to do Umoya Repo Baseline setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion
            string ResourceToAdd = "HelloWorldData.csv";
            string ResourceVersion = "1.0.0";
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff), "Action output is not matched with expected one. Diff : " + OutputDiff);
            string ResourceExpectedPath = ZMODPath + Constants.PathSeperator + Constants.DataDirName + Constants.PathSeperator + ResourceToAdd;
            Assert.True(File.Exists(ResourceExpectedPath), "Data Resource is not found in ZMOD after the  ADD command is run.");
           TestAPIs.StopRepo();
            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }
    }
}