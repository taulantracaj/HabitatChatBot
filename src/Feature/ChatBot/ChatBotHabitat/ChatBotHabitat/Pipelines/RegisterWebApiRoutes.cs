namespace Sitecore.Feature.ChatBotHabitat.Pipelines
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Sitecore.Pipelines;
    using System.Web.Http;
    public class RegisterWebApiRoutes
  {
    public void Process(PipelineArgs args)
    {
      RouteTable.Routes.MapHttpRoute("Feature.ChatBotHabitat.Api", "api/chatbot/Messages/{action}", new
                                                                                  {
                                                                                    Controller = "Messages"
                                                                                  });
    }
  }
}