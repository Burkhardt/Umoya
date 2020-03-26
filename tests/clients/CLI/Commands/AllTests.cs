
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
        public void ResourceDataPresentInRepoTest()
        {
            string TestName = "AddCommand";
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
            // TestAPIs.StopRepo();
            #region Clean up
            //FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }

        [Fact, Order(6)]
        [Trait("Category", "UmoyaTestCase")]
        #region Umoya list local resources with already added resources (baseline)
        //Baseline    
        //1. Resource HelloWorld.pmml@1.0.0 , HelloWorldData.csv@1.0.0, HelloWorldCode.ipynb@1.0.0 is present in Repo
        //2. User has initialized ZMOD.    Folders are created in ZMODHome/Models , Code  Data 
        //3. Captured expected output.      
        #endregion
        public void LocalListWithBaselineResourcesTest()
        {
            string TestName = "ListCommand";
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
            // TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");
            ResourceToAdd = "HelloWorld.pmml";
            //    TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");
            TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");

            ResourceToAdd = "HelloWorldData.csv";
            //TestAPIs.CaptureConsoleOutPut("add", ResourceToAdd + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator + TestScenariosName + ".txt");
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
        }

        #region Umoya list repo resources with baseline resources
        //Baseline    
        //1. Repo has HelloWorld.pmml@1.0.0, HelloWorldCode.ipynb@1.0.0 and HelloWorldData.csv@1.0.0

        [Fact, Order(7)]
        [Trait("Category", "UmoyaTestCase")]
        public void RepoListWithBaselineResourcesTest()
        {
            string TestName = "ListCommand";
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
        }
        #endregion

        [Fact, Order(8)]
        [Trait("Category", "UmoyaTestCase")]

        #region Umoya delete local resource which was added before.
        #endregion
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

        }
        [Fact, Order(9)]
        [Trait("Category", "UmoyaTestCase")]
        #region Publish single resource withiout dependency
        //Baseline
        //1. Repo should not have HelloWorld.pmml@1.0.0
        //2. HelloWorld.pmml should be in local test-data folder
        public void ResourceWithoutDependency()
        {
            string TestName = "PublishCommand";
            #region Setup
            string TestScenariosName = "ResourceWithoutDependency";
            //Need to do Umoya Repo Baseline setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator +
            Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region Parameters to be used for command
            string ResourceToPublish = Constants.DefaultTestDataDir + Constants.PathSeperator + "cli-resources" +
            Constants.PathSeperator + "HelloWorld.pmml";
            string desc = " --description \"Hello World PMML Demo Model\" --authors Rainer --owners Vinay -t MyModel,Testing";
            string ResourceVersion = "3.0.0 " + desc;
            #endregion

            TestAPIs.CaptureConsoleOutPut("publish", ResourceToPublish + "@" + ResourceVersion, ZMODPath, Constants.DefaultTestDataDir
            + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator +
            TestScenariosName + ".txt");

            #region Capture Output for list for above resource
            string ListParams = "--from repo --query " + Path.GetFileName(ResourceToPublish);
            TestAPIs.CaptureConsoleOutPut("list", ListParams, ZMODPath, Constants.DefaultTestDataDir
                       + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator +
                       TestScenariosName + ".List" + Path.GetFileName(ResourceToPublish) + ".txt");
            string ActualOutputFilePath = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator +
             TestName + Constants.PathSeperator + TestScenariosName + ".List" + Path.GetFileName(ResourceToPublish) + ".txt";
            Assert.True(File.ReadLines(ActualOutputFilePath).Any(line => line.Contains(Path.GetFileName(ResourceToPublish))));
            #endregion
            
        }
        #endregion

        [Fact, Order(10)]
        [Trait("Category", "UmoyaTestCase")]
        #region Publish single resource with dependency
        //Baseline
        //1. Repo should not have HelloWorld.pmml@1.0.0
        //2. HelloWorld.pmml should be in local test-data folder
        //3. HelloWorldData.csv should be present in local test-data folder
        //  [Fact]
        public void ResourceWithDependency()
        {
            string TestName = "PublishCommand";
            #region Setup
            string TestScenariosName = "ResourceWithDependency";
            //Need to do Umoya Repo Baseline setup
            string ZMODPath = Constants.DefaultTestDataDir + Constants.PathSeperator + "temp" + Constants.PathSeperator +
            Guid.NewGuid();
            Directory.CreateDirectory(ZMODPath);
            Assert.True(TestAPIs.InitZMOD(ZMODPath), "ZMOD init failed.");
            #endregion

            #region Parameters to be used for Publish command with dependency
            string ResourceToPublish = Constants.DefaultTestDataDir + Constants.PathSeperator + "cli-resources" +
            Constants.PathSeperator + "HelloWorld.pmml";
            string desc = " --description \"Hello World PMML Demo Model\" --authors Rainer --owners Vinay -t MyModel,Testing";
            string DependencyResource = Constants.DefaultTestDataDir + Constants.PathSeperator + "HelloWorldData.csv";
            string DependencyVersion = "1.0.0";
            string ResourceVersion = "3.0.0";
            string PublishCommand = ResourceToPublish + "@" + ResourceVersion + desc + " -u " + DependencyResource + "@" + DependencyVersion;
            #endregion

            TestAPIs.CaptureConsoleOutPut("publish", PublishCommand, ZMODPath, Constants.DefaultTestDataDir
            + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator +
            TestScenariosName + ".txt");

            #region Capture Output for list for above resource

            string ListParams = " --from repo --query " + Path.GetFileName(ResourceToPublish);
            string ListParamsWithDependency = " --from repo --query " + Path.GetFileName(DependencyResource);
            TestAPIs.CaptureConsoleOutPut("list", ListParams, ZMODPath, Constants.DefaultTestDataDir
                       + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator +
                       TestScenariosName + ".List" + Path.GetFileName(ResourceToPublish) + ".txt");

            string ActualOutputFilePathForResource = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator +
                         TestName + Constants.PathSeperator + TestScenariosName + ".List" + Path.GetFileName(ResourceToPublish) + ".txt";
            Assert.True(File.ReadLines(ActualOutputFilePathForResource).Any(line => line.Contains(Path.GetFileName(ResourceToPublish))));
            TestAPIs.CaptureConsoleOutPut("list", ListParamsWithDependency, ZMODPath, Constants.DefaultTestDataDir
            + Constants.PathSeperator + "actual-output" + Constants.PathSeperator + TestName + Constants.PathSeperator +
            TestScenariosName + ".ListDependency" + Path.GetFileName(DependencyResource) + ".txt");

            string ActualOutputFilePathForDependentResource = Constants.DefaultTestDataDir + Constants.PathSeperator + "actual-output" + Constants.PathSeperator +
                                     TestName + Constants.PathSeperator + TestScenariosName + ".ListDependency" + Path.GetFileName(DependencyResource) + ".txt";
            Assert.True(File.ReadLines(ActualOutputFilePathForDependentResource).Any(line => line.Contains(Path.GetFileName(DependencyResource))));
            #endregion

        }
        #endregion

        [Fact, Order(11)]
        [Trait("Category", "UmoyaTestCase")]
        //CleanUp
        public void CleanUp()
        {
            TestAPIs.StopRepo();
            FSOps.DeleteDirectory(Constants.DefaultTestDataDir);
        }
    }
}