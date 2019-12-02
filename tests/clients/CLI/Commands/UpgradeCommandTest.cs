
using System;
using Xunit;

namespace CLI.Tests
{
    public class UpgradeCommandTest
    {
        #region Upgrade resources with no dependencies
        //Baseline
            //1. ZMOD is configured
            //2. It should have Model HelloWorld.pmml@1.0.0, HelloWorld.ipynb@1.0.0 and HelloWorldData.csv@1.0.0
            //3. Repo should have HelloWorld@2.0.0, HelloWorld.ipynb@2.0.0 and HelloWorldData.csv@2.0.0
        //Compare output
        //Call list local and compare output
        [Fact]
        public void ResourcesWithoutDependenciesTest()
        {
            Assert.True(true);
        }       
        #endregion

        #region Upgrade resources with dependencies
        //Baseline
            //1. ZMOD is configured
            //2. It should have Model HelloWorld.pmml@2.0.0
            //3. Repo should have HelloWorld@3.0.0 which has dependencies HelloWorld.ipynb@3.0.0 and HelloWorldData.csv@3.0.0
        //Compare output
        //Call list local and compare output
        [Fact]
        public void ResourcesWithDependenciesTest()
        {
            Assert.True(true);
        }       
        #endregion
    }
}
