<%@ Control Language="C#" %>
<%@ Register TagPrefix="sitefinity" Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.Fields" %>
<div>Landing Page : </div>

<sitefinity:PageField 
            ID="pageField" 
            runat="server"     
            BindOnLoad="false" 
            DisplayMode="Write" 
            WebServiceUrl="~/Sitefinity/Services/Pages/PagesService.svc/"
            CssClass="sfMLeft15 sfMTop5 sfMBottom10" />