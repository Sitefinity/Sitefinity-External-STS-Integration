Type.registerNamespace("OauthExternalAuthentication.Web.UI");


OauthExternalAuthentication.Web.UI.oAuthLoginDesigner = function (element) {
    OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.initializeBase(this, [element]);
    this._pageSelectorOpenedDelegate = null;
    this._pageSelectorClosedDelegate = null;
    this._pagePicker = null;
}

OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.prototype = {
    initialize: function () {
        OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.callBaseMethod(this, 'initialize');

        this._pageSelectorOpenedDelegate = Function.createDelegate(this, this._pageSelectorOpened);
        this._pageSelectorClosedDelegate = Function.createDelegate(this, this._pageSelectorClosed);
        this.get_pagePicker().add_selectorOpened(this._pageSelectorOpenedDelegate);
        this.get_pagePicker().add_selectorClosed(this._pageSelectorClosedDelegate);

    },

    dispose: function () {

        if (this.oAuthAuthenticationDelegate) {

            delete this.oAuthAuthenticationDelegate;
        }

        if (this._pageSelectorOpenedDelegate != null)
            delete this._pageSelectorOpenedDelegate;
        if (this._pageSelectorClosedDelegate != null)
            delete this._pageSelectorClosedDelegate;

        OauthExternalAuthentication.Web.UI.oAuthLoginDesigner.callBaseMethod(this, 'dispose');
    },

    _pageSelectorOpened: function () {
        dialogBase.resizeToContent();
    },

    _pageSelectorClosed: function () {
        dialogBase.resizeToContent();
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