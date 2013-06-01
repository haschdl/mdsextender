using System;

namespace MDS.SharePoint
{
    [Serializable]
    public class SiteWorkflowParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}