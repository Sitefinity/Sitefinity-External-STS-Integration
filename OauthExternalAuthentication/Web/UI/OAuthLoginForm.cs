using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ControlDesign;
using Telerik.Sitefinity.Web.UI.PublicControls;

namespace OauthExternalAuthentication.Web.UI
{
    [ControlDesigner(typeof(oAuthLoginDesigner))]
    public class OAuthLoginForm : LoginWidget
    {
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            var oauthLoginControls = this.Page.LoadControl(oauthLoginControlsTemplate);
            this.Controls.Add(oauthLoginControls);
        }

        public Guid RedirectPageIdSuccess
        {
            get;
            set;
        }

        public string ReturnURLSuccess
        {
            get
            {
                string url = string.Empty;
                if (this.RedirectPageIdSuccess != Guid.Empty)
                {
                    //It may be that the page is restricted, and at the time of rendering the login form the node is not accessible to them
                    //(since they are not logged in).
                    PageManager pman = PageManager.GetManager();

                    bool originalSecurityChecks = pman.Provider.SuppressSecurityChecks;
                    pman.Provider.SuppressSecurityChecks = true;

                    PageNode node = pman.GetPageNodes().FirstOrDefault(pagenode => pagenode.Id == this.RedirectPageIdSuccess);
                    if (node != null)
                    {
                        url = node.GetFullUrl().Replace("~", string.Empty);
                    }
                    pman.Provider.SuppressSecurityChecks = originalSecurityChecks;
                }
                return url;
            }
        }

        public override IEnumerable<System.Web.UI.ScriptDescriptor> GetScriptDescriptors()
        {
            var scriptDescriptors = base.GetScriptDescriptors();
            foreach (var descriptor in scriptDescriptors)
            {
                if (descriptor is ScriptControlDescriptor)
                {
                    var scriptControlDescriptor = descriptor as ScriptControlDescriptor;
                    if (scriptControlDescriptor.Type == typeof(LoginWidget).FullName)
                    {
                        scriptControlDescriptor.Type = this.GetType().FullName;

                        var realm = this.Page.Request.Url.GetLeftPart(UriPartial.Authority);
                        var serviceUrl = ServiceUrl;
                        if (!serviceUrl.Contains("?"))
                            serviceUrl += "?";
                        else
                            serviceUrl += "&";

                        //if the querystring contains ReturnUrl variable, it takes precedence
                        string querystringReturnUrl = HttpContext.Current.Request.QueryString["ReturnUrl"];
                        string domainUrl = UrlPath.GetDomainUrl();
                        if (querystringReturnUrl != null && querystringReturnUrl.StartsWith(domainUrl))
                        {
                            querystringReturnUrl = querystringReturnUrl.Remove(querystringReturnUrl.IndexOf(domainUrl), domainUrl.Length);
                        }

                        scriptControlDescriptor.AddProperty("oAuthServiceUrl", serviceUrl
                            + "realm=" + this.Page.Server.UrlEncode(realm)
                            + "&redirect_uri=" + this.Page.Server.UrlEncode(querystringReturnUrl ?? this.ReturnURLSuccess)
                            + "&redirect_url_failure=" + this.Page.Server.UrlEncode(this.Page.Request.Url.AbsoluteUri)
                            );
                    }
                }
            }
            return scriptDescriptors;
        }

        //&realm=http%3a%2f%2flocalhost%3a51089%2f
        public override IEnumerable<ScriptReference> GetScriptReferences()
        {
            List<ScriptReference> res = new List<ScriptReference>{ 
                new ScriptReference(JsComponentPath, this.GetType().Assembly.FullName),
            };
            return base.GetScriptReferences().Union(res);
        }

        internal const string JsComponentPath = "OauthExternalAuthentication.Web.UI.OAuthLoginForm.js";
        internal const string oauthLoginControlsTemplate = "~/oauth/OauthExternalAuthentication.Web.UI.OAuthLoginForm.ascx";
    }
}