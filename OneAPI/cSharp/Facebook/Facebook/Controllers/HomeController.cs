using Newtonsoft.Json;
using RestSharp;
using System;
using System.Web.Mvc;

namespace Facebook.Controllers
{
    public class HomeController : Controller
    {
        // **** Enter you API Space and security token details here ****
        const string APISPACE = "YOUR API SPACE ID";
        const string TOKEN = "YOUR SECURITY TOKEN";
        public ActionResult Index()
        {
            var viewData = new Models.HomeIndexViewModel();

            // If logged in create the Facebook metadata for the logged in user using the Comapi web service.
            if (Request.IsAuthenticated)
            {
                try
                {
                    viewData.FacebookMetaData = GetFacebookMetaData(User.Identity.Name);
                }
                catch (Exception ex)
                {
                    // An error occurred.
                    viewData.TestMessageResult = new Models.ResultFeedback()
                    {
                        Success = false,
                        FeedbackMessage = string.Format(@"Failed to generate Facebook metadata the error message was: {0}", ex.Message)
                    };
                }
            }

            // Render the view with the model
            return View(viewData);
        }

        public ActionResult TestMessage()
        {
            var viewData = new Models.HomeIndexViewModel();

            try
            {
                // Create the Facebook metadata for the logged in user using the Comapi web service.
                viewData.FacebookMetaData = GetFacebookMetaData(User.Identity.Name);

                // Send a test message via the Comapi "One" API

                // Create the request
                var myRequest = new FacebookSendRequest()
                {
                    to = new FacebookSendRequest.toStruct() { profileId = User.Identity.Name }, // Current logged in user
                    body = "A test message sent via Comapi!"
                };

                // Send it
                SendFacebookMessage(myRequest);

                // Set the result
                viewData.TestMessageResult = new Models.ResultFeedback()
                {
                    Success = true,
                    FeedbackMessage = "Test message sent successfully, check Facebook"
                };
            }
            catch (Exception ex)
            {
                // An error occurred.
                viewData.TestMessageResult = new Models.ResultFeedback()
                {
                    Success = false,
                    FeedbackMessage = string.Format(@"The web service call failed: {0}", ex.Message)
                };
            }

            // Render the view with the model
            return View("Index", viewData);
        }

        public ActionResult TestRichMessage()
        {
            var viewData = new Models.HomeIndexViewModel();

            try
            {
                // Create the Facebook metadata for the logged in user using the Comapi web service.
                viewData.FacebookMetaData = GetFacebookMetaData(User.Identity.Name);

                // Send a test message via the Comapi "One" API

                // Create the request
                var myRequest = new FacebookSendRequest()
                {
                    to = new FacebookSendRequest.toStruct { profileId = User.Identity.Name }, // Current logged in user
                    customBody = new FacebookSendRequest.customBodyStruct { fbMessenger = @"
                        {
                          ""attachment"": {
                            ""type"": ""image"",
                            ""payload"": {
                                        ""url"": ""https://scontent.xx.fbcdn.net/v/t1.0-1/p200x200/17156020_1871286216424427_1662368582524349363_n.jpg?oh=22685c22a19fc2e28e69634e6a920972&oe=592FD3D1""
                            }
                                }
                        }" }
                };

                // Send it
                SendFacebookMessage(myRequest);

                // Set the result
                viewData.TestMessageResult = new Models.ResultFeedback()
                {
                    Success = true,
                    FeedbackMessage = "Test message sent successfully, check Facebook"
                };
            }
            catch (Exception ex)
            {
                // An error occurred.
                viewData.TestMessageResult = new Models.ResultFeedback()
                {
                    Success = false,
                    FeedbackMessage = string.Format(@"The web service call failed: {0}", ex.Message)
                };
            }

            // Render the view with the model
            return View("Index", viewData);
        }

        private static void SendFacebookMessage(FacebookSendRequest FacebookRequest)
        {
            // Setup a REST client object using the message send URL with our API Space incorporated
            var client = new RestClient(string.Format("https://api.comapi.com/apispaces/{0}/messages", APISPACE));
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + TOKEN); // Add the security token

            // Serialise our Facebook request object to JSON for submission
            string requestJson = JsonConvert.SerializeObject(FacebookRequest, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
        /// Retrieves the encrypted meta data to allow Comapi to associate a FB id with a profile.
        /// </summary>
        /// <param name="ProfileId">The Comapi profile id you want the FB id saved to</param>
        /// <returns>The encrypted Facebook meta data</returns>
        private static String GetFacebookMetaData(String ProfileId)
        {
            // Create the request JSON                              
            var requestJson = string.Format(@"{{ ""profileId"": ""{0}"" }}", ProfileId);

            // Setup a REST client object using the message send URL with our API Space incorporated
            var client = new RestClient(string.Format("https://api.comapi.com/apispaces/{0}/channels/facebook/state", APISPACE));
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + TOKEN); // Add the security token
            request.AddParameter("application/json", requestJson, ParameterType.RequestBody);

            // Make the web service call
            IRestResponse response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // Something went wrong.
                throw new InvalidOperationException(string.Format("Call to Comapi failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
            }

            // Grab the Facebook metadata for the profileId stripping the double quotes
            return response.Content.Replace(@"""", "");
        }


        /// <summary>
        /// JSON.Net Converter for raw JSON
        /// </summary>
        public class RawJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteRawValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(string).IsAssignableFrom(objectType);
            }

            public override bool CanRead
            {
                get { return false; }
            }
        }

        /// <summary>
        /// This object represents an Facebook send for the Comapi "One" API
        /// </summary>
        public class FacebookSendRequest
        {
            public FacebookSendRequest()
            {
                // Default the Comapi channel rules to Facebook
                this.rules = new string[] { "fbMessenger" };
            }

            #region "Structs"
            public struct toStruct
            {
                public toStruct(string profileId)
                {
                    this.profileId = profileId;
                }

                /// <summary>
                /// The profileId you wnat to send the message to
                /// </summary>
                public string profileId;
            }

            public struct customBodyStruct
            {
                /// <summary>
                /// This can be any valid Facebook message body types in JSON format, see the Facebook graph API docs for more info
                /// </summary>
                [JsonConverter(typeof(RawJsonConverter))]
                public string fbMessenger { get; set; }
            }

            #endregion

            /// <summary>
            /// The message body in text format
            /// </summary>
            public string body { get; set; }

            /// <summary>
            /// The addressing information
            /// </summary>
            public toStruct to { get; set; }

            /// <summary>
            /// The option custom body for the request
            /// </summary>
            public customBodyStruct customBody { get; set; }

            /// <summary>
            /// The Comapi API channel rules
            /// </summary>
            public string[] rules { get; set; }
        }
    }
}
