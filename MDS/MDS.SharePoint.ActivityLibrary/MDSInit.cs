using System;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using System.Activities;
using System.Workflow.Activities;
using System.Workflow.ComponentModel.Compiler;
using System.Xml;
using Microsoft.SharePoint.WorkflowActions;

namespace MDS.SharePoint.ActivityLibrary
{
    public partial class MDSInit : SequenceActivity
    {
        #region Dependency Properties

        public static DependencyProperty __ContextProperty = DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(MDSInit));

        [Description("The site context")]
        [Category("User")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(__ContextProperty)));
            }
            set
            {
                base.SetValue(__ContextProperty, value);
            }
        }

        public static DependencyProperty __ActivationPropertiesProperty = DependencyProperty.Register("__ActivationProperties", typeof(Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties), typeof(MDSInit));
        [ValidationOption(ValidationOption.Required)]
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties __ActivationProperties
        {
            get
            {
                return (Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties)base.GetValue(__ActivationPropertiesProperty);
            }
            set
            {
                base.SetValue(__ActivationPropertiesProperty, value);
            }
        }


        public static DependencyProperty ModelNameProperty = DependencyProperty.Register("ModelName", typeof(string), typeof(MDSInit), new PropertyMetadata(""));
        [DescriptionAttribute("The model name in MDS")]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Optional)]
        public string ModelName
        {
            get
            {
                return ((string)(GetValue(ModelNameProperty)));
            }
            set
            {
                SetValue(ModelNameProperty, value);
            }
        }


        public static DependencyProperty EntityNameProperty = DependencyProperty.Register("EntityName", typeof(string), typeof(MDSInit), new PropertyMetadata(""));
        [DescriptionAttribute("The entity name in MDS")]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Optional)]
        public string EntityName
        {
            get
            {
                return ((string)(GetValue(EntityNameProperty)));
            }
            set
            {
                SetValue(EntityNameProperty, value);
            }
        }

        public static DependencyProperty MemberCodeProperty = DependencyProperty.Register("MemberCode", typeof(string), typeof(MDSInit), new PropertyMetadata(""));
        [DescriptionAttribute("The code of member in MDS")]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Optional)]
        public string MemberCode
        {
            get
            {
                return ((string)(GetValue(MemberCodeProperty)));
            }
            set
            {
                SetValue(MemberCodeProperty, value);
            }
        }

        public static DependencyProperty MemberNameProperty = DependencyProperty.Register("MemberName", typeof(string), typeof(MDSInit), new PropertyMetadata(""));
        [DescriptionAttribute("The name of the member in MDS")]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Optional)]
        public string MemberName
        {
            get
            {
                return ((string)(GetValue(MemberNameProperty)));
            }
            set
            {
                SetValue(MemberNameProperty, value);
            }
        }

        #endregion
        public MDSInit()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            //Cancels if no initiation data is present
            if (__ActivationProperties == null)
            {
                throw new Exception("It was not possible to fetch MDS data. Activation properties is null.");
            }

            if (__ActivationProperties.InitiationData == null)
            {
                throw new Exception("It was not possible to fetch MDS data. Initiation data is null.");
            }

            var externalAction = new XmlDocument();

            try
            {
                externalAction.LoadXml(__ActivationProperties.InitiationData);
            }
            catch (Exception exc)
            {
                throw new Exception(
                    "It was not possible to load the external action XML from the workflow initiation data." +
                    exc.Message);
            }


            var modelName = externalAction.SelectSingleNode(ExternalActionXPaths.ModelName);
            if (modelName != null)
                ModelName = modelName.InnerText;

            var entityName = externalAction.SelectSingleNode(ExternalActionXPaths.EntityName);
            if (entityName != null)
                EntityName = entityName.InnerText;

            var memberCode = externalAction.SelectSingleNode(ExternalActionXPaths.MemberCode);
            if (memberCode != null)
                MemberCode = memberCode.InnerText;

            var memberName = externalAction.SelectSingleNode(ExternalActionXPaths.MemberName);
            if (memberName != null)
                MemberName = memberName.InnerText;


            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            return base.HandleFault(executionContext, exception);
        }
    }

    internal class ExternalActionXPaths
    {
        public const string ModelName = "//ExternalAction/Model_Name";
        public const string EntityName = "//ExternalAction/Entity_Name";
        public const string MemberCode = "//ExternalAction/MemberData/Code";
        public const string MemberName = "//ExternalAction/MemberData/Name";
        public const string LastChgUserName = "//ExternalAction/MemberData/LastChgUserName";
        public const string LastChgDTM = "//ExternalAction/MemberData/LastChgDTM";
    }
}
