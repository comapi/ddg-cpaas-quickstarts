package com.dynmark;

import java.io.IOException;
import java.util.Scanner;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.mashape.unirest.http.HttpResponse;
import com.mashape.unirest.http.Unirest;
import com.mashape.unirest.http.exceptions.UnirestException;

public class SendSMSWithValidate {
	private static String userName = "@username@";
	private static String password = "@Password@";
	private static ObjectMapper objectMapper = new ObjectMapper();

	private static boolean SendMessage(String phoneNumber) throws UnirestException, IOException {
		HttpResponse<String> response = Unirest.post("https://services.dynmark.com/webapi/message/send")
				.header("content-type", "application/json")
				.header("accept", "application/json")
				.basicAuth(userName, password)
				.body("[{"
						+ "'from': 'Example',"
						+ "'to': '" + phoneNumber + "',"
						+ "'body': 'Hello, this is a test message',"
						+ "}]")
				.asString();

		if (response.getStatus() == 202)
		{
			MessageResponse[] body = objectMapper.readValue(response.getBody(), MessageResponse[].class);

			if (body[0].isSuccessful())
			{
				System.out.println("We sent you a message.");
				return true;
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
				return false;
			}
		}
		else if (response.getStatus() == 401)
		{
			System.out.println("Your username and password are incorrect");
			return false;
		}
		else if (response.getStatus() == 400)
		{
			System.out.println("Bad request format");
			return false;
		}
		else
		{
			System.out.println("Non success response " + response.getStatus());
			return false;
		}
	}

	private static int ValidateNumber(String phoneNumber) throws UnirestException, IOException {
		HttpResponse<String> response = 
				Unirest.post("https://services.dynmark.com/webapi/phonenumbervalidation/validatenumber")
				.header("content-type", "application/json")
				.header("accept", "application/json")
				.basicAuth(userName, password)
				.body("{"
						+ "\"Number\": \"" + phoneNumber + "\","
						+ "\"IsInternational\": true"
						+ "}")
				.asString();

		if (response.getStatus() == 200)
		{
			ValidateNumberResponse body = objectMapper.readValue(response.getBody(), ValidateNumberResponse.class);

			return body.getNumberStatus();        	
		}
		if (response.getStatus() == 401)
		{
			System.out.println("Your username and password are incorrect");
			return 0;
		}
		else if (response.getStatus() == 400)
		{
			System.out.println("Bad request format");
			return 0;
		}
		else
		{
			System.out.println("Non success response " + response.getStatus());
			return 0;
		}
	}

	public static void main(String[] args) throws UnirestException, IOException {
		Scanner scanIn = new Scanner(System.in);
		System.out.println("Enter your phone number: ");
		String phoneNumber = scanIn.nextLine();
		scanIn.close();

		switch (ValidateNumber(phoneNumber))
		{
		case 2:
			System.out.println("Your phone is on, we're sending you a message.");
			SendMessage(phoneNumber);
			break;
		case 3:
			System.out.println("Your phone is off, please switch it on and try again in a few minutes.");
			break;
		case 4:
			System.out.println("Your phone number appears to be dead.");
			break;
		case 5:
			System.out.println("Your phone number isn't on a mobile network.");
			break;
		case 6:
			System.out.println("It looks like you entered something that isn't a phone number.");
			break;
		}
	}
}
