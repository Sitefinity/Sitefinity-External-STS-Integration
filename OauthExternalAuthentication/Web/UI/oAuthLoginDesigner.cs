using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI.ControlDesign;
using Telerik.Sitefinity.Web.UI.Fields;

namespace OauthExternalAuthentication.Web.UI
{
    public class oAuthLoginDesigner : ControlDesignerBase
    {
        protected PageField PageSelector
        {
            get
            {
                return Container.GetControl<PageField>("pageField", true);
            }
        }

        protected override void InitializeControls(Telerik.Sitefinity.Web.UI.GenericContainer container)
        {
            this.PageSelector.RootNodeID = Telerik.Sitefinity.Abstractions.SiteInitializer.CurrentFrontendRootNodeId;
        }

        public override IEnumerable<System.Web.UI.ScriptDescriptor> GetScriptDescriptors()
        {
            var scriptDescriptors = base.GetScriptDescriptors();
            var descriptor = (ScriptControlDescriptor)scriptDescriptors.Last();
            descriptor.AddComponentProperty("pagePicker", this.PageSelector.ClientID);
            return scriptDescriptors;
        }

        public override IEnumerable<System.Web.UI.ScriptReference> GetScriptReferences()
        {
            var scripts = base.GetScriptReferences().ToList();
            scripts.Add(new System.Web.UI.ScriptReference("OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.js", this.GetType().Assembly.GetName().ToString()));
            return scripts;
        }

        protected override string LayoutTemplateName
        {
            get
            {
                return "OauthExternalAuthentication.Web.UI.oAuthLoginDesignerTemplate.ascx";
            }
        }
    }
}
