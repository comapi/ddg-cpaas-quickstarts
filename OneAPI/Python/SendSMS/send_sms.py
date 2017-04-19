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
        "phoneNumber": "447990766636"
    },
    "body": "This is an SMS via Comapi \"One\" API",
    "channelOptions":
    {
        "sms": {
            "from": "Comapi",
            "allowUnicode": False
        }
    },
    "rules": ["sms"]
}

print("")
print("Request JSON: ")
print(json.dumps(myRequest))

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
print(data.decode("utf-8"))
print("")