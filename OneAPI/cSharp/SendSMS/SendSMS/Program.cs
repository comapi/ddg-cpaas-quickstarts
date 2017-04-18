/******************************************************************************
 * Description: Simple SMS send example using the Comapi "One" API
 * Author:      Dave Baddeley
 *****************************************************************************/
 
using Newtonsoft.Json;
using RestSharp;
using System;

namespace SendSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Comapi \"One\" API SMS send example");
                Console.ForegroundColor = ConsoleColor.White;

                // Create an SMS request.
                var myRequest = new SMSSendRequest();
                myRequest.to = new SMSSendRequest.toStruct("ENTER A MOBILE NUMBER");
                myRequest.body = "This is an SMS via Comapi \"One\" API";

                // Set the channel options; optional step, comment out to use a local number to send from automatically
                var myChannelOptions = new SMSSendRequest.channelOptionsStruct();
                myChannelOptions.sms = new SMSSendRequest.smsChannelOptions() { from = "Comapi", allowUnicode = false };
                myRequest.channelOptions = myChannelOptions;

                // Send it.
                SendSMS(myRequest);

                // All good
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("SMS sent successfully");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                // Error
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", ex);
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Wait
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        private static void SendSMS(SMSSendRequest smsRequest)
        {
            // **** Enter you API Space and security token details here ****
            const string APISPACE = "YOUR API SPACE ID";
            const string TOKEN = "YOUR SECURITY TOKEN";

            // Setup a REST client object using the message send URL with our API Space incorporated
            var client = new RestClient(string.Format("https://api.comapi.com/apispaces/{0}/messages", APISPACE));
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + TOKEN); // Add the security token
            
            // Serialise our SMS request object to JSON for submission
            string requestJson = JsonConvert.SerializeObject(smsRequest);
            request.AddParameter("application/json", requestJson, ParameterType.RequestBody);

            // Make the web service call
            IRestResponse response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                // Something went wrong.
                throw new InvalidOperationException(string.Format("Call to Comapi failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
            }
        }

        /// <summary>
        /// This object represents an SMS send for the Comapi "One" API
        /// </summary>
        public class SMSSendRequest
        {
            public SMSSendRequest()
            {
                // Default the Comapi channel rules to SMS
                this.rules = new string[] { "sms" };
            }

            #region "Structs"
            public struct toStruct
            {
                public toStruct(string mobileNumber)
                {
                    this.phoneNumber = mobileNumber;
                }

                /// <summary>
                /// The phone number you want to send to in international format e.g. 447123123123
                /// </summary>
                public string phoneNumber;
            }

            public struct smsChannelOptions
            {
                /// <summary>
                /// The originator the SMS is from, this could be a phone number, shortcode or alpha
                /// </summary>
                public string from;

                /// <summary>
                /// Flag to indicate whether unicode messages are allowed to be sent
                /// </summary>
                public bool allowUnicode;
            }

            public struct channelOptionsStruct
            {
                /// <summary>
                /// The SMS channels options
                /// </summary>
                public smsChannelOptions sms;
            }
            #endregion

            /// <summary>
            /// The SMS message body
            /// </summary>
            public string body { get; set; }

            /// <summary>
            /// The addressing information
            /// </summary>
            public toStruct to { get; set; }

            /// <summary>
            /// The channel options for the request
            /// </summary>
            public channelOptionsStruct channelOptions { get; set; }

            /// <summary>
            /// The Comapi API channel rules
            /// </summary>
            public string[] rules { get; set; }
        }
    }
}
