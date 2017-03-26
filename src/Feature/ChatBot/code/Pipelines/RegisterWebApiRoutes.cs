namespace Sitecore.Feature.ChatBotHabitat.Pipelines
{
    //using System.Web.Mvc;
    using System.Web.Routing;
    using Sitecore.Pipelines;
    using System.Web.Http;
    public class RegisterWebApiRoutes
  {
    public void Process(PipelineArgs args)
    {
      //RouteTable.Routes.MapHttpRoute("Feature.ChatBotHabitat.Api", 
      //    "api/ChatBotHabitat/{action}", 
      //    new { Controller = "ChatBot"   }
      //    );
            RouteTable.Routes.MapHttpRoute("Feature.ChatBotHabitat.Api",
          "api/{controller}/{action}"//,
          //new { Controller = "ChatBot" }
          );
          //  config.Routes.MapHttpRoute(
          //    name: "DefaultApi",
          //    routeTemplate: "api/{controller}/{id}",
          //    defaults: new { id = RouteParameter.Optional }
          //);
        }
  }
}