
using System;
using Xunit;
using System.IO;
using Xunit.Extensions.Ordering;
using System.Linq;
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
        [Trait("Category", "UmoyaTestCase")]
        //Setup for test cases
        public void Setup()
        {
            //Check repo is running
            //If not then need to start repo.
            //Need to setup baseline in repo publish folder.
            //  Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            #region Check and start the repo server, if not already started.
            if (!TestAPIs.IsRepoRunning())
            {
                TestAPIs.StopRepo();
                //UnZip publish folder in umoya repo bin folder
                Assert.True(TestAPIs.UnZipPublishFolder(), "Failed to exract publish folder to get binary file.");
            }
            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
            TestAPIs.StartRepo();
            #endregion
        }
        #region Umoya first time initializing
        //Baseline   
        //1. Blank folder
        //Zmod is not configured..
        //capture the output to run the command  umoya init 
        // To initialize ZMOD here, Use command : umoya init
        //[Fact, Order(2)]
        //[Trait("Category", "UmoyaTestCase")]
        public void WithBlankFolderTest()
        {
            string TestName = "InitCommand";
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
            //TestAPIs.StopRepo();
            #region Clean up
            //FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }
        #endregion
        [Fact, Order(2)]
        [Trait("Category", "UmoyaTestCase")]
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
        }
        #region Umoya add model resource type which is present in repo
        //Baseline 
        //1. Resource HelloWorld.pmml@1.0.0 is present in Repo
        //2. User has initialized ZMOD.
        //3. Captured expected output.       
        // [Fact]
        #endregion
        [Fact, Order(3)]
        [Trait("Category", "UmoyaTestCase")]
        public void ResourceModelPresentInRepoTest()
        {
            string TestName = "AddCommand";
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
        }

        [Fact, Order(4)]
        [Trait("Category", "UmoyaTestCase")]
        public void ResourceCodePresentInRepoTest()
        {
            string TestName = "AddCommand";
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
        }

        [Fact, Order(5)]
        [Trait("Category", "UmoyaTestCase")]
        //CleanUp
        public void CleanUp()
        {
            TestAPIs.StopRepo();
            FSOps.DeleteDirectory(Constants.DefaultTestDataDir);
        }
    }
}