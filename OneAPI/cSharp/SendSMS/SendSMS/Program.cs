/******************************************************************************
 * Description: Simple SMS send example using the Enterprise Communications API
 * Author:      Dave Baddeley
 *****************************************************************************/
 
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SendSMS
{
    class Program
    {
        // **** Enter you API Space and security token details here ****
        private const string APISPACE = "YOUR_API_SPACE_ID";
        private const string TOKEN = "YOUR_ACCESS_TOKEN";

        // **** Enter your mobile number here ****
        private const string MOBILE_NUMBER = "447123123123";
        private const int BATCH_SIZE = 3;

        static async Task Main(string[] args)
        {            
            try
            {
                // Ensure we use later versions of TLS for security
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768;

                // Start the console
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Dotdigital enterprise Communications API SMS send example");
                Console.ForegroundColor = ConsoleColor.White;

                string input, mode = null;
                SMSSendRequest myRequest = null;

                // Ask the user what demo mode they want
                do
                {                    
                    Console.WriteLine("Send a `single` message or a `batch`, enter your choice now?");
                    input = Console.ReadLine().ToLower();
                    switch (input)
                    {
                        case "s":
                        case "single":
                            mode = "single";
                            Console.WriteLine("Performing a single send");
                            break;
                        case "b":
                        case "batch":
                            mode = "batch";
                            Console.WriteLine("Performing a batch send of {0} messages", BATCH_SIZE);
                            break;
                    }

                    if (!string.IsNullOrEmpty(mode)) break;
                 
                } while (true);

                // Set the channel options; optional step, comment out to use a local number to send from automatically
                var myChannelOptions = new SMSSendRequest.channelOptionsStruct();
                myChannelOptions.sms = new SMSSendRequest.smsChannelOptions() { from = "Example", allowUnicode = false };

                // Send the messages
                switch (mode)
                {
                    case "single":
                        // Create an SMS request.
                        myRequest = new SMSSendRequest();
                        myRequest.to = new SMSSendRequest.toStruct(MOBILE_NUMBER);
                        myRequest.body = "This is an SMS via Dotdigital Enterprise Communications API";
                        myRequest.channelOptions = myChannelOptions;

                        // Send it.
                        await SendSMS(myRequest);

                        break;
                    case "batch":
                        // Create a couple of requests in an array to create a batch of requests
                        SMSSendRequest[] myBatch = new SMSSendRequest[BATCH_SIZE];

                        for (int i = 0; i < BATCH_SIZE; i++)
                        {
                            // Create a message send request 
                            myRequest = new SMSSendRequest();
                            myRequest.to = new SMSSendRequest.toStruct(MOBILE_NUMBER);
                            myRequest.body = "This is message " + i;
                            myRequest.channelOptions = myChannelOptions;

                            // Add to batch array
                            myBatch[i] = myRequest;
                        }

                        // Send them
                        await SendSMSBatch(myBatch);

                        break;
                }
                
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

        private async static Task SendSMS(SMSSendRequest smsRequest)
        {
            // Setup a REST client object using the message send URL with our API Space incorporated
            var options = new RestClientOptions(string.Format("https://api.comapi.com/apispaces/{0}/messages", APISPACE))
            {
                ThrowOnAnyError = false,
                Timeout = 30000
            };

            var client = new RestClient(options);
            client.AddDefaultHeader("Content-Type", "application/json");
            client.AddDefaultHeader("Accept", "application/json");

            var request = new RestRequest();
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "Bearer " + TOKEN); // Add the security token
            
            // Serialise our SMS request object to JSON for submission
            string requestJson = JsonConvert.SerializeObject(smsRequest, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddStringBody(requestJson, ContentType.Json);

            // Make the web service call
            var response = await client.ExecutePostAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                // Something went wrong.
                throw new InvalidOperationException(string.Format("Call to Dotdigital Enterprise Communications API failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
            }
            else
            {
                // Sucess output the response body
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(FormatJson(response.Content));
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private async static Task SendSMSBatch(SMSSendRequest[] smsRequests)
        {
            // Setup a REST client object using the message send URL with our API Space incorporated
            var options = new RestClientOptions(string.Format("https://api.comapi.com/apispaces/{0}/messages/batch", APISPACE))
            {
                ThrowOnAnyError = false,
                Timeout = 30000
            };

            var client = new RestClient(options);
            client.AddDefaultHeader("Content-Type", "application/json");
            client.AddDefaultHeader("Accept", "application/json");

            var request = new RestRequest();
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "Bearer " + TOKEN); // Add the security token

            // Serialise our SMS request object to JSON for submission
            string requestJson = JsonConvert.SerializeObject(smsRequests, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddStringBody(requestJson, ContentType.Json);

            // Make the web service call
            var response = await client.ExecutePostAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // Something went wrong.
                throw new InvalidOperationException(string.Format("Call to Dotdigital Enterprise Communications API failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
            }
            else
            {
                // Sucess output the response body
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(FormatJson(response.Content));
                Console.ForegroundColor = ConsoleColor.White;
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
                public bool? allowUnicode;
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
            public channelOptionsStruct? channelOptions { get; set; }

            /// <summary>
            /// The Comapi API channel rules
            /// </summary>
            public string[] rules { get; set; }
        }

        /// <summary>
        /// Formats JSON to make it more readable.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Formatted JSON string</returns>
        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
