namespace Sitecore.Feature.ChatBotHabitat.Forms
{
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#pragma warning disable 649
    [Serializable]
    class FlightForm
    {
        [Prompt("Please specify Date? Note date must be in 'yyyy-mm-dd' format and not older than today ")]
        public string Date { get; set; }
        [Template(TemplateUsage.EnumSelectOne, "How many adults: {&} {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
        public AdultNumber AdultCount { get; set; }
        [Prompt("Please specify departure airport code? Note departure must have 3 characters")]
        public string Origin { get; set; }
        [Prompt("Please specify Destination airport code? Note Destination must have 3 characters")]
        public string Destination { get; set; }
        public enum AdultNumber
        {
            One = 1, Two
        };
        public static IForm<FlightForm> BuildForm()
        {
            return new FormBuilder<FlightForm>()
                .Field(nameof(Date), validate: DateCompareAndFormat)
                .Field(nameof(Origin), validate: MaxNumberOfCharOrigin)
                .Field(nameof(Destination), validate: MaxNumberOfCharDestination)
                .Field(nameof(AdultCount))
                .AddRemainingFields()
                .Build();
        }
        private static Task<ValidateResult> ValidateContactInformation(FlightForm state, object response)
        {
            var result = new ValidateResult();
            string contactInfo = string.Empty;
            if (GetTwitterHandle((string)response, out contactInfo) || GetEmailAddress((string)response, out contactInfo))
            {
                result.IsValid = true;
                result.Value = contactInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter valid email address or twitter handle. Make sure twitter handle starts with @.";
            }
            return Task.FromResult(result);
        }
        private static bool GetEmailAddress(string response, out string contactInfo)
        {
            contactInfo = string.Empty;
            var match = Regex.Match(response, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            if (match.Success)
            {
                contactInfo = match.Value;
                return true;
            }
            return false;
        }
        private static bool GetTwitterHandle(string response, out string contactInfo)
        {
            contactInfo = string.Empty;
            if (!response.StartsWith("@"))
                return false;
            contactInfo = response;
            return true;
        }
        private static Task<ValidateResult> DateCompareAndFormat(FlightForm state, object response)
        {
            var result = new ValidateResult();
            string dateInfo = string.Empty;
            if (DateCompare((string)response, out dateInfo))//DateCompare((string)response) || 
            {
                result.IsValid = true;
                result.Value = dateInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter valid Date. Make sure date is  graeter or equal than today's date and the format is yyyy-MM-dd .";
            }
            return Task.FromResult(result);
        }
        private static bool DateFormat(string response)
        {
           // contactInfo = string.Empty;
            var match = Regex.Match(response, @"^\d{4}\-(0?[1-9]|1[012])\-(0?[1-9]|[12][0-9]|3[01])$");
            if (match.Success)
            {
                //contactInfo = match.Value;
                return true;
            }
            return false;
        }
        private static bool DateCompare(string response, out string dateInfo)
        {
            dateInfo = string.Empty;
            DateTime dateValue;
            if (DateTime.TryParseExact(response, "yyyy-MM-dd",
                              new CultureInfo("en-US"),
                              DateTimeStyles.None,
                              out dateValue))
            {
                DateTime strDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                if (dateValue >= strDate)
                {
                    dateInfo = dateValue.ToString("yyyy-MM-dd");
                    return true;
                }
              
            }

            return false;
        }
        private static Task<ValidateResult> MaxNumberOfCharOrigin(FlightForm state, object response)
        {
            var result = new ValidateResult();
            string stringValue = string.Empty;
            if (MaxNumberOfChar((string)response, out stringValue))
            {
                result.IsValid = true;
                result.Value = stringValue;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter valid Departure. Make sure Departure handle is 3 char long.";
            }
            return Task.FromResult(result);
        }
        private static Task<ValidateResult> MaxNumberOfCharDestination(FlightForm state, object response)
        {
            var result = new ValidateResult();
            string stringValue = string.Empty;
            if (MaxNumberOfChar((string)response, out stringValue))
            {
                result.IsValid = true;
                result.Value = stringValue;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter valid Destination. Make sure Destination handle is 3 char long.";
            }
            return Task.FromResult(result);
        }
        private static bool MaxNumberOfChar(string response, out string stringValue)
        {
            stringValue = string.Empty;
            if (response.Length == 3)
            {
                stringValue = response;
                return true;
            }

            return false;
        }
    };
}