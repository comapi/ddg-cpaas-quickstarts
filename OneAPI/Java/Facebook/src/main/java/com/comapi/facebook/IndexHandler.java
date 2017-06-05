package com.comapi.facebook;

import com.mashape.unirest.http.HttpResponse;
import com.mashape.unirest.http.JsonNode;
import com.mashape.unirest.http.Unirest;
import com.mashape.unirest.http.exceptions.UnirestException;

/**
 * Controller behind the Index page to perform the web service calls
 * @author dave.baddeley
 */
public class IndexHandler {
    private String metadata;
    private String testButtonsDisplay;
    private Feedback feedback;
    
    // Comapi settings
    private static String APISPACE = "***ADD YOUR API SPACE ID HERE***";
    private static String TOKEN = "***ADD YOUR SECURITY TOKEN HERE***";
    
    // Profile string is hard coded but this would normally be the logged in user
    private String profileId = "joe.blogs@acme.com";
    
    public IndexHandler()
    {
        metadata = null;
        feedback = null;
        testButtonsDisplay = "none";
    }
    
    // Send a Facebook message using Comapi
    public void SendFacebookMessage(String profileId, String body, String customBody)
    {
        System.out.println("");
        System.out.println("Sending FB Message using Comapi and Java");
        System.out.println("----------------------------------------");
       
        if (this.feedback == null)
        {
            this.feedback = new Feedback();
        }
                
        // Create Comapi request, this is just a string but you could use objects and serialise to JSON
        String request;
        
        if (customBody != null)
        {
            // Rich Facebook message send
            request = String.format(""
                + "{"
                + "  \"body\": \"%s\","
                + "  \"to\": {"
                + "    \"profileId\": \"%s\""
                + "  },"
                + "  \"customBody\": %s"
                + "  ,"
                + "  \"rules\": ["
                + "    \"fbMessenger\""
                + "  ]"
                + "}", body, profileId, customBody);
        }
        else
        {
            // Basic text based send
            request = String.format(""
                + "{"
                + "  \"body\": \"%s\","
                + "  \"to\": {"
                + "    \"profileId\": \"%s\""
                + "  },"
                + "  \"rules\": ["
                + "    \"fbMessenger\""
                + "  ]"
                + "}", body, profileId);
        }

        System.out.println(String.format("Request: \n%s", request));
        
        // Call Comapi
        try {
            System.out.println("Calling Comapi...");
            HttpResponse<JsonNode> response = Unirest.post("https://api.comapi.com/apispaces/" + APISPACE + "/messages")
                    .header("authorization", "Bearer " + TOKEN)
                    .header("content-type", "application/json")
                    .header("accept", "application/json")
                    .header("cache-control", "no-cache")
                    .body(request)
                    .asJson();
            
            System.out.println("Call returned status code: (" + response.getStatus() + ") " + response.getStatusText());
            System.out.println(response.getBody().toString());
            System.out.println();
            
            if (response.getStatus() == 201)
            {
                // Sent
                this.getFeedback().setSucceeded(true);
                this.getFeedback().setMessage("Message sent, check your Facebook account now!");
            }
            else
            {
                // Failed
                this.getFeedback().setSucceeded(false);
                this.getFeedback().setMessage("ERROR: " + response.getBody().toString());
            }
            
        } catch (UnirestException ex) {
            // Error calling service
            System.out.println("ERROR: " + ex.getLocalizedMessage());
            this.getFeedback().setSucceeded(false);
            this.getFeedback().setMessage("ERROR: " + ex.getLocalizedMessage());
        }
    }
    
    
    /**
     * @return the encrypted Facebook meta data required to associate a Facebook Messenger Id with a Comapi profile
     * @throws com.mashape.unirest.http.exceptions.UnirestException
     */
    public String getMetadata() throws UnirestException {
        // Call Comapi to create the Facebook metadata if required
        if (metadata == null || metadata.isEmpty())
        {    
            // Create Comapi request, this is just a string but you could use objects and serialise to JSON
            // This request just contains the profileId you want the Facebook Id associated with.
            String request = String.format(""
                    + "{"
                    + "\"profileId\": \"%s\""
                    + "}", getProfileId());

            System.out.println("Facebook metsdata request JSON: " + request);

            // Call Comapi
            try {
                System.out.println("Calling Comapi...");
                HttpResponse<String> response = Unirest.post("https://api.comapi.com/apispaces/" + APISPACE + "/channels/facebook/state")
                        .header("authorization", "Bearer " + TOKEN)
                        .header("content-type", "application/json")
                        .header("cache-control", "no-cache")
                        .body(request)
                        .asString();

                System.out.println("Call returned status code: (" + response.getStatus() + ") " + response.getStatusText());
                System.out.println(response.getBody());
                System.out.println();

                if (response.getStatus() == 200)
                {
                   // Call suceeded
                   metadata = response.getBody().replace("\"", ""); // Strip off double quotes from JSON string
                }
            } catch (UnirestException ex) {
                // Error calling service
                System.out.println("ERROR: " + ex.getLocalizedMessage());
                throw ex;
            }
        }
        
        return metadata;
    }

    /**
     * @param metadata the metadata to set
     */
    public void setMetadata(String metadata) {
        this.metadata = metadata;
    }

    /**
     * @return the testButtonsDisplay
     */
    public String getTestButtonsDisplay() {
        return testButtonsDisplay;
    }

    /**
     * @param testButtonsDisplay the testButtonsDisplay to set
     */
    public void setTestButtonsDisplay(String testButtonsDisplay) {
        this.testButtonsDisplay = testButtonsDisplay;
    }

    /**
     * @return the feedback
     */
    public Feedback getFeedback() {   
        return feedback;
    }

    /**
     * @param feedback the feedback to set
     */
    public void setFeedback(Feedback feedback) {
        this.feedback = feedback;
    }

    /**
     * @return the profileId
     */
    public String getProfileId() {
        return profileId;
    }
}
