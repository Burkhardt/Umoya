
using System;
using Xunit;
using System.IO;

namespace Repo.Clients.CLI.Commands.Tests
{
    public class InfoCommandTest
    {
        public static string TestName = "InfoCommand";

        public InfoCommandTest()
        {
            //Check repo is running
            //If not then need to start repo.
            //Need to setup baseline in repo publish folder.
          //  Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
           #region Check and start the repo server, if not already started.
             if (!TestAPIs.IsRepoRunning())
            {
                Assert.True(TestAPIs.StartRepo(), "Repo server could not be started.");
            }
            #endregion

            Assert.True(TestAPIs.InitializeTestDataSetup(), "Test data setup is not done successfully.");
            //Copy repo-resources from umoya-testdata to repo publish folder
            Assert.True(TestAPIs.SetUpRepoServer(), "Failed to setup resources and db for repo.");
        }
        #region Umoya Info with default configurations

        // BaseLine
        //  1. ZMOD is initialized.
        // If not Run umoya init 
        // 2. Capture the Expected Output
        #endregion

       // [Fact]
        public void WithDefaultConfigurationsPresentInfoTest()
        {
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
            FSOps.DeleteDirectory(ZMODPath);
            #endregion
        }
    }
}
