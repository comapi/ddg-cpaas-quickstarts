
import com.mashape.unirest.http.HttpResponse;
import com.mashape.unirest.http.JsonNode;
import com.mashape.unirest.http.Unirest;
import com.mashape.unirest.http.exceptions.UnirestException;

/**
 * Basic example of how to send an SMS using the Comapi "One" API
 *
 * @author dave.baddeley
 */
public class Main {

    // Comapi settings
    private static String APISPACE = "***ADD YOUR API SPACE ID HERE***";
    private static String TOKEN = "***ADD YOUR SECURITY TOKEN HERE***";

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        System.out.println("Sending SMS using Comapi and Java");
        System.out.println("---------------------------------");

        // Create Comapi request, this is just a string but you could use objects and serialise to JSON
        String request = ""
                + "{"
                + "  \"body\": \"Your SMS message\","
                + "  \"to\": {"
                + "    \"phoneNumber\": \"447123123123\""
                + "  },"
                + "  \"channelOptions\": {"
                + "    \"sms\": {"
                + "      \"from\": \"Comapi\","
                + "      \"allowUnicode\": true"
                + "    }"
                + "  },"
                + "  \"rules\": ["
                + "    \"sms\""
                + "  ]"
                + "}";

        // Call Comapi
        try {
            System.out.println("Calling Comapi...");
            HttpResponse<JsonNode> response = Unirest.post("https://api.comapi.com/apispaces/c124cf6e-4352-4b26-a71a-c3032bea7a01/messages")
                    .header("authorization", "Bearer " + TOKEN)
                    .header("content-type", "application/json")
                    .header("cache-control", "no-cache")
                    .body(request)
                    .asJson();
            
            System.out.println("Call returned status code: (" + response.getStatus() + ") " + response.getStatusText());
            System.out.println(response.getBody().toString());
            System.out.println();
        } catch (UnirestException ex) {
            // Error calling service
            System.out.println("ERROR: " + ex.getLocalizedMessage());
        }
    }
}
