using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OauthExternalAuthentication.AmazonProvider
{
    public class Requires
    {
        public static string NotNullOrEmpty(string value, string parameterName)
        {
            Requires.NotNull<string>(value, parameterName);
            Requires.True(value.Length > 0, parameterName, "Parameter value can't be null or empty string");
            return value;
        }

        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if ((object)value == null)
                throw new ArgumentNullException(parameterName);
            else
                return value;
        }

        internal static void True(bool condition, string parameterName = null, string message = null)
        {
            if (!condition)
                throw new ArgumentException(message , parameterName);
        }
    }
}
