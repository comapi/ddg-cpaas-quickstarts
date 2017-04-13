package com.dynmark;

import java.io.IOException;
import java.lang.Integer;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

@WebServlet(urlPatterns = { "/receipt" })
public class ReceiptServlet extends HttpServlet {
	private static final long serialVersionUID = 1L;
	public static ArrayList<Receipt> receiptStore = new ArrayList<>();

	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		// Do some basic parameter validation before processing
		if (request.getParameter("datetime") != null)
		{
			SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");

			Receipt receipt = new Receipt();
			try {
				receipt.setDate(dateFormat.parse(request.getParameter("datetime")));
				receipt.setRecipient(request.getParameter("recipient"));
				receipt.setStatusId(Integer.parseInt(request.getParameter("statusid")));
				receipt.setStatusDescription(request.getParameter("statusdescription"));
				receipt.setClientRef(request.getParameter("clientref"));

				receiptStore.add(receipt);
			} catch (ParseException e) {
				response.setStatus(HttpServletResponse.SC_BAD_REQUEST);
			}
		}

		response.setStatus(HttpServletResponse.SC_OK);
	}
}
