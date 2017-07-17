/*
 * A basic example of receiving data from Comapi using webhooks
 */
package com.comapi.webhook;

import com.cedarsoftware.util.io.JsonReader;
import com.cedarsoftware.util.io.JsonWriter;
import java.util.Map;
import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;
import javax.xml.bind.DatatypeConverter;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

@Controller
@SpringBootApplication
public class Main {

    public static void main(String[] args) throws Exception {
        // Run the spring server.
        SpringApplication.run(Main.class, args);
    }

    /////////
    // Routes
    /////////
    @RequestMapping("/")
    public String index() {
        // Return a simple test page to make it easy to test the forwarding end point.
        return "index";
    }

    @RequestMapping(value = "/", method = RequestMethod.POST, consumes = MediaType.ALL_VALUE, produces = MediaType.TEXT_PLAIN_VALUE)
    @ResponseBody
    public ResponseEntity<String> indexPost(@RequestBody String raw_body, @RequestHeader("x-comapi-signature") String request_hmac) {
        // Process data receivied to ensure it is from Comapi and hasn't been tampered with.

        if (raw_body == null || raw_body.isEmpty()) // No body, bad request.
        {
            return new ResponseEntity<>("Bad request - No JSON body found!", HttpStatus.BAD_REQUEST);
        }

        // We have a request body so lets look at what we have.
        // First lets ensure it hasn't been tampered with and it came from Comapi.
        // We do this by checking the HMAC from the X-Comapi-Signature header.
        if (request_hmac == null || request_hmac.isEmpty()) // No HMAC, invalid request.
        {
            return new ResponseEntity<>("Invalid request: No HMAC value found!", HttpStatus.UNAUTHORIZED);
        }

        // Validate the HMAC, ensure you has exposed the rawBody, see app.js for how to do this.
        String hash = createHmacSha1(raw_body, ">>>YOUR SECRET<<<");

        if (request_hmac.compareToIgnoreCase(hash) != 0) // The request is not from Comapi or has been tampered with
        {
            return new ResponseEntity<>("Invalid request: HMAC hash check failed!", HttpStatus.UNAUTHORIZED);
        }

        // Store the received event for later processing, remember you only have 10 secs to process, in this simple example we output to the console.
        Map eventObj = JsonReader.jsonToMaps(raw_body);

        System.out.println("");
        System.out.println(String.format("Received a %s event id: %s", eventObj.get("name"), eventObj.get("eventId")));
        System.out.println(JsonWriter.formatJson(raw_body)); // Pretty System.out.println the JSON
        System.out.println("");

        // You could use queuing tech such as RabbitMQ, or possibly a distributed cache such as Redis.
        // All good
        return new ResponseEntity<>("Data accepted", HttpStatus.OK);
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
