Type.registerNamespace("OauthExternalAuthentication.Web.UI");

OauthExternalAuthentication.Web.UI.OAuthLoginForm = function (element) {
    OauthExternalAuthentication.Web.UI.OAuthLoginForm.initializeBase(this, [element]);

    this.oAuthAuthenticationDelegate = null;
    this._oAuthServiceUrl = null;
}

OauthExternalAuthentication.Web.UI.OAuthLoginForm.prototype = {
    initialize: function () {
        OauthExternalAuthentication.Web.UI.OAuthLoginForm.callBaseMethod(this, 'initialize');

        jQuery.support.cors = true;
        this.oAuthAuthenticationDelegate = Function.createDelegate(this, this.oAuthAuthentication);
        var items = $("#oAuthProviders a");
        for (var i = 0; i < items.length; i++) {
            var button = items[i];
            $addHandler(button, "click", this.oAuthAuthenticationDelegate);
        }
       
         
    },

    dispose: function () {
        
        if (this.oAuthAuthenticationDelegate) {
         
            delete this.oAuthAuthenticationDelegate;
        }

        OauthExternalAuthentication.Web.UI.OAuthLoginForm.callBaseMethod(this, 'dispose');
    },

    get_oAuthServiceUrl: function () {
        return this._oAuthServiceUrl;
    },
    set_oAuthServiceUrl: function (value) {
        this._oAuthServiceUrl = value;
    },

    oAuthAuthentication: function (args) {
         
        var provider = args.target.id;
        var returnURL = $(location).attr("href");
        var loginUrl = this.get_oAuthServiceUrl() + "&oauthProvider=" + provider;
        $(location).attr('href', loginUrl);
    }
}

OauthExternalAuthentication.Web.UI.OAuthLoginForm.registerClass('OauthExternalAuthentication.Web.UI.OAuthLoginForm', Telerik.Sitefinity.Web.UI.PublicControls.LoginWidget);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();