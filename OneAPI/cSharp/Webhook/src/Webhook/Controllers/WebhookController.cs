/******************************************************************************
 * Description: Simple webhook implementation for receiving events from Comapi
 * Author:      Dave Baddeley
 *****************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Comapi webhook controller
/// </summary>
namespace Webhook.Controllers
{
    [Route("webhook")]
    public class WebhookController : Controller
    {
        // GET: /webhook
        [HttpGet, Produces("text/html")]
        public string Get()
        {
            return "<!DOCTYPE html><html> <head> <title>Comapi webhook page</title> </head> <body> <h1>Comapi webhook page</h1> <p>Configure this page as your Comapi webhook location to start receiving data.</p> </body></html>";
        }

        // POST /webhook
        [HttpPost]
        public IActionResult Post()
        {
            // Process data received from Comapi
            try
            {
                // Grab the body and parse to a JSON object
                string rawBody = GetDocumentContents(Request);
                if (string.IsNullOrEmpty(rawBody))
                {
                    // No body, bad request.
                    return BadRequest("Bad request - No JSON body found!");
                }

                // We have a request body so lets look at what we have

                // First lets ensure it hasn't been tampered with and it came from Comapi
                // We do this by checking the HMAC from the X-Comapi-Signature header
                string hmac = Request.Headers["x-comapi-signature"];

                if (String.IsNullOrEmpty(hmac))
                {
                    // No HMAC, invalid request.
                    Debug.WriteLine("Invalid request: No HMAC value found!");
                    return Unauthorized();
                }
                else
                {
                    // Validate the HMAC, ensure you has exposed the rawBody, see app.js for how to do this
                    var hash = CreateHMAC(rawBody, ">>>YOUR SECRET<<<");

                    if (hmac != hash)
                    {
                        // The request is not from Comapi or has been tampered with
                        Debug.WriteLine("Invalid request: HMAC hash check failed!");
                        return Unauthorized();
                    }
                }

                // Parse the recieved JSON to extract data
                dynamic eventObj = JsonConvert.DeserializeObject(rawBody);

                // Store the received event for later processing, remember you only have 10 secs to process, in this simple example we output to the console
                Debug.WriteLine("");
                Debug.WriteLine("Received a {0} event id: {1}", (string)eventObj.name, (string)eventObj.eventId);
                Debug.WriteLine(FormatJson(rawBody));

                // You could use queuing tech such as RabbitMQ, MSMQ or possibly a distributed cache such as Redis

                // All good return a 200
                return Ok("Data accepted");
            }
            catch (Exception err)
            {
                // An error occurred
                var msg = "An error occurred receiving data, the error was: " + err.ToString();
                Debug.WriteLine(msg);
                throw;
            }
        }

        /// <summary>
        /// Creates a HMAC-SHA1 hash
        /// </summary>
        /// <param name="data">The data to be hashed</param>
        /// <param name="key">The secret to use as a crypto key</param>
        /// <returns>HMAC-SHA1 hash for the data</returns>
        private string CreateHMAC(string data, string key)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);

            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(data);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);

            return ByteToString(hashmessage);
        }

        /// <summary>
        /// Converts a byte array to hex string
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary.ToLower());
        }

        /// <summary>
        /// Retrieves the body of a HTTP request as a string
        /// </summary>
        /// <param name="Request">The HTTP Request</param>
        /// <returns>The body data as a string</returns>
        private string GetDocumentContents(HttpRequest Request)
        {
            string documentContents;
            using (Stream receiveStream = Request.Body)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
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

