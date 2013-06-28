using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OauthExternalAuthentication.AmazonProvider
{
    public class UriUtilities
    {
        public static void AppendQueryStringArgument(StringBuilder queryStringBuilder, string paramenter, string value)
        {
            if (queryStringBuilder.Length> 0)
            {
                queryStringBuilder.Append("&");
            }
            queryStringBuilder.Append(paramenter);
            queryStringBuilder.Append("=");
            queryStringBuilder.Append(value);

        }

        public static Uri GetPublicFacingUrl(HttpRequestBase request)
        {
            NameValueCollection serverVariables = request.ServerVariables;
            if (serverVariables["HTTP_HOST"] == null)
                return new Uri(request.Url, request.RawUrl);
            string str = serverVariables["HTTP_X_FORWARDED_PROTO"] ?? (string.Equals(serverVariables["HTTP_FRONT_END_HTTPS"], "on", StringComparison.OrdinalIgnoreCase) ? Uri.UriSchemeHttps : request.Url.Scheme);
            Uri uri = new Uri(str + Uri.SchemeDelimiter + serverVariables["HTTP_HOST"]);
            return new UriBuilder(request.Url)
            {
                Scheme = str,
                Host = uri.Host,
                Port = uri.Port
            }.Uri;
        }

        public static Uri ConvertToAbsoluteUri(string returnUrl, HttpContextBase context)
        {
            if (Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute))
                return new Uri(returnUrl, UriKind.Absolute);
            if (!VirtualPathUtility.IsAbsolute(returnUrl))
                returnUrl = VirtualPathUtility.ToAbsolute(returnUrl);
            return new Uri(GetPublicFacingUrl(context.Request), returnUrl);
        }
    }
}
