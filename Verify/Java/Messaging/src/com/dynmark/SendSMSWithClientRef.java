package com.dynmark;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.mashape.unirest.http.HttpResponse;
import com.mashape.unirest.http.Unirest;
import com.mashape.unirest.http.exceptions.UnirestException;

public class SendSMSWithClientRef {
	public static void main(String[] args) throws UnirestException {
		String userName = "@username@";
		String password = "@Password@";
		int messageId = 0;

		
		HttpResponse<MessageResponse[]> response = Unirest.post("https://services.dynmark.com/webapi/message/send")
		  .header("content-type", "application/json")
		  .header("accept", "application/json")
		  .basicAuth(userName, password)
		  .body("[{"
		  		+ "'from': 'Example',"
		  		+ "'to': '44770090000000',"
		  		+ "'body': 'Hello, this is a test message',"
		  		+ "'clientRef': " + String.format("msg-%d", ++messageId)
		  		+ "}]")
		  .asObject(MessageResponse[].class);
		
        if (response.getStatus() == 202)
        {
        	if (response.getBody()[0].isSuccessful())
        	{
        		System.out.println("We sent you a message.");
        	}
        	else
        	{
        		System.out.println("Sorry, we couldn't send you a message.");
        		for (ApiValidationFailure failure : response.getBody()[0].getValidationFailures())
        		{
        			if (failure.getFailureCode().equals("ToInvalid"))
        			{
        				System.out.println("Your phone number doesn't look like a valid number.");
        			}
        		}
        	}
        }
        else if (response.getStatus() == 401)
        {
        	System.out.println("Your username and password are incorrect");
        }
        else if (response.getStatus() == 400)
        {
        	System.out.println("Bad request format");
        }
        else
        {
    		System.out.println("Non success response " + response.getStatus());
        }
    }
}
