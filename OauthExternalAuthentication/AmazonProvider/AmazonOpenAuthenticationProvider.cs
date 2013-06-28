using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using DotNetOpenAuth.AspNet;

namespace OauthExternalAuthentication.AmazonProvider
{
    public class AmazonOpenAuthenticationProvider : IAuthenticationClient
    {
        #region Properties

        private string AppID { get; set; }
        private string SecretKey { get; set; }
        private string AuthorizationEndpoint { get { return "https://www.amazon.com/ap/oa"; } }
        private string AccessTokenEndpoint { get { return "https://api.amazon.com/auth/o2/token"; } }
        private string ProfileEndpoint { get { return "https://api.amazon.com/user/profile"; } }


        public string ProviderName
        {
            get { return "Amazon"; }
        }

        #endregion

        #region Constructor

        public AmazonOpenAuthenticationProvider(string appId, string secretKey)
        {
            Requires.NotNullOrEmpty("appId", appId);
            Requires.NotNullOrEmpty("secretKey", secretKey);

            this.AppID = appId;
            this.SecretKey = secretKey;
        }

        #endregion

        #region RequestAuthentication

        public void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            var serviceUrl = this.GetServiceLoginUrl(returnUrl);
            context.Response.Redirect(serviceUrl.AbsoluteUri, true);
        }

        private Uri GetServiceLoginUrl(Uri returnUrl)
        {
            UriBuilder builder = new UriBuilder(AuthorizationEndpoint);


            StringBuilder queryStringBuilder = new StringBuilder();
            
            UriUtilities.AppendQueryStringArgument(queryStringBuilder, "client_id", this.AppID);
            var state = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(returnUrl.Query));

            UriUtilities.AppendQueryStringArgument(queryStringBuilder, "state", state);
            UriUtilities.AppendQueryStringArgument(queryStringBuilder, "scope", "profile");
            UriUtilities.AppendQueryStringArgument(queryStringBuilder, "response_type", "code");

            UriUtilities.AppendQueryStringArgument(queryStringBuilder, "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path));

            builder.Query = queryStringBuilder.ToString();
            return builder.Uri;
        }

        public AuthenticationResult VerifyAuthentication(HttpContextBase context)
        {
            try
            {
                var code = context.Request.QueryString["code"];


                var accessToken = QueryAccessToken(UriUtilities.ConvertToAbsoluteUri(context.Request.RawUrl, context), code);

                var result = GetCustomerProfile(accessToken);

                AuthenticationResult authenticationResult = new AuthenticationResult(true, this.ProviderName, result["user_id"], result["name"], result);
                return authenticationResult;
            }
            catch (Exception ex)
            {
                AuthenticationResult authenticationResult = new AuthenticationResult(ex);
                return authenticationResult;
            }
        }



        private Dictionary<string, string> GetCustomerProfile(string accessToken)
        {
            UriBuilder builder = new UriBuilder(this.ProfileEndpoint);
            StringBuilder queryStringBuilder = new StringBuilder();
            UriUtilities.AppendQueryStringArgument(queryStringBuilder, "access_token", accessToken);

            builder.Query = queryStringBuilder.ToString();

            Dictionary<string, string> result = new Dictionary<string, string>();

            using (WebClient webClient = new WebClient())
            {
                string response = webClient.DownloadString(builder.Uri);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                result = serializer.Deserialize<Dictionary<string, string>>(response);
            }

            return result;
        }

        private string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            string postData = "grant_type=authorization_code";
            postData += "&code=" + authorizationCode;
            postData += "&redirect_uri=" + returnUrl.GetLeftPart(UriPartial.Path);
            postData += "&client_id=" + this.AppID;
            postData += "&client_secret=" + this.SecretKey;

            var response = SSLWebRequest(this.AccessTokenEndpoint, postData);

            return response["access_token"];
        }
        #endregion

        #region Private Methods

        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        private Dictionary<string, string> SSLWebRequest(string url, string postData)
        {
            Dictionary<string, string> result = null;

            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(url);

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            UTF8Encoding encoding = new UTF8Encoding();

            byte[] data = encoding.GetBytes(postData);

            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;

            using (Stream stream = httpWReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse())
            {

                using (var responseStream = response.GetResponseStream())
                {
                    string responseString = new StreamReader(responseStream).ReadToEnd();

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    result = serializer.Deserialize<Dictionary<string, string>>(responseString);
                }
            }

            return result;
        }

        #endregion


    }
}
