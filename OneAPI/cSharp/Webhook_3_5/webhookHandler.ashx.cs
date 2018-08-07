/******************************************************************************************
 * Description: Simple webhook implementation for receiving events from Comapi for .Net 3.5
 * Author:      Dave Baddeley
 ******************************************************************************************/
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Webhook_3_5.Utils;
using Webhook_3_5.Contracts;

namespace Webhook_3_5
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class webhookHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod == "GET")
            {
                // Return helper page to aid testing
                ReturnMessage(HttpStatusCode.OK, "Configure this page as your Comapi webhook location to start receiving data.", context.Response);
            }
            else if (context.Request.HttpMethod == "POST")
            {
                try
                {
                    // Process posted data
                    // Grab the body and parse to a JSON object
                    string rawBody = GetDocumentContents(context.Request);
                    if (string.IsNullOrEmpty(rawBody))
                    {
                        // No body, bad request.
                        ReturnMessage(HttpStatusCode.BadRequest, "Bad request - No JSON body found!", context.Response);
                        return;
                    }

                    // We have a request body so lets look at what we have

                    // First lets ensure it hasn't been tampered with and it came from Comapi
                    // We do this by checking the HMAC from the X-Comapi-Signature header
                    string hmac = context.Request.Headers["x-comapi-signature"];

                    if (String.IsNullOrEmpty(hmac))
                    {
                        // No HMAC, invalid request.
                        RollingLogger.LogMessage("Invalid request: No HMAC value found!");
                        ReturnMessage(HttpStatusCode.Unauthorized, "Invalid request: No HMAC value found!", context.Response);
                        return;
                    }
                    else
                    {
                        // Validate the HMAC; ensure you update the secret with the one you have configured on your webhook in Comapi
                        var hash = CreateHMAC(rawBody, ">>>YOUR SECRET<<<"); // <<<<<< Change this!!!!!!

                        if (hmac != hash)
                        {
                            // The request is not from Comapi or has been tampered with
                            RollingLogger.LogMessage("Invalid request: HMAC hash check failed!");
                            ReturnMessage(HttpStatusCode.Unauthorized, "Invalid request: HMAC hash check failed!", context.Response);
                        }
                    }

                    // Parse the recieved JSON to extract data; we strongly recommend using .Net 4 and dynamic objects where possible.
                    Event eventObj = null;
                    using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(rawBody)))
                    {
                        System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(new Event().GetType());
                        eventObj = serializer.ReadObject(ms) as Event;
                        ms.Close();
                    }

                    // Store the received event for later processing, remember you only have 10 secs to process, in this simple example we output to the console
                    RollingLogger.LogMessage("");
                    RollingLogger.LogMessage(String.Format("Received a {0} event id: {1}", eventObj.name, eventObj.eventId.ToString()));
                    RollingLogger.LogMessage(rawBody);

                    // You could use queuing tech such as RabbitMQ, MSMQ or possibly a distributed cache such as Redis

                    // All good return a 200
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch (Exception err)
                {
                    // An error occurred
                    var msg = "An error occurred receiving data, the error was: " + err.ToString();
                    RollingLogger.LogMessage(msg);
                    throw;
                }
            }
        }

        // Indicate that the handler results canot be cached
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Output a HTML message as the response.
        /// </summary>
        /// <param name="status">The HTTP status to send</param>
        /// <param name="message">The message to be delivered to the caller</param>
        /// <param name="response">The ASP.Net response object to use</param>
        private void ReturnMessage(HttpStatusCode status, string message, HttpResponse response)
        {
            response.ContentType = "text/html";
            response.StatusCode = (int)status;
            response.Write(string.Format("<!DOCTYPE html><html> <head> <title>Comapi webhook page</title> </head> <body> <h1>Comapi webhook page</h1> <p>{0}</p> </body></html>", message));
        }

        /// <summary>
        /// Creates a HMAC-SHA1 hash
        /// </summary>
        /// <param name="data">The data to be hashed</param>
        /// <param name="key">The secret to use as a crypto key</param>
        /// <returns>HMAC-SHA1 hash for the data</returns>
        private string CreateHMAC(string data, string key)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
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
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }
    }
}
