using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis;
namespace Sitecore.Feature.ChatBotHabitat.Dialogs
{
    using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis.Models;
//using MeBot.Internal;
//using MeBot.Entities;
using System.Threading;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;
using System.Configuration;
using Sitecore.Feature.ChatBotHabitat.Entities;
using Sitecore.Feature.ChatBotHabitat.Utils;
using Google.Apis.QPXExpress.v1;
using Google.Apis.Services;
using Google.Apis.QPXExpress.v1.Data;


    [LuisModel("7b9addce-10fa-4fe3-aa89-91eeab1d33b2", "5cb7b7eec1c54d51a79cf9bb13ce1d36")]
    [Serializable]
    public class ChatBotLuisDialog : LuisDialog<object>
    {
        private const string BLOG_BASE_URL = "https://ankitbko.github.io";

        public ChatBotLuisDialog(params ILuisService[] services) : base(services)
        {
        }


        [LuisIntent("TestFlight")]
        public async Task TestFlight(IDialogContext context, LuisResult result)
        {

            QPXExpressService service = new QPXExpressService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDrkQXN1uB7_BHJgWmH_ZDX1gDQO6GMoi4",
                ApplicationName = "FligjtsSC",
            });

            TripsSearchRequest x = new TripsSearchRequest();
            x.Request = new TripOptionsRequest();
            x.Request.Passengers = new PassengerCounts { AdultCount = 2 };
            x.Request.Slice = new List<SliceInput>();
            x.Request.Slice.Add(new SliceInput() { Origin = "ADD", Destination = "NBO", Date = "2017-03-08" });
            x.Request.Solutions = 10;
            var result1 = service.Trips.Search(x).Execute();


            //QPXExpressService service = new QPXExpressService(new BaseClientService.Initializer()
            //{
            //    ApiKey = "AIzaSyDrkQXN1uB7_BHJgWmH_ZDX1gDQO6GMoi4",
            //    ApplicationName = "FligjtsSC",
            //});
            //TripsSearchRequest x = new TripsSearchRequest();
            //x.Request = new TripOptionsRequest();
            //x.Request.Passengers = new PassengerCounts { AdultCount = 2 };
            ////x.Request.Slice = new SliceInfo List();
            //x.Request.Slice.Add(new SliceInput() { Origin = "JFK", Destination = "BOS", Date = "2017-003-04" });
            //x.Request.Solutions = 10;
            //var result1 = service.Trips.Search(x).Execute();

            foreach (var aircraft in result1.Trips.Data.Aircraft)
            {

                Console.WriteLine(aircraft.Name + aircraft.Code);

            }

            // Airport
            foreach (var airport in result1.Trips.Data.Airport)
            {
                Console.WriteLine(airport.Name + " - " + airport.City);
            }
            foreach (var carrier in result1.Trips.Data.Carrier)
            {
                Console.WriteLine(carrier.Name);
            }


            foreach (var trip in result1.Trips.TripOption)
            {

                Console.WriteLine("Flight Number: " + trip.Slice.FirstOrDefault().Segment.FirstOrDefault().Flight.Number);
                Console.WriteLine("    Duration: " + trip.Slice.FirstOrDefault().Duration);
                Console.WriteLine("     Cabin: " + trip.Slice.FirstOrDefault().Segment.FirstOrDefault().Cabin);
                Console.WriteLine("   price: " + trip.Pricing.FirstOrDefault().BaseFareTotal.ToString());

                await context.PostAsync(trip.Slice.FirstOrDefault().Segment.FirstOrDefault().Flight.Number);
            }

            
            context.Wait(MessageReceived);
        }
        
        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            var cts = new CancellationTokenSource();
            await context.Forward(new GreetingsDialog(), GreetingDialogDone, await message, cts.Token);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Responses.HelpMessage);
            context.Wait(MessageReceived);
        }

        [LuisIntent("AboutMe")]
        public async Task AboutMe(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(@"Ankit is a Software Engineer currently working in Microsoft Center of Excellence team at Mindtree. 
                He started his professional career in 2013 after completing his graduation as Bachelor in Computer Science.");
            await context.PostAsync(@"He is a technology enthusiast and loves to dig in emerging technologies. 
                Most of his working hours are spent on creating architecture, evaluating upcoming products and developing frameworks.");
            context.Wait(MessageReceived);
        }

        //[LuisIntent("BlogSearch")]
        //public async Task BlogSearch(IDialogContext context, LuisResult result)
        //{
        //    string tag = string.Empty;
        //    string replyText = string.Empty;
        //    List<Post> posts = new List<Post>();

        //    try
        //    {
        //        if (result.Entities.Count > 0)
        //        {
        //            tag = result.Entities.FirstOrDefault(e => e.Type == "Tag").Entity;
        //        }

        //        if (!string.IsNullOrWhiteSpace(tag))
        //        {
        //            var bs = new BlogSearch();
        //            posts = bs.GetPostsWithTag(tag);
        //        }

        //        replyText = GenerateResponseForBlogSearch(posts, tag);
        //        await context.PostAsync(replyText);
        //    }
        //    catch (Exception)
        //    {
        //        await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
        //    }
        //    finally
        //    {
        //        context.Wait(MessageReceived);
        //    }
        //}

        [LuisIntent("Feedback")]
        public async Task Feedback(IDialogContext context, LuisResult result)
        {
            try
            {
                await context.PostAsync("That's great. You will need to provide few details about yourself before giving feedback.");
                var feedbackForm = new FormDialog<FeedbackForm>(new FeedbackForm(), FeedbackForm.BuildForm, FormOptions.PromptInStart);
                context.Call(feedbackForm, FeedbackFormComplete);
            }
            catch (Exception)
            {
                await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
                context.Wait(MessageReceived);
            }
        }


        #region Private
        private static string recipientEmail = ConfigurationManager.AppSettings["RecipientEmail"];
        private static string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];

        private string GenerateResponseForBlogSearch(List<Post> posts, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return "I didn't get what topic you are searching for. It might be that Ankit has not written any article so I am not able to recognize that topic. You may try to change the topic and try again.";
            if (posts.Count == 0)
                return "Ankit has not written any article regarding " + tag + ". Contact him on Twitter to let him know you are interested in ." + tag;

            string replyMessage = string.Empty;
            replyMessage += $"I got {posts.Count} articles on {tag} \n\n";
            foreach (var post in posts)
            {
                replyMessage += $"* [{post.Name}]({BLOG_BASE_URL}{post.Url})\n\n";
            }
            replyMessage += $"Have fun reading. Post a comment if you like them.";
            return replyMessage;
        }

        private async Task GreetingDialogDone(IDialogContext context, IAwaitable<bool> result)
        {
            var success = await result;
            if (!success)
                await context.PostAsync("I'm sorry. I didn't understand you.");

            context.Wait(MessageReceived);
        }

        private async Task FeedbackFormComplete(IDialogContext context, IAwaitable<FeedbackForm> result)
        {
            try
            {
                var feedback = await result;
                string message = GenerateEmailMessage(feedback);
                var success = await EmailSender.SendEmail(recipientEmail, senderEmail, $"Email from {feedback.Name}", message);
                if (!success)
                    await context.PostAsync("I was not able to send your message. Something went wrong.");
                else
                {
                    await context.PostAsync("Thanks for the feedback.");
                    await context.PostAsync("What else would you like to do?");
                }

            }
            catch (FormCanceledException)
            {
                await context.PostAsync("Don't want to send feedback? That's ok. You can drop a comment below.");
            }
            catch (Exception)
            {
                await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
            }
            finally
            {
                context.Wait(MessageReceived);
            }
        }

        private string GenerateEmailMessage(FeedbackForm feedback)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Message from: {feedback.Name}");
            sb.AppendLine($"Contact: {feedback.Contact}");
            sb.AppendLine($"Message: {feedback.Feedback}");
            return sb.ToString();
        }

        #endregion
    }
}