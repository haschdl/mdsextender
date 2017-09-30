Adds a new workflow type to used as an external action in MDS Business Rules.

The component is a DLL which contains an implementation of IWorkflowTypeExtender [http://msdn.microsoft.com/en-us/library/microsoft.masterdataservices.workflowtypeextender.iworkflowtypeextender.aspx](http://msdn.microsoft.com/en-us/library/microsoft.masterdataservices.workflowtypeextender.iworkflowtypeextender.aspx).

The DLL must be copied to MDS bin folder and .config file ammended.

Note that the **workflow integration Windows service** must be configured in the MDS service. More information: [SQL Server MDS Workflow Integration](SQL-Server-MDS-Workflow-Integration)