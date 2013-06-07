Type.registerNamespace("OauthExternalAuthentication.Web.UI");


OauthExternalAuthentication.Web.UI.oAuthLoginDesigner = function (element) {
    OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.initializeBase(this, [element]);
    
    this._pagePicker = null;
}

OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.prototype = {
    initialize: function () {
        OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.callBaseMethod(this, 'initialize');

        

    },

    dispose: function () {

        if (this.oAuthAuthenticationDelegate) {

            delete this.oAuthAuthenticationDelegate;
        }

        OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.callBaseMethod(this, 'dispose');
    },

    refreshUI: function () {
        var data = this._propertyEditor.get_control();
        var pageSelector = this.get_pagePicker();
        var pageID = data.RedirectPageIdSuccess;
        if (pageID) pageSelector.set_value(pageID);
    },

    applyChanges: function(){
        var controlData = this._propertyEditor.get_control();
        controlData.RedirectPageIdSuccess = this.get_pagePicker().get_value();
    },

    get_pagePicker: function () {
        return this._pagePicker;
    },
    set_pagePicker: function (value) {
        this._pagePicker = value;
    }
}

OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.registerClass('OauthExternalAuthentication.Web.UI.oAuthLoginDesigner', Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesignerBase);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();