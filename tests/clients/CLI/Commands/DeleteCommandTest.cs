using System;
using Xunit;
using System.IO;


namespace Repo.Clients.CLI.Commands.Tests
{
    public class DeleteCommandTest
    {
        public static string TestName = "DeleteCommand";

        #region Umoya DELETE  resource type which is present in Local 
        //Baseline 
        //1. Verify the Resource HelloWorld.pmml@1.0.0 is present in Local
        //2. User has initialized ZMOD.
        //3. Captured expected output. 
        #endregion
        public DeleteCommandTest()
        {
            // Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            #region Check and start the repo server, if not already started.
            if (!TestAPIs.IsRepoRunning())
            {
                //UnZip publish folder in umoya repo bin folder
                Assert.True(TestAPIs.UnZipPublishFolder(), "Failed to exract publish folder to get binary file.");
                TestAPIs.StartRepo();
            }
            #endregion

            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
        }


        #region Umoya delete local resource which was added before.
        #endregion
        [Fact]
        public void ResourceDeletedInLocalTest()
        {
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
            #region Clean up
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }
    }
}
