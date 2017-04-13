package com.dynmark;

import java.io.IOException;
import java.util.Scanner;
import java.util.UUID;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.mashape.unirest.http.HttpResponse;
import com.mashape.unirest.http.Unirest;
import com.mashape.unirest.http.exceptions.UnirestException;

public class Verify {
	private static String userName = "@username@";
	private static String password = "@Password@";
	private static UUID requestId;
	private static ObjectMapper objectMapper = new ObjectMapper();

	private static boolean SendCode(String phoneNumber) throws UnirestException, IOException {
		HttpResponse<String> response = 
				Unirest.post("https://services.dynmark.com/webapi/verify")
				.header("content-type", "application/json")
				.header("accept", "application/json")
				.basicAuth(userName, password)
				.body("{"
						+ "'from': 'Example',"
						+ "'to': '" + phoneNumber + "',"
						+ "}")
				.asString();

		if (response.getStatus() == 201)
		{
			SendCodeResponse body = objectMapper.readValue(response.getBody(), SendCodeResponse.class);
			requestId = body.getRequestId();
			return true;
		}
		else if (response.getStatus() == 401)
		{
			System.out.println("Your username and password are incorrect");
			return false;
		}
		else if (response.getStatus() == 400)
		{
			ApiValidationFailure[] failures = objectMapper.readValue(response.getRawBody(), ApiValidationFailure[].class);

			System.out.println("Sorry, we couldn't send you a code.");
			for (ApiValidationFailure failure : failures)
			{
				if (failure.getFailureCode().equals("ToInvalid"))
				{
					System.out.println("Your phone number doesn't look like a valid number.");
				}
			}

			return false;
		}
		else
		{
			System.out.println("Non success response " + response.getStatus());
			return false;
		}
	}

	private static boolean ValidateCode(String code) throws UnirestException, IOException {
		HttpResponse<String> response = 
				Unirest.put("https://services.dynmark.com/webapi/verify/" + requestId.toString() + "/validate")
				.header("content-type", "application/json")
				.header("accept", "application/json")
				.basicAuth(userName, password)
				.body("{"
						+ "'code': '" + code + "'"
						+ "}")
				.asString();

		if (response.getStatus() == 200)
		{
			ValidateResponse body = objectMapper.readValue(response.getBody(), ValidateResponse.class);

			if (body.getStatus().equals("CodeVerified"))
			{
				System.out.println("Your code was correct and is verified.");
				return true;
			}

			System.out.println("Sorry, that code was not recognised. Reason: " + body.getStatus());
			return false;
		}
		if (response.getStatus() == 204)
		{
			System.out.println("Request id not recognised");
			return true;
		}
		else if (response.getStatus() == 401)
		{
			System.out.println("Your username and password are incorrect");
			return false;
		}
		else if (response.getStatus() == 400)
		{
			ApiValidationFailure[] failures = objectMapper.readValue(response.getRawBody(), ApiValidationFailure[].class);

			System.out.println("Sorry, we couldn't send you a code.");
			for (ApiValidationFailure failure : failures)
			{
				if (failure.getFailureCode().equals("ToInvalid"))
				{
					System.out.println("Your phone number doesn't look like a valid number.");
				}
			}

			return false;
		}
		else
		{
			System.out.println("Non success response " + response.getStatus());
			return false;
		}
	}

	public static void main(String[] args) throws UnirestException, IOException {
		Scanner scanIn = new Scanner(System.in);
		System.out.println("Enter your phone number: ");
		if (SendCode(scanIn.nextLine()))
		{
			System.out.println("Enter the code you received: ");
			while (!ValidateCode(scanIn.nextLine()))
			{
			}
		}

		scanIn.close();         
	}
}
