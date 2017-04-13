package com.dynmark;

import java.util.ArrayList;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class MessageResponse {
	@JsonProperty("to")
	private String to;

	@JsonProperty("successful")
	private boolean successful;

	@JsonProperty("validationFailures")
	private ArrayList<ApiValidationFailure> validationFailures;

	public String getTo() {
		return to;
	}
	public void setTo(String to) {
		this.to = to;
	}
	public boolean isSuccessful() {
		return successful;
	}
	public void setSuccessful(boolean successful) {
		this.successful = successful;
	}
	public ArrayList<ApiValidationFailure> getValidationFailures() {
		return validationFailures;
	}
	public void setValidationFailures(ArrayList<ApiValidationFailure> validationFailures) {
		this.validationFailures = validationFailures;
	}
}
