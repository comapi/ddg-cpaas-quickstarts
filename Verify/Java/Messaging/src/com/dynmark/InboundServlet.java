package com.dynmark;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

@WebServlet(urlPatterns = { "/inbound" })
public class InboundServlet extends HttpServlet {
	private static final long serialVersionUID = 1L;
	public static ArrayList<InboundMessage> inboundStore = new ArrayList<>();

	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
	    // Do some basic parameter validation before processing
	    if (request.getParameter("msgid") != null)
	    {
	    	SimpleDateFormat dateFormat = new SimpleDateFormat("dd MMM yyyy HH:mm:ss");
	    	InboundMessage message = new InboundMessage();

	    	try {
		    	message.setMessageId(Long.parseLong(request.getParameter("msgid")));
		    	message.setDate(dateFormat.parse(request.getParameter("datetime")));
				message.setSender(request.getParameter("sender"));
				message.setRecipient(request.getParameter("recipient"));
	            message.setBody(request.getParameter("content"));
			} catch (ParseException e) {
			    response.setStatus(HttpServletResponse.SC_BAD_REQUEST);
			}
	    	
	        inboundStore.add(message);
	    }
		
	    response.setStatus(HttpServletResponse.SC_OK);
	}
}
