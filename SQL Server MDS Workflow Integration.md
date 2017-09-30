Before using the workflow extension for SharePoint it is required to install the Windows Service which reads the SQL Server Service Broker queues and calls the external workflows.
If you have .Net and SQL Server installed in default locations, the following commands might suffice.

{{
cd C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin

C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil Microsoft.MasterDataServices.Workflow.exe
}}
After running the command a new Windows service is created`, “SQL Server MDS Workflow Integration”.  Make sure to set the service to start automatically if you are planning to use the External Workflow actions in MDS.

The detailed steps can be found in:

http://social.technet.microsoft.com/wiki/contents/articles/7879.configuring-workflow-integration-with-master-data-services.aspx#Create_a_workflow_extender

 

Output
{{
Running a transacted installation.

Beginning the Install phase of the installation.
See the contents of the log file for the C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.exe assembly's progress.
The file is located at C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.InstallLog.
Installing assembly 'C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.exe'.
Affected parameters are:
   logtoconsole =
   logfile = C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.InstallLog
   assemblypath = C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.exe
Installing service MdsWorkflow...
Service MdsWorkflow has been successfully installed.
Creating EventLog source MdsWorkflow in log Application...

The Install phase completed successfully, and the Commit phase is beginning.
See the contents of the log file for the C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.exe assembly's progress.
The file is located at C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.InstallLog.
Committing assembly 'C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.exe'.
Affected parameters are:
   logtoconsole =
   logfile = C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.InstallLog
   assemblypath = C:\Program Files\Microsoft SQL Server\110\Master Data Services\WebApplication\bin\Microsoft.MasterDataServices.Workflow.exe

The Commit phase completed successfully.

The transacted install has completed.
}}