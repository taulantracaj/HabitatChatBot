using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sitecore.Feature.ChatBotHabitat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(WebApiConfig), "Start")]
namespace Sitecore.Feature.ChatBotHabitat
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           // Web API configuration and services

            // Web API routes
            // config.MapHttpAttributeRoutes();

            //force json responses only
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            // Json settings
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
    }
}
