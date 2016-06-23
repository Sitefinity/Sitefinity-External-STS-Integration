using System;
using System.Linq;
using System.Text;
using System.Web;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.GoogleOAuth2;
using Microsoft.AspNet.Membership.OpenAuth;
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
            if (!String.IsNullOrEmpty(context.Request.QueryString["state"]))
            {
                this.TranslateStateArgument(context);
            }

            if (!String.IsNullOrWhiteSpace(context.Request.QueryString["redirectOauth"]) && context.Request.QueryString["redirectOauth"] == "true")
            {
                var authResult = OpenAuth.VerifyAuthentication(HttpContext.Current.Request.RawUrl);

                if (authResult.IsSuccessful)
                {
                    this.HandleSuccessfullOAuth(authResult, context);
                }
                else
                {
                    this.HandleFailureOAuth(authResult, context);
                }
            }

            base.ProcessRequest(context);
        }

        private void TranslateStateArgument(HttpContext context)
        {
            var stateQueryString = context.Request.QueryString["state"];
            var decodedQueryString = stateQueryString;

            if (stateQueryString == null || !stateQueryString.Contains("__provider__=google"))
            {
                decodedQueryString = System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(stateQueryString));
                if (decodedQueryString.StartsWith("?"))
                    decodedQueryString = decodedQueryString.Substring(1);
            }

            UriBuilder builder = new UriBuilder(context.Request.Url.GetLeftPart(UriPartial.Path));
            var queryStringBuilder = new StringBuilder();
            var otherKeys = context.Request.QueryString.AllKeys.Where(p => p != "state");

            queryStringBuilder.Append(decodedQueryString);

            foreach (var key in otherKeys)
            {
                if (!queryStringBuilder.ToString().Contains(key + "="))
                {
                    queryStringBuilder.Append("&");
                    queryStringBuilder.Append(key);
                    queryStringBuilder.Append("=");
                    queryStringBuilder.Append(context.Request.QueryString[key]);
                }
            }

            builder.Query = queryStringBuilder.ToString();
            context.Response.Redirect(builder.Uri.AbsoluteUri);
        }

        private void HandleSuccessfullOAuth(AuthenticationResult authResult, System.Web.HttpContext context)
        {
            var providerName = authResult.Provider;
            var providerUserId = authResult.ProviderUserId;
            var provierUserName = authResult.UserName;
            var name = string.Empty;
            if (authResult.ExtraData.ContainsKey("name"))
            {
                name = authResult.ExtraData["name"];
            }

            var userManager = UserManager.GetManager();
            var currentUser = userManager.GetUsers().Where(user => user.UserName == providerUserId).FirstOrDefault();
            if (currentUser == null)
            {
                var nameParts = name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Count() == 2)
                    SystemManager.RunWithElevatedPrivilege(p => { CreateUser(providerUserId, nameParts[0], nameParts[1], provierUserName); });
                else
                {
                    SystemManager.RunWithElevatedPrivilege(p => { CreateUser(providerUserId, string.Empty, string.Empty, provierUserName); });
                }

                currentUser = userManager.GetUsers().Where(user => user.UserName == providerUserId).FirstOrDefault();
            }

            var vals = context.Request.RequestContext.RouteData.Values;
            var service = ((string)vals["Service"]).ToLower();

            this.SetAuthCookie(currentUser);
            var reqMessage = RequestMessage.Empty;
            this.SendToken(context, currentUser, reqMessage, service);
        }

        private void HandleFailureOAuth(AuthenticationResult authResult, System.Web.HttpContext context)
        {
            if (!String.IsNullOrWhiteSpace(context.Request.QueryString["redirect_url_failure"]))
            {
                context.Response.Redirect(context.Request.QueryString["redirect_url_failure"]);

                context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        private void CreateUser(string username, string firstname, string lastname, string email)
        {
            string transaction = "FacebookLoginCreateUserTransaction" + Guid.NewGuid().ToString();
            var userManager = UserManager.GetManager(string.Empty, transaction);
            var profileManager = UserProfileManager.GetManager(string.Empty, transaction);
            var roleManager = RoleManager.GetManager("AppRoles", transaction);

            var currentUser = userManager.CreateUser(username);

            var adminRole = roleManager.GetRoles().Where(role => role.Name == "Users").FirstOrDefault();
            currentUser.IsBackendUser = false;
            currentUser.Email = email;

            roleManager.AddUserToRole(currentUser, adminRole);

            TransactionManager.CommitTransaction(transaction);

            var profile = profileManager.CreateProfile(currentUser, "Telerik.Sitefinity.Security.Model.SitefinityProfile") as SitefinityProfile;

            if (!String.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(lastname) && !string.IsNullOrEmpty(username))
            {
                profile.FirstName = firstname;
                profile.LastName = lastname;
                profile.Nickname = username;
                profileManager.RecompileItemUrls<SitefinityProfile>(profile);

                TransactionManager.CommitTransaction(transaction);
            }
        }

        protected override void SendLoginForm(HttpContext context, string message)
        {
            this.SentLoginForm();
            base.SendLoginForm(context, message);
        }

        protected override void SendLoginForm(HttpContextBase context, string message)
        {
            this.SentLoginForm();
            base.SendLoginForm(context, message);
        }

        private void SentLoginForm()
        {
            if (!String.IsNullOrWhiteSpace(SystemManager.CurrentHttpContext.Request.QueryString["oaprovider"]))
            {
                OpenAuth.RequestAuthentication(SystemManager.CurrentHttpContext.Request.QueryString["oaprovider"],
                    SystemManager.CurrentHttpContext.Request.RawUrl + "&redirectOauth=true"
                    );
            }
        }
    }
}
