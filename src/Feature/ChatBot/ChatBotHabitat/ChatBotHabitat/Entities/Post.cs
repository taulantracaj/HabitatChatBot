namespace Sitecore.Feature.ChatBotHabitat.Entities
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class Post
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Url { get; set; }
    }
}