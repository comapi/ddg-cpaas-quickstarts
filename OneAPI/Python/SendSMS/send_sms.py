import http.client
import json

print("")
print("Sending SMS using Comapi and Python")
print("-----------------------------------")

conn = http.client.HTTPSConnection("api.comapi.com")

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

headers = {
    'authorization': "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0N2E2YTQ5YS0zZTBkLTQ0MjUtODI3ZS1hMmE4YTNhYzIwNWIiLCJpc3MiOiJodHRwczovL2FwaS5jb21hcGkuY29tL2FjY2Vzc3Rva2VucyIsImF1ZCI6Imh0dHBzOi8vYXBpLmNvbWFwaS5jb20iLCJhY2NvdW50SWQiOjM0NzI3LCJhcGlTcGFjZUlkIjoiYzEyNGNmNmUtNDM1Mi00YjI2LWE3MWEtYzMwMzJiZWE3YTAxIiwicGVybWlzc2lvbnMiOlsiY2hhbjpyIiwibXNnOmFueTpzIiwibXNnOnIiLCJwcm9mOnJhIiwiYXBpczpybyJdLCJzdWIiOiI0N2E2YTQ5YS0zZTBkLTQ0MjUtODI3ZS1hMmE4YTNhYzIwNWIiLCJwcm9maWxlSWQiOiJBY21lIiwibmFtZSI6IkdlbmVyYWxTZW5kIiwiaWF0IjoxNDkxNDA0NDMxfQ.3tGzifbQKvSISAZSQyEjeUxqqU3JwASPeqY2T7gLNn4",
    'content-type': "application/json",
    'cache-control': "no-cache"
}

print("")
print("Calling Comapi...")
conn.request("POST", "/apispaces/c124cf6e-4352-4b26-a71a-c3032bea7a01/messages",
             json.dumps(myRequest), headers)

res = conn.getresponse()
data = res.read()

print("")
print("Call returned status code: " + str(res.status))
print(data.decode("utf-8"))
print("")