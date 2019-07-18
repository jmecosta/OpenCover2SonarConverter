namespace OpenCoverConverImpTest
{
    using Moq;
    using NUnit.Framework;
    using OpenCoverConverterImp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using TestImpactRunnerApi;

    public class TestXmlConverters
    {
        string testPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""));
        

        [Test]
        public void CollectAllCoveredMethodsTest()
        {
            var mockLogger = new Mock<ITestImpactLogger>();
            ICoverageHandle converter = new CollectAllCoveredMethodsProvider("", testPath);
            var impactTest = new ImpactedTest();
            var parseData = converter.GenerateImpactMapForTest(impactTest, new SortedSet<string>(), @"c:\", mockLogger.Object);
            Assert.That(parseData.Files.Count, Is.EqualTo(38));
        }

        [Test]
        public void CollectAllTrackedTestMethodsTest()
        {
            var mockLogger = new Mock<ITestImpactLogger>();
            ICoverageHandle converter = new CollectAllTrackedTestMethodsProvider("", testPath, "", "", "", "", mockLogger.Object);
            var impactTest = new ImpactedTest();
            var parseData = converter.GenerateImpactMapForTest(impactTest, new SortedSet<string>(), @"c:\", mockLogger.Object);
            Assert.That(parseData.Files.Count, Is.EqualTo(16));
        }

        [Test]
        public void CollectTrackedTestMethodsTest()
        {
            var mockLogger = new Mock<ITestImpactLogger>();
            ICoverageHandle converter = new CollectCoveredMethodsForTestProvider("", testPath);
            var impactTest = new ImpactedTest();
            impactTest.TestName = "TestName";
            impactTest.TestMethod = "TestMethodName";
            var parseData = converter.GenerateImpactMapForTest(impactTest, new SortedSet<string>(), @"c:\", mockLogger.Object);
            Assert.That(parseData.Files.Count, Is.EqualTo(1));
        }

        [Test]
        public void CollectTrackedTestMethodsTestForInvalidName()
        {
            var mockLogger = new Mock<ITestImpactLogger>();
            ICoverageHandle converter = new CollectCoveredMethodsForTestProvider("", testPath);
            var impactTest = new ImpactedTest();
            impactTest.TestName = "TestName";
            impactTest.TestMethod = "AnotherTestMethodName";
            var parseData = converter.GenerateImpactMapForTest(impactTest, new SortedSet<string>(), @"c:\", mockLogger.Object);
            Assert.That(parseData.Files.Count, Is.EqualTo(0));
        }
    }
}
