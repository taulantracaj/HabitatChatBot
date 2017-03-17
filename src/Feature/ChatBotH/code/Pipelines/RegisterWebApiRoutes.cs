namespace Sitecore.Feature.ChatBot.Pipelines
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Sitecore.Pipelines;
    using System.Web.Http;
    public class RegisterWebApiRoutes
  {
    public void Process(PipelineArgs args)
    {
      RouteTable.Routes.MapHttpRoute("Feature.ChatBot.Api", "api/chatbot/ChatBot/{action}", new
                                                                                  {
                                                                                    Controller = "ChatBot"
      });
    }
  }
}