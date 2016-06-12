<%@ Control Language="C#" %>
<%@ Import Namespace="Microsoft.AspNet.Membership.OpenAuth" %>
<%@ Import Namespace="System.Linq" %>
<div id="oAuthProviders">
    <% if (OpenAuth.AuthenticationClients.GetAll().Any(p => p.ProviderName == "myfacebook"))
       { %>
    <a  href="javascript:void(0);" class="oauthloginprovider">
        <img id="facebook" src="<%=this.Page.ClientScript.GetWebResourceUrl(typeof(OauthExternalAuthentication.Web.UI.OAuthLoginForm), "OauthExternalAuthentication.Web.UI.Images.Facebook.png")%>" alt="Facebook" />
    </a>
    <br />
    <%} %>
    <% if (OpenAuth.AuthenticationClients.GetAll().Any(p => p.ProviderName == "google"))
       { %>
    <a href="javascript:void(0);" class="oauthloginprovider">
        <img id="google" src="<%=this.Page.ClientScript.GetWebResourceUrl(typeof(OauthExternalAuthentication.Web.UI.OAuthLoginForm), "OauthExternalAuthentication.Web.UI.Images.Google.png")%>" alt="Google" />
    </a>
    <br />
    <%} %>
    <% if (OpenAuth.AuthenticationClients.GetAll().Any(p => p.ProviderName == "Amazon"))
       { %>
    <a href="javascript:void(0);" class="oauthloginprovider">
        <img id="Amazon" src="<%=this.Page.ClientScript.GetWebResourceUrl(typeof(OauthExternalAuthentication.Web.UI.OAuthLoginForm), "OauthExternalAuthentication.Web.UI.Images.Amazon.png")%>" alt="Amazon" />
    </a>
    <%} %>
</div>
