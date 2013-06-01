using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.Xml;
using MDS.WorkflowExtenders.SiteWorkflowService;
using Microsoft.MasterDataServices.WorkflowTypeExtender;

namespace MDS.WorkflowExtenders
{
    /// <summary>
    /// Implements a workflow type extender for SharePoint workflows. Once built, the assembly that contains this 
    /// class should be put in the same folder as the workflow listener (Microsoft.MasterDataServices.Workflow.exe), 
    /// and the listener's config file should be updated to reference this class, like this:
    /// <code>
    ///     <setting name="WorkflowTypeExtenders" serializeAs="String">
    ///         <value>SPWF=MDS.WorkflowExtenders.SharePointWorkflowExtender, MDS.WorkflowExtenders.SharePointWorkflow, Version=1.0.0.0</value>
    ///     </setting>
    /// </code>
    /// </summary>
    public class SharePointWorkflowExtender : IWorkflowTypeExtender
    {
        #region Fields

        /// <summary>
        /// Workflow type name for SharePoint workflows.
        /// </summary>
        private const string WorkflowTypeSharePoint = "SPWF";

        /// <summary>
        /// A cache of SharePoint sites. 
        ///    Key = serverUrl 
        ///    Value = SharePoint site
        /// </summary>
        private Dictionary<string, Uri> Sites = new Dictionary<string, Uri>();

        #endregion Fields

        /// <summary>
        /// Starts a workflow of the given type, if it is a SharePoint workflow.
        /// </summary>
        /// <param name="workflowType">The workflow type. The method does nothing if it is not a SharePoint workflow.</param>
        /// <param name="dataElement">The data passed to the workflow.</param>
        public void StartWorkflow(string workflowType, XmlElement dataElement)
        {
            SiteWorkflowActivationServiceSoapClient serviceClient = null;

            try
            {

                // Ignore non-SharePoint workflows.
                if (string.Equals(workflowType, WorkflowTypeSharePoint, StringComparison.OrdinalIgnoreCase))
                {
                    string serverUrl = dataElement["Server_URL"].InnerText;
                    string workflowName = dataElement["Action_ID"].InnerText;

                    // Look for the site in the cache.
                    Uri site = null;
                    if (!this.Sites.TryGetValue(serverUrl, out site))
                    {
                        // Site not in cache, so add it.
                        site = new Uri(serverUrl);
                        this.Sites[serverUrl] = site;
                    }

                    // Start the specified workflow.
                    serviceClient = GetClient(site);
                    serviceClient.StartSiteWorkflow(serverUrl, workflowName, dataElement.OuterXml);
#if(DEBUG)
                    Console.Out.WriteLine("SharePoint Workflow Extender: Starting workflow " + workflowName);
#endif

                }
            }
            catch (Exception exception)
            {
#if(DEBUG)
                Console.Out.WriteLine("SharePoint Workflow Extender: An exception occurred.");
                Console.Out.WriteLine(exception);

#endif


                throw;
            }
            finally
            {
                if (serviceClient != null) serviceClient.Close();
            }
        }

        internal static Uri GetServiceUri(Uri baseUri)
        {
            Uri newBaseUri = null;
            if (!baseUri.ToString().EndsWith(".aspx") && !baseUri.ToString().EndsWith("/"))
            {
                newBaseUri = new Uri(baseUri + "/");
            }
            else
            {
                newBaseUri = baseUri;
            }

            const string webServiceRelativeUrl = "_layouts/MDS.SharePoint/SiteWorkflowActivationService.asmx";

            Uri result;

            if (Uri.TryCreate(newBaseUri, webServiceRelativeUrl, out result))
            {
                return result;
            }

            throw new Exception("It was not possible to fetch the web service URL.");
        }

        internal static SiteWorkflowActivationServiceSoapClient GetClient(Uri remoteSharePointUri)
        {
            var remoteAddress = new EndpointAddress(GetServiceUri(remoteSharePointUri));

#if(DEBUG)
            Console.Out.WriteLine("SharePoint Workflow Extender: Connecting to " + GetServiceUri(remoteSharePointUri));
#endif

            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;


            var client = new SiteWorkflowActivationServiceSoapClient(binding, remoteAddress);
            client.ClientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

            //Unknown SQL Exception 1346 occurred. Additional error information from SQL Server is included below.
            //A transport-level error has occurred when sending the request to the server. (provider: Shared Memory Provider, error: 0 - Either a required impersonation level was not provided, or the provided impersonation level is invalid.)
            //TokenImpersonationLevel.Identification, Impersonation
            client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;

            return client;
        }
    }
}
