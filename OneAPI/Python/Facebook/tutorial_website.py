# -*- coding: utf-8 -*-
# A basic website to demonstrate Facebook opt-in using Comapi and Python.

import http.client
import json
import os
from flask import Flask, render_template, url_for, send_from_directory, request

# Your Comapi settings
APISPACE = "***ADD YOUR API SPACE ID HERE***"
TOKEN = "***ADD YOUR SECURITY TOKEN HERE***"

# Your user, hard coded for demo but would come from your way of identifying a user or visitor
profileId = "joe.blogs@acme.com"

# Create our website app
app = Flask(__name__)

#######################
# Methods and functions
#######################
def createIndexModel():
    model = {
        "metadata": None,
        "testButtonsDisplay": "none",
    }

    return model

def sendFacebookMessage(profileId, message, customBody):
    # Setup the http connection
    conn = http.client.HTTPSConnection("api.comapi.com")

    # Construct the Comapi send message API request
    if (customBody == ""):
        # No custom body
        myRequest = {
            "to": {
                "profileId": profileId
            },
            "body": message,    
            "rules": ["fbMessenger"]
        }
    else:
        # Custom body
        myRequest = {
            "to": {
                "profileId": profileId
            },
            "body": message,
            "customBody": {
                "fbMessenger": customBody
            },
            "rules": ["fbMessenger"]
        }

    print("Message send request JSON: ")
    print(json.dumps(myRequest))

    # Setup the http headers
    headers = {
        "authorization": "Bearer " + TOKEN,
        "content-type": "application/json",
        "cache-control": "no-cache",
        "accept": "application/json"
    }

    # Make the webservice request to send the message
    print("Calling Comapi...")
    conn.request("POST", "/apispaces/" + APISPACE + "/messages", json.dumps(myRequest), headers)

    res = conn.getresponse()
    responseBody = res.read().decode("utf-8")

    if (res.status == 201):
        # Message sent
        print("Message sent: " + responseBody)
    else:
        # Error
        raise IOError("Web service call failed with ({0}) - {1}".format(res.status, responseBody))

#################
# Register routes
#################
@app.route("/favicon.ico")
def get_favicon():
    return send_from_directory(os.path.join(app.root_path, 'static'), 'favicon.ico', mimetype='image/vnd.microsoft.icon') 

@app.route("/")
def get_index():
    # Setup the http connection
    conn = http.client.HTTPSConnection("api.comapi.com")

    # Construct the Comapi API request
    myRequest = {
        "profileId": profileId
    }

    print("")
    print("Request JSON: ")
    print(json.dumps(myRequest))

    # Setup the http headers
    headers = {
        "authorization": "Bearer " + TOKEN,
        "content-type": "application/json",
        "cache-control": "no-cache",
        "accept": "application/json"
    }

    # Make the webservice request to create the secure meta data with the profile details
    print("")
    print("Calling Comapi...")
    conn.request("POST", "/apispaces/" + APISPACE + "/channels/facebook/state", json.dumps(myRequest), headers)

    res = conn.getresponse()
    responseBody = res.read().decode("utf-8")

    if (res.status == 200):
        # Meta data returned
        print("Meta data: " + responseBody)
    else:
        # Error
        raise IOError("Web service call failed with (" + res.status + ") - " + responseBody)
    
    # Create page model data
    model = createIndexModel()
    model["metadata"] = json.loads(responseBody)

    # Render the page passing the model data
    return render_template('index.html', model=model)

@app.route('/', methods=['POST'])
def post_index():
    # Create page model data
    model = createIndexModel()

    # Set the test buttons to visible
    model["testButtonsDisplay"] = "block"

    # Do the action for the button
    try:
        if 'SimpleTest' in request.form:
            print ("Simple test")

            # Do the send
            sendFacebookMessage(profileId, "A simple text message sent via Comapi", "")
        
        if 'RichTest' in request.form:
            print ("Rich test")

            # Create Facebook message object
            facebookMessage = {
                "text": "Pick a color:",
                "quick_replies": [
                    {
                        "content_type": 'text',
                        "title": "Red",
                        "payload": "DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_RED"
                    },
                    {
                        "content_type": "text",
                        "title": "Green",
                        "payload": "DEVELOPER_DEFINED_PAYLOAD_FOR_PICKING_GREEN"
                    }
                ]
            }

            # Do the send
            sendFacebookMessage(profileId, "", facebookMessage)

        # Send worked
        model["feedback"] = {}
        model["feedback"]["succeeded"] = True
        model["feedback"]["message"] = "Message sent, check Facebook / Facebook Messenger"

    except Exception as ex:
        # Send failed
        model["feedback"] = {}
        model["feedback"]["succeeded"] = False
        model["feedback"]["message"] = "{0}".format(ex)

    return render_template('index.html', model=model)

# Fire up the local server
if __name__ == "__main__":
    app.run()