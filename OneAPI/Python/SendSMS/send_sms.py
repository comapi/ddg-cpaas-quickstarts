import http.client
import json

print("")
print("Sending SMS using Comapi and Python")
print("-----------------------------------")

# Your Comapi settings
APISPACE = "***ADD YOUR API SPACE ID HERE***"
TOKEN = "***ADD YOUR SECURITY TOKEN HERE***"

# Setup the http connection
conn = http.client.HTTPSConnection("api.comapi.com")

# Construct the Comapi API request
myRequest = {
    "to": {
        "phoneNumber": "447123123123"
    },
    "body": "This is an SMS via Comapi \"One\" API",    
    "rules": ["sms"]
}

print("")
print("Request JSON: ")
print(json.dumps(myRequest, indent=2))

# Setup the http headers
headers = {
    'authorization': "Bearer " + TOKEN,
    'content-type': "application/json",
    'cache-control': "no-cache"
}

# Make the webservice request
print("")
print("Calling Comapi...")
conn.request("POST", "/apispaces/" + APISPACE + "/messages",
             json.dumps(myRequest), headers)

res = conn.getresponse()
data = res.read()

print("")
print("Call returned status code: " + str(res.status))
print(json.dumps(json.loads(data.decode("utf-8")), indent=2)) # Pretty print the JSON
print("")