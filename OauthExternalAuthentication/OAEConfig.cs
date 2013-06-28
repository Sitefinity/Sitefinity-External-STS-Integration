using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;

namespace OauthExternalAuthentication
{
    public class OAEConfig : ConfigSection
    {

        #region Facebook 

        [ConfigurationProperty("FacebookAPPID")]
        [ObjectInfo(typeof(OauthExternalAuthenticationResources), Title = "FacebookAPPIDTitle", Description = "FacebookAPPIDDescription")]
        public string FacebookAPPID
        {
            get
            {
                return (string)this["FacebookAPPID"];
            }
            set
            {
                this["FacebookAPPID"] = value;
            } 
        }



        [ConfigurationProperty("FacebookAPPSecretKey")]
        [ObjectInfo(typeof(OauthExternalAuthenticationResources), Title = "FacebookAPPSecretKeyTitle", Description = "FacebookAPPSecretKeyDescription")]
        public string FacebookAPPSecretKey
        {
            get
            {
                return (string)this["FacebookAPPSecretKey"];
            }
            set
            {
                this["FacebookAPPSecretKey"] = value;
            }
        }

        #endregion

        #region Google Plus 

        [ConfigurationProperty("EnableGooglePlus")]
        [ObjectInfo(typeof(OauthExternalAuthenticationResources), Title = "EnableGooglePlusTitle", Description = "EnableGooglePlusDescription")]
        public bool EnableGooglePlus
        {
            get
            {
                return (bool)this["EnableGooglePlus"];
            }
            set
            {
                this["EnableGooglePlus"] = value;
            }
        }

        #endregion

        #region Amazon 

        [ConfigurationProperty("AmazonAPPID")]
        [ObjectInfo(typeof(OauthExternalAuthenticationResources), Title = "AmazonAPPIDTitle", Description = "AmazonAPPIDDescription")]
        public string AmazonAPPID
        {
            get
            {
                return (string)this["AmazonAPPID"];
            }
            set
            {
                this["AmazonAPPID"] = value;
            }
        }

        [ConfigurationProperty("AmazonAPPSecretKey")]
        [ObjectInfo(typeof(OauthExternalAuthenticationResources), Title = "AmazonAPPSecretKeyTitle", Description = "AmazonAPPSecretKeyDescription")]
        public string AmazonAPPSecretKey
        {
            get
            {
                return (string)this["AmazonAPPSecretKey"];
            }
            set
            {
                this["AmazonAPPSecretKey"] = value;
            }
        }

        #endregion
    }
}
