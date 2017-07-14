using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Reflection;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json.Linq;

namespace OauthExternalAuthentication
{
    public class FacebookClientUpdated : OAuth2Client
    {
        #region Constants and Fields

        /// <summary>
        /// The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://www.facebook.com/dialog/oauth";

        /// <summary>
        /// The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://graph.facebook.com/oauth/access_token";

        /// <summary>
        /// The _app id.
        /// </summary>
        private readonly string appId;

        /// <summary>
        /// The _app secret.
        /// </summary>
        private readonly string appSecret;

        /// <summary>
        /// The scope.
        /// </summary>
        private readonly string[] scope;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookClient"/> class
        /// with "email" as the scope.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        public FacebookClientUpdated(string appId, string appSecret)
            : this(appId, appSecret, "email") {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookClient"/> class.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        /// <param name="scope">
        /// The scope of authorization to request when authenticating with Facebook. The default is "email".
        /// </param>
        public FacebookClientUpdated(string appId, string appSecret, params string[] scope)
            : base("facebook") {
            this.appId = appId;
            this.appSecret = appSecret;
            this.scope = scope;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get service login url.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>An absolute URI.</returns>
        protected override Uri GetServiceLoginUrl(Uri returnUrl) {
            // Note: Facebook doesn't like us to url-encode the redirect_uri value
            var builder = new UriBuilder(AuthorizationEndpoint);
            builder.AppendQueryArgs(
                new Dictionary<string, string> {
                    { "client_id", this.appId },
                    { "redirect_uri", returnUrl.AbsoluteUri },
                    { "scope", string.Join(" ", this.scope) },
                });
            return builder.Uri;
        }

        /// <summary>
        /// The get user data.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <returns>A dictionary of profile data.</returns>
        protected override IDictionary<string, string> GetUserData(string accessToken) {
            FacebookGraphData graphData;
            var escapeUriDataStringRfc3986 = typeof(MessagingUtilities).GetMethod("EscapeUriDataStringRfc3986", BindingFlags.Static | BindingFlags.NonPublic);
            var request =
                WebRequest.Create(
                    "https://graph.facebook.com/me?access_token=" + (string)escapeUriDataStringRfc3986.Invoke(null, new[] { accessToken }) + "&fields=id,name,email,birthday,gender,link");
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(FacebookGraphData));
                    graphData = (FacebookGraphData)serializer.ReadObject(responseStream);
                }
            }

            // this dictionary must contains 
            var userData = new Dictionary<string, string>();
            userData.AddItemIfNotEmpty("id", graphData.Id);
            userData.AddItemIfNotEmpty("username", graphData.Email);
            userData.AddItemIfNotEmpty("name", graphData.Name);
            userData.AddItemIfNotEmpty("link", graphData.Link == null ? null : graphData.Link.AbsoluteUri);
            userData.AddItemIfNotEmpty("gender", graphData.Gender);
            userData.AddItemIfNotEmpty("birthday", graphData.Birthday);
            return userData;
        }

        /// <summary>
        /// Obtains an access token given an authorization code and callback URL.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="authorizationCode">
        /// The authorization code.
        /// </param>
        /// <returns>
        /// The access token.
        /// </returns>
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode) {
            // Note: Facebook doesn't like us to url-encode the redirect_uri value
            var builder = new UriBuilder(TokenEndpoint);
            builder.AppendQueryArgs(
                new Dictionary<string, string> {
                    { "client_id", this.appId },
                    { "redirect_uri", NormalizeHexEncoding(returnUrl.AbsoluteUri) },
                    { "client_secret", this.appSecret },
                    { "code", authorizationCode },
                    { "scope", "email" },
                });

            using (WebClient client = new WebClient()) {
                string data = client.DownloadString(builder.Uri);
                if (string.IsNullOrEmpty(data)) {
                    return null;
                }

                // update in order to be compliant with section 5.1 of RFC 6749
                // [Oauth Access Token] Format - The response format of https://www.facebook.com/v2.3/oauth/access_token 
                // returned when you exchange a code for an access_token now return valid JSON instead of being URL encoded. 
                // The new format of this response is {"access_token": {TOKEN}, "token_type":{TYPE}, "expires_in":{TIME}}. 
                // update to be compliant with section 5.1 of RFC 6749.
                JObject response = JObject.Parse(data);
                string accessToken = response.Value<string>("access_token");
                return accessToken;
            }
        }

        /// <summary>
        /// Converts any % encoded values in the URL to uppercase.
        /// </summary>
        /// <param name="url">The URL string to normalize</param>
        /// <returns>The normalized url</returns>
        /// <example>NormalizeHexEncoding("Login.aspx?ReturnUrl=%2fAccount%2fManage.aspx") returns "Login.aspx?ReturnUrl=%2FAccount%2FManage.aspx"</example>
        /// <remarks>
        /// There is an issue in Facebook whereby it will rejects the redirect_uri value if
        /// the url contains lowercase % encoded values.
        /// </remarks>
        private static string NormalizeHexEncoding(string url) {
            var chars = url.ToCharArray();
            for (int i = 0; i < chars.Length - 2; i++) {
                if (chars[i] == '%') {
                    chars[i + 1] = char.ToUpperInvariant(chars[i + 1]);
                    chars[i + 2] = char.ToUpperInvariant(chars[i + 2]);
                    i += 2;
                }
            }
            return new string(chars);
        }

        #endregion
    }
}
