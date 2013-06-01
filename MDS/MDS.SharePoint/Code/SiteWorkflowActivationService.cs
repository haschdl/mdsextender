using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using System.Text;
using System.Xml;

namespace MDS.SharePoint
{
    ///<summary>
    ///This service fills a gap in Sharepoints Workflow web service, specifically the fact that it does not allow starting of a Site Workflow.
    ///Note that the location of the service is mentioned in the feature description, please update there as well in case of refactoring
    /// </summary>
    [WebService(Namespace = "http://sharePoint.workflow")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SiteWorkflowActivationService : WebService
    {
        /// <summary>
        /// Declarative workflow parameter xml format template used when sending parameter values to the site worklfow manager
        /// </summary>
        private const string InitiationXmlTemplate =
            @"<dfs:myFields xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:dms=""http://schemas.microsoft.com/office/2009/documentManagement/types""
                            xmlns:dfs=""http://schemas.microsoft.com/office/infopath/2003/dataFormSolution"" xmlns:q=""http://schemas.microsoft.com/office/infopath/2009/WSSList/queryFields"" xmlns:d=""http://schemas.microsoft.com/office/infopath/2009/WSSList/dataFields"" xmlns:ma=""http://schemas.microsoft.com/office/2009/metadata/properties/metaAttributes"" xmlns:pc=""http://schemas.microsoft.com/office/infopath/2007/PartnerControls"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
				<dfs:queryFields/>
				<dfs:dataFields>
					<d:SharePointListItem_RW>
					 {0}
					</d:SharePointListItem_RW>
				</dfs:dataFields>
			</dfs:myFields>";

        /// <summary>
        /// Starts a site workflow in SharePoint.
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="workflowName">The name of the workflow to start.</param>
        /// <param name="eventData">The event data to be passed to the workflow instance.</param>
        /// <returns>The ID of the workflow instance.</returns>
        [WebMethod]
        public Guid StartSiteWorkflow(string siteUrl,string workflowName, string eventData)
        {
            if (siteUrl == null)
                throw new ArgumentNullException("siteUrl");

            SPSite site = null;
            SPWeb web = null;
            try
            {
                site = new SPSite(siteUrl);
                web = site.OpenWeb();

                //find workflow to start
                var assoc = web.WorkflowAssociations.GetAssociationByName(workflowName, System.Threading.Thread.CurrentThread.CurrentCulture);

                if (assoc == null)
                {
                    //we could not find the workflow association on this site
                    throw new SoapException(string.Format("Workflow Association not found! Name = {0}, Site Url = {1}. You can call GetSiteWorkflowAssociations to fetch valid association names.", workflowName, web.Url),
                                            new XmlQualifiedName("Error"));
                }
                //this is the call to start the workflow
                var result = site.WorkflowManager.StartWorkflow( null, assoc, eventData, SPWorkflowRunOptions.Asynchronous);
                return result.InstanceId;
            }
            catch (Exception e)
            {
                //EventLog.WriteEntry("Application", string.Format("Error activating workflow: {0} (Name={1}; Url={2})", e, workflowName, HttpContext.Current.Request.Url), EventLogEntryType.Error);

                throw new SoapException(e.ToString(),
                                        new XmlQualifiedName("Error"));
            }
            finally
            {
                if (web != null) web.Close();
                if (site != null) site.Close();
                
            }
        }

        [WebMethod]
        public string[] GetSiteWorkflowAssociations()
        {
            try
            {
                var web = SPContext.Current.Web;
                return (from SPWorkflowAssociation item in web.WorkflowAssociations where item.Enabled select item.Name).ToArray();

            }
            catch (Exception e)
            {
                //EventLog.WriteEntry("Application", string.Format("Error getting workflows: {0} (Url={1})", e, HttpContext.Current.Request.Url), EventLogEntryType.Error);
                throw new SoapException(e.ToString(), new XmlQualifiedName("Error"));
            }
        }

        [WebMethod]
        public SiteWorkflowInfo GetSiteWorkflowDetails(string workflowName)
        {

            try
            {
                var web = SPContext.Current.Web;

                //find workflow associations
                var assoc = web.WorkflowAssociations.GetAssociationByName(workflowName, System.Threading.Thread.CurrentThread.CurrentCulture);

                if (assoc == null)
                {

                    string assocs = string.Empty;

                    foreach (SPWorkflowAssociation item in web.WorkflowAssociations)
                    {
                        assocs += item.Name + ", ";
                    }

                    throw new InvalidOperationException(string.Format("Workflow Association not found! Name = {0}, Available Assocs= {1}, Site Url = {2}", workflowName, assocs, web.Url));
                }

                List<SiteWorkflowParameterInfo> wkflowParams = null;

                if (assoc.IsDeclarative)
                    wkflowParams = GetDecrarativeWorkflowParameters(assoc);

                else
                    wkflowParams = GetCodeBesideWorkflowParameters(assoc);


                return new SiteWorkflowInfo { WorkflowName = workflowName, WorkflowTitle = workflowName, WorkflowParameters = wkflowParams };

            }
            catch (Exception e)
            {
                //SPDiagnosticsService.Local.WriteEvent(125, new SPDiagnosticsCategory(), )
                //EventLog.WriteEntry("Application", string.Format("Error getting details for workflow: {0} (Name={1}; Url={2})", e, workflowName, HttpContext.Current.Request.Url), EventLogEntryType.Error);

                throw new SoapException(e.ToString(), new XmlQualifiedName("Error"));

            }

        }

        [WebMethod]
        public void StartDeclarativeSiteWorkflow(SiteWorkflowInfo workflow)
        {

            try
            {
                var site = SPContext.Current.Site;
                var web = SPContext.Current.Web;

                //find workflow to start
                var assoc = web.WorkflowAssociations.GetAssociationByName(workflow.WorkflowName, System.Threading.Thread.CurrentThread.CurrentCulture);

                if (assoc == null)
                {
                    //we could not find the workflow association on this site
                    //we are building a list of available site workflows to return in the error message, this should help troubleshooting
                    string assocs = string.Empty;

                    foreach (SPWorkflowAssociation item in web.WorkflowAssociations)
                    {
                        assocs += item.Name + ", ";

                    }

                    throw new InvalidOperationException(string.Format("Workflow Association not found! Name = {0}, Available Assocs= {1}, Site Url = {2}", workflow.WorkflowName, assocs, web.Url));
                }

                var eventData = BuildInitiationData(workflow);

                site.WorkflowManager.StartWorkflow(null, assoc, eventData, SPWorkflowRunOptions.Asynchronous);

            }
            catch (Exception e)
            {
                //EventLog.WriteEntry("Application" , string.Format("Error activating workflow: {0} (Name={1}; Url={2})", e, workflow.WorkflowName, HttpContext.Current.Request.Url), EventLogEntryType.Error);

                throw new SoapException(e.ToString(), new XmlQualifiedName("Error"));

            }

        }

        private static List<SiteWorkflowParameterInfo> GetCodeBesideWorkflowParameters(SPWorkflowAssociation assoc)
        {
            var wkflowParams = new List<SiteWorkflowParameterInfo>();

            var xDoc = new XmlDocument();

            xDoc.LoadXml(assoc.BaseTemplate.Xml);


            var workflowSetNode = xDoc.SelectSingleNode("/WorkflowTemplate/WorkflowTemplateIdSet");

            if (workflowSetNode != null)
            {

                var workflowClassName = workflowSetNode.Attributes["CodeBesideClass"].Value;
                var workflowAssembly = workflowSetNode.Attributes["CodeBesideAssm"].Value;

                var workflowType = Type.GetType(workflowClassName + ", " + workflowAssembly);

                var properties = workflowType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);


                foreach (var prop in properties.Where(x => x.CanWrite))
                {
                    var wPram = new SiteWorkflowParameterInfo { Name = prop.Name, Type = prop.PropertyType.FullName };

                    if (IsSupportedWorkflowParamerter(wPram))
                        wkflowParams.Add(wPram);

                }
            }

            return wkflowParams;

        }

        private List<SiteWorkflowParameterInfo> GetDecrarativeWorkflowParameters(SPWorkflowAssociation assoc)
        {
            var wkflowParams = new List<SiteWorkflowParameterInfo>();

            var xDocTemplate = new XmlDocument();

            xDocTemplate.LoadXml(assoc.BaseTemplate.Xml);

            var paramXmlNode = xDocTemplate.SelectSingleNode("/WorkflowTemplate/Metadata/Instantiation_FieldML/string");

            if (paramXmlNode != null)
            {
                var paramXmlString = UnescapeXml(paramXmlNode.InnerText);

                var xDoc = new XmlDocument();

                xDoc.LoadXml(paramXmlString);

                foreach (XmlNode node in xDoc.SelectNodes("/Fields/Field"))
                {
                    var wkInfo = new SiteWorkflowParameterInfo { Name = node.Attributes["Name"].Value, Type = MapSPDTypeToCLRType(node.Attributes["Type"].Value) };

                    if (IsSupportedWorkflowParamerter(wkInfo))
                        wkflowParams.Add(wkInfo);
                }
            }

            return wkflowParams;
        }

        private static string MapSPDTypeToCLRType(string spdType)
        {
            switch (spdType)
            {
                case "Text":
                    return "System.String";
                case "Boolean":
                    return "System.Boolean";
                case "Number":
                    return "System.Double";
                case "DateTime":
                    return "System.DateTime";
                default:
                    return "";
            }
        }

        private static string BuildInitiationData(SiteWorkflowInfo info)
        {
            if (info.WorkflowParameters == null || info.WorkflowParameters.Count == 0)
                return string.Empty;

            var initPSb = new StringBuilder();

            foreach (var param in info.WorkflowParameters.Where(x => !string.IsNullOrEmpty(x.Value)))
            {
                initPSb.AppendFormat("<d:{0}>{1}</d:{0}>", param.Name, param.Value);
            }

            return string.Format(InitiationXmlTemplate, initPSb);
        }

        private static bool IsSupportedWorkflowParamerter(SiteWorkflowParameterInfo wkInfo)
        {
            //we only support simple types for workflow parameters
            if (wkInfo.Type == "System.String" || wkInfo.Type == "System.Boolean" || wkInfo.Type == "System.Double" || wkInfo.Type == "System.DateTime")
                return true;

            return false;
        }

        private static string UnescapeXml(string xmlString)
        {
            xmlString = xmlString.Replace("&lt;", "<");
            xmlString = xmlString.Replace("&gt;", ">");
            xmlString = xmlString.Replace("&amp;", "&");
            xmlString = xmlString.Replace("&quot;", "\"");
            xmlString = xmlString.Replace("&apos;", "’");
            xmlString = xmlString.Replace("&#39;", "’");
            return xmlString;

        }
    }
}