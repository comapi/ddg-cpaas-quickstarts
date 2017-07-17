/*
 * A basic example of receiving data from Comapi using webhooks
 */
package com.comapi.webhook;

import com.cedarsoftware.util.io.JsonReader;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Scanner;
import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;
import com.cedarsoftware.util.io.JsonWriter;
import java.util.Map;
import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.xml.bind.DatatypeConverter;

/**
 *
 * @author dave.baddeley
 */
@WebServlet(name = "WebhookServlet", urlPatterns = {"/"})
public class WebhookServlet extends HttpServlet {

    /**
     * Handles the HTTP <code>GET</code> method, and returns a simple holding
     * page to allow easy end point hosting testing.
     *
     * @param request servlet request
     * @param response servlet response
     * @throws ServletException if a servlet-specific error occurs
     * @throws IOException if an I/O error occurs
     */
    @Override
    protected void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // Return a simple test page to make it easy to test the forwarding end point
        response.setContentType("text/html;charset=UTF-8");
        try (PrintWriter out = response.getWriter()) {
            out.println("<!DOCTYPE html>");
            out.println("<html>");
            out.println("<head>");
            out.println("<title>Comapi webhook page</title>");
            out.println("</head>");
            out.println("<body>");
            out.println("<h1>Comapi webhook page</h1>");
            out.println("<p>Configure this page as your Comapi webhook location to start receiving data.</p>");
            out.println("</body>");
            out.println("</html>");
        }
    }

    /**
     * Handles the HTTP <code>POST</code> method, and receives the data sent
     * from Comapi
     *
     * @param request servlet request
     * @param response servlet response
     * @throws ServletException if a servlet-specific error occurs
     * @throws IOException if an I/O error occurs
     */
    @Override
    protected void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        // Process data receivied to ensure it is from Comapi and hasn't been tampered with.

        // Grab the body data
        Scanner s = new Scanner(request.getInputStream(), "UTF-8").useDelimiter("\\A");
        String raw_body = s.hasNext() ? s.next() : "";

        if (raw_body == null || raw_body.isEmpty()) // No body, bad request.
        {
            response.setStatus(HttpServletResponse.SC_BAD_REQUEST);
            response.getWriter().println("Bad request - No JSON body found!");
            return;
        }

        // We have a request body so lets look at what we have.
        // First lets ensure it hasn't been tampered with and it came from Comapi.
        // We do this by checking the HMAC from the X-Comapi-Signature header.
        String request_hmac = request.getHeader("x-comapi-signature");

        if (request_hmac == null || request_hmac.isEmpty()) // No HMAC, invalid request.
        {
            response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
            response.getWriter().println("Invalid request: No HMAC value found!");
            return;
        }

        // Validate the HMAC, ensure you has exposed the rawBody, see app.js for how to do this.
        String hash = createHmacSha1(raw_body, "a secret");

        if (request_hmac.compareToIgnoreCase(hash) != 0) // The request is not from Comapi or has been tampered with
        {
            response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
            response.getWriter().println("Invalid request: HMAC hash check failed!");
            return;
        }

        // Store the received event for later processing, remember you only have 10 secs to process, in this simple example we output to the console.
        Map eventObj = JsonReader.jsonToMaps(raw_body);

        System.out.println("");
        System.out.println(String.format("Received a %s event id: %s", eventObj.get("name"), eventObj.get("eventId")));
        System.out.println(JsonWriter.formatJson(raw_body)); // Pretty System.out.println the JSON
        System.out.println("");

        // You could use queuing tech such as RabbitMQ, or possibly a distributed cache such as Redis.
        
        // All good
        response.setStatus(HttpServletResponse.SC_OK);
        response.getWriter().println("Data accepted");
    }

    /**
     * Returns a short description of the servlet.
     *
     * @return a String containing servlet description
     */
    @Override
    public String getServletInfo() {
        return "This is a basic example of a servlet written for Java to receive data from Comapi's webhooks.";
    }

    //////////////////
    // Private methods
    //////////////////
    private static String createHmacSha1(String data, String key) {
        try {
            // Get an hmac_sha1 key from the raw key bytes
            byte[] keyBytes = key.getBytes();
            SecretKeySpec signingKey = new SecretKeySpec(keyBytes, "HmacSHA1");

            // Get an hmac_sha1 Mac instance and initialize with the signing key
            Mac mac = Mac.getInstance("HmacSHA1");
            mac.init(signingKey);

            // Compute the hmac on input data bytes
            byte[] rawHmac = mac.doFinal(data.getBytes());

            //  Covert array of bytes to a hex string
            return DatatypeConverter.printHexBinary(rawHmac).toLowerCase();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}
