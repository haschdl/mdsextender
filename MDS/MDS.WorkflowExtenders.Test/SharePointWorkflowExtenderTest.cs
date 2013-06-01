using System.Net;
using System.Xml;
using MDS.WorkflowExtenders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MDS.Test
{
    
    
    /// <summary>
    ///This is a test class for SharePointWorkflowExtenderTest and is intended
    ///to contain all SharePointWorkflowExtenderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SharePointWorkflowExtenderTest
    {
        private TestContext testContextInstance;
        private static XmlDocument WorkflowInitiationData;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
         
        //You can use the following additional attributes as you write your tests:
        
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            WorkflowInitiationData = new XmlDocument();
            WorkflowInitiationData.Load("../../../MDSWorkflowInitiationData.xml");
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetClient
        ///</summary>
        [TestMethod()]
        public void GetClientTest()
        {
             var siteUrl = "http://xxxxx:82/";
            var remoteAddressUrl = new Uri(siteUrl);

            //Get client appends the _layouts/MDS.SharePoint/SiteWorkflowActivationService.asmx
            var actual = SharePointWorkflowExtender.GetClient(remoteAddressUrl);
            Assert.IsNotNull(actual);

            var instanceId = actual.StartSiteWorkflow(siteUrl, "MDS Action Test", WorkflowInitiationData.OuterXml);

            Console.Out.WriteLine("The workflow instance ID is " + instanceId);
        }

        /// <summary>
        ///A test for GetServiceUri
        ///</summary>
        [TestMethod()]
        public void GetServiceUriTest()
        {
            Uri baseUri = new Uri("http://anyserver");
            var actual = SharePointWorkflowExtender.GetServiceUri(baseUri);
            Console.Out.WriteLine("The constructed URL is " + actual);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetServiceUri
        ///</summary>
        [TestMethod()]
        public void GetServiceUriTestWithSlashAndPort()
        {
            Uri baseUri = new Uri("http://anyserver:82/");
            var actual = SharePointWorkflowExtender.GetServiceUri(baseUri);
            Console.Out.WriteLine("The constructed URL is " + actual.ToString());
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetServiceUri
        ///</summary>
        [TestMethod()]
        public void GetServiceUriTestWithSlashAndPortSubsite()
        {
            const string baseUriString = "http://anyserver:82/sites/site1";
            Uri baseUri = new Uri(baseUriString);
            var actual = SharePointWorkflowExtender.GetServiceUri(baseUri);
            Console.Out.WriteLine("The constructed URL is " + actual.ToString());
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.ToString(), "http://anyserver:82/sites/site1/_layouts/MDS.SharePoint/SiteWorkflowActivationService.asmx");
        }
    }
}
