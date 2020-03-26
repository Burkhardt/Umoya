
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Repo.Clients.CLI.Commands.Tests
{
    public class PublishCommandTest
    {
        public static string TestName = "PublishCommand";
        public PublishCommandTest()
        {
            //Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            #region Check and start the repo server, if not already started.
            TestAPIs.StopRepo();
            if (Directory.Exists(Constants.DefaultTestDataDir))
                FSOps.DeleteDirectory(Constants.DefaultTestDataDir);
            if (!TestAPIs.IsRepoRunning())
            {
                //UnZip publish folder in umoya repo bin folder
                Assert.True(TestAPIs.UnZipPublishFolder(), "Failed to exract publish folder to get binary file.");

            }
            #endregion

            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
            Assert.True(TestAPIs.StartRepo(), "Repo server could not be started.");
        }
        #region Publish single resource withiout dependency
        //Baseline
        //1. Repo should not have HelloWorld.pmml@1.0.0
        //2. HelloWorld.pmml should be in local test-data folder
      //  [Fact]
        public void ResourceWithoutDependency()
        {
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
            #region Clean up
            //  FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }
        #endregion

        #region Publish single resource with dependency
        //Baseline
        //1. Repo should not have HelloWorld.pmml@1.0.0
        //2. HelloWorld.pmml should be in local test-data folder
        //3. HelloWorldData.csv should be present in local test-data folder
      //  [Fact]
        public void ResourceWithDependency()
        {
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
            #region Clean up
            //  FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }
        #endregion
    }
}
