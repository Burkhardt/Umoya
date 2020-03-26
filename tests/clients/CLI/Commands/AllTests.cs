
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