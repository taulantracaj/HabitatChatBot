namespace ChatBotHabitat.Pipelines.GetPageRendering
{
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

    public class RegisterHttpRoutes
    {
        public void Process(PipelineArgs args)
        {
            GlobalConfiguration.Configure(Configure);
        }

        protected void Configure(HttpConfiguration configuration)
        {
            var routes = configuration.Routes;
            routes.MapHttpRoute("MessagesApi", "sitecore/api/messages", new
            {
                controller = "Messages",
                action = "Post"
            });

        }
    }
}
