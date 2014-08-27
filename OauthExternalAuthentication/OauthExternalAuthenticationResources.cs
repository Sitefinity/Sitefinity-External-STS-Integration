using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Localization;

namespace OauthExternalAuthentication
{
    [ObjectInfo(typeof(OauthExternalAuthenticationResources), Title = "OAEResourcesTitle", Description = "OAEDescription")]
    public class OauthExternalAuthenticationResources : Resource 
    {
        #region Class Description

        /// <summary>
        /// Help
        /// </summary>
        [ResourceEntry("OAEResourcesTitle",
                       Value = "Open Authentican Module Resources",
                       Description = "The title of this class.",
                       LastModified = "2013/06/01")]
         public string OAEResourcesTitle
        {
            get
            {
                return this["OAEResourcesTitle"];
            }
        }

        /// <summary>
        /// Contains localizable resources for help information such as UI elements descriptions, usage explanations, FAQ and etc.
        /// </summary>
        [ResourceEntry("OAEDescription",
                       Value = "Contains localizable resources for security user interface.",
                       Description = "The description of this class.",
                       LastModified = "2013/06/01")]
        public string OAEDescription
        {
            get
            {
                return this["OAEDescription"];
            }
        }

        #endregion

        #region Config Descriptions

        #region Facebook 

        #region Config Property - FacebookAPPID
        [ResourceEntry("FacebookAPPIDTitle",
                       Value = "Facebook Application ID Title",
                       Description = "Facebook Application ID ",
                       LastModified = "2013/06/01")]
        public string FacebookAPPIDTitle
        {
            get
            {
                return this["FacebookAPPIDTitle"];
            }
        }

        [ResourceEntry("FacebookAPPIDDescription",
                      Value = "Facebook Application ID Description",
                      Description = "Facebook Application ID Description",
                      LastModified = "2013/06/01")]
        public string FacebookAPPIDDescription
        {
            get
            {
                return this["FacebookAPPIDDescription"];
            }
        }

        #endregion

        #region Config Property - FacebookAPPSecretKey

        [ResourceEntry("FacebookAPPSecretKeyTitle",
                    Value = "Facebook Application Secret Key Title",
                    Description = "Facebook Application Secret Key",
                    LastModified = "2013/06/01")]
        public string FacebookAPPSecretKeyTitle
        {
            get
            {
                return this["FacebookAPPSecretKeyTitle"];
            }
        }

        [ResourceEntry("FacebookAPPSecretKeyDescription",
                      Value = "Facebook Application Secret Key Description",
                      Description = "Facebook Application Secret Key Description",
                      LastModified = "2013/06/01")]
        public string FacebookAPPSecretKeyDescription
        {
            get
            {
                return this["FacebookAPPSecretKeyDescription"];
            }
        }

        #endregion

        #endregion

        #region GooglePlus


        [ResourceEntry("GoogleClientIDTitle",
                      Value = "Google Plus Client ID",
                      Description = "Enable Google Plus",
                      LastModified = "2014/08/026")]
        public string GoogleClientIDTitle
        {
            get
            {
                return this["GoogleClientIDTitle"];
            }
        }

        [ResourceEntry("GoogleClientIDDescription",
                   Value = "Google Plus Client ID",
                   Description = "Enable Google Plus",
                   LastModified = "2014/08/026")]
        public string GoogleClientIDDescription
        {
            get
            {
                return this["GoogleClientIDDescription"];
            }
        }


        [ResourceEntry("GoogleClientSecretTitle",
                   Value = "Google Client Secret",
                   Description = "Enable Google Plus",
                   LastModified = "2014/08/026")]
        public string GoogleClientSecretTitle
        {
            get
            {
                return this["GoogleClientSecretTitle"];
            }
        }

        [ResourceEntry("GoogleClientSecretDescription",
                Value = "Google Client Secret",
                Description = "Enable Google Plus",
                LastModified = "2014/08/026")]
        public string GoogleClientSecretDescription
        {
            get
            {
                return this["GoogleClientSecretDescription"];
            }
        }

      

        #endregion

        #region Amazon

        #region Config Property - AmazonAPPID

        [ResourceEntry("AmazonAPPIDTitle",
                       Value = "Amazon Application ID Title",
                       Description = "Amazon Application ID ",
                       LastModified = "2013/06/01")]
        public string AmazonAPPIDTitle
        {
            get
            {
                return this["AmazonAPPIDTitle"];
            }
        }

        [ResourceEntry("AmazonAPPIDDescription",
                      Value = "Amazon Application ID Description",
                      Description = "Amazon Application ID Description",
                      LastModified = "2013/06/01")]
        public string AmazonAPPIDDescription
        {
            get
            {
                return this["AmazonDescription"];
            }
        }

        #endregion

        #region Config Property - FacebookAPPSecretKey

        [ResourceEntry("AmazonAPPSecretKeyTitle",
                    Value = "Amazon Application Secret Key Title",
                    Description = "Amazon Application Secret Key",
                    LastModified = "2013/06/01")]
        public string AmazonAPPSecretKeyTitle
        {
            get
            {
                return this["AmazonAPPSecretKeyTitle"];
            }
        }

        [ResourceEntry("AmazonAPPSecretKeyDescription",
                      Value = "Amazon Application Secret Key Description",
                      Description = "Amazon Application Secret Key Description",
                      LastModified = "2013/06/01")]
        public string AmazonAPPSecretKeyDescription
        {
            get
            {
                return this["AmazonAPPSecretKeyDescription"];
            }
        }

        #endregion

        #endregion

        #endregion

        #region UI

        [ResourceEntry("LandingPage",
                      Value = "Landing Page :",
                      Description = "Landing page label",
                      LastModified = "2013/06/01")]
        public string LandingPage
        {
            get
            {
                return this["LandingPage"];
            }
        }

        [ResourceEntry("LoginUsing",
                      Value = "Login using :",
                      Description = "Login using label",
                      LastModified = "2013/06/01")]
        public string LoginUsing
        {
            get
            {
                return this["LoginUsing"];
            }
        }
        

        #endregion
    }
}
