<%@Application Language='C#' Inherits="Sitecore.Web.Application" %>
<script Language='C#' runat="server">
  protected void Application_PostAuthorizeRequest()
{
    System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
}

</script>