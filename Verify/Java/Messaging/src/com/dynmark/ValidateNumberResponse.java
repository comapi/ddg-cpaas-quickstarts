package com.dynmark;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class ValidateNumberResponse {
	
	@JsonProperty("NumberStatus")
	private int numberStatus;
	
	@JsonProperty("NormalisedNumber")
	private String normalisedNumber;
	
	public int getNumberStatus() {
		return numberStatus;
	}
	public void setNumberStatus(int numberStatus) {
		this.numberStatus = numberStatus;
	}
	public String getNormalisedNumber() {
		return normalisedNumber;
	}
	public void setNormalisedNumber(String normalisedNumber) {
		this.normalisedNumber = normalisedNumber;
	}
	
}
