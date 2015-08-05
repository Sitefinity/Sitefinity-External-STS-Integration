using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using OauthExternalAuthentication.Web.UI;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;
using DotNetOpenAuth.AspNet;
using OauthExternalAuthentication.AmazonProvider;
using Microsoft.AspNet.Membership.OpenAuth;
using DotNetOpenAuth.GoogleOAuth2;


namespace OauthExternalAuthentication
{
    public class OAuthAuthenticationModule : IModule
    {
        Guid moduleId = new Guid("{3D113D67-0F63-442E-AD97-330DD702E2C2}");


        public Guid ModuleId
        {
            get { return this.moduleId; }
        }

        public string Name
        {
            get { return "OauthExternalAuthentication"; }
        }

        public string Title
        {
            get { return "OauthExternalAuthentication"; }
        }

        public string Description
        {
            get { return "OauthExternalAuthentication"; }
        }

        public string ClassId
        {
            get { return null; }
        }

        public Guid LandingPageId
        {
            get { return Guid.NewGuid(); }
        }

        public StartupType Startup
        {
            get
            {
                return StartupType.OnApplicationStart;
            }
            set
            {

            }
        }

        public bool IsApplicationModule
        {
            get { return true; }
        }

        public void Initialize(ModuleSettings settings)
        {
            Res.RegisterResource<OauthExternalAuthenticationResources>();

            Config.RegisterSection<OAEConfig>();
       
            Bootstrapper.Initialized += Bootstrapper_Initialized;

            var oaeConfig = Config.Get<OAEConfig>();

            //Facebook
            if (!String.IsNullOrEmpty(oaeConfig.FacebookAPPID) && !String.IsNullOrEmpty(oaeConfig.FacebookAPPSecretKey))
            {
                ProviderDetails faecbookClient = OpenAuth.AuthenticationClients.GetAll().FirstOrDefault(client => client.ProviderName == "facebook");
                if (faecbookClient == null)
                {
                    OpenAuth.AuthenticationClients.AddFacebook(
                        appId: oaeConfig.FacebookAPPID,
                     appSecret: oaeConfig.FacebookAPPSecretKey);
                }
            }

            //Google
            if (!String.IsNullOrEmpty(oaeConfig.GoogleClientID) && !String.IsNullOrEmpty(oaeConfig.GoogleClientSecret))
            {
                 ProviderDetails googleClient = OpenAuth.AuthenticationClients.GetAll().FirstOrDefault(client => client.ProviderName.ToLower() == "google");
                 if (googleClient == null)
                 {
                     OpenAuth.AuthenticationClients.Add("Google", () => new GoogleOAuth2Client(oaeConfig.GoogleClientID, oaeConfig.GoogleClientSecret));﻿
                 }
            }
            //Amazon
            if (!String.IsNullOrEmpty(oaeConfig.AmazonAPPID) && !String.IsNullOrEmpty(oaeConfig.AmazonAPPSecretKey))
            {
                ProviderDetails amazonClient = OpenAuth.AuthenticationClients.GetAll().FirstOrDefault(client => client.ProviderName == "Amazon");
                if (amazonClient == null)
                {
                    //OpenAuth.AuthenticationClients.Add("Amazon", (Func<IAuthenticationClient>)(() => (IAuthenticationClient)new AmazonOpenAuthenticationProvider("amzn1.application-oa2-client.19782be9a3c54a29ba54ad753e9e4630", "ffb32157e1b759ec2e8a165a08456022a69d83a55b2ad5b80bce3dcdb2fd0da0")), null);
                    OpenAuth.AuthenticationClients.Add("Amazon", (Func<IAuthenticationClient>)(() =>
                        (IAuthenticationClient)new AmazonOpenAuthenticationProvider(oaeConfig.AmazonAPPID, oaeConfig.AmazonAPPSecretKey)), null);
                }
            }
        }

        void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "RegisterRoutes")
            {
                ReplaceDefaultRoute(e.Data as IQueryable<RouteBase>);
            }
        }

        
        public void Install(Telerik.Sitefinity.Abstractions.SiteInitializer initializer, Version upgradeFrom)
        {
            var config = initializer.Context.GetConfig<ToolboxesConfig>();
            var pageControls = config.Toolboxes["PageControls"];

            var section = pageControls.Sections
                .Where<ToolboxSection>(e => e.Name == "Login")
                .FirstOrDefault();

            if (!section.Tools.Any<ToolboxItem>(e => e.Name == "OAuthLogin"))
            {
                var blogsList = new ToolboxItem(section.Tools)
                {
                    Name = "OAuthLogin",
                    Title = "OAuthLogin",
                    Description = "OAuthLogin",
                    ControlType = typeof(OAuthLoginForm).AssemblyQualifiedName
                };
                section.Tools.Add(blogsList);
            }



            var virtualPathConfig = initializer.Context.GetConfig<VirtualPathSettingsConfig>();
            if (!virtualPathConfig.VirtualPaths.Elements.Any(vp => vp.VirtualPath == "~/oauth/*"))
            {
                var moduleVirtualPathConfig = new VirtualPathElement(virtualPathConfig.VirtualPaths)
                {
                    VirtualPath = "~/oauth/*",
                    ResolverName = "EmbeddedResourceResolver",
                    ResourceLocation = "OauthExternalAuthentication"
                };
                virtualPathConfig.VirtualPaths.Add(moduleVirtualPathConfig);
            }


        }

        private void ReplaceDefaultRoute(IQueryable<RouteBase> routes)
        {
            var path = Telerik.Sitefinity.Security.Claims.Constants.LocalService + "/{*Service}";

            foreach (var routeBase in routes)
            {
                if (routeBase is Route)
                {
                    var route = routeBase as Route;

                    if (route != null && route.Url.Equals(path))
                    {
                        (route as Route).RouteHandler = new RouteHandler<OAuthAuthenticationHttpHandler>();
                    }
                }
            }
        }

        public void Load()
        {

        }

        public void Unload()
        {

        }

        public void Uninstall(Telerik.Sitefinity.Abstractions.SiteInitializer initializer)
        {

        }

        public Telerik.Sitefinity.Web.UI.Backend.IControlPanel GetControlPanel()
        {
            return null;
        }

        public Type[] Managers
        {
            get { return null; }
        }
    }
}
