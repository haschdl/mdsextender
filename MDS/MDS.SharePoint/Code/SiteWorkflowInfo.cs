using System;
using System.Collections.Generic;

namespace MDS.SharePoint
{
    [Serializable]
    public class SiteWorkflowInfo
    {
        public string WorkflowName { get; set; }
        public string WorkflowTitle { get; set; }
        public List<SiteWorkflowParameterInfo> WorkflowParameters { get; set; }
    }
}