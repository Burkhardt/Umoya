
using System;
using Xunit;
using System.IO;
using Xunit.Extensions.Ordering;
//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace Repo.Clients.CLI.Commands.Tests
{
    public class AllTests
    {
        [Fact, Order(1)]
        [Trait("Category", "TestCase")]
        public void Setup()
        {
            //Check repo is running
            //If not then need to start repo.
            //Need to setup baseline in repo publish folder.
            //  Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            #region Check and start the repo server, if not already started.
            TestAPIs.StopRepo();
            //if (Directory.Exists(Constants.DefaultTestDataDir))
           // FSOps.DeleteDirectory(Constants.DefaultTestDataDir);
            if (!TestAPIs.IsRepoRunning())
            {
                //UnZip publish folder in umoya repo bin folder
                Assert.True(TestAPIs.UnZipPublishFolder(), "Failed to exract publish folder to get binary file.");

               // System.Console.WriteLine("Repo is not started. Need to start Repo.");

            }
            #endregion
            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
            Assert.True(TestAPIs.StartRepo(), "Repo server could not be started.");

        }
        #region Umoya Info with default configurations

        // BaseLine
        //  1. ZMOD is initialized.
        // If not Run umoya init 
        // 2. Capture the Expected Output
        #endregion
        [Fact, Order(2)]
        [Trait("Category", "TestCase")]
        public void WithDefaultConfigurationsPresentInfoTest()
        {
            string TestName = "InfoCommand";
            #region Setup
            string TestScenariosName = "WithDefaultConfigurationsPresentInfoTest";
            //Need to do Umoya Repo Baseline setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region Capture expected output
            string ExpectedOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "expected-output" +
            Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            string ExpectedContents = File.ReadAllText(ExpectedOutputFilePath);
            ExpectedContents = ExpectedContents.Replace("${UmoyaHome}", ZMODPath + Constants.PathSeperator + ".umoya");
            ExpectedContents = ExpectedContents.Replace("${ZMODHome}", ZMODPath).Replace("${Owner}", Constants.OwnerAsCurrentUser)
            .Replace("${Version}", Console.GetVersion()).Replace('\n', ' ').Replace('\t', ' ').Replace('\r', ' ').Replace("  ", string.Empty)
            .Replace("   ", string.Empty).Replace(" ", string.Empty);
            File.WriteAllText(ExpectedOutputFilePath, ExpectedContents);
            #endregion

            #region Capture Actual content for info command
            string ActualContentFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" +
            Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt";
            TestAPIs.CaptureConsoleOutPut("info", string.Empty, ZMODPath, ActualContentFilePath);

            string ActualContents = File.ReadAllText(ActualContentFilePath);
            ActualContents = ActualContents.Replace('\n', ' ').Replace('\t', ' ').Replace('\r', ' ').Replace("  ", string.Empty)
            .Replace("   ", string.Empty).Replace(" ", string.Empty);
            File.WriteAllText(ActualContentFilePath, ActualContents);
            #endregion
            #region Check difference between expected and Actual content captured from above

            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff),
            "Action output is not matched with expected one. Diff : " + OutputDiff);
            #endregion
            #region Clean up
            #endregion
            #region Stop Server
            #endregion
        }
        [Fact, Order(3)]
        [Trait("Category", "TestCase")]
        public void ResourceDeletedInLocalTest()
        {
             string TestName = "DeleteCommand";
            #region Setup
            string TestScenariosName = "ResourceDeletedInLocalTest";
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" +
            Constants.PathSeperator + Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region to add a repo resource to local..
            string ResourceToAdd = "HelloWorld.pmml";
            string ResourceVersion = "1.0.0";
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath,
            Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator +
            TestName + Constants.PathSeperator + "ResourcePresentInLocalTest" + ".txt");
            #endregion

            #region to delete local resources that were added in above step and capture output..
            string ResourceToDelete = ResourceToAdd + "@" + ResourceVersion;
            TestAPIs.CaptureConsoleOutPut("delete", ResourceToDelete, ZMODPath, Constants.DefaultTestDataDir +
             Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator +
             TestScenariosName + ".txt");
            string OutputDiff = string.Empty;
            Assert.True(TestAPIs.CompareActualAndExpectedOutput(TestName, TestScenariosName, out OutputDiff),
            "Action output is not matched with expected one. Diff : " + OutputDiff);
            #endregion

            #region Verify resource is placed into local ZMOD directory
            string ResourceExpectedPath = ZMODPath + Constants.PathSeperator + Constants.ModelDirName + Constants.PathSeperator + ResourceToDelete;
            Assert.True(!File.Exists(ResourceExpectedPath), "Model Resource is not found in ZMOD after the  ADD command is run.");
            #endregion
            //TestAPIs.StopRepo();
            #region Clean up
            //FSOps.DeleteDirectory(Constants.DefaultTestDataDir);
            #endregion
        }
        [Fact, Order(4)]
        [Trait("Category", "TestCase")]
        public void CleanUp()
        {
            TestAPIs.StopRepo();
            FSOps.DeleteDirectory(Constants.DefaultTestDataDir);
        }
    }
}