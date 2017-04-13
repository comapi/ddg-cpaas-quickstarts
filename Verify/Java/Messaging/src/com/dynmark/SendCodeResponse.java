package com.dynmark;

import java.util.UUID;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SendCodeResponse {

	@JsonProperty("requestId")
	private UUID requestId;
	
	@JsonProperty("to")
	private String to;
	
	@JsonProperty("numberStatus")
	private String numberStatus;
	
	public UUID getRequestId() {
		return requestId;
	}
	public void setRequestId(UUID requestId) {
		this.requestId = requestId;
	}
	public String getTo() {
		return to;
	}
	public void setTo(String to) {
		this.to = to;
	}
	public String getNumberStatus() {
		return numberStatus;
	}
	public void setNumberStatus(String numberStatus) {
		this.numberStatus = numberStatus;
	}
}
