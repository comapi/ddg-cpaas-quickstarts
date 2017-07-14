require 'uri'
require 'net/http'
require 'openssl'

class ApplicationController < ActionController::Base

  ################################
  # GET handler for the Index page
  def index
    # Render the view
    render :index
  end

  ############################################################
  # POST handler for the Index page, the webhook receiver page
  def indexPost
    # Grab the body data
    raw_body = request.raw_post

    if (raw_body == nil)
        # No body, bad request.
        render :inline => "Bad request - No JSON body found!", :status => 400
        return
    end

    # We have a request body so lets look at what we have

    # First lets ensure it hasn't been tampered with and it came from Comapi
    # We do this by checking the HMAC from the X-Comapi-Signature header
    request_hmac = request.headers["x-comapi-signature"]

    if (request_hmac == nil)
        # No HMAC, invalid request.
        render :inline => "Invalid request: No HMAC value found!", :status => 401
        return
    else
        # Validate the HMAC, ensure you has exposed the rawBody, see app.js for how to do this
        hash = createHmacSha1(raw_body, ">>>YOUR SECRET<<<")

        if (request_hmac != hash)
            # The request is not from Comapi or has been tampered with
            render :inline => "Invalid request: HMAC hash check failed!", :status => 401
            return
        end
    end

    # Store the received event for later processing, remember you only have 10 secs to process, in this simple example we output to the console
    eventObj = JSON.parse(raw_body)

    puts ("")        
    puts ("Received a #{eventObj["name"]} event id: #{eventObj["eventId"]}")
    puts(JSON.pretty_unparse(eventObj)) # Pretty puts the JSON
    puts ("")

    # You could use queuing tech such as RabbitMQ, or possibly a distributed cache such as Redis       

    # Send worked
    render :inline => "Data accepted", :status => 200
    return
  end

  ###########################################################################################
  private

  #######################################################
  # Create a HMAC hash sing SHA-1
  def createHmacSha1(data, key)
    digest = OpenSSL::Digest.new('sha1')
    hmac = OpenSSL::HMAC.hexdigest(digest, key, data)

    return hmac
  end

end