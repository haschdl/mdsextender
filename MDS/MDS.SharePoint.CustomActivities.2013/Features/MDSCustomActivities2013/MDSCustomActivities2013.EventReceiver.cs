using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using MDS.SharePoint.ActivityLibrary;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace MDS.SharePoint.CustomActivities.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("c76c0de9-dc68-45bb-99a1-f0f61a43cfa7")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWebService contentService = SPWebService.ContentService;
                contentService.WebConfigModifications.Add(GetConfigModification());

                // Serialize the Web application state and propagate changes across the farm. 
                contentService.Update();

                // Save Web.config changes.
                contentService.ApplyWebConfigModifications();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }


        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWebService contentService = SPWebService.ContentService;
                contentService.WebConfigModifications.Remove(GetConfigModification());

                // Serialize the Web application state and propagate changes across the farm. 
                contentService.Update();

                // Save Web.config changes.
                contentService.ApplyWebConfigModifications();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }




        public SPWebConfigModification GetConfigModification()
        {
            string assemblyValue = typeof(MDSInit).Assembly.FullName;
            string namespaceValue = typeof(MDSInit).Namespace;

            var modification = new SPWebConfigModification(string.Format(CultureInfo.CurrentCulture,
                "authorizedType[@Assembly='{0}'][@Namespace='{1}'][@TypeName='*'][@Authorized='True']", assemblyValue, namespaceValue),
                "configuration/System.Workflow.ComponentModel.WorkflowCompiler/authorizedTypes")
                                   {
                                       Owner = "Master Data Services",
                                       Sequence = 0,
                                       Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode,
                                       Value = string.Format(CultureInfo.CurrentCulture,
                                                             "<authorizedType Assembly=\"{0}\" Namespace=\"{1}\" TypeName=\"*\" Authorized=\"True\" />",
                                                             assemblyValue,
                                                             namespaceValue)
                                   };

            Trace.TraceInformation("SPWebConfigModification value: {0}", modification.Value);

            return modification;
        }

    }
}