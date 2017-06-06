import com.cedarsoftware.util.io.JsonWriter;
import com.mashape.unirest.http.HttpResponse;
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
                + "  \"rules\": ["
                + "    \"sms\""
                + "  ]"
                + "}";

        // Call Comapi
        try {
            System.out.println("Calling Comapi...");
            HttpResponse<String> response = Unirest.post("https://api.comapi.com/apispaces/" + APISPACE + "/messages")
                    .header("authorization", "Bearer " + TOKEN)
                    .header("content-type", "application/json")
                    .header("accept", "application/json")
                    .header("cache-control", "no-cache")
                    .body(request)
                    .asString();
            
            System.out.println("Call returned status code: (" + response.getStatus() + ") " + response.getStatusText());
            
            // Check result
            if (response.getStatus() == 201)
            {
                // All ok
                System.out.println("Call succeeded");
                System.out.println(JsonWriter.formatJson(response.getBody().toString()));
                System.out.println();
            }
            else
            {
                // Failed
                System.out.println("Call failed!");
                System.out.println(response.getBody().toString());
                System.out.println();
            }
            
        } catch (UnirestException ex) {
            // Error calling service
            System.out.println("ERROR: " + ex.getLocalizedMessage());
        }
    }
}
