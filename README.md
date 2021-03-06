# Background
The business rules created in SQL Server Master Data Services can trigger external workflows, such as SharePoint site workflows. Without additional customization, this integration is only supported if MDS and SharePoint are installed in the same server, which is not a common scenario. The purpose of this project was de-couple MDS and SharePoint, so that MDS could trigger a SharePoint workflow in a separate server. This enables business users to create approval workflows in SharePoint, extending MDS capabilities. Without this tool, workflows must be created by developers only, using Visual Studio.

Technically, this is achieved by deploying a custom web service to a SharePoint server, and a MDS custom workflow. For more information on MDS custom workflows, see the documentation at https://docs.microsoft.com/en-us/sql/master-data-services/develop/create-a-custom-workflow-master-data-services.

## Update
This project was migrated from Codeplex and its being kept for reference purposes only. While the MDS related code is still relevant for newer versions of SQL Server, the MDS components are not applicable to Office 365 and might require changes to work with SharePoint 2016. 
 
# Project Description
This project consists of three components:
![](docs/Home_MDS_Custom_Tools.png)

## 1 - [Web Service for Site Workflow activation](docs/Web-Service-for-Site-Workflow-activation.md)
A web service to be deployed to SharePoint server. This web service overcomes a limitation in SharePoint web services, which prevents site workflows to be initiated via web service calls. This component is independent from the others, and be used by other systems or solutions. Note: I don’t know if this limitation is still present in SharePoint 2013.
![](docs/Home_FastCapture_031_2.png)

## 2 -  [Master Data Services workflow extender](docs/Master-Data-Services-workflow-extender.md)
A workflow extender, to be deployed to Master Data Services server. This workflow extender allows business rules to initiate SharePoint workflows located in a different server. It requires the component 1 to be deployed in the target SharePoint server.

Note that additional configuration is required in MDS to enable external workflows. More information [here](here)

## 3 - [SharePoint Designer Custom Workflow Action](docs/SharePoint-Designer-Custom-Workflow-Action.md)
A custom workflow action for SharePoint designer.

![](docs/Home_SPD_MDS.png)

# About the author
**Half Scheidl**
IT Consultant with an international master’s degree in IT Management, specialized in Business Intelligence and Master Data Management. Based in Helsinki, Finland.
[http://fi.linkedin.com/in/scheidlh](http://fi.linkedin.com/in/scheidlh)
