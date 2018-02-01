/// <summary>
/// Description: Simple example to retrieve a messages status using the Comapi "One" API
/// Note:        We strongly recommend using webhooks instead of polling for efficency
/// Author:      Dave Baddeley
/// </summary>
namespace GetMessageStatus
{
    using Newtonsoft.Json;
    using RestSharp;
    using System;

    /// <summary>
    /// Simple example console app to retrieve a messages status using the Comapi "One" API
    /// </summary>
    public class Program
    {
        // **** Enter you API Space and security token details here ****
        private const string APISPACE = "YOUR_API_SPACE_ID";
        private const string TOKEN = "YOUR_ACCESS_TOKEN";

        /// <summary>
        /// Console app main entry point
        /// </summary>
        /// <param name="args">command line args</param>
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Comapi \"One\" API Get message status example");
                Console.ForegroundColor = ConsoleColor.White;

                string input = null;
                Guid messageId;

                // Ask the user what message they want the status for
                do
                {
                    Console.WriteLine("Please enter the message id (GUID) you want the status for or press enter to exit:");
                    input = Console.ReadLine().ToLower();

                    // Allow quit using empty input
                    if (string.IsNullOrEmpty(input))
                    {
                        break;
                    }

                    // Check the message id is valid
                    if (!Guid.TryParse(input, out messageId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid message id format; please ensure you message id is a GUID e.g. 862b3da5-20bc-496d-a3c8-e7e6868e0433");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        // Get the message status
                        GetMessageStatusCall(messageId);
                    }
                } while (true);

                // All good
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Finished");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                // Error
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", ex);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void GetMessageStatusCall(Guid messageId)
        {
            // Setup a RESTful client object using the message status URL with our API Space and message id incorporated
            var client = new RestClient(string.Format("https://api.comapi.com/apispaces/{0}/messages/{1}", APISPACE, messageId.ToString("D")));
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "Bearer " + TOKEN); // Add the security token

            // Make the web service call
            IRestResponse response = client.Execute(request);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    // Sucess output the response body
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(FormatJson(response.Content));
                    Console.ForegroundColor = ConsoleColor.White;

                    // Inspect the status
                    dynamic statusObject = JsonConvert.DeserializeObject(response.Content);

                    // Check to see if the message is delivered
                    Console.WriteLine(string.Format("The message ({0}) is currently in the {1} status.", messageId.ToString("D"), statusObject.status));
                    Console.WriteLine(string.Format("It was sent on the {0} channel.", statusObject.statusDetails.channel));

                    Console.WriteLine(string.Empty);
                    break;
                case System.Net.HttpStatusCode.NotFound:
                    // Sucess output the response body
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("The message ({0}) was not found!", messageId.ToString("D")));
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    // Something went wrong.
                    throw new InvalidOperationException(string.Format("Call to Comapi failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
            }
        }

        /// <summary>
        /// Formats JSON to make it more readable.
        /// </summary>
        /// <param name="json">The JSON string to format</param>
        /// <returns>Formatted JSON string</returns>
        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
