﻿namespace Sitecore.Feature.ChatBot.Controllers
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Sitecore.Feature.ChatBot.Dialogs;
using Sitecore.Feature.ChatBot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.FormFlow;
using System.Diagnostics;

    [BotAuthentication]
    [RoutePrefix("api/ChatBotHabitat")]
    [Route("{action=Index}")]
    public class ChatBotHabitatController : ApiController
    {
        [AcceptVerbs("POST")]
        [HttpPost]
        public async Task<HttpResponseMessage> Index([FromBody]Activity activity)
        {

            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new ChatBotLuisDialog());
            }
            else
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var reply = HandleSystemMessage(activity);
                if (reply != null)
                    await connector.Conversations.ReplyToActivityAsync(reply);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                string replyMessage = Responses.WelcomeMessage;
                return message.CreateReply(replyMessage);
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

    }
}
