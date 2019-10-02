/******************************************************************************************************
 * Description: SMS send example using the "One" API and impersonation tokens for resllers and partners
 * Author:      Dave Baddeley
 ******************************************************************************************************/

using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace SendSMS
{
    class Program
    {
        // **** Enter your reseller / partner impersonation token details here
        // **** Note: In a production environment these should be held securely
        private const string RESELLER_TOKEN_ISSUER = "YOUR ISS VALUE YOU REGISTERED WITH DOTDIG";
        private const string RESELLER_TOKEN_SECRET = "YOUR SECRET YOU REGISTERED WITH DOTDIG";
        private const int RESELLER_ACCOUNT_ID = 0; // Your reseller / partner account number

        // **** Enter you customer API Space details here ****
        private const int CUSTOMER_ACCOUNT_ID = 0;
        private const string CUSTOMER_APISPACE = "CUSTOMERS API SPACE";

        // **** Enter your mobile number here ****
        private const string MOBILE_NUMBER = "447123123123";
        private const int BATCH_SIZE = 3;

        static void Main(string[] args)
        {
            try
            {
                // Ensure we use later versions of TLS for security
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768;

                // Start the console
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("dotdigital CPaaS \"One\" API SMS send using reseller / partner impersonation tokens example");
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
                myChannelOptions.sms = new SMSSendRequest.smsChannelOptions() { from = "OneAPI", allowUnicode = false };

                // Send the messages
                switch (mode)
                {
                    case "single":
                        // Create an SMS request.
                        myRequest = new SMSSendRequest();
                        myRequest.to = new SMSSendRequest.toStruct(MOBILE_NUMBER);
                        myRequest.body = "This is an SMS via dotdigital CPaaS \"One\" API";
                        myRequest.channelOptions = myChannelOptions;

                        // Send it.
                        SendSMS(myRequest);

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
                        SendSMSBatch(myBatch);

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

        private static void SendSMS(SMSSendRequest smsRequest)
        {
            // Setup a REST client object using the message send URL with our API Space incorporated
            var client = new RestClient(string.Format("https://api.comapi.com/apispaces/{0}/messages", CUSTOMER_APISPACE));
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "Bearer " + 
                CreateImpersonationToken(
                    CUSTOMER_ACCOUNT_ID, 
                    RESELLER_ACCOUNT_ID, 
                    RESELLER_TOKEN_ISSUER, 
                    RESELLER_TOKEN_SECRET)); // Add the security token

            // Serialise our SMS request object to JSON for submission
            string requestJson = JsonConvert.SerializeObject(smsRequest, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", requestJson, ParameterType.RequestBody);

            // Make the web service call
            IRestResponse response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                // Something went wrong.
                throw new InvalidOperationException(string.Format("Call to Comapi failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
            }
            else
            {
                // Sucess output the response body
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(FormatJson(response.Content));
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void SendSMSBatch(SMSSendRequest[] smsRequests)
        {
            // Setup a REST client object using the message send URL with our API Space incorporated
            var client = new RestClient(string.Format("https://api.comapi.com/apispaces/{0}/messages/batch", CUSTOMER_APISPACE));
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "Bearer " + 
                CreateImpersonationToken(
                CUSTOMER_ACCOUNT_ID, 
                RESELLER_ACCOUNT_ID, 
                RESELLER_TOKEN_ISSUER, 
                RESELLER_TOKEN_SECRET)); // Add the security token

            // Serialise our SMS request object to JSON for submission
            string requestJson = JsonConvert.SerializeObject(smsRequests, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", requestJson, ParameterType.RequestBody);

            // Make the web service call
            IRestResponse response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // Something went wrong.
                throw new InvalidOperationException(string.Format("Call to Comapi failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
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
        /// Creates a JWT using the reseller / partners impersonation token settings and the customers account id
        /// </summary>
        /// <param name="impersonatedAccountId">The account id of the customer you want to impersonate</param>
        /// <param name="resellerId">The account id for the reseller / partner</param>
        /// <param name="issuer">Your unique JWT issuer you provided to the dotdigital CPaaS team</param>
        /// <param name="secret">Your unique JWT secret you provided to the dotdigital CPaaS team</param>
        /// <returns></returns>
        private static string CreateImpersonationToken(int impersonatedAccountId, int resellerId, string issuer, string secret)
        {
            // Create Security key using the secret
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                              (securityKey, SecurityAlgorithms.HmacSha256);

            //  Create a token
            var header = new JwtHeader(signingCredentials);

            // Create the JWT payload including the details on the customer to impersonate.
            var payload = new JwtPayload
           {
                { "accountId", impersonatedAccountId },
                { "resellerId", RESELLER_ACCOUNT_ID },
                { "aud", "https://api.comapi.com" },
                { "iss", issuer },
                { "exp", (long)(DateTime.UtcNow.AddMinutes(115) - new DateTime(1970, 1, 1)).TotalSeconds } // Expires 115 minutes in the the future based on UTC
           };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Export the JWT as a string
            return handler.WriteToken(secToken);
        }

        /// <summary>
        /// This object represents an SMS send for the dotdigital CPaaS "One" API
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
