require 'uri'
require 'net/http'
require 'openssl'

# Your Comapi settings
ApiSpace = "***ADD YOUR API SPACE ID HERE***"
Token = "***ADD YOUR SECURITY TOKEN HERE***"

puts ""
puts "Sending SMS using Comapi and Ruby"
puts "---------------------------------"

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
request["accept"] = 'application/json'
request["cache-control"] = 'no-cache'

# Create the Comapi request JSON
request.body = 
    "{
      \"body\": \"Your SMS message\",
      \"to\": {
          \"phoneNumber\": \"447123123123\"
        },
      \"channelOptions\": {
          \"sms\": {
              \"from\": \"Comapi\",
              \"allowUnicode\": true
            }
        },
     \"rules\": [ \"sms\" ]
     }"

# Call the web service
puts ""
puts "Calling Comapi..."
response = http.request(request)

puts ""
puts "Call returned status code: " + response.code
puts JSON.pretty_unparse(JSON.parse(response.read_body)) # Pretty print the JSON