require 'uri'
require 'net/http'
require 'openssl'

class ApplicationController < ActionController::Base

  # Your Comapi settings
  ApiSpace = "***ADD YOUR API SPACE ID HERE***"
  Token = "***ADD YOUR SECURITY TOKEN HERE***"

  # Your user, hard coded for demo but would come from your way of identifying a user or visitor
  ProfileId = "joe.blogs@acme.com"

  ################################
  # GET handler for the Index page
  def index
    # Create model for page
    @model = IndexModel.new

    # Retrieve Facebook metadata for the profile (user) from Comapi
    result = getFacebookMetaData(ProfileId)
    
    # Check the results
    if (result.succeeded)
      @model.metadata = result.data
    else
      # Error
      @model.feedback = result.feedback
    end

    # Render the view
    render :index
  end

  #################################
  # POST handler for the Index page
  def indexPost
    # Create model for page
    @model = IndexModel.new

    # Retrieve Facebook metadata for the profile (user) from Comapi
    result = getFacebookMetaData(ProfileId)
    
    # Check the results
    if (result.succeeded)
      @model.metadata = result.data

      # Which button was pressed?
      if (params['SimpleTest'] != nil)
        # Send a simple text based message
        result = sendFacebookMessage(ProfileId, "This is a simple text based message from Comapi!", nil)
      elsif (params['RichTest'] != nil)
        # Send a Facebook message object
        facebookMessage = "{
                \"text\": \"Pick a color:\",
                \"quick_replies\": [
                    {
                        \"content_type\": \"text\",
                        \"title\": \"Red\",
                        \"payload\": \"DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_RED\"
                    },
                    {
                        \"content_type\": \"text\",
                        \"title\": \"Green\",
                        \"payload\": \"DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_GREEN\"
                    }
                ]
            }"

        result = sendFacebookMessage(ProfileId, nil, facebookMessage)
      end

      # Feedback
      @model.feedback = result.feedback
    else
      # Error
      @model.feedback = result.feedback
    end

    # Show the buttons area
    @model.testButtonsDisplay = "block"

    # Render the view
    render :index
  end

  ###########################################################################################
  private

  #######################################################
  # Send a message to Facebook using the Comapi "One" API
  def sendFacebookMessage(profileId, body, customBody)
    result = WebserviceResult.new

    # Create the RESTful URL
    url = URI("https://api.comapi.com/apispaces/" + ApiSpace + "/messages")

    # Setup the conneciton object
    http = Net::HTTP.new(url.host, url.port)
    http.use_ssl = true
    http.verify_mode = OpenSSL::SSL::VERIFY_NONE # This should be removed and SSL certs validated for production environments

    # Setup the HTTP request
    request = Net::HTTP::Post.new(url)
    request["authorization"] = 'Bearer ' + Token
    request["content-type"] = 'application/json'
    request["cache-control"] = 'no-cache'
    request["accept"] = 'application/json'

    # Create the Comapi request JSON
    if (body != nil)
      request.body = 
      "{
        \"to\": {
            \"profileId\": \"%s\"
          },
      \"body\": \"%s\",
      \"rules\": [ \"fbMessenger\" ]
      }" % [profileId, body]
    elsif (customBody != nil)
      request.body = 
      "{
        \"to\": {
            \"profileId\": \"%s\"
          },
      \"customBody\": {
          \"fbMessenger\": %s
          },
      \"rules\": [ \"fbMessenger\" ]
      }" % [profileId, customBody] 
    end

    # Call the web service
    response = http.request(request)

    # Check the results
    if (response.code == "201")
      result.succeeded = true
      result.feedback = Feedback.new
      result.feedback.succeeded = true
      result.feedback.message = "Check your Facebook to see your message"
    else
      # Error
      result.succeeded = false
      result.feedback = Feedback.new
      result.feedback.succeeded = false
      result.feedback.message = "(Returned: %s): %s" % [response.code, response.read_body]
    end

    return result
  end

  ###########################################################################################
  # Retrieve the encrypted Facebook metadata used to tie a Facebook id to a profile in Comapi
  def getFacebookMetaData(profileId)
    result = WebserviceResult.new

    # Create the RESTful URL
    url = URI("https://api.comapi.com/apispaces/" + ApiSpace + "/channels/facebook/state")

    # Setup the conneciton object
    http = Net::HTTP.new(url.host, url.port)
    http.use_ssl = true
    http.verify_mode = OpenSSL::SSL::VERIFY_NONE # This should be removed and SSL certs validated for production environments

    # Setup the HTTP request
    request = Net::HTTP::Post.new(url)
    request["authorization"] = 'Bearer ' + Token
    request["content-type"] = 'application/json'
    request["cache-control"] = 'no-cache'
    request["accept"] = 'application/json'

    # Setup the request body to request metadata for our user/profile
    # this would usually be the logged in user id, butit is hardcoded for the demo
    request.body = 
    "{
      \"profileId\": \"%s\"
     }" % [ profileId ]

    # Call the web service
    response = http.request(request)

    # Check the results
    if (response.code == "200")
      result.succeeded = true
      result.data = response.read_body.gsub! '"', '' # Strip off double quotes as this is a JSON string
    else
      # Error
      result.succeeded = false
      result.feedback = Feedback.new
      result.feedback.succeeded = false
      result.feedback.message = "(Returned: %s): %s" % [response.code, response.read_body]
    end

    return result
  end

end