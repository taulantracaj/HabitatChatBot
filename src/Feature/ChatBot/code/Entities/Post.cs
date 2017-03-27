﻿namespace Sitecore.Feature.ChatBot.Entities
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class Post
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Url { get; set; }
    }
}
