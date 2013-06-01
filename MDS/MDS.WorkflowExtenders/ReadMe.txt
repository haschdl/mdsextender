
    /// Implements a workflow type extender for SharePoint workflows. Once built, the assembly that contains this 
    /// class should be put in the same folder as the workflow listener (Microsoft.MasterDataServices.Workflow.exe), 
    /// and the listener's config file should be updated to reference this class, like this:
    /// <code>
    ///     <setting name="WorkflowTypeExtenders" serializeAs="String">
    ///         <value>SPWF=MDS.WorkflowExtenders.SharePointWorkflowExtender, MDS.WorkflowExtenders.SharePointWorkflow, Version=1.0.0.0</value>
    ///     </setting>
    /// </code>

	/// Default location (SQL Server 2012): C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin