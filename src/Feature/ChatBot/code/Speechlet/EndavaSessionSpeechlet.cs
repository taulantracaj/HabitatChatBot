//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Web.UI.WebControls;
using Alexa.SkillsKit.Slu;
using Alexa.SkillsKit.Speechlet;
using Alexa.SkillsKit.UI;
using Newtonsoft.Json;
using NLog;

namespace Alexa.Endpoint.Controllers
{
    public class EndavaSessionSpeechlet : Alexa.SkillsKit.Speechlet.Speechlet
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        // Note: CALENDAR_KEY being a JSON property key gets camelCased during serialization
        private const string CalendarKey = "room";
        private const string CalendarSlot = "Room";
        //startTime} until {endTime} with title {eventTitle} and invite {namePerson}

        private const string StartTimeKey = "startTime";
        private const string StartTimeSlot = "startTime";

        private const string NamePersonKey = "namePerson";
        private const string NamePersonSlot = "namePerson";

        private const string EndTimeKey = "endTime";
        private const string EndTimeSlot = "endTime";

        private const string EventTitleKey = "eventTitle";
        private const string EventTitleSlot = "eventTitle";


        public override void OnSessionStarted(SessionStartedRequest request, Session session) {            
          //  Log.Info("OnSessionStarted requestId={0}, sessionId={1}", request.RequestId, session.SessionId);
        }


        public override SpeechletResponse OnLaunch(LaunchRequest request, Session session) {
            //Log.Info("OnLaunch requestId={0}, sessionId={1}", request.RequestId, session.SessionId);
            return GetWelcomeResponse();
        }


        public override SpeechletResponse OnIntent(IntentRequest request, Session session) {
            //Log.Info("OnIntent requestId={0}, sessionId={1}", request.RequestId, session.SessionId);

            // Get intent from the request object.
            var intent = request.Intent;

            var intentName = intent?.Name;

            switch (intentName)
            {
                case "MyCalendarIsIntent":
                {
                    return SetRoomSettingInSessionAndWelcome(intent, session);
                }
                case "WhatsMyEventIntent":
                {
                    return GetCalendarFromSessionAndListAllMyEvents(intent, session);
                 }

                case "AddEventIntent":
                    {
                        return AddCalendarEventToMyRoom(intent, session);
                    }
                default:
                {
                        return SetRoomSettingInSessionAndWelcome(intent, session);
                        // throw new SpeechletException("Invalid Intent");
                 }

            }
           
        }


        public override void OnSessionEnded(SessionEndedRequest request, Session session)
        {
            var a = session;
            //Log.Info("OnSessionEnded requestId={0}, sessionId={1}", request.RequestId, session.SessionId);
        }


        /**
         * Creates and returns a {@code SpeechletResponse} with a welcome message.
         * 
         * @return SpeechletResponse spoken and visual welcome message
         */
        private SpeechletResponse GetWelcomeResponse() {
            // Create the welcome message.
           // string speechOutput =  "Welcome master,please tell which room do you want to open by saying my room is Ohrid for example";
            string speechOutput =  "Ouch";

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return BuildSpeechletResponse("Welcome", speechOutput, false);
        }


        /**
         * Creates a {@code SpeechletResponse} for the intent and stores the extracted name in the
         * Session.
         * 
         * @param intent
         *            intent for the request
         * @return SpeechletResponse spoken and visual response the given intent
         */
        private SpeechletResponse SetRoomSettingInSessionAndWelcome(Intent intent, Session session) {
            // Get the slots from the intent.
            Dictionary<string, Slot> slots = intent.Slots;

            if (slots == null)
            {
                session.Attributes[CalendarKey] = "Gmail";
                return BuildSpeechletResponse(intent.Name, $"Master, Now your wish is my command.", false);
            }
            // Get the name slot from the list slots.
            Slot calendarSlot = slots[CalendarSlot];
            string speechOutput = "";

            // Check for name and create output to user.
            if (calendarSlot != null) {
                // Store the user's name in the Session and create response.
                string calendarSetting = calendarSlot.Value;
                session.Attributes[CalendarKey] = calendarSetting;
                speechOutput = $"Master, Now your wish is my command.";
            } 
            else {
                // Render an error since we don't know what the users name is.
                speechOutput = "Don't lie to me Master, please try again";
            }

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return BuildSpeechletResponse(intent.Name, speechOutput, false);
        }



        private SpeechletResponse AddCalendarEventToMyRoom(Intent intent, Session session)
        {
            string speechOutput = "";
            bool shouldEndSession = false;
            var serverName = ConfigurationManager.AppSettings["hostServerName"];

            // Get the user's name from the session.
            var startDateTime = (String)session.Attributes[StartTimeKey];
            var endDateTime = (String)session.Attributes[EndTimeKey];
            var namePerson = (String)session.Attributes[NamePersonKey];

            Dictionary<string, Slot> slots = intent.Slots;
            Slot startTimeSlot = slots[StartTimeSlot];
            Slot endTimeSlot = slots[EndTimeSlot];
            Slot eventTitleSlot = slots[EventTitleSlot];

            EventModel em = new EventModel()
            {
                title = slots[EventTitleSlot].Value.ToString(),
                start = slots[StartTimeSlot].Value.ToString(),
                end = slots[EndTimeSlot].Value.ToString(),
                actions = "",
                color = "",
                provider = ""
            };


            //Dictionary<string,string> tmDictionary = new Dictionary<string, string>();
           
            //tmDictionary.Add(slots[StartTimeSlot].Name.ToString(), slots[StartTimeSlot].Value.ToString());
            //tmDictionary.Add(slots[EndTimeSlot].Name.ToString(), slots[EndTimeSlot].Value.ToString());
            //tmDictionary.Add(slots[EventTitleSlot].Name.ToString(), slots[EventTitleSlot].Value.ToString());


            if (em != null)
            { 
                var jsonResponse = JsonConvert.SerializeObject(em);

                //using (var client = new HttpClient())
                //{
                //    var tmpResult = client.PostAsJsonAsync(serverName + "/Alexa.Google/google/addevent", jsonResponse).Result;
                //    speechOutput = tmpResult.ToString();
                //}

                var bytes = Encoding.ASCII.GetBytes(jsonResponse);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(serverName + "/Alexa.Google/google/addevent"));
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();


                shouldEndSession = true;
            }

            return BuildSpeechletResponse(intent.Name, speechOutput, shouldEndSession);
        }


        /**
         * Creates a {@code SpeechletResponse} for the intent and get the user's name from the Session.
         * 
         * @param intent
         *            intent for the request
         * @return SpeechletResponse spoken and visual response for the intent
         */
        private SpeechletResponse GetCalendarFromSessionAndListAllMyEvents(Intent intent, Session session) {
            string speechOutput = "";
            bool shouldEndSession = false;

            // Get the user's name from the session.
            var calendarSetting = (String)session.Attributes[CalendarKey];
            var serverName = ConfigurationManager.AppSettings["hostServerName"];
            

            speechOutput = $"You have  three events today in your {calendarSetting}, goodbye";
           
            using (var client = new HttpClient())
            {
                var productDetailUrl = serverName + @"/Alexa.google/google/listevents";
                var model = client
                            .GetAsync(productDetailUrl)
                            .Result
                            .Content.ReadAsStringAsync().Result;

                speechOutput = model.ToString();
            }
            

            shouldEndSession = true;
            

            return BuildSpeechletResponse(intent.Name, speechOutput, shouldEndSession);
        }


        /**
         * Creates and returns the visual and spoken response with shouldEndSession flag
         * 
         * @param title
         *            title for the companion application home card
         * @param output
         *            output content for speech and companion application home card
         * @param shouldEndSession
         *            should the session be closed
         * @return SpeechletResponse spoken and visual response for the given input
         */
        private SpeechletResponse BuildSpeechletResponse(string title, string output, bool shouldEndSession) {
            // Create the Simple card content.
            SimpleCard card = new SimpleCard();
            card.Title = String.Format("SessionSpeechlet - {0}", title);
            card.Subtitle = String.Format("SessionSpeechlet - Sub Title");
            card.Content = String.Format("SessionSpeechlet - {0}", output);

            // Create the plain text output.
            PlainTextOutputSpeech speech = new PlainTextOutputSpeech();
            speech.Text = output;

            // Create the speechlet response.
            SpeechletResponse response = new SpeechletResponse();
            response.ShouldEndSession = shouldEndSession;
            response.OutputSpeech = speech;
            response.Card = card;
            return response;
        }
    }

    public class EventModel
    {

        [JsonProperty("title")]
        public virtual string title { get; set; }

        [JsonProperty("start")]
        public virtual string start { get; set; }

        [JsonProperty("end")]
        public virtual string end { get; set; }

        [JsonProperty("provider")]
        public virtual string provider { get; set; }

        [JsonProperty("color")]
        public virtual string color { get; set; }

        [JsonProperty("actions")]
        public virtual string actions { get; set; }

    }
}