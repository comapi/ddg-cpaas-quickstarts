package com.dynmark;

import java.io.IOException;
import java.util.Scanner;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.mashape.unirest.http.HttpResponse;
import com.mashape.unirest.http.Unirest;
import com.mashape.unirest.http.exceptions.UnirestException;

public class SendSMS {
	private static String userName = "@username@";
	private static String password = "@Password@";
	private static ObjectMapper objectMapper = new ObjectMapper();

	public static void main(String[] args) throws UnirestException, IOException {

		Scanner scanIn = new Scanner(System.in);
		System.out.println("Enter your phone number: ");
		String phoneNumber = scanIn.nextLine();
		scanIn.close();

		HttpResponse<String> response = Unirest.post("https://services.dynmark.com/webapi/message/send")
		  .header("content-type", "application/json")
		  .header("accept", "application/json")
		  .basicAuth(userName, password)
		  .body("[{"
		  		+ "'from': 'Example',"
		  		+ "'to': '" + phoneNumber + "',"
		  		+ "'body': 'Hello World. This is a test message',"
		  		+ "}]")
		  .asString();
		
        if (response.getStatus() == 202)
        {
        	MessageResponse[] body = objectMapper.readValue(response.getBody(), MessageResponse[].class);

        	if (body[0].isSuccessful())
        	{
        		System.out.println("We sent you a message.");
        	}
        	else
        	{
        		System.out.println("Sorry, we couldn't send you a message.");
        		for (ApiValidationFailure failure : body[0].getValidationFailures())
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
