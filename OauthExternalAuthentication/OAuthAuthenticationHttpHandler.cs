using Microsoft.AspNet.Membership.OpenAuth;
using System;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;

namespace OauthExternalAuthentication
{
    public class OAuthAuthenticationHttpHandler : SecurityTokenServiceHttpHandler
    {
        public override void ProcessRequest(System.Web.HttpContext context)
        {
            if (!String.IsNullOrWhiteSpace(context.Request.QueryString["redirectOauth"]) && context.Request.QueryString["redirectOauth"] == "true")
            {
                var authResult = OpenAuth.VerifyAuthentication(context.Request.RawUrl);

                if (authResult.IsSuccessful)
                {
                    var providerName = authResult.Provider;
                    var providerUserId = authResult.ProviderUserId;
                    var provierUserName = authResult.UserName;
                    var name = authResult.ExtraData["name"];

                    var userManager = UserManager.GetManager();
                    var currentUser = userManager.GetUsers().Where(user => user.UserName == providerUserId).FirstOrDefault();
                    if (currentUser == null)
                    {
                        var nameParts = name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameParts.Count() == 2)
                            SystemManager.RunWithElevatedPrivilege(p => { CreateUser(providerUserId, nameParts[0], nameParts[1], provierUserName); });
                        else
                        {
                            SystemManager.RunWithElevatedPrivilege(p => { CreateUser(providerUserId, name, "", provierUserName); });
                        }

                        currentUser = userManager.GetUsers().Where(user => user.UserName == providerUserId).FirstOrDefault();
                    }
                    var vals = context.Request.RequestContext.RouteData.Values;
                    var service = ((string)vals["Service"]).ToLower();

                    this.SetAuthCookie(currentUser);
                    var reqMessage = RequestMessage.Empty;
                    this.SendToken(context, currentUser, reqMessage, service);
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(context.Request.QueryString["redirect_url_failure"]))
                    {
                        context.Response.Redirect(context.Request.QueryString["redirect_url_failure"]);

                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                }

            }

            base.ProcessRequest(context);
        }

        private void CreateUser(string username, string firstname, string lastname, string email)
        {
            string transaction = "FacebookLoginCreateUserTransaction" + Guid.NewGuid().ToString();
            var userManager = UserManager.GetManager("", transaction);
            var profileManager = UserProfileManager.GetManager("", transaction);
            var roleManager = RoleManager.GetManager("AppRoles", transaction);


            var currentUser = userManager.CreateUser(username);

            var adminRole = roleManager.GetRoles().Where(role => role.Name == "Users").FirstOrDefault();
            currentUser.IsBackendUser = false;

            roleManager.AddUserToRole(currentUser, adminRole);

            TransactionManager.CommitTransaction(transaction);

            var profile = profileManager.CreateProfile(currentUser, "Telerik.Sitefinity.Security.Model.SitefinityProfile") as SitefinityProfile;

            profile.FirstName = firstname;
            profile.LastName = lastname;
            profile.Nickname = username;
            profileManager.RecompileItemUrls<SitefinityProfile>(profile);

            TransactionManager.CommitTransaction(transaction);
        }

        protected override void SendLoginForm(HttpContext context, string message)
        {
            if (!String.IsNullOrWhiteSpace(context.Request.QueryString["oauthProvider"]) && String.IsNullOrWhiteSpace(context.Request.QueryString["redirectOauth"]))
            {
                OpenAuth.RequestAuthentication(context.Request.QueryString["oauthProvider"], context.Request.RawUrl + "&redirectOauth=true");
            }

            base.SendLoginForm(context, message);
        }
    }
}
