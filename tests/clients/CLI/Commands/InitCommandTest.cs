using System;
using Xunit;
using System.IO;
using Xunit.Extensions.Ordering;
using Xunit.Abstractions;
//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace Repo.Clients.CLI.Commands.Tests
{
    public class InitCommandTest
    {
        public static string TestName = "InitCommand";
        private readonly ITestOutputHelper output;
        public InitCommandTest(ITestOutputHelper output)
        {
            this.output = output;
            //TestAPIs.StopRepo();
            #region Check and start the repo server, if not already started.
            if (!TestAPIs.IsRepoRunning())
            {
                //UnZip publish folder in umoya repo bin folder
                Assert.True(TestAPIs.UnZipPublishFolder(), "Failed to exract publish folder to get binary file.");
                TestAPIs.StartRepo();
            }
            #endregion
            //If not then need to start repo.
            //Need to setup baseline in repo publish folder.
            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
        }
        #region Umoya first time initializing
        //Baseline   
        //1. Blank folder
        //Zmod is not configured..
        //capture the output to run the command  umoya init 
        // To initialize ZMOD here, Use command : umoya init

       // [Fact, Order(1)]
        //[Trait("Category", "Init")]
        public void WithBlankFolderTest()
        {
            output.WriteLine("WithBlankFolderTest");
            #region SetUp
            string TestScenariosName = "WithBlankFolderTest";
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region Capturing Expected output
            string ExpectedOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "expected-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            string ExpectedContents = File.ReadAllText(ExpectedOutputFilePath);
            File.WriteAllText(ExpectedOutputFilePath, ExpectedContents);
            #endregion

            #region Capturing Expected output
            string ActualContentFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            string ActualContents = File.ReadAllText(ActualContentFilePath);
            TestAPIs.CaptureConsoleOutPut("init", string.Empty, ZMODPath, ActualContentFilePath);
            File.WriteAllText(ActualContentFilePath, ActualContents);
            #endregion

            #region Check difference between expected and actual output
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff), "Action output is not matched with expected one. Diff : " + OutputDiff);
            #endregion
TestAPIs.StopRepo();
            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            //TestAPIs.StopRepo();
            #endregion
        }
        #endregion
        //BaseLine
        //Assumption : Zmod is configured.
        //Check for the Directory Exists 
        //Capture and compare output ->
        #region Umoya already present or initialized
        //[Fact, Order(2)]
       // [Trait("Category", "Init")]
        public void WithTempFolderExistingTest()
        {
            output.WriteLine("WithTempFolderExistingTest");
            #region Setup
            string TestScenariosName = "WithTempFolderExistingTest";
            //Need to do Umoya Repo Baseline setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region Capturing Expected output
            string ExpectedOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "expected-output" +
             Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            string ExpectedContents = File.ReadAllText(ExpectedOutputFilePath);
            File.WriteAllText(ExpectedOutputFilePath, ExpectedContents);
            #endregion

            #region Check difference between expected and actual output
            string ActualContentFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" +
            Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            string ActualContents = File.ReadAllText(ActualContentFilePath);
            TestAPIs.CaptureConsoleOutPut("init", string.Empty, ZMODPath, ActualContentFilePath);
            #endregion

            #region Check difference between expected and actual output
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff), "Action output is not matched with expected one. Diff : " + OutputDiff);
            #endregion

            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }
        #endregion
    }
}